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
                putKoma(new BoardPoint(x, 2), EKomaKind.Hu, EKomaTeam.Ally);
            
            putKoma(new BoardPoint(1, 1), EKomaKind.Kaku, EKomaTeam.Ally);
            putKoma(new BoardPoint(7, 1), EKomaKind.Hisha, EKomaTeam.Ally);
            
            putKoma(new BoardPoint(0, 0), EKomaKind.Kyosha, EKomaTeam.Ally);
            putKoma(new BoardPoint(1, 0), EKomaKind.Keima, EKomaTeam.Ally);
            putKoma(new BoardPoint(2, 0), EKomaKind.Gin, EKomaTeam.Ally);
            putKoma(new BoardPoint(3, 0), EKomaKind.Kin, EKomaTeam.Ally);
            putKoma(new BoardPoint(4, 0), EKomaKind.Oh, EKomaTeam.Ally);
            putKoma(new BoardPoint(5, 0), EKomaKind.Kin, EKomaTeam.Ally);
            putKoma(new BoardPoint(6, 0), EKomaKind.Gin, EKomaTeam.Ally);
            putKoma(new BoardPoint(7, 0), EKomaKind.Keima, EKomaTeam.Ally);
            putKoma(new BoardPoint(8, 0), EKomaKind.Kyosha, EKomaTeam.Ally);
        }
        

        void putKoma(BoardPoint point, EKomaKind kind, EKomaTeam team)
        {
            var koma = Instantiate(komaUnitPrefab, transform);
            boardMapRef.TakePiece(point).PutKoma(koma);
            koma.InitProps(komaViewPropsList[(int)kind], team);
        }
        
        
        
         
    }
}