using System;
using Photon.Pun;
using UnityEngine;

namespace RtShogi.Scripts.Battle
{
    public class BattleRpcaller : MonoBehaviourPunCallbacks
    {
        
        public bool IsRoomHost()
        {
            return PhotonNetwork.IsMasterClient;
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