#nullable enable
using System.Linq;
using UnityEngine;

namespace RtShogi.Scripts.Battle
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private BoardManager boardManagerRef;
        private BoardMap boardMapRef => boardManagerRef.BoardMap;
        
        private const int leftButtonId = 0;
        private KomaUnit? _clickingKoma = null;
        
        [EventFunction]
        private void Update()
        {
            if (!isClicking())
                checkDownLeftMouse();
            else 
                checkUpLeftMouse();
        }

        private void checkDownLeftMouse()
        {
            if (!Input.GetMouseButtonDown(leftButtonId)) return;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out var hit)) return;

            GameObject? clickedGameObject = hit.collider.gameObject;

            if (clickedGameObject == null) return;

            if (!clickedGameObject.TryGetComponent<BoardPiece>(out var piece)) return;
            _clickingKoma = piece.Holding;

            if (_clickingKoma == null) return;
            
            var movableList = new KomaMovableRoute(
                _clickingKoma.Kind,
                piece.Point,
                (point) =>
                    boardMapRef.IsInMapRange(point) &&
                    boardMapRef.TakePiece(point).Holding == null
            )
                .GetMovablePoints()
                .Select(p => boardMapRef.TakePiece(p)).ToList();

            foreach (var movable in movableList)
            {
                movable.EnableHighlight(true);
            }
        }

        private void checkUpLeftMouse()
        {
            if (!Input.GetMouseButtonUp(leftButtonId)) return;
            // 左クリックを離した
            
            boardMapRef.ForEach(piece => piece.EnableHighlight(false));

            _clickingKoma = null;
        }

        private bool isClicking()
        {
            return _clickingKoma != null;
        }
    }
}