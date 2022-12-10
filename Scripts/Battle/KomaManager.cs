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
using Sirenix.Utilities;
using UnityEngine;

namespace RtShogi.Scripts.Battle
{
    public record KomaInitialPutInfo(BoardPoint Point, EKomaKind Kind) {}
    
    public class KomaManager : MonoBehaviour
    {
        [SerializeField] private KomaUnit komaUnitPrefab;
        [SerializeField] private KomaViewProps[] komaViewPropsList;
        [SerializeField] private BoardManager boardManagerRef;
        [SerializeField] private BattleRpcaller battleRpcaller;
        
        [SerializeField] private BattleCanvas battleCanvas;
        public BattleCanvas BattleCanvas => battleCanvas;
        
        private BoardMap boardMapRef => boardManagerRef.BoardMap;

        private readonly IntCounter _createdLocalCounter = new IntCounter();

        private readonly BoardKomaList _boardKomaList = new BoardKomaList();
        public IBoardKomaListGetter List => _boardKomaList;
        

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
            installKomaInternal(point, kind, team, false);
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

            var boardPiece = boardMapRef.TakePiece(putInfo.Point);
            boardPiece.PutKoma(koma);
            koma.transform.position = boardPiece.GetKomaPos();

            // 持ち駒の個数を減らす
            if (putInfo.IsFromObtainedKoma) battleCanvas.GetObtainedKomaGroup(koma.Team).FindAndDecElement(koma.Kind);
        }

        public KomaUnit CreateVirtualKoma(EKomaKind kind, ETeam team)
        {
            return createKomaInternal(kind, team, KomaId.InvalidId);
        }

        private KomaUnit createKomaInternal(EKomaKind kind, ETeam team, KomaId id)
        {
            var koma = Instantiate(komaUnitPrefab, transform);
            koma.InitProps(this, komaViewPropsList[(int)kind], team, id);
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

            await koma.AnimKilled();

            var unformedKind = new KomaKind(koma.Kind).IsUnformed()
                ? koma.Kind
                : new KomaKind(koma.Kind).ToUnformed();
            if (unformedKind==null) return;
            
            var viewForIcon = GetViewProps(unformedKind.Value);

            var obtainedTeam = TeamUtil.FlipTeam(koma.Team);
            battleCanvas.GetObtainedKomaGroup(obtainedTeam).IncElement(new ObtainedKomaElementProps(
                unformedKind.Value,
                viewForIcon.SprIcon,
                obtainedTeam));
            

        }
        
    }
}