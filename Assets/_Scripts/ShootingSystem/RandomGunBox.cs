using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.ShootingSystem;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;


    public class RandomGunBox : NetworkBehaviour
    {
        public Gun gun;
        public float rotationDuration = 1f;  // toplam donme suresi
        public float rotationInterval = 0.1f;// her bir silah degisimi arasindaki sure
        public bool isPlayerNear;

        private const float CheckInterval = 1.0f; // Her saniyede bir kontrol eder.
        private const float CheckDistanceThreshold = 5.0f; // 5 birim mesafedeki oyuncuları kabul eder.
        private float _nextCheckTime;

        private GameObject _currentGun;  
        private GameObject _fakeCurrentGun; 
        private enum InteractionState
        {
            SpinGun,
            PickupGun
        }
        private void Update()
        {
        
            if (!isServer)
            {
                return;
            }
       
       
            if (Time.time > _nextCheckTime)
            {
                CheckClosestPlayer();
                _nextCheckTime = Time.time + CheckInterval;
            }
        
        }
        private InteractionState _currentState = InteractionState.SpinGun;//silahın dönüp dönmeyeceğini veya oyuncunun silahı alıp alamayacağını belirlemek için iki durumu temsil ediyor.
   
   
    
         public List<GameObject> gunPrefabList;

        [SyncVar(hook = nameof(OnChangeEquipment))]
        public Gun.EquippedItems equippedItem;
   
    
        public  void OnChangeEquipment(Gun.EquippedItems oldEquippedItem, Gun.EquippedItems newEquippedItem)//OnChangeEquipment fonksiyonu, equippedItem değişikliği olduğunda çağrılır ve seçilen yeni silahın ekranda gösterilmesini sağlar.
        {
            StartCoroutine(SelectedGun(newEquippedItem));
            // SelectedGun(newEquippedItem);
        }
    
    
        [Server]
        public void ServerChangeEquippedItem(Gun.EquippedItems selectedItem)//CmdChangeEquippedItem fonksiyonu, silah değişikliği olduğunda çağrılır ve seçilen silahın tüm oyunculara bildirilmesini sağlar.
        {
       
            equippedItem = selectedItem;
            _currentState = InteractionState.PickupGun;
        

        }
        [ClientRpc]
        public void RpcChangeEquippedItem(Gun.EquippedItems selectedItem)//CmdChangeEquippedItem fonksiyonu, silah değişikliği olduğunda çağrılır ve seçilen silahın tüm oyunculara bildirilmesini sağlar.
        {
       
            equippedItem = selectedItem;
            _currentState = InteractionState.PickupGun;
        

        }

        public void DestroyCurrentGun()
        {
            if (_currentGun != null)
            {
                _currentGun.SetActive(false);
                _currentGun = null;
           
            }
        
        }
    
    
 

        #region Server

        [Server]
        public void InputPlayer()
        {
            Debug.Log("'F' key was pressed.");
            if (gun==null)
            {
                return;   
            }
            if (_currentState == InteractionState.SpinGun)
            {
                RpcReturnGun();
            }
            else if (_currentState == InteractionState.PickupGun)
            {
            
            
                ChangeEquippedIttem(equippedItem);
                NonActiveGun();

            }
        }
    
   
    
        [Server]
        public void RpcReturnGun()
        {
            StartCoroutine(SpinGunWheel());
       
        }
    
        [Server]
        public void ChangeEquippedIttem(Gun.EquippedItems selectedItems)
        {
            gun.equippedItems = selectedItems;
            _currentState = InteractionState.SpinGun;
        }

        #endregion


        #region Client
        [ClientRpc]
        public void NonActiveGun()
        {
        
            foreach (var gun in gunPrefabList)
            {
                gun.SetActive(false);
            }
        }
        #endregion

        #region IEnumerator

        public IEnumerator SpinGunWheel()
        {

            float elapsed = 0;

            // Generate a random list of guns for the duration of the spin
            List<Gun.EquippedItems> randomGunSequence = new List<Gun.EquippedItems>();

            while (elapsed < rotationDuration)
            {
                randomGunSequence.Add(
                    (Gun.EquippedItems)UnityEngine.Random.Range(1, 10)); // Random gun between Pistol and Bazooka
                elapsed += rotationInterval;
                yield return new WaitForSeconds(rotationInterval);
            }

            foreach (var gun in randomGunSequence)
            {
                yield return new WaitForSeconds(rotationInterval);
            

                switch (gun)
                {

                    case Gun.EquippedItems.Smg:
                        DestroyCurrentGun();
                        _currentGun = gunPrefabList[7];
                        _currentGun.SetActive(true);
                        break;

                    case Gun.EquippedItems.Shotgun:
                        DestroyCurrentGun();
                        _currentGun = gunPrefabList[6];
                        _currentGun.SetActive(true);

                        break;

                    case Gun.EquippedItems.Sniper:
                        DestroyCurrentGun();
                        _currentGun = gunPrefabList[8];
                        _currentGun.SetActive(true);

                        break;

                    case Gun.EquippedItems.Bazooka:
                        DestroyCurrentGun();
                        _currentGun = gunPrefabList[3];
                        _currentGun.SetActive(true);

                        break;

                    case Gun.EquippedItems.Pistol:
                        DestroyCurrentGun();
                        _currentGun = gunPrefabList[5];
                        _currentGun.SetActive(true);

                        break;


                    case Gun.EquippedItems.Ak47:
                        DestroyCurrentGun();
                        _currentGun = gunPrefabList[0];
                        _currentGun.SetActive(true);

                        break;

                    case Gun.EquippedItems.M4A1:
                        DestroyCurrentGun();
                        _currentGun = gunPrefabList[4];
                        _currentGun.SetActive(true);

                        break;

                    case Gun.EquippedItems.ARLong:
                        DestroyCurrentGun();
                        _currentGun = gunPrefabList[1];
                        _currentGun.SetActive(true);

                        break;

                    case Gun.EquippedItems.ARPro:
                        DestroyCurrentGun();
                        _currentGun = gunPrefabList[2];
                        _currentGun.SetActive(true);
                        break;
                }
          
            
                ServerChangeEquippedItem(randomGunSequence.Last());
            }
      
        }
        public IEnumerator SelectedGun(Gun.EquippedItems newEquippedItem)
        {
            foreach (var gun in gunPrefabList)
            {
                gun.SetActive(false);
            }
        
            switch (newEquippedItem)
            {
           
                case Gun.EquippedItems.Smg:
                
                    gunPrefabList[7].SetActive(true);
                
                    break;

                case Gun.EquippedItems.Shotgun:
                
                    gunPrefabList[6].SetActive(true);
           

                    break;

                case Gun.EquippedItems.Sniper:
               
                    gunPrefabList[8].SetActive(true);
               

                    break;

                case Gun.EquippedItems.Bazooka:
                
                    gunPrefabList[3].SetActive(true);
              

                    break;

                case Gun.EquippedItems.Pistol:
              
                    gunPrefabList[5].SetActive(true);
                

                    break;


                case Gun.EquippedItems.Ak47:
               
                    gunPrefabList[0].SetActive(true);
              

                    break;

                case Gun.EquippedItems.M4A1:
               
                    gunPrefabList[4].SetActive(true);
                

                    break;

                case Gun.EquippedItems.ARLong:
               
                    gunPrefabList[1].SetActive(true);
                

                    break;

                case Gun.EquippedItems.ARPro:
               
                    gunPrefabList[2].SetActive(true);
                
                    break;
                
            }


            yield return equippedItem;
        }

        #endregion
  
        #region SetGunScript
        private Transform FindClosestPlayer()
        {
            float closestDistance = float.MaxValue;
            Transform closestPlayer = null;

            foreach (Transform playerTransform in PlayerManager.ClientTransforms)
            {
                float distance = Vector3.Distance(transform.position, playerTransform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPlayer = playerTransform;
                }
            }

            return closestPlayer;
        }

        private void CheckClosestPlayer()
        {
            Transform closestPlayer = FindClosestPlayer();

            if (closestPlayer != null)
            {
                float distanceSqr = (transform.position - closestPlayer.position).sqrMagnitude;

                if (distanceSqr < CheckDistanceThreshold)
                {
                    isPlayerNear = true;
                    gun = closestPlayer.gameObject.GetComponent<Gun>();
                    // Eğer Gun bileşeninin bir alt bileşeni veya bir fonksiyonu bu bileşene erişmek istiyorsa:
                    if (gun != null)
                    {
                        gun._playerAction.randomGun = this;
                    }
                }
                else
                {
                    isPlayerNear = false;
                    gun = null;
                }
            }

        }
    

        #endregion

        #region CanSeeGunForClient
         public IEnumerator FakeSpinGunWheel()
        {

            float elapsed = 0;

            // Generate a random list of guns for the duration of the spin
            List<Gun.EquippedItems> randomGunSequence = new List<Gun.EquippedItems>();

            while (elapsed < rotationDuration)
            {
                randomGunSequence.Add(
                    (Gun.EquippedItems)UnityEngine.Random.Range(1, 10)); // Random gun between Pistol and Bazooka
                elapsed += rotationInterval;
                yield return new WaitForSeconds(rotationInterval);
            }

            foreach (var gun in randomGunSequence)
            {
                yield return new WaitForSeconds(rotationInterval);
            

                switch (gun)
                {

                    case Gun.EquippedItems.Smg:
                        DestroyCurrentGun();
                        _currentGun = gunPrefabList[7];
                        _currentGun.SetActive(true);
                        break;

                    case Gun.EquippedItems.Shotgun:
                        DestroyCurrentGun();
                        _currentGun = gunPrefabList[6];
                        _currentGun.SetActive(true);

                        break;

                    case Gun.EquippedItems.Sniper:
                        DestroyCurrentGun();
                        _currentGun = gunPrefabList[8];
                        _currentGun.SetActive(true);

                        break;

                    case Gun.EquippedItems.Bazooka:
                        DestroyCurrentGun();
                        _currentGun = gunPrefabList[3];
                        _currentGun.SetActive(true);

                        break;

                    case Gun.EquippedItems.Pistol:
                        DestroyCurrentGun();
                        _currentGun = gunPrefabList[5];
                        _currentGun.SetActive(true);

                        break;


                    case Gun.EquippedItems.Ak47:
                        DestroyCurrentGun();
                        _currentGun = gunPrefabList[0];
                        _currentGun.SetActive(true);

                        break;

                    case Gun.EquippedItems.M4A1:
                        DestroyCurrentGun();
                        _currentGun = gunPrefabList[4];
                        _currentGun.SetActive(true);

                        break;

                    case Gun.EquippedItems.ARLong:
                        DestroyCurrentGun();
                        _currentGun = gunPrefabList[1];
                        _currentGun.SetActive(true);

                        break;

                    case Gun.EquippedItems.ARPro:
                        DestroyCurrentGun();
                        _currentGun = gunPrefabList[2];
                        _currentGun.SetActive(true);
                        break;
                }
          
            
               
            }
      
        }
        
         public void FakeDestroyCurrentGun()
         {
             if (_currentGun != null)
             {
                 _currentGun.SetActive(false);
                 _currentGun = null;
           
             }
        
         }
        #endregion
    
  


    }

