using System;
using Cysharp.Threading.Tasks;
using RtShogi.Scripts.Battle;
using UnityEngine;

namespace RtShogi.Scripts.Online
{
    public class MatchingDebugger : MonoBehaviour
    {
        [SerializeField] private MatchMakingManager matchMakingManager;

        [EventFunction]
        private void Start()
        {
            var playerName = makeDebugPlayerName();
            Logger.Print("local player name: " + playerName);
            startProcess(playerName).Forget();
        }

        private async UniTask startProcess(string playerName)
        {
            await matchMakingManager.ProcessConnectToJoinRoom(new MatchPlayerRank(1), playerName, 3600);
            Logger.Print("finished connect");
            
            BattleRoot.Instance.KomaManager.InitAllKomaOnBoard();
            Logger.Print("initialized koma on board");
        }

        private string makeDebugPlayerName()
        {
            return "debugger_" + DateTime.Now.ToString("HH_mm_ss");
        }
    }
}