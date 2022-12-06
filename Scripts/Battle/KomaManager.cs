using System;
using System.Collections.Generic;
using System.Linq;
using RtShogi.Scripts.Battle;
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
        
        private BoardMap boardMapRef => boardManagerRef.BoardMap;

        private IntCounter _createdLocalCounter = new IntCounter();

        [EventFunction]
        private void Awake()
        {
            Debug.Assert(komaViewPropsList.Length == KomaKind.NumUnformed);
            komaViewPropsList.Sort((a, b) => (int)a.Kind - (int)b.Kind);
        }

        [EventFunction]
        private void Start()
        {
            initAllKomaOnBoard();
        }

        private void initAllKomaOnBoard()
        {
            _createdLocalCounter.Reset();
            
            const int startZ = 6;

            foreach(int x in Enumerable.Range(0, 9))
                PutNewKoma(new BoardPoint(x, 2), EKomaKind.Hu, ETeam.Ally);
            
            PutNewKoma(new BoardPoint(1, 1), EKomaKind.Kaku, ETeam.Ally);
            PutNewKoma(new BoardPoint(7, 1), EKomaKind.Hisha, ETeam.Ally);
            
            PutNewKoma(new BoardPoint(0, 0), EKomaKind.Kyosha, ETeam.Ally);
            PutNewKoma(new BoardPoint(1, 0), EKomaKind.Keima, ETeam.Ally);
            PutNewKoma(new BoardPoint(2, 0), EKomaKind.Gin, ETeam.Ally);
            PutNewKoma(new BoardPoint(3, 0), EKomaKind.Kin, ETeam.Ally);
            PutNewKoma(new BoardPoint(4, 0), EKomaKind.Oh, ETeam.Ally);
            PutNewKoma(new BoardPoint(5, 0), EKomaKind.Kin, ETeam.Ally);
            PutNewKoma(new BoardPoint(6, 0), EKomaKind.Gin, ETeam.Ally);
            PutNewKoma(new BoardPoint(7, 0), EKomaKind.Keima, ETeam.Ally);
            PutNewKoma(new BoardPoint(8, 0), EKomaKind.Kyosha, ETeam.Ally);
        }
        

        public void PutNewKoma(BoardPoint point, EKomaKind kind, ETeam team)
        {
            _createdLocalCounter.CountNext();
            var id = KomaId.PublishId(_createdLocalCounter.Value, battleRpcaller.IsRoomHost());
            var koma = createKomaInternal(kind, team, id);
            var boardPiece = boardMapRef.TakePiece(point);
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
            koma.InitProps(komaViewPropsList[(int)kind], team, id);
            return koma;
        }

        public KomaViewProps GetViewProps(EKomaKind kind)
        {
            var result = komaViewPropsList[(int)kind];
            Debug.Assert(result.Kind == kind);
            return result;
        }
        
        
         
    }
}