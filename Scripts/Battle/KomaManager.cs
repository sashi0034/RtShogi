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
                putKoma(new BoardPoint(x, 2), EKomaKind.Hu, ETeam.Ally);
            
            putKoma(new BoardPoint(1, 1), EKomaKind.Kaku, ETeam.Ally);
            putKoma(new BoardPoint(7, 1), EKomaKind.Hisha, ETeam.Ally);
            
            putKoma(new BoardPoint(0, 0), EKomaKind.Kyosha, ETeam.Ally);
            putKoma(new BoardPoint(1, 0), EKomaKind.Keima, ETeam.Ally);
            putKoma(new BoardPoint(2, 0), EKomaKind.Gin, ETeam.Ally);
            putKoma(new BoardPoint(3, 0), EKomaKind.Kin, ETeam.Ally);
            putKoma(new BoardPoint(4, 0), EKomaKind.Oh, ETeam.Ally);
            putKoma(new BoardPoint(5, 0), EKomaKind.Kin, ETeam.Ally);
            putKoma(new BoardPoint(6, 0), EKomaKind.Gin, ETeam.Ally);
            putKoma(new BoardPoint(7, 0), EKomaKind.Keima, ETeam.Ally);
            putKoma(new BoardPoint(8, 0), EKomaKind.Kyosha, ETeam.Ally);
        }
        

        void putKoma(BoardPoint point, EKomaKind kind, ETeam team)
        {
            var koma = Instantiate(komaUnitPrefab, transform);
            var boardPiece = boardMapRef.TakePiece(point);
            boardPiece.PutKoma(koma);
            koma.transform.position = boardPiece.GetKomaPos();
            koma.InitProps(komaViewPropsList[(int)kind], team);
        }
        
        
        
         
    }
}