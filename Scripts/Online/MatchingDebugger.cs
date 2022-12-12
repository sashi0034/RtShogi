using System;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using RtShogi.Scripts.Battle;
using RtShogi.Scripts.Param;
using TMPro;
using UnityEngine;

namespace RtShogi.Scripts.Online
{
    public class MatchingDebugger : MonoBehaviour
    {
        [SerializeField] private MatchMakingManager matchMakingManager;
        [SerializeField] private TextMeshProUGUI textWaitForOtherPlayer;

        [EventFunction]
        private void Awake()
        {
            if (DebugParameter.Instance.IsClearDebug) Util.DestroyGameObject(gameObject);
        }
        
        public void StartDebug()
        {
            var playerName = makeDebugPlayerName();
            Logger.Print("local player name: " + playerName);

#if UNITY_EDITOR
            if (DebugParameter.Instance.IsDebugBattleOfflineMode)
                startDebugBattleOfflineMode();
            else
                startProcess(playerName).Forget();
#else
            // 実機デバッグ用
            startProcess(playerName).Forget();
#endif
        }

        private void startDebugBattleOfflineMode()
        {
            Logger.Print("start debug battle offline mode");
            
            PhotonNetwork.OfflineMode = true;
            PhotonNetwork.JoinRoom("Offline");
            BattleRoot.Instance.InvokeStartBattle();
            BattleRoot.Instance.KomaManager.SetupAllEnemyKomaOnBoardAsDebug();
        }

        private async UniTask startProcess(string playerName)
        {
            textWaitForOtherPlayer.gameObject.SetActive(true);
            
            await matchMakingManager.ProcessConnectAllFlows(new MatchPlayerRank(1), playerName, 3600);
            Logger.Print("finished connect");
            
            BattleRoot.Instance.KomaManager.SetupAllAllyKomaOnBoard();
            Logger.Print("initialized koma on board");
            
            textWaitForOtherPlayer.gameObject.SetActive(false);
        }

        private string makeDebugPlayerName()
        {
            return "debugger_" + DateTime.Now.ToString("HH_mm_ss");
        }
    }
}