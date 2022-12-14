using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Michsky.MUIP;
using RtShogi.Scripts.Matching;
using RtShogi.Scripts.Param;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace RtShogi.Scripts.Lobby
{
    record MatchingSessionTempResult(bool IsSuccess, int PassedTime);
 
    public class ButtonStartMatching : MonoBehaviour
    {
        [SerializeField] private LobbyCanvas lobbyCanvas;

        [SerializeField] private ButtonManager buttonStart;
        // public ButtonManager ButtonStart => buttonStart;

        [SerializeField] private TextMeshProUGUI textMatchingInProgress;
        // public TextMeshProUGUI TextMatchingInProgress => textMatchingInProgress;

        [SerializeField] private Image mascotGreenOnion;

        [SerializeField] private TextMeshProUGUI textMatchingRecommendedTime;

        private MatchMakingManager matchMakingManager => lobbyCanvas.MatchMakingManagerRef;

        // とりあえず今はプレイヤーのマッチングランクは一定
        private MatchPlayerRank _playerRank = new MatchPlayerRank(ConstParameter.MinPlayerRank);

        private string playerName => lobbyCanvas.InputPlayerName.name;

        // とりあえず今は1分を1セッション
        private const int maxOneSessionTimeSec = 60;

        private Subject<Unit> _onCompletedMatchMaking = new();
        public IObservable<Unit> OnCompletedMatchMaking => _onCompletedMatchMaking;


        public void ResetBeforeLobby()
        {
            gameObject.SetActive(true);
            buttonStart.enabled = true;
            Util.ResetScaleAndActivate(buttonStart);
            textMatchingInProgress.gameObject.SetActive(false);
            mascotGreenOnion.gameObject.SetActive(false);
            textMatchingRecommendedTime.gameObject.SetActive(false);
        }

        [EventFunction]
        public void OnClickButtonStart()
        {
            buttonStart.enabled = false;
            processStartMatchig().Forget();
        }

        private async UniTask processStartMatchig()
        {
            SeManager.Instance.PlaySe(SeManager.Instance.SeStartMatchMaking);
            
            await buttonStart.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack);
            buttonStart.gameObject.SetActive(false);
            animMascotGreenOnionWhileMatching(mascotGreenOnion, _onCompletedMatchMaking).Forget();
            
            // 状況メッセージを表示
            textMatchingInProgress.gameObject.SetActive(true);
            textMatchingInProgress.transform.localScale = Vector3.zero;

            textMatchingInProgress.text = "通信を開始します...";
            await textMatchingInProgress.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);

            // テキストを大きくしたり小さくしたりアニメーション
            var animScaling = animSlowScalingLoop(textMatchingInProgress.transform);

            // マスターに接続開始
            textMatchingInProgress.text = "マスターに接続しています...";
            await matchMakingManager.ProcessConnectToMaster(_playerRank, playerName);

            // 対戦相手が見つかるまでループ
            MatchingSessionTempResult sessionTemp = new MatchingSessionTempResult(false, 0);
            while ((sessionTemp = await tryFindOpponentPlayer(sessionTemp.PassedTime)).IsSuccess == false) { } ;

            // 見つかった時の演出
            SeManager.Instance.PlaySe(SeManager.Instance.SeEndMatchMaking);
            textMatchingInProgress.text = "対戦相手が見つかりました";
            animScaling.Kill();
            textMatchingInProgress.transform.DOScale(1.5f, 3.0f).SetEase(Ease.InOutBack);
            
            matchMakingManager.SetupRpcallerRef.RpcallSetupPlayerData(
                lobbyCanvas.LabelRating.PlayerRating.Value, 
                lobbyCanvas.InputPlayerName.PlayerName);

            await UniTask.Delay(3.0f.ToIntMilli());
            
            _onCompletedMatchMaking.OnNext(Unit.Default);
        }

        private async UniTask<MatchingSessionTempResult> tryFindOpponentPlayer(int passedTime)
        {
            var processJoinRoom =
                matchMakingManager.ProcessConnectToJoinRoom(_playerRank, maxOneSessionTimeSec);
            int waitingSec = passedTime;

            // タスク終了まで待つ
            while (processJoinRoom.Status == UniTaskStatus.Pending)
            {
                textMatchingInProgress.text = "対戦相手を探しています...\n" + waitingSec + "秒";

                await UniTask.Delay(1000);
                waitingSec++;

                const int secShowRecommended = 30;
                if (waitingSec == secShowRecommended) showTextMatchingRecommendedTime();
            }

            return new MatchingSessionTempResult(
                await processJoinRoom == MatchMakingResult.Succeeded,
                waitingSec);
        }

        // マッチングおすすめ時間を表示
        private void showTextMatchingRecommendedTime()
        {
            textMatchingRecommendedTime.gameObject.SetActive(true);
            textMatchingRecommendedTime.transform.localScale = Vector3.zero;
            textMatchingRecommendedTime.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        }

        private static Tween animSlowScalingLoop(Transform transform)
        {
            return DOTween.Sequence()
                .Append(transform.DOScale(1.1f, 1.0f).SetEase(Ease.OutSine))
                .Append(transform.DOScale(1.0f, 1.0f).SetEase(Ease.InSine))
                .SetLoops(-1);
        }

        private static async UniTask animMascotGreenOnionWhileMatching(Image greenOnion, IObservable<Unit> onCompletedMatching)
        {
            // ネギをぐるぐる回すアニメ
            greenOnion.gameObject.SetActive(true);
            greenOnion.transform.localScale = Vector3.zero;
            greenOnion.transform.rotation = Quaternion.Euler(Vector3.zero);

            await greenOnion.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);

            var anim = DOTween.Sequence(greenOnion)
                .Append(greenOnion.transform.DORotate(new Vector3(0, 0, 360 * 5), 5.0f).SetRelative(true).SetEase(Ease.InOutQuint))
                .Append(greenOnion.transform.DORotate(new Vector3(0, 0, 360 * -5), 5.0f).SetRelative(true).SetEase(Ease.InOutBounce))
                .Append(DOVirtual.DelayedCall(1.0f, ()=>{})).SetLoops(-1).Play();
            
            await onCompletedMatching.Take(1);
            anim.Kill();
        }
    }
}