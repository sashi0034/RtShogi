#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using RtShogi.Scripts.Battle.UI;
using RtShogi.Scripts.Param;
using UnityEngine;

namespace RtShogi.Scripts.Battle.Player
{
    public record CommanderAction(
        KomaManager KomaManagerRef,
        BoardManager BoardManagerRef,
        BattleCanvas BattleCanvas,
        BattleRpcaller Rpcaller,
        EffectManager EffectManager)
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

            foreach (var movable in movableList) 
                movable.EnableHighlight( new BoardPieceHighlightIntensity(0));
        }

        private bool canMoveAllyUnit(ImBoardPoint point)
        {
            var koma = boardMapRef.TakePiece(point.ToReal(true)).Holding;
            return koma==null || koma.Team==ETeam.Enemy;
        }

        public void HighlightInstallableList(EKomaKind kind)
        {
            var movableList = new KomaInstallablePoints(
                    kind,
                    ETeam.Ally,
                    (point) => boardMapRef.TakePiece(point.Raw).Holding)
                .GetInstallablePoints()
                .Select(p => boardMapRef.TakePiece(p.Raw)).ToList();

            foreach (var movable in movableList) 
                movable.EnableHighlight(new BoardPieceHighlightIntensity(
                    isKillablePoint(kind, movable) ? 2 : 1));
        }

        private bool isKillablePoint(EKomaKind kind, BoardPiece piece)
        {
            return new KomaMovableRoute(
                    kind,
                    ImBoardPoint.FromReal(piece.Point, true),
                    ETeam.Enemy,
                    (point) => boardMapRef.TakePieceNullable(point.ToReal(true))?.Holding
                )
                .GetMovablePoints()
                .Any(p => boardMapRef.TakePiece(p.Raw).Holding != null);
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
            
            var highlightIntensity = destPiece.HighlightIntensity;
            if (!destPiece.IsActiveHighlight() || highlightIntensity == null) return new PlayerCooldownTime(0);
            var cooldownTime = highlightIntensity.GetCooldownTime();
            
            // クールダウンバーを画面に出す
            await CooldownAnimation.ShowCooldown(cooldownBar, cooldownTime.Seconds);
            
            // 裏返す
            var formAble = KomaFormingChecker.CheckFormAble(
                clickingKoma.Kind,
                new ImBoardPoint(clickingKoma.MountedPiece.Point), 
                new ImBoardPoint(destPiece.Point));
            operateFormAbleKomaAfterMove(clickingKoma, formAble);

            // 盤上処理
            bool isKill = destPiece.Holding != null;
            if (isKill) await performKill(destPiece.Holding);
            Rpcaller.RpcallMoveKomaOnBoard(clickingKoma, destPiece);

            return cooldownTime;
        }

        private async UniTask performKill(KomaUnit killedKoma)
        {
            var effect = EffectManager.Produce(EffectManager.EffectClashing);
            if (effect != null)
            {
                effect.transform.position = killedKoma.transform.position;
                await UniTask.Delay(0.2f.ToIntMilli());
            }
            
            Rpcaller.RpcallSendToObtainedKoma(killedKoma);
        }

        private void operateFormAbleKomaAfterMove(KomaUnit clickingKoma, EKomaFormAble formAble)
        {
            switch (formAble)
            {
            case EKomaFormAble.FormForced:
                Rpcaller.RpcallBecomeFormed(clickingKoma);
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

            var highlightIntensity = destPiece.HighlightIntensity;
            if (!destPiece.IsActiveHighlight() || highlightIntensity == null) return new PlayerCooldownTime(0);
            var cooldownTime = highlightIntensity.GetCooldownTime();

            // クールダウンバーを画面に出す
            await CooldownAnimation.ShowCooldown(cooldownBar, cooldownTime.Seconds);
            
            // 盤上処理
            komaManager.ConsumeObtainedAndInstallKoma(destPiece.Point, dragging.Kind, ETeam.Ally);

            return cooldownTime;
        }
    }
}