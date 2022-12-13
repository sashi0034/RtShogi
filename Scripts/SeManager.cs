using System;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.Serialization;

namespace RtShogi.Scripts
{
    public class SeManager : MonoBehaviour
    {
        public static SeManager Instance { get; private set; }
        
        [SerializeField] private AudioSource audioSource;
        
        [SerializeField] private AudioClip sePopUp;
        public AudioClip SePopUp => sePopUp;

        [SerializeField] private AudioClip seOk;
        public AudioClip SeOk => seOk;

        [SerializeField] private AudioClip seStartMatchMaking;
        public AudioClip SeStartMatchMaking => seStartMatchMaking;

        [SerializeField] private AudioClip seEndMatchMaking;
        public AudioClip SeEndMatchMaking => seEndMatchMaking;

        [SerializeField] private AudioClip seStartBattle;
        public AudioClip SeStartBattle => seStartBattle;

        [SerializeField] private AudioClip seChangeCamera;
        public AudioClip SeChangeCamera => seChangeCamera;

        [SerializeField] private AudioClip seKomaForm;
        public AudioClip SeKomaForm => seKomaForm;

        [SerializeField] private AudioClip seKomaBirth;
        public AudioClip SeKomaBirth => seKomaBirth;

        [SerializeField] private AudioClip seKomaKillPlayer;
        public AudioClip SeKomaKillPlayer => seKomaKillPlayer;
        
        [SerializeField] private AudioClip seKomaKillEnemy;
        public AudioClip SeKomaKillEnemy => seKomaKillEnemy;

        [SerializeField] private AudioClip seKomaMoveAlly;
        public AudioClip SeKomaMoveAlly => seKomaMoveAlly;

        [SerializeField] private AudioClip seKomaMoveEnemy;
        public AudioClip SeKomaMoveEnemy => seKomaMoveEnemy;

        [SerializeField] private AudioClip endCooldown;
        public AudioClip EndCooldown => endCooldown;

        [SerializeField] private AudioClip seBattleWin;
        public AudioClip SeBattleWin => seBattleWin;

        [SerializeField] private AudioClip seBattleLose;
        public AudioClip SeBattleLose => seBattleLose;

        [SerializeField] private AudioClip seCountRating;
        public AudioClip SeCountRating => seCountRating;
        
        
        

        public SeManager()
        {
            Instance = this;
        }

        public void PlaySe(AudioClip clip)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}