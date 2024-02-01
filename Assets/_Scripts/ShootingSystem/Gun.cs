using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mirror.Examples;
using UnityEngine;
using MoreMountains.Feedbacks;


    public class Gun : NetworkBehaviour
    {
        [SerializeField] private int bulletCount;
        public GameObject bulletPrefab;
        public GunData _gunData;
        public GameObject GunHolder;
        public PlayerAction _playerAction;
        [SerializeField] private Animator playerAnimation;
        [SerializeField] private MMF_Player MMFeedbacksForPistol;
        public int currentAmmoForGun;
        public int maxAmmoForGun;
        [HideInInspector]private bool isReloading ;
    
        public enum EquippedItems : byte
        {       
            Pistol,
            Ak47,
            M4A1,
            ARLong,
            ARPro,
            Smg,
            Shotgun,
            Sniper,
            Bazooka,
        }
        [SyncVar(hook = nameof(OnChangeEquipment))]
        public EquippedItems equippedItems;


        [Header("References")] [SerializeField]
    
   
        private float _nextShootTime;

        //public float _nextShootTime;
        Vector3 _direction;
       
        public Player _player;
        private bool gunActiveForShooting =true;
    
        public List<GameObject> GunList;

        private void Awake()
        {
            _player = GetComponent<Player>();
            GunActive();
           
            
        } 
    
    

        private void Update()
        {
            if (!isServer)
            {
                return;
            }
        }

        #region Shooting



        
        public void Shoot()
        {
            if (!CanShoot()) return;
            _nextShootTime = Time.time + _gunData.fireRate;
            var position = _player.ShootPoint.position;
            var rotation = _player.ShootPoint.rotation;
            GameObject bullet = PrefabPool.singleton.Get(position, rotation); 
            NetworkServer.Spawn(bullet);
    
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            bulletComponent.InitializeBullet(gameObject, position, rotation, transform.forward * _gunData.bulletSpeed);

           
            CallFeedbacks();   
            currentAmmoForGun--;  
        }
        
        [ClientRpc]
        public void CallFeedbacks(){
            MMFeedbacksForPistol.PlayFeedbacks();
        }
        
        
        private bool CanShoot()
        {
            if (currentAmmoForGun<=0)
            {
                StartReload();
                currentAmmoForGun = maxAmmoForGun;
            }
            Debug.Log("Ammo: "+currentAmmoForGun);

            return (Time.time >= _nextShootTime) && currentAmmoForGun > 0 && !IsReloading&&gunActiveForShooting;
        }
        
        #endregion

        #region Reloading
        public void StartReload()
        {
            if (!IsReloading)
            {
           
                StartCoroutine(Reload());
            }else
            {
                Debug.Log("Reload true bu yüzden atış  yapamazsın");
            }
        
        }

        public IEnumerator Reload()
        {
            IsReloading = true;
            yield return new WaitForSeconds(_gunData.reloadTime);
            currentAmmoForGun = maxAmmoForGun;
            IsReloading = false;
            Debug.Log("Reloaded bitti");
        }
      
  
   

   
        public bool IsReloading
        {
            get { return isReloading; }
            set { isReloading = value; }
        }

        private void OnEnable()
        {
            isReloading = false;
        }

    

        #endregion

        #region Guns

    
        public void GunActive()
        {
           GunSetSetting(0);
           
        }
    
        void OnChangeEquipment(EquippedItems oldEquippedItems, EquippedItems newEquippedItems)
        {
            StartCoroutine(ChangeEquipment(newEquippedItems));
        }

        IEnumerator ChangeEquipment(EquippedItems newEquippedItem)
        {
            foreach (var Gun in GunList)
            {
                Gun.SetActive(false);
            }

            switch (newEquippedItem)
            {
                case EquippedItems.Ak47:
                    GunSetSetting(0);
                    break;
                case EquippedItems.ARLong:
                    GunSetSetting(1);
                    break;
            
                case EquippedItems.ARPro:
                    GunSetSetting(2);
                    break;
                case EquippedItems.Bazooka:
                    GunSetSetting(3);
                    break;
                case EquippedItems.M4A1:
                    GunSetSetting(4);
                    break;
                case EquippedItems.Pistol:
                    GunSetSetting(5);
                    break;
                case EquippedItems.Shotgun:
                    GunSetSetting(6);
                    break;
                case EquippedItems.Smg:
                    GunSetSetting(7);
                    break;

                case EquippedItems.Sniper:
                    GunSetSetting(8);
                    break;


            }

        
            yield return null;
        }
        public void GunSetSetting(int GunIndex)
        {
            GunList[GunIndex].SetActive(true);
            _gunData=  GunList[GunIndex].GetComponent<Weapon>().data;
            currentAmmoForGun= _gunData.currentAmmo;
            maxAmmoForGun = _gunData.maxAmmo;
            
            float fireRateAnimation = playerAnimation.GetFloat("FireRate");
            float increaseAnimationSpeed = fireRateAnimation - _gunData.fireRate;
            float newAnimationSpeed = fireRateAnimation + increaseAnimationSpeed;
            playerAnimation.SetFloat("FireRate",newAnimationSpeed);
        }

        #endregion

  
    
    
    
    }

