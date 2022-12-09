#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using RtShogi.Scripts.Battle.Param;
using RtShogi.Scripts.Battle.UI;
using UnityEngine;

namespace RtShogi.Scripts.Battle.Player
{
    public record CommanderAction(
        KomaManager KomaManagerRef,
        BoardManager BoardManagerRef,
        BattleCanvas BattleCanvas,
        BattleRpcaller Rpcaller)
    {
        private BoardMap boardMapRef => BoardManagerRef.BoardMap;


        /// <summary>
        /// 獲得した駒を移動させているときにカーソルを移動
        /// </summary>
        /// <param name="dragging"></param>
        public void MoveObtainedKomaHoverCursor(PlayerDraggingObtainedKoma dragging, bool canPutMousePosNow)
        {
            var canvasRect = BattleCanvas.RectTransform;
            
            dragging.HoverCursor.gameObject.SetActive(!canPutMousePosNow);
                
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, 
                Input.mousePosition, 
                BattleCanvas.ParentCanvas.worldCamera, 
                out var localPoint);
            dragging.HoverCursor.anchoredPosition = localPoint;
        }
        
        /// <summary>
        /// 選択中の駒をhighlight表示
        /// </summary>
        public void HighlightMovableList(KomaUnit clickingKoma)
        {
            var movableList = new KomaMovableRoute(
                    clickingKoma.Kind,
                    ImBoardPoint.FromReal(clickingKoma.MountedPiece.Point, true),
                    ETeam.Enemy,
                    (point) => boardMapRef.TakePieceNullable(point.ToReal(true))?.Holding
                )
                .GetMovablePoints()
                .Select(p => boardMapRef.TakePiece(p.Raw)).ToList();

            hilightPieceList(movableList);
        }

        private bool canMoveAllyUnit(ImBoardPoint point)
        {
            var koma = boardMapRef.TakePiece(point.ToReal(true)).Holding;
            return koma==null || koma.Team==ETeam.Enemy;
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
                    ETeam.Ally,
                    (point) => boardMapRef.TakePiece(point.Raw).Holding)
                .GetInstallablePoints()
                .Select(p => boardMapRef.TakePiece(p.Raw)).ToList();

            hilightPieceList(movableList);
        }
        
        /// <summary>
        /// クールダウンバーを出して駒を目的地までアニメーション 
        /// </summary>
        [UsingBattleRpcaller]
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
            var formAble = KomaFormingChecker.CheckFormAble(
                clickingKoma.Kind,
                new ImBoardPoint(clickingKoma.MountedPiece.Point), 
                new ImBoardPoint(destPiece.Point));
            operateFormAbleKomaAfterMove(clickingKoma, formAble);

            // 盤上処理
            bool isKill = destPiece.Holding != null;
            if (isKill) Rpcaller.RpcallSendToObtainedKoma(destPiece.Holding);
            Rpcaller.RpcallMoveKomaOnBoard(clickingKoma, destPiece);

            return new PlayerCooldownTime(delay);
        }

        private void operateFormAbleKomaAfterMove(KomaUnit clickingKoma, EKomaFormAble formAble)
        {
            switch (formAble)
            {
            case EKomaFormAble.FormForced:
                clickingKoma.FormSelf();
                break;
            case EKomaFormAble.FormAble:
                BattleCanvas.ButtonBecomeFormed
                    .EnableFormAbleKoma(clickingKoma, ConstParameter.Instance.KomaFormAbleEffectiveTime)
                    .Forget();
                break;
            case EKomaFormAble.Impossible:
                break;
            default:
                throw new ArgumentOutOfRangeException();
            }
        }

        [FromBattleRpcaller]
        public static void MoveKomaOnBoard(KomaUnit movingKoma, BoardPiece destPiece)
        {
            movingKoma.MountedPiece.RemoveKoma();
            destPiece.PutKoma(movingKoma);
            movingKoma.transform.DOMove(destPiece.GetKomaPos(), 0.3f).SetEase(Ease.OutQuart);
        }

        /// <summary>
        /// クールダウンバーを出して駒を設置する
        /// </summary>
        public async UniTask<PlayerCooldownTime> PerformInstallObtainedKomaToDestination(
            PlayerDraggingObtainedKoma? dragging, 
            BoardPiece? destPiece,
            UI.CooldownBar cooldownBar,
            KomaManager komaManager)
        {
            if (dragging==null || destPiece==null) return new PlayerCooldownTime(0);
            if (!destPiece.IsActiveHighlight()) return new PlayerCooldownTime(0);
            
            const float delay = 2.0f;
            
            // クールダウンバーを画面に出す
            await CooldownAnimation.ShowCooldown(cooldownBar, delay);
            
            // 盤上処理
            komaManager.ConsumeObtainedAndInstallKoma(destPiece.Point, dragging.Kind, ETeam.Ally);

            return new PlayerCooldownTime(delay);
        }
    }
}