using System;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;

namespace RtShogi.Scripts.Battle
{
    public partial class BattleRpcaller
    {
        public void RpcallBecomeFormed(KomaUnit koma)
        {
            Logger.Print(nameof(RpcallBecomeFormed));
            
            photonView.RPC(nameof(becomeFormed), RpcTarget.AllViaServer,
                // getLocalActorNumber(), // int photonActorNumber,
                koma.Id.Value // int komaId
            );
        }

        [PunRPC]
        private void becomeFormed(
            // int photonActorNumber,
            int komaId
        )
        {
            Logger.Print("called " + nameof(becomeFormed));
            
            var koma = komaManager.List.GetOf(new KomaId(komaId));
            if (checkNull(koma)) return;
            
            koma.BecomeFormed();
        }

    }


}