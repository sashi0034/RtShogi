using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Photon.Pun;
using RtShogi.Scripts.Battle;
using RtShogi.Scripts.Lobby;
using RtShogi.Scripts.Matching;
using RtShogi.Scripts.Param;
using RtShogi.Scripts.Storage;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.VisualScripting;
using UnityEngine;

namespace RtShogi.Scripts
{
    public class GameRoot : MonoBehaviour
    {
        [SerializeField] private BattleCanvas battleCanvas;
        [SerializeField] private BattleRoot battleRoot;

        [SerializeField] private LobbyCanvas lobbyCanvas;
        public LobbyCanvas LobbyCanvas => lobbyCanvas;

        [SerializeField] private MatchingDebugger matchingDebugger;

        [SerializeField] private SaveData saveData = new SaveData();
        public SaveData SaveData => saveData;
        
        
        public void Start()
        {
            lobbyCanvas.SleepOutLobby();
            battleRoot.SleepOutBattle();
            
#if UNITY_EDITOR
            if (DebugParameter.Instance.IsStartBattleImmediately)
            {
                debugBattleImmediately();
                return;
            }
#endif
            processGame().Forget();
        }

#if UNITY_EDITOR

        private void debugBattleImmediately()
        {
            battleRoot.ResetBeforeBattle();
            matchingDebugger.StartDebug();
        }
#endif

        private async UniTask processGame()
        {
            readSaveData();
            lobbyCanvas.ResetBeforeLobby(this, ELobbyResetOption.AfterInit);
            
            while (gameObject != null)
            {
                await processLobby();

                await processBattleBetweenLobby();
            }
        }
        
        private async UniTask processLobby()
        {
            Util.ResetScaleAndActivate(lobbyCanvas);
            
            // ロビーに入る前にSetupRpcallerを初期化して、マッチング完了時には初期化が終わってる状態にしておく
            battleRoot.SetupRpcallerRef.ResetParam();
            
            await lobbyCanvas.ProcessLobby();
            
            writeSaveData();
        }
        
        private async UniTask processBattleBetweenLobby()
        {
            battleRoot.ResetBeforeBattle();

            await lobbyCanvas.transform.DOScale(0f, 0.5f).SetEase(Ease.InBack);
            lobbyCanvas.SleepOutLobby();

            // バトル開始
            var (battleResult, battleLog) = await battleRoot.ProcessBattle(this);
            saveData.SetPlayerRating(battleResult.NewPlayerRating);
            saveData.PushBattleLog(battleLog);
            writeSaveData();
            
            lobbyCanvas.ResetBeforeLobby(this, ELobbyResetOption.AfterBattle);
            
            await lobbyCanvas.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);

            battleRoot.SleepOutBattle();
            
            // レート加算の演出
            await lobbyCanvas.PerformAfterBattle(battleResult);
        }

        private void writeSaveData()
        {
            saveData.UpdateByCopyDataFromSomeObjects(this);
            string jsonData = JsonUtility.ToJson(saveData);
            ES3.Save(ConstParameter.SaveDataKey, jsonData);
            
            Logger.Print("write save data:\n" + jsonData);
        }

        private void readSaveData()
        {
            string jsonData = ES3.Load<string>(ConstParameter.SaveDataKey, defaultValue: "");
            if (jsonData.IsNullOrWhitespace()) return;
            
            Logger.Print("read save data:\n" + jsonData);

            var temp = JsonUtility.FromJson<SaveData>(jsonData);
            Debug.Assert(temp != null);
            if (temp == null) return;
            saveData = temp;
        }

#if UNITY_EDITOR
        [Button]
        public void DebugResetPlayerRate()
        {
            saveData.SetPlayerRating(ConstParameter.InitialPlayerRating);
            lobbyCanvas.ResetBeforeLobby(this, ELobbyResetOption.AfterInit);
        }
#endif

    }
}