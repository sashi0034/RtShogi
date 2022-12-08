using System;
using Photon.Pun;
using UnityEngine;

namespace RtShogi.Scripts.Battle
{
    public record KomaPutInfo(
        BoardPoint Point, 
        EKomaKind Kind, 
        ETeam Team, 
        KomaId Id,
        bool IsFromObtainedKoma);

    public partial class BattleRpcaller
    {
        public void RpcallPutNewKoma(KomaPutInfo putInfo)
        {
            photonView.RPC(nameof(putNewKoma), RpcTarget.AllViaServer,
                getLocalActorNumber(), // int photonActorNumber,
                putInfo.Point.SerializeToBytes(), // byte[] point,
                putInfo.Kind, // EKomaKind kind,
                putInfo.Team, // ETeam team,
                putInfo.Id.Value, // int id
                putInfo.IsFromObtainedKoma // bool isFromObtained
                );
        }

        [PunRPC]
        private void putNewKoma(
            int photonActorNumber,
            byte[] point,
            EKomaKind kind,
            ETeam team,
            int id,
            bool isFromObtained
            )
        {
            var actualPoint = correctReceivesPoint(photonActorNumber, BoardPoint.DeserializeFromBytes(point));
            var actualTeam = correctReceivedTeam(photonActorNumber, team);
            
            komaManager.PutNewKoma(new KomaPutInfo(actualPoint, kind, actualTeam, new KomaId(id), isFromObtained));
            
            // Logger.Print("called put new koma");
        }

    }


}