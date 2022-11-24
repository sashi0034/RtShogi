using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RtShogi.Scripts
{
    public class BoardMap : MonoBehaviour
    {
        [SerializeField] private BoardPiece boardPiecePrefab;
        [ReadOnly] [SerializeField] private BoardPiece[] boardPieces; 


        private void clearPieces()
        {
            foreach (var piece in gameObject.transform.GetChildren())
            {
                Util.DestroyGameObjectInEditor(piece.gameObject);
            }
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
                    var piece = GameObject.Instantiate(boardPiecePrefab, transform);
                    piece.transform.position = new Vector3(x-size.W / 2, 0, z-size.H / 2);
                    piece.gameObject.name = $"Piece({x}, {z})";
                    created.Add(piece);
                }
            }
            boardPieces = created.ToArray();
            Debug.Log($"map children: {gameObject.transform.childCount}");
        }

    }
}