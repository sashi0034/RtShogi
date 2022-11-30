using UnityEngine;

namespace RtShogi.Scripts.Battle
{
    public record IntSize(int W, int H){}
    
    public class BoardManager : MonoBehaviour
    {
        public static int SideLength => BoardMap.SideLength;
        public static IntSize BoardSize => BoardMap.BoardSize;
        
        [SerializeField] private BoardMap boardMap;
        public BoardMap BoardMap => boardMap;





    }
}