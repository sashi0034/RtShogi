#nullable enable

using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace RtShogi.Scripts.Battle.Player
{
    public record CommanderAction(
        BoardManager BoardManagerRef)
    {
        private BoardMap boardMapRef => BoardManagerRef.BoardMap;
        
        /// <summary>
        /// 選択中の駒をhighlight表示
        /// </summary>
        public void HighlightMovableList(KomaUnit clickingKoma)
        {
            var movableList = new KomaMovableRoute(
                    clickingKoma.Kind,
                    ImBoardPoint.FromReal(clickingKoma.MountedPiece.Point, true),
                    (point) =>
                        boardMapRef.IsInMapRange(point.ToReal(true)) &&
                        boardMapRef.TakePiece(point.ToReal(true)).Holding == null
                )
                .GetMovablePoints()
                .Select(p => boardMapRef.TakePiece(p.Raw)).ToList();

            hilightPieceList(movableList);
        }

        private static void hilightPieceList(List<BoardPiece> list)
        {
            foreach (var movable in list)
            {
                movable.EnableHighlight(true);
            }
        }

        public void HighlightInstallableList(EKomaKind kind)
        {
            var movableList = new KomaInstallablePoints(
                    kind,
                    (point) =>
                    {
                        var holding = boardMapRef.TakePiece(point.Raw).Holding;
                        return holding != null ? holding.Kind : null;
                    })
                .GetInstallablePoints()
                .Select(p => boardMapRef.TakePiece(p.Raw)).ToList();

            hilightPieceList(movableList);
        }
        
        /// <summary>
        /// クールダウンバーを出して駒を目的地までアニメーション 
        /// </summary>
        public async UniTask<PlayerCooldownTime> PerformMoveClickingKomaToDestination(
            KomaUnit? clickingKoma, 
            BoardPiece? destPiece,
            UI.CooldownBar cooldownBar)
        {
            if (clickingKoma==null || destPiece==null) return new PlayerCooldownTime(0);
            if (!destPiece.IsActiveHighlight()) return new PlayerCooldownTime(0);
            
            const float delay = 1.5f;
            
            // クールダウンバーを画面に出す
            await CooldownAnimation.ShowCooldown(cooldownBar, delay);
            
            // 裏返す
            var canForm = KomaFormingChecker.CheckFormAble(
                clickingKoma.Kind,
                new ImBoardPoint(clickingKoma.MountedPiece.Point), 
                new ImBoardPoint(destPiece.Point));
            if (canForm==EKomaFormAble.FormForced) clickingKoma.FormSelf();
            
            clickingKoma.MountedPiece.RemoveKoma();
            destPiece.PutKoma(clickingKoma);
            //clickingKoma.transform.position = destPiece.GetKomaPos();
            clickingKoma.transform.DOMove(destPiece.GetKomaPos(), 0.3f).SetEase(Ease.OutQuart);

            return new PlayerCooldownTime(delay);
        }
    }
}