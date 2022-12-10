using System;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;

namespace RtShogi.Scripts.Battle
{
    public partial class BattleRpcaller
    {
        public void RpcallSendToObtainedKoma(KomaUnit koma)
        {
            Logger.Print(nameof(RpcallSendToObtainedKoma));
            
            photonView.RPC(nameof(sendToObtainedKoma), RpcTarget.AllViaServer,
                getLocalActorNumber(), // int photonActorNumber,
                koma.Id.Value // int komaId
            );
        }

        [PunRPC]
        private void sendToObtainedKoma(
            int photonActorNumber,
            int komaId
        )
        {
            Logger.Print("called " + nameof(sendToObtainedKoma));
            
            var koma = komaManager.List.GetOf(new KomaId(komaId));
            if (checkNull(koma)) return;
            
            komaManager.SendToObtainedKoma(koma).Forget();
        }

    }


}