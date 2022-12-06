using System;
using Cysharp.Threading.Tasks;
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
            matchMakingManager.ProcessConnectToJoinRoom(new MatchPlayerRank(1), playerName).Forget();
        }

        private string makeDebugPlayerName()
        {
            return "debugger_" + DateTime.Now.ToString("HH_mm_ss");
        }
    }
}