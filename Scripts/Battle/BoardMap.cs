using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace RtShogi.Scripts.Battle
{
    public class BoardMap : MonoBehaviour
    {
        public const int SideLength = 9;
        public static IntSize BoardSize = new IntSize(SideLength, SideLength);
        [SerializeField] private BoardPiece boardPiecePrefab;
        [ReadOnly] [SerializeField] private BoardPiece[] boardPieces; 


        private void clearPieces()
        {
            foreach (var piece in gameObject.transform.GetChildren())
            {
                Util.DestroyGameObjectInEditor(piece.gameObject);
            }
        }

        public bool IsInMapRange(BoardPoint point)
        {
            return
                new IntRange(0, BoardManager.BoardSize.W - 1).IsInRange(point.X) &&
                new IntRange(0, BoardManager.BoardSize.H - 1).IsInRange(point.Z);
        }

        public void ForEach(Action<BoardPiece> action)
        {
            foreach (var piece in boardPieces)
            {
                action(piece);
            }
        }
        
        public BoardPiece TakePiece(BoardPoint point)
        {
            return boardPieces[point.Z + point.X * BoardManager.BoardSize.W];
        }
        public BoardPiece TakePiece(int x, int z)
        {
            return TakePiece(new BoardPoint(x, z));
        }
        

        [Button]
        public void ResetBoard()
        {
            clearPieces();
            var size = BoardManager.BoardSize;
            var created = new List<BoardPiece>() { };
            for (int x = 0; x < size.W; ++x)
            {
                for (int z = 0; z < size.H; ++z)
                {
                    var piece = PrefabUtility.InstantiatePrefab(boardPiecePrefab, transform) as BoardPiece;
                    piece.transform.position = new Vector3(x-size.W / 2, 0, z-size.H / 2);
                    piece.gameObject.name = $"Piece({x}, {z})";
                    piece.Initialize(new Vector2Int(x, z));
                    created.Add(piece);
                }
            }
            boardPieces = created.ToArray();
            Debug.Log($"map children: {gameObject.transform.childCount}");
        }

    }
}