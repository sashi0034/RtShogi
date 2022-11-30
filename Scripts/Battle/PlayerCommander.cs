#nullable enable

using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using RtShogi.Scripts.Battle.UI;
using UnityEngine;

namespace RtShogi.Scripts.Battle
{
    record PlayerCooldownTime(float Seconds);
    
    public class PlayerCommander : MonoBehaviour
    {
        [SerializeField] private BoardManager boardManagerRef;
        [SerializeField] private Material matTransparentBlue;
        [SerializeField] private Material matTransparentBlack;
        [SerializeField] private CooldownBar cooldownBar;
        
        private BoardMap boardMapRef => boardManagerRef.BoardMap;
        
        private const int leftButtonId = 0;
        private KomaUnit? _clickingKoma = null;
        private BoardPiece? _destPiece = null;
        private KomaUnit? _destKomaGhost = null;
        
        private Camera mainCamera => Camera.main;

        [EventFunction]
        private void Start()
        {
            controlProcessAsync().Forget();
        }

        [EventFunction]
        private void Update()
        {

        }

        private async UniTask controlProcessAsync()
        {
            while (true)
            {
                var cooldown = await processUpAndDownLeftClick();
                if (await checkStopProcess()) return;
                
                if (cooldown.Seconds > 0) await PlayerCooldown.StartCooldown(cooldownBar, cooldown.Seconds);
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
            return onUpLeftMouse();
        }

        private async UniTask<bool> checkStopProcess()
        {
            await UniTask.DelayFrame(1);
            return false;
        }

        private void checkDownLeftMouse()
        {
            if (!Input.GetMouseButtonDown(leftButtonId)) return;

            var piece = findPieceRayedByMousePos();

            if (piece == null) return;

            _clickingKoma = piece.Holding;

            if (_clickingKoma == null) return;

            hilightMovableList(piece);

            createDestKomaGohst();

            animPieceSelected(piece).Forget();
        }

        private static async UniTask animPieceSelected(BoardPiece piece)
        {
            var holding = piece.Holding;
            if (holding==null) return;
            await holding.gameObject.transform.DOScale(1.3f, 0.3f).SetEase(Ease.InOutBack);
            await holding.gameObject.transform.DOScale(1.0f, 0.3f).SetEase(Ease.OutBack);
        }

        private BoardPiece? findPieceRayedByMousePos()
        {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out var hit)) return null;

            GameObject? clickedGameObject = hit.collider.gameObject;

            if (clickedGameObject == null) return null;

            return !clickedGameObject.TryGetComponent<BoardPiece>(out var piece) ? null : piece;
        }

        private void hilightMovableList(BoardPiece piece)
        {
            var movableList = new KomaMovableRoute(
                    _clickingKoma.Kind,
                    ImBoardPoint.FromReal(piece.Point, true),
                    (point) =>
                        boardMapRef.IsInMapRange(point.ToReal(true)) &&
                        boardMapRef.TakePiece(point.ToReal(true)).Holding == null
                )
                .GetMovablePoints()
                .Select(p => boardMapRef.TakePiece(p)).ToList();

            foreach (var movable in movableList)
            {
                movable.EnableHighlight(true);
            }
        }

        private void createDestKomaGohst()
        {
            _destKomaGhost = Instantiate(_clickingKoma, transform);

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

        private PlayerCooldownTime onUpLeftMouse()
        {
            // 駒を盤上で目的地に移動させる
            var cooldown = moveClickingKomaToDest(_clickingKoma, _destPiece);

            // 進める場所のハイライト解除
            boardMapRef.ForEach(piece => piece.EnableHighlight(false));
            
            // クリック中の駒を解除
            _clickingKoma = null;
            
            // ゴースト削除
            Util.DestroyGameObject(_destKomaGhost.gameObject);
            _destKomaGhost = null;
            
            return cooldown;
        }

        private static PlayerCooldownTime moveClickingKomaToDest(KomaUnit? clickingKoma, BoardPiece? destPiece)
        {
            if (clickingKoma==null || destPiece==null) return new PlayerCooldownTime(0);
            if (!destPiece.IsActiveHighlight()) return new PlayerCooldownTime(0);

            var canForm = KomaFormingChecker.CheckFormAble(
                    clickingKoma.Kind,
                    new ImBoardPoint(clickingKoma.MountedPiece.Point), 
                    new ImBoardPoint(destPiece.Point));
            
            if (canForm==EKomaFormAble.FormForced) clickingKoma.FormSelf();
            
            clickingKoma.MountedPiece.RemoveKoma();
            destPiece.PutKoma(clickingKoma);
            //clickingKoma.transform.position = destPiece.GetKomaPos();
            clickingKoma.transform.DOMove(destPiece.GetKomaPos(), 0.3f).SetEase(Ease.OutQuart);

            const float delay = 1.5f;
            return new PlayerCooldownTime(delay);
        }

        private bool isClicking()
        {
            return _clickingKoma != null;
        }
    }
}