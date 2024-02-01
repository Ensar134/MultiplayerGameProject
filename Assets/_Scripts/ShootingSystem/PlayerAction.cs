using Mirror;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;


    public class PlayerAction : NetworkBehaviour
    {
        public Gun gun;
        private StarterAssetsInputs _starterAssetsInputs;
        public WallGunGiver wallGunGiver;
        public Duvar duvar;
        public Animator _animator;
   
        public RandomGunBox randomGun;


        private void Awake()
        {
            gun = GetComponent<Gun>();
            _starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        
        }
    

        private void Update()
        {
            if (!isLocalPlayer) return;
        
            if (_starterAssetsInputs.oneShoot )
            {
                _starterAssetsInputs.oneShoot = false;
                _animator.SetBool("Fire",true);
                CmdShoot();
            }
            else if (_starterAssetsInputs.BurstShoot)
            {
                _animator.SetBool("Fire",true);
                CmdShoot();
            }
            else
            {
                _animator.SetBool("Fire",false);
            }
        
            if (_starterAssetsInputs.Reloading)
            {
            
                Reload();
                _starterAssetsInputs.Reloading = false;
            
            }

            if (_starterAssetsInputs.PicupGun)
            {
                if (randomGun!=null)
                {
                    CmdTakeGunFromRandomBox();
                }

                if (wallGunGiver!=null)
                {
                    CmdTakeGunFromWall();
                }
                _starterAssetsInputs.PicupGun = false;
           
            }

            if (_starterAssetsInputs.DropOdun)
            {
                CmdDropOdun();
                _starterAssetsInputs.DropOdun = false;
            }

            if (_starterAssetsInputs.repair)
            {
                CmdRepair();
                _starterAssetsInputs.repair = false;
            
            }
           
           
        }


        #region DropOdun
            [Command]
            public void CmdDropOdun()
            {
              
                if (!isServer)
                {
                    RpcDropOdun();
                }
                else
                {
                    DropOdun();
                }
            }
        [ClientRpc]
        public void RpcDropOdun()
        {
            DropOdun();
        }
        
        public void DropOdun()
        {
            if (duvar != null && duvar.isPlayerNear)
            {
                duvar.Test();
            }
        }

        #endregion
       

        #region Repair 

        [Command] // q ya basılınca çalışır
        public void CmdRepair()
        {
            if (!isServer)
            {
                RpcRepair();
            }
            else
            {
                Repair();
            }
        }
    
        [ClientRpc]
        public void RpcRepair()
        {
            Repair();
        }

        public void Repair()
        {
            if (duvar != null&& duvar.isPlayerNear)
            {
                duvar.OdunlariResetle();
            }
        }
    

        #endregion
        #region TakeGunGunFromWall

        [Command]
        public void CmdTakeGunFromWall()
        {
            if (!isServer)
            {
                RpcTakeGunGunFromWall();
            }
            else
            {
                TakeGunGunFromWall();
            }
        }

        [ClientRpc]
        public void RpcTakeGunGunFromWall()
        {
            TakeGunGunFromWall();
        }

        public void TakeGunGunFromWall()
        {
            if ( wallGunGiver!=null && wallGunGiver.isPlayerNear)
            {
                wallGunGiver.InputPlayer();
            }
        }

        #endregion
        #region TakeGunGunFromRandomBox

        [Command]
        private void CmdTakeGunFromRandomBox()
        {
            if (!isServer)
            {
                RpcTakeGunGunFromRandomBox();
            }
            else
            {
                TakeGunGunFromRandomBox();
            }
        }

        [ClientRpc]
        private void RpcTakeGunGunFromRandomBox()
        {
            TakeGunGunFromRandomBox();
        }

        public void TakeGunGunFromRandomBox()
        {
            if (randomGun != null && randomGun.isPlayerNear)
            {
                randomGun.InputPlayer();
            }
        }



        #endregion
        #region NetworkShoot

        [Command]
        private void CmdShoot()
        {
            if (!isServer)
            {
                RpcShoot();
            }
            else
            {
                gun.Shoot();
            }
        }

        [ClientRpc]
        private void RpcShoot()
        {
            gun.Shoot();
        }


        #endregion
        #region NetworkReload

        [Command]
        private void Reload()
        {
            if (!isServer)
            {
                RpcReload();
            
            }
            else
            {
                gun.StartReload();
            
            }
        }
    
        [ClientRpc]
        private void RpcReload()
        {
            gun.StartReload();
        }
    
        #endregion
    
   
    
    }
