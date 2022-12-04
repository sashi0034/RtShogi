﻿#nullable enable

using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace RtShogi.Scripts.Battle.Player
{
    public record CommanderAction(
        BoardManager BoardManagerRef,
        BattleCanvas BattleCanvas)
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
            
            // 盤上処理
            clickingKoma.MountedPiece.RemoveKoma();
            destPiece.PutKoma(clickingKoma);
            //clickingKoma.transform.position = destPiece.GetKomaPos();
            clickingKoma.transform.DOMove(destPiece.GetKomaPos(), 0.3f).SetEase(Ease.OutQuart);

            return new PlayerCooldownTime(delay);
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
            komaManager.PutNewKoma(destPiece.Point, dragging.Kind, ETeam.Ally);

            return new PlayerCooldownTime(delay);
        }
    }
}