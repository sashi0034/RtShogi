using System;
using UnityEngine;

namespace RtShogi.Scripts.Battle
{
    public class BattleRoot : MonoBehaviour
    {
        public static BattleRoot Instance;
        public BattleRoot()
        {
            Instance = this;
        }
        
        [SerializeField] private KomaManager komaManager;
        public KomaManager KomaManager => komaManager;

        [SerializeField] private PlayerCommander playerCommander;
        public PlayerCommander PlayerCommander => playerCommander;

        [SerializeField] private BoardManager boardManager;
        public BoardManager BoardManager => boardManager;

        [SerializeField] private BattleCanvas battleCanvasRef;
        public BattleCanvas BattleCanvasRef => battleCanvasRef;

    }
}