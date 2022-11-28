using UnityEngine;

namespace RtShogi.Scripts.Board
{
    public record IntSize(int W, int H){}
    
    public class BoardManager : MonoBehaviour
    {
        public static IntSize BoardSize = new IntSize(9, 9);
        
        [SerializeField] private BoardMap boardMap;
        public BoardMap BoardMap => boardMap;





    }
}