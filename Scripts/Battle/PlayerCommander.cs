#nullable enable

using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using RtShogi.Scripts.Battle.Player;
using RtShogi.Scripts.Battle.UI;
using UnityEngine;

namespace RtShogi.Scripts.Battle
{
    public record PlayerCooldownTime(float Seconds);
    
    public interface IPlayerClickable{}

    public record PlayerClickingObtainedKoma(
        EKomaKind Kind,
        GameObject HoverCursor) : IPlayerClickable
    {
        private EKomaKind kind;
    }
    
    public class PlayerCommander : MonoBehaviour
    {
        [SerializeField] private BoardManager boardManagerRef;
        [SerializeField] private Material matTransparentBlue;
        [SerializeField] private Material matTransparentBlack;
        [SerializeField] private BattleCanvas battleCanvas;
        
        private BoardMap boardMapRef => boardManagerRef.BoardMap;
        
        private const int leftButtonId = 0;
        private IPlayerClickable? _clickingKoma = null;
        private BoardPiece? _destPiece = null;
        private KomaUnit? _destKomaGhost = null;
        private CommanderAction _myAction;
        
        private Camera mainCamera => Camera.main;

        [EventFunction]
        private void Awake()
        {
            _myAction = new CommanderAction(boardManagerRef);
        }

        [EventFunction]
        private void Start()
        {
            controlProcessAsync().Forget();
        }

        private async UniTask controlProcessAsync()
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
            while (!isClicking())
            {
                // 左クリックを押して選択するまで
                checkDownLeftMouse();
                if (await checkStopProcess()) return new PlayerCooldownTime(0);
            }

            while (!Input.GetMouseButtonUp(leftButtonId))
            {
                // 左クリックを押してるとき
                updateWhileDownLeftMouse();
                if (await checkStopProcess()) return new PlayerCooldownTime(0);
            }

            // 左クリックを離した
            return await performOnUpLeftMouse();
        }

        private async UniTask<bool> checkStopProcess()
        {
            await UniTask.DelayFrame(1);
            return false;
        }

        private void checkDownLeftMouse()
        {
            if (!Input.GetMouseButtonDown(leftButtonId)) return;

            // 盤上のクリックした駒を取得
            _clickingKoma = findKomaOnPieceRayedByMousePos();

            if (_clickingKoma == null) return;
            
            switch (_clickingKoma)
            {
                case KomaUnit fieldKoma:
                    _myAction.HighlightMovableList(fieldKoma);
                    animPieceSelected(fieldKoma).Forget();
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

        private KomaUnit? findKomaOnPieceRayedByMousePos()
        {
            var piece = findPieceRayedByMousePos();
            return piece == null ? null : piece.Holding;
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
            _destKomaGhost = _clickingKoma switch
            {
                KomaUnit fieldKoma => Instantiate(fieldKoma, transform),
                _ => throw new NotImplementedException()
            };

            var mesh = _destKomaGhost.GetComponentInChildren<MeshRenderer>();
            mesh.materials = mesh.sharedMaterials
                .Select(material => (material.name == "body") ? matTransparentBlue : matTransparentBlack)
                .ToArray();
            _destKomaGhost.gameObject.SetActive(false);
        }

        private void updateWhileDownLeftMouse()
        {
            var piece = findPieceRayedByMousePos();
            _destPiece = piece;
            
            bool canPut = piece != null && piece.IsActiveHighlight(); 
            _destKomaGhost.gameObject.SetActive(canPut);
            if (!canPut) return;

            var destPos = piece.transform.position;
            destPos.y = BoardPiece.KomaPosY;
            _destKomaGhost.transform.position = destPos;
        }

        private async UniTask<PlayerCooldownTime> performOnUpLeftMouse()
        {
            // 駒を盤上で目的地に移動させる
            var cooldown = _clickingKoma switch
            {
                KomaUnit fieldKoma => 
                    await _myAction.PerformMoveClickingKomaToDestination(fieldKoma, _destPiece, battleCanvas.CooldownBar),
                _ => throw new NotImplementedException()
            };

            // 進める場所や置ける場所ハイライト解除
            boardMapRef.ForEach(piece => piece.EnableHighlight(false));
            
            // クリック中の駒を解除
            _clickingKoma = null;
            
            // ゴースト削除
            Util.DestroyGameObject(_destKomaGhost.gameObject);
            _destKomaGhost = null;
            
            // クールダウンバーの値を0まで減らす演出
            if (cooldown.Seconds > 0) await CooldownAnimation.GoDownToZeroCooldown(battleCanvas.CooldownBar, cooldown.Seconds);
            
            return cooldown;
        }

        private bool isClicking()
        {
            return _clickingKoma != null;
        }
    }
}