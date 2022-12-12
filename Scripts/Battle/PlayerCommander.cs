#nullable enable

using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using RtShogi.Scripts.Battle.Player;
using RtShogi.Scripts.Battle.UI;
using RtShogi.Scripts.Param;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RtShogi.Scripts.Battle
{
    public class PlayerCooldownTime
    {
        public readonly float Seconds;

        public PlayerCooldownTime(float seconds)
        {
            Seconds = seconds;
#if UNITY_EDITOR
            if (DebugParameter.Instance.IsNoCooldownTime) Seconds = 0.1f;
#endif
        }
    }
    
    public interface IPlayerClickable{}

    public record PlayerClickedBoardKoma(KomaUnit Koma) : IPlayerClickable;

    public record PlayerDraggingObtainedKoma(
        ObtainedKomaElement DraggingElement,
        EKomaKind Kind,
        RectTransform HoverCursor) :
        IPlayerClickable;

    public class PlayerCommander : MonoBehaviour
    {
        [SerializeField] private BattleRoot battleRoot;
        [SerializeField] private Material matTransparentBlue;
        [SerializeField] private Material matTransparentBlack;

        private BoardManager boardManagerRef => battleRoot.BoardManager;
        private BattleCanvas battleCanvas => battleRoot.BattleCanvasRef;
        private KomaManager komaManager => battleRoot.KomaManager;
        private BattleRpcaller rpcaller => battleRoot.Rpcaller;
        
        
        private BoardMap boardMapRef => boardManagerRef.BoardMap;
        
        private const int leftButtonId = 0;
        private IPlayerClickable? _selectingKoma = null;
        private BoardPiece? _destPiece = null;
        private KomaUnit? _destKomaGhost = null;
        
        private CommanderAction _myAction;
        public CommanderAction Act => _myAction; 
        
        private Camera mainCamera => Camera.main;

        [EventFunction]
        private void Awake()
        {
            _myAction = 
                new CommanderAction(komaManager, boardManagerRef, battleCanvas, rpcaller, battleRoot.EffectManager);
        }

        [EventFunction]
        private void Start()
        {
            // ProcessPlayer().Forget();
        }

        public async UniTask ProcessPlayer()
        {
            while (true)
            {
                var cooldown = await processUpAndDownLeftClick();
                if (await checkStopProcess()) return;
                
                if (cooldown.Seconds > 0) await CooldownAnimation.ChargeThenHideCooldown(battleCanvas.CooldownBar, cooldown.Seconds);
                if (await checkStopProcess()) return;
            }
        }

        private async UniTask<PlayerCooldownTime> processUpAndDownLeftClick()
        {
            while (!isSelecting())
            {
                // 左クリックを押して選択するまで
                checkDownLeftMouse();
                if (await checkStopProcess()) return new PlayerCooldownTime(0);
            }
            
            Logger.Print("start drag");

            while (!isEndDragging(_selectingKoma))
            {
                // 左クリックでドラッグ中
                updateWhileDragging();
                if (await checkStopProcess()) return new PlayerCooldownTime(0);
            }
            
            Logger.Print("end drag");

            // 左クリックを離した
            return await performOnUpLeftMouse();
        }

        private async UniTask<bool> checkStopProcess()
        {
            await UniTask.DelayFrame(1);
            return komaManager.BattleStatus is BattleStatusFinished;
        }

        private void checkDownLeftMouse()
        {
            bool justLeftClickDown = Input.GetMouseButtonDown(leftButtonId);
            
            _selectingKoma = 
                // 盤上のクリックした駒があったら取得
                (justLeftClickDown ? findKomaOnPieceRayedByMousePos(ETeam.Ally) : null) ?? 
                // 獲得駒をドラッグしてたら取得
                (IPlayerClickable?)checkDragObtainedKoma();

            if (_selectingKoma == null) return;
            
            Logger.Print("select koma and start highlighting pieces");
            
            switch (_selectingKoma)
            {
                case PlayerClickedBoardKoma fieldKoma:
                    _myAction.HighlightMovableList(fieldKoma.Koma);
                    animPieceSelected(fieldKoma.Koma).Forget();
                    break;
                case PlayerDraggingObtainedKoma obtainedKoma:
                    _myAction.HighlightInstallableList(obtainedKoma.Kind);
                    break;
                default:
                    throw new NotImplementedException();
            }

            createDestKomaGohst();
        }

        private static async UniTask animPieceSelected(KomaUnit? holding)
        {
            if (holding==null) return;
            await holding.gameObject.transform.DOScale(1.3f, 0.3f).SetEase(Ease.InOutBack);
            await holding.gameObject.transform.DOScale(1.0f, 0.3f).SetEase(Ease.OutBack);
        }

        private PlayerClickedBoardKoma? findKomaOnPieceRayedByMousePos(ETeam targetTeam)
        {
            var piece = findPieceRayedByMousePos();
            if (piece == null) return null;

            var holding = piece.Holding;
            if (holding == null) return null;
            if (holding.Team != targetTeam) return null;
            
            return new PlayerClickedBoardKoma(holding);
        }

        private PlayerDraggingObtainedKoma? checkDragObtainedKoma()
        {
            var dragging = battleCanvas.ObtainedKomaAlly.FindDraggingElem();
            if (dragging == null) return null;
            
            Debug.Log("start drag obtained koma");

            EKomaKind kind = dragging.Kind;
            var cursor = dragging.CreateIconCursorIcon(battleCanvas.ParentCanvas.transform);
            return new PlayerDraggingObtainedKoma(dragging, kind, cursor.rectTransform);
        }

        private BoardPiece? findPieceRayedByMousePos()
        {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out var hit)) return null;

            GameObject? clickedGameObject = hit.collider.gameObject;

            if (clickedGameObject == null) return null;

            return !clickedGameObject.TryGetComponent<BoardPiece>(out var piece) ? null : piece;
        }

        private void createDestKomaGohst()
        {
            _destKomaGhost = _selectingKoma switch
            {
                PlayerClickedBoardKoma fieldKoma => Instantiate(fieldKoma.Koma, transform),
                PlayerDraggingObtainedKoma obtainedKoma => komaManager.CreateVirtualKoma(obtainedKoma.Kind, ETeam.Ally),
                _ => throw new NotImplementedException()
            };

            var mesh = _destKomaGhost.MeshRenderer;
            mesh.materials = mesh.sharedMaterials
                .Select(material => (material.name == KomaUnit.NameMatBody) ? matTransparentBlue : matTransparentBlack)
                .ToArray();
            _destKomaGhost.gameObject.SetActive(false);
        }

        private void updateWhileDragging()
        {
            var piece = findPieceRayedByMousePos();
            _destPiece = piece;
            
            bool canPut = piece != null && piece.IsActiveHighlight(); 
            _destKomaGhost.gameObject.SetActive(canPut);
            
            // 獲得したドラッグ中の駒はカーソルの移動
            if (_selectingKoma is PlayerDraggingObtainedKoma draggingObtained) 
                _myAction.MoveObtainedKomaHoverCursor(draggingObtained, canPut);
            
            if (!canPut) return;

            var destPos = piece.transform.position;
            destPos.y = BoardPiece.KomaPosY;
            _destKomaGhost.transform.position = destPos;
        }

        private async UniTask<PlayerCooldownTime> performOnUpLeftMouse()
        {
            // 駒を盤上で目的地に移動させる
            var cooldown = _selectingKoma switch
            {
                PlayerClickedBoardKoma fieldKoma => 
                    await _myAction.PerformMoveClickingKomaToDestination(
                        fieldKoma.Koma, _destPiece, battleCanvas.CooldownBar),
                PlayerDraggingObtainedKoma obtainedKoma => 
                    await _myAction.PerformInstallObtainedKomaToDestination(
                        obtainedKoma, _destPiece, battleCanvas.CooldownBar, komaManager),
                _ => throw new ArgumentOutOfRangeException(nameof(_selectingKoma)),
            };

            // 進める場所や置ける場所ハイライト解除
            boardMapRef.ForEach(piece => piece.DisableHighlight());
            
            // クリック中の駒を解除
            onUnselectSelectedKoma(cooldown);
            _selectingKoma = null;
            
            // ゴースト削除
            if (_destKomaGhost != null) Util.DestroyGameObject(_destKomaGhost.gameObject);
            _destKomaGhost = null;
            
            // クールダウンバーの値を0まで減らす演出
            if (cooldown.Seconds > 0) await CooldownAnimation.GoDownToZeroCooldown(battleCanvas.CooldownBar, cooldown.Seconds);
            
            return cooldown;
        }

        private void onUnselectSelectedKoma(PlayerCooldownTime cooldownTime)
        {
            switch (_selectingKoma)
            {
                case PlayerClickedBoardKoma:
                    break;
                case PlayerDraggingObtainedKoma obtainedKoma:
                    if (obtainedKoma.HoverCursor != null) Util.DestroyGameObject(obtainedKoma.HoverCursor.gameObject);
                    // if (cooldownTime.Seconds<=0) break;
                    // battleCanvas.ObtainedKomaAlly.DecElement(obtainedKoma.DraggingElement);
                    break;
            }
        }

        private bool isSelecting()
        {
            return _selectingKoma != null;
        }
        
        
        private static bool isEndDragging(IPlayerClickable clicking)
        {
            return clicking switch
            {
                PlayerClickedBoardKoma komaUnit => Input.GetMouseButtonUp(leftButtonId),
                PlayerDraggingObtainedKoma obtainedKoma => obtainedKoma.DraggingElement.HasEndDrag.TakeFlag(),
                _ => throw new ArgumentOutOfRangeException(nameof(clicking))
            };
        }

#if UNITY_EDITOR
        [Button]
        public void DebugForceObtainKoma(EKomaKind kind)
        {
            var view = komaManager.GetViewProps(kind);
            battleCanvas.ObtainedKomaAlly.IncElement(new ObtainedKomaElementProps(
                kind,
                view.SprIcon,
                ETeam.Ally));
        }
#endif
    }
}