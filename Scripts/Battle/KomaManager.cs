using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Resolvers;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Photon.Pun.UtilityScripts;
using RtShogi.Scripts.Battle;
using RtShogi.Scripts.Battle.UI;
using RtShogi.Scripts.Param;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace RtShogi.Scripts.Battle
{
    public record KomaInitialPutInfo(BoardPoint Point, EKomaKind Kind) {}

    public interface IBattleStatus{};
    public class BattleStatusContinuing : IBattleStatus { }
    public record BattleStatusFinished(
        bool IsWin
        ) : IBattleStatus { }

    public class KomaManager : MonoBehaviour
    {
        [SerializeField] private KomaUnit komaUnitPrefab;
        [SerializeField] private KomaViewProps[] komaViewPropsList;
        [SerializeField] private BattleRoot battleRoot;
        
        private BoardManager boardManagerRef => battleRoot.BoardManager;
        private BattleRpcaller battleRpcaller => battleRoot.Rpcaller;
        private BattleCanvas battleCanvas => battleRoot.BattleCanvasRef;
        public BattleCanvas BattleCanvas => battleCanvas;
        
        private BoardMap boardMapRef => boardManagerRef.BoardMap;

        private readonly IntCounter _createdLocalCounter = new IntCounter();

        private readonly BoardKomaList _boardKomaList = new BoardKomaList();
        public IBoardKomaListGetter List => _boardKomaList;

        private IBattleStatus _status = new BattleStatusContinuing();
        public IBattleStatus BattleStatus => _status;
        

        [EventFunction]
        private void Awake()
        {
            Debug.Assert(komaViewPropsList.Length == KomaKind.NumUnformed);
            komaViewPropsList.Sort((a, b) => (int)a.Kind - (int)b.Kind);
            ResetBeforeBattle();
        }

        public void ResetBeforeBattle()
        {
            _createdLocalCounter.Reset();
            _boardKomaList.Clear();
            _status = new BattleStatusContinuing();
        }

        public void SetupAllAllyKomaOnBoard()
        {
            Logger.Print("setup all ally koma");
            
            foreach (var info in ConstParameter.KomaInitialPutInfos)
            {
                createAndInstallKoma(info.Point, info.Kind, ETeam.Ally);
            }
        }
        public void SetupAllEnemyKomaOnBoardAsDebug()
        {
#if UNITY_EDITOR
            Logger.Print("setup all enemy koma");
            
            foreach (var info in ConstParameter.KomaInitialPutInfos)
            {
                createAndInstallKoma(info.Point.ToReversed(), info.Kind, ETeam.Enemy);
            }
#else
            throw new NotImplementedException();
#endif
        }
        

        [UsingBattleRpcaller]
        private void createAndInstallKoma(BoardPoint point, EKomaKind kind, ETeam team)
        {
            var actualKind = kind != EKomaKind.Oh
                ? kind
                : battleRpcaller.IsRoomHost()
                    ? EKomaKind.Oh
                    : EKomaKind.Gyoku; 
            installKomaInternal(point, actualKind, team, false);
        }
        
        [UsingBattleRpcaller]
        public void ConsumeObtainedAndInstallKoma(BoardPoint point, EKomaKind kind, ETeam team)
        {
            installKomaInternal(point, kind, team, true);
        }

        [UsingBattleRpcaller]
        private void installKomaInternal(BoardPoint point, EKomaKind kind, ETeam team, bool isFromObtained)
        {
            _createdLocalCounter.CountNext();
            var id = KomaId.PublishId(_createdLocalCounter.Value, battleRpcaller.IsRoomHost());
            battleRpcaller.RpcallPutNewKoma(new KomaPutInfo(point, kind, team, id, isFromObtained));
        }

        [FromBattleRpcaller]
        public void PutNewKoma(KomaPutInfo putInfo)
        {
            var koma = createKomaInternal(putInfo.Kind, putInfo.Team, putInfo.Id);
            _boardKomaList.AddUnit(koma);
#if UNITY_EDITOR
            koma.gameObject.name = $"{putInfo.Kind} ({putInfo.Team})";
#endif

            var boardPiece = boardMapRef.TakePiece(putInfo.Point);
            boardPiece.PutKoma(koma);
            koma.transform.position = boardPiece.GetKomaPos();

            // 持ち駒の個数を減らす
            if (putInfo.IsFromObtainedKoma) battleCanvas.GetObtainedKomaGroup(koma.Team).FindAndDecElement(koma.Kind);

            // エフェクトもつける
            if (putInfo.IsFromObtainedKoma) effectBirth(koma).Forget();
        }

        private async UniTask effectBirth(KomaUnit koma)
        {
            var effect = battleRoot.EffectManager.Produce(battleRoot.EffectManager.EffectBirth);
            if (effect == null) return;

            koma.transform.localScale = Vector3.zero;

            // 魔法陣を出して
            await effect.AnimAppear(koma.transform.position);
            
            // 駒にアニメーションをかけて 
            koma.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
            const float moveY = 0.5f;
            koma.transform.DOMoveY(moveY, 0.5f).SetEase(Ease.OutBack).SetRelative(true);
            const int numRotate = 3;
            await koma.transform.DORotate(new Vector3(0, 360 * numRotate, 0), 0.3f * numRotate)
                .SetEase(Ease.InOutCubic).SetRelative(true);

            koma.transform.DOMoveY(-moveY, 0.3f).SetEase(Ease.InQuart).SetRelative(true);

            // 魔法陣を消す
            await effect.AnimDisappear();
        }

        public KomaUnit CreateVirtualKoma(EKomaKind kind, ETeam team)
        {
            return createKomaInternal(kind, team, KomaId.InvalidId);
        }

        private KomaUnit createKomaInternal(EKomaKind kind, ETeam team, KomaId id)
        {
            var koma = Instantiate(komaUnitPrefab, transform);
            koma.InitProps(battleRoot, komaViewPropsList[(int)kind], team, id);
            return koma;
        }

        public KomaViewProps GetViewProps(EKomaKind kind)
        {
            var result = komaViewPropsList[(int)kind];
            Debug.Assert(result.Kind == kind);
            return result;
        }
        
        
        [FromBattleRpcaller]
        public async UniTask SendToObtainedKoma(KomaUnit koma)
        {
            _boardKomaList.RemoveUnit(koma);

            if (new KomaKind(koma.Kind).IsKing())
                await performKilledKing(koma);
            else
                await sendToObtainedKomaInternal(koma);
        }

        private async UniTask sendToObtainedKomaInternal(KomaUnit koma)
        {
            await koma.AnimKilled();

            var unformedKind = new KomaKind(koma.Kind).IsNonFormed()
                ? koma.Kind
                : new KomaKind(koma.Kind).ToUnformed();
            
            Debug.Assert(unformedKind != null);
            if (unformedKind == null) return;

            var viewForIcon = GetViewProps(unformedKind.Value);

            var obtainedTeam = TeamUtil.FlipTeam(koma.Team);
            battleCanvas.GetObtainedKomaGroup(obtainedTeam).IncElement(new ObtainedKomaElementProps(
                unformedKind.Value,
                viewForIcon.SprIcon,
                obtainedTeam));
        }

        private async UniTask performKilledKing(KomaUnit kingUnit)
        {
            Logger.Print("killed king");
            
            kingUnit.AnimKilled().Forget();

            bool isWin = kingUnit.Team == ETeam.Enemy;

            _status = new BattleStatusFinished(isWin);

            await (isWin
                ? battleCanvas.MessageWinLose.PerformWin()
                : battleCanvas.MessageWinLose.PerformLose());
        }

#if UNITY_EDITOR
        // 駒に角度がついていたほうが見やすいかもと思い追加
        [Button]
        public void TestKomaViewAngle(float rotX)
        {
            foreach (var unit in _boardKomaList.RawList)
            {
                unit.transform.rotation = Quaternion.Euler(
                    unit.transform.rotation.eulerAngles
                        .FixX(rotX)); 
            }
        }
#endif
    }
}