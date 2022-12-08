using System;
using Photon.Pun;
using UnityEngine;

namespace RtShogi.Scripts.Battle
{
    public partial class BattleRpcaller
    {
        public void RpcallMoveKomaOnBoard(KomaUnit koma, BoardPiece destPiece)
        {
            photonView.RPC(nameof(moveKomaOnBoard), RpcTarget.AllViaServer,
                getLocalActorNumber(), // int photonActorNumber,
                koma.Id.Value, // int komaId,
                destPiece.Point.SerializeToBytes() // byte[] destPoint
                );
        }

        [PunRPC]
        private void moveKomaOnBoard(
            int photonActorNumber,
            int komaId,
            byte[] destPoint
        )
        {
            var koma = komaManager.List.GetOf(new KomaId(komaId));
            if (checkNull(koma)) return;
            
            var actualDestPoint = correctReceivesPointFromBytes(photonActorNumber, destPoint);
            
            Player.CommanderAction.MoveKomaOnBoard(koma, boardManager.BoardMap.TakePiece(actualDestPoint));
        }

    }


}