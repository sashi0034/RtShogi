using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Michsky.MUIP;
using RtShogi.Scripts.Online;
using RtShogi.Scripts.Param;
using TMPro;
using UnityEngine;

namespace RtShogi.Scripts.Lobby
{
    public class ButtonStartMatching : MonoBehaviour
    {
        [SerializeField] private LobbyCanvas lobbyCanvas;

        [SerializeField] private ButtonManager buttonStart;
        // public ButtonManager ButtonStart => buttonStart;

        [SerializeField] private TextMeshProUGUI textMatchingInProgress;
        // public TextMeshProUGUI TextMatchingInProgress => textMatchingInProgress;

        private MatchMakingManager matchMakingManager => lobbyCanvas.MatchMakingManagerRef;

        // とりあえず今はプレイヤーのマッチングランクは一定
        private MatchPlayerRank _playerRank = new MatchPlayerRank(ConstParameter.MinPlayerRank);

        private string playerName => lobbyCanvas.InputPlayerName.name;

        // とりあえず今は5分を1セッション
        private const int maxOneSessionTimeSec = 60 * 5;
        

        public void Setup()
        {
            buttonStart.gameObject.SetActive(true);
            textMatchingInProgress.gameObject.SetActive(false);
        }

        [EventFunction]
        public void OnClickButtonStart()
        {
            buttonStart.enabled = false;
            processStartMatchig().Forget();
        }

        private async UniTask processStartMatchig()
        {
            await buttonStart.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack);
            buttonStart.gameObject.SetActive(false);
            
            // 状況メッセージを表示
            textMatchingInProgress.gameObject.SetActive(true);
            textMatchingInProgress.transform.localScale = Vector3.zero;

            textMatchingInProgress.text = "通信を開始します...";
            await textMatchingInProgress.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);

            // テキストを大きくしたり小さくしたりアニメーション
            var cancelAnim = new CancellationTokenSource();
            animSlowScalingLoop(cancelAnim.Token, textMatchingInProgress.transform).Forget();

            // マスターに接続開始
            textMatchingInProgress.text = "マスターに接続しています...";
            await matchMakingManager.ProcessConnectToMaster(_playerRank, playerName);

            // 対戦相手が見つかるまでループ
            while (await tryFindOpponentPlayer() == false) { } ;

            textMatchingInProgress.text = "対戦相手が見つかりました";
            cancelAnim.Cancel();
        }

        private async UniTask<bool> tryFindOpponentPlayer()
        {
            var processJoinRoom =
                matchMakingManager.ProcessConnectToJoinRoom(_playerRank, maxOneSessionTimeSec);
            int waitingSec = 0;

            // タスク終了まで待つ
            while (processJoinRoom.Status == UniTaskStatus.Pending)
            {
                textMatchingInProgress.text = "対戦相手を探しています...\n" + waitingSec + "秒";

                await UniTask.Delay(1000);
                waitingSec++;
            }

            return (await processJoinRoom == MatchMakingResult.Succeeded);
        }

        private static async UniTask animSlowScalingLoop(CancellationToken cancel, Transform transform)
        {
            while (true)
            {
                if (cancel.IsCancellationRequested) return;
                await transform.DOScale(1.1f, 1.0f).SetEase(Ease.OutSine);
                await transform.DOScale(1.0f, 1.0f).SetEase(Ease.InSine);
            }
            
        }
    }
}