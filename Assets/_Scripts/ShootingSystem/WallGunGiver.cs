using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Serialization;


public class WallGunGiver : NetworkBehaviour
{
    [Header("Settings")] [SerializeField] private List<GameObject> gunPrefabList;
    [SerializeField] private Gun.EquippedItems equippedItem;




    private const float CheckInterval = 1.0f; // Her saniyede bir kontrol eder.
    private const float CheckDistanceThreshold = 5.0f; // 5 birim mesafedeki oyuncuları kabul eder.

    private float _nextCheckTime;
    public bool isPlayerNear;

    public Gun gun;

    private enum
        InteractionState //GunVisible: Duvara asılı olan silah görünür, GunInvisible: Duvara asılı olan silah görünmez.
    {
        GunVisible,
        GunInvisible
    }

    private InteractionState _currentState = InteractionState.GunVisible;







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





    #region Server



    public void ChangeEquippedIttem(Gun.EquippedItems selectedItems)
    {
        gun.equippedItems = selectedItems;
        _currentState = InteractionState.GunInvisible;
    }

    [Server]
    public void InputPlayer()
    {
        Debug.Log("'G' key was pressed.");
        if (gun == null)
        {
            return;
        }
        else
        {
            if (_currentState == InteractionState.GunVisible)
            {

                ChangeEquippedIttem(equippedItem);
                GunInvisible();

            }
        }


    }

    #endregion


    #region Client

    [ClientRpc]
    public void GunInvisible()
    {
        foreach (var gunPrefab in gunPrefabList)
        {
            gunPrefab.SetActive(false);
        }
    }



    public void SelectedGun(Gun.EquippedItems newEquippedItem)
    {

        switch (newEquippedItem)
        {

            case Gun.EquippedItems.Smg:

                gunPrefabList[7].SetActive(true);
                _currentState = InteractionState.GunVisible;

                break;

            case Gun.EquippedItems.Shotgun:

                gunPrefabList[6].SetActive(true);
                _currentState = InteractionState.GunVisible;


                break;

            case Gun.EquippedItems.Sniper:

                gunPrefabList[8].SetActive(true);
                _currentState = InteractionState.GunVisible;


                break;

            case Gun.EquippedItems.Bazooka:

                gunPrefabList[3].SetActive(true);
                _currentState = InteractionState.GunVisible;


                break;

            case Gun.EquippedItems.Pistol:

                gunPrefabList[5].SetActive(true);
                _currentState = InteractionState.GunVisible;


                break;


            case Gun.EquippedItems.Ak47:

                gunPrefabList[0].SetActive(true);
                _currentState = InteractionState.GunVisible;


                break;

            case Gun.EquippedItems.M4A1:

                gunPrefabList[4].SetActive(true);
                _currentState = InteractionState.GunVisible;


                break;

            case Gun.EquippedItems.ARLong:

                gunPrefabList[1].SetActive(true);
                _currentState = InteractionState.GunVisible;


                break;

            case Gun.EquippedItems.ARPro:

                gunPrefabList[2].SetActive(true);
                _currentState = InteractionState.GunVisible;

                break;

        }



    }

    #endregion






    #region SetGunScript

    private Transform FindClosestPlayer()
    {
        var closestDistance = float.MaxValue;
        Transform closestPlayer = null;

        foreach (var playerTransform in PlayerManager.ClientTransforms)
        {
            var distance = Vector3.Distance(transform.position, playerTransform.position);
            if (!(distance < closestDistance)) continue;
            closestDistance = distance;
            closestPlayer = playerTransform;
        }

        return closestPlayer;
    }

    private void CheckClosestPlayer()
    {
        var closestPlayer = FindClosestPlayer();

        if (closestPlayer == null) return;
        var distanceSqr = (transform.position - closestPlayer.position).sqrMagnitude;

        if (distanceSqr < CheckDistanceThreshold)
        {
            isPlayerNear = true;
            gun = closestPlayer.gameObject.GetComponent<Gun>();
            // Eğer Gun bileşeninin bir alt bileşeni veya bir fonksiyonu bu bileşene erişmek istiyorsa:
            if (gun != null)
            {
                gun._playerAction.wallGunGiver = this;
            }
        }
        else
        {
            isPlayerNear = false;
            gun = null;
        }

    }


    #endregion

}