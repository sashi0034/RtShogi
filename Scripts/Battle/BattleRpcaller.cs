using System;
using Photon.Pun;
using UnityEngine;

namespace RtShogi.Scripts.Battle
{
    public partial class BattleRpcaller : MonoBehaviourPunCallbacks
    {
        [SerializeField] private KomaManager komaManager;
        public bool IsRoomHost()
        {
            return PhotonNetwork.IsMasterClient;
        }

        private int getLocalActorNumber()
        {
            return PhotonNetwork.LocalPlayer.ActorNumber;
        }
        
        private ETeam correctReceivedTeam(int photonActorNumber, ETeam receivedTeam)
        {
            if (IsLocalPhotonActor(photonActorNumber)) return receivedTeam;

            return receivedTeam switch
            {
                ETeam.Enemy => ETeam.Ally,
                ETeam.Ally => ETeam.Enemy,
                _ => throw new ArgumentOutOfRangeException(nameof(receivedTeam), receivedTeam, null)
            };
        }

        private BoardPoint correctReceivesPoint(int photonActorNumber, BoardPoint point)
        {
            return IsLocalPhotonActor(photonActorNumber) ? point : point.ToReversed();
        }
        
        public static bool IsLocalPhotonActor(int photonActorNumber)
        {
            return PhotonNetwork.LocalPlayer.ActorNumber == photonActorNumber;
        }
    }
    
    [AttributeUsage(AttributeTargets.Method)]
    public class UsingBattleRpcallerAttribute : Attribute
    {
        public UsingBattleRpcallerAttribute() { }
    }
    
    [AttributeUsage(AttributeTargets.Method)]
    public class FromBattleRpcallerAttribute : Attribute
    {
        public FromBattleRpcallerAttribute() { }
    }
}