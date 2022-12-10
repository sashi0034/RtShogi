using System;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using RtShogi.Scripts.Battle;
using RtShogi.Scripts.Param;
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

#if UNITY_EDITOR
            if (DebugParameter.Instance.IsDebugBattleOfflineMode)
                startDebugBattleOfflineMode();
            else
                startProcess(playerName).Forget();
#endif
        }

        private void startDebugBattleOfflineMode()
        {
            Logger.Print("start debug battle offline mode");
            
            PhotonNetwork.OfflineMode = true;
            PhotonNetwork.JoinRoom("Offline");
            BattleRoot.Instance.KomaManager.SetupAllAllyKomaOnBoard();
            BattleRoot.Instance.KomaManager.SetupAllEnemyKomaOnBoardAsDebug();
        }

        private async UniTask startProcess(string playerName)
        {
            await matchMakingManager.ProcessConnectToJoinRoom(new MatchPlayerRank(1), playerName, 3600);
            Logger.Print("finished connect");
            
            BattleRoot.Instance.KomaManager.SetupAllAllyKomaOnBoard();
            Logger.Print("initialized koma on board");
        }

        private string makeDebugPlayerName()
        {
            return "debugger_" + DateTime.Now.ToString("HH_mm_ss");
        }
    }
}