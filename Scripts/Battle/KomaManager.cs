using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Resolvers;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Photon.Pun.UtilityScripts;
using RtShogi.Scripts.Battle;
using RtShogi.Scripts.Battle.UI;
using Sirenix.Utilities;
using UnityEngine;

namespace RtShogi.Scripts.Battle
{
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
        }

        [EventFunction]
        private void Start()
        {
            ResetBeforeBattle();
        }

        public void ResetBeforeBattle()
        {
            _createdLocalCounter.Reset();
            _boardKomaList.Clear();
        }

        public void InitAllKomaOnBoard()
        {
            foreach(int x in Enumerable.Range(0, 9))
                CreateAndInstallKoma(new BoardPoint(x, 2), EKomaKind.Hu, ETeam.Ally);
            
            CreateAndInstallKoma(new BoardPoint(1, 1), EKomaKind.Kaku, ETeam.Ally);
            CreateAndInstallKoma(new BoardPoint(7, 1), EKomaKind.Hisha, ETeam.Ally);
            
            CreateAndInstallKoma(new BoardPoint(0, 0), EKomaKind.Kyosha, ETeam.Ally);
            CreateAndInstallKoma(new BoardPoint(1, 0), EKomaKind.Keima, ETeam.Ally);
            CreateAndInstallKoma(new BoardPoint(2, 0), EKomaKind.Gin, ETeam.Ally);
            CreateAndInstallKoma(new BoardPoint(3, 0), EKomaKind.Kin, ETeam.Ally);
            CreateAndInstallKoma(new BoardPoint(4, 0), EKomaKind.Oh, ETeam.Ally);
            CreateAndInstallKoma(new BoardPoint(5, 0), EKomaKind.Kin, ETeam.Ally);
            CreateAndInstallKoma(new BoardPoint(6, 0), EKomaKind.Gin, ETeam.Ally);
            CreateAndInstallKoma(new BoardPoint(7, 0), EKomaKind.Keima, ETeam.Ally);
            CreateAndInstallKoma(new BoardPoint(8, 0), EKomaKind.Kyosha, ETeam.Ally);
        }
        
        

        [UsingBattleRpcaller]
        public void CreateAndInstallKoma(BoardPoint point, EKomaKind kind, ETeam team)
        {
            _createdLocalCounter.CountNext();
            var id = KomaId.PublishId(_createdLocalCounter.Value, battleRpcaller.IsRoomHost());
            battleRpcaller.RpcallPutNewKoma(new KomaPutInfo(point, kind, team, id));
        }

        [FromBattleRpcaller]
        public void PutNewKoma(KomaPutInfo putInfo)
        {
            var koma = createKomaInternal(putInfo.Kind, putInfo.Team, putInfo.Id);
            _boardKomaList.AddUnit(koma);

            var boardPiece = boardMapRef.TakePiece(putInfo.Point);
            boardPiece.PutKoma(koma);
            koma.transform.position = boardPiece.GetKomaPos();
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
                
            var viewForIcon = GetViewProps(koma.Kind);
            
            switch (koma.Team)
            {
            case ETeam.Ally:
                // TODO
                Debug.Log("TODO");
                break;
            case ETeam.Enemy:
                // ローカルプレイヤーが獲得
                battleCanvas.ObtainedKomaGroup.IncElement(new ObtainedKomaElementProps(
                    koma.Kind,
                    viewForIcon.SprIcon));
                break;
            default:
                throw new ArgumentOutOfRangeException();
            }

            await koma.transform.DOScale(0, 0.5f).SetEase(Ease.InOutBack);
            Util.DestroyGameObject(koma.gameObject);
        }
         
    }
}