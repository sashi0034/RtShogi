using UnityEngine;

namespace RtShogi.Scripts.Battle
{
    public record IntSize(int W, int H){}
    
    public class BoardManager : MonoBehaviour
    {
        public const int SideLength = 9;
        public static IntSize BoardSize = new IntSize(SideLength, SideLength);
        
        [SerializeField] private BoardMap boardMap;
        public BoardMap BoardMap => boardMap;





    }
}