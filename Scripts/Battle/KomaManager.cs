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
        private BoardMap boardMapRef => boardManagerRef.BoardMap;

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
            const int startZ = 6;

            foreach(int x in Enumerable.Range(0, 9))
                PutKoma(new BoardPoint(x, 2), EKomaKind.Hu, ETeam.Ally);
            
            PutKoma(new BoardPoint(1, 1), EKomaKind.Kaku, ETeam.Ally);
            PutKoma(new BoardPoint(7, 1), EKomaKind.Hisha, ETeam.Ally);
            
            PutKoma(new BoardPoint(0, 0), EKomaKind.Kyosha, ETeam.Ally);
            PutKoma(new BoardPoint(1, 0), EKomaKind.Keima, ETeam.Ally);
            PutKoma(new BoardPoint(2, 0), EKomaKind.Gin, ETeam.Ally);
            PutKoma(new BoardPoint(3, 0), EKomaKind.Kin, ETeam.Ally);
            PutKoma(new BoardPoint(4, 0), EKomaKind.Oh, ETeam.Ally);
            PutKoma(new BoardPoint(5, 0), EKomaKind.Kin, ETeam.Ally);
            PutKoma(new BoardPoint(6, 0), EKomaKind.Gin, ETeam.Ally);
            PutKoma(new BoardPoint(7, 0), EKomaKind.Keima, ETeam.Ally);
            PutKoma(new BoardPoint(8, 0), EKomaKind.Kyosha, ETeam.Ally);
        }
        

        public void PutKoma(BoardPoint point, EKomaKind kind, ETeam team)
        {
            var koma = Instantiate(komaUnitPrefab, transform);
            var boardPiece = boardMapRef.TakePiece(point);
            boardPiece.PutKoma(koma);
            koma.transform.position = boardPiece.GetKomaPos();
            koma.InitProps(komaViewPropsList[(int)kind], team);
        }

        public KomaViewProps GetViewProps(EKomaKind kind)
        {
            var result = komaViewPropsList[(int)kind];
            Debug.Assert(result.Kind == kind);
            return result;
        }
        
        
         
    }
}