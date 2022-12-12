using UnityEngine;

namespace RtShogi.Scripts.Battle
{
    public record IntSize(int W, int H){}
    
    public class BoardManager : MonoBehaviour
    {
        [SerializeField] private BoardMap boardMap;
        public BoardMap BoardMap => boardMap;

        public void ResetBeforeBattle()
        {
            boardMap.ResetBeforeBattle();
        }



    }
}