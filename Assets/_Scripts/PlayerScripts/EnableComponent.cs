using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.ShootingSystem;
using UnityEngine;
using Mirror;   
using Cinemachine;
using RootMotion.FinalIK;
using Telepathy;
using UnityEngine.InputSystem;


namespace StarterAssets
{
    public class EnableComponent : NetworkBehaviour
    {
        public Transform target;
        //private PlayerInput playerInput;
       
        

       

        private void Start()
        {
            
            if (isLocalPlayer)
            {
                CharacterController cc= GetComponent<CharacterController>();
                cc.enabled = true;
                ThirdPersonController tpc = GetComponent<ThirdPersonController>();
                tpc.enabled = true;
                PlayerInput pi = GetComponent<PlayerInput>();
                pi.enabled = true;
                GameObject pfc = GameObject.FindGameObjectWithTag("PlayerFollowCamera");
                CinemachineVirtualCamera cvc = pfc.GetComponent<CinemachineVirtualCamera>();
                cvc.Follow = target;
                Gun gun = GetComponent<Gun>();
                gun.enabled = true;
                PlayerAction pa = GetComponent<PlayerAction>();
                pa.enabled = true;
                HealtDisplay hd = GetComponent<HealtDisplay>();
                hd.enabled = true;
              

            }
        }
    }
}
