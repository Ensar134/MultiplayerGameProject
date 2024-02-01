using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class PlayerHealth : NetworkBehaviour, IDamageableInterface
{
    //[SerializeField] private TextMeshProUGUI playerName;
    
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject[] disableUponDeath;
    [SyncVar] private bool isDead = false;
    public float maxHealth = 100;
    private float lerpSpeed;
    [SyncVar(hook = nameof(HandleHealtUpdated))]
    public float currentHealth;
    
    
    public event Action<int,int> ClientOnHealthUpdated;
    
    

    #region Client

   
    private void HandleHealtUpdated(float oldHealth, float newHealth)
    {
       ClientOnHealthUpdated?.Invoke((int)oldHealth,(int)newHealth);
    }

    #endregion
  
    

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
    }

    
    
    
   [Server]
    public void TakeDamage(int damageAmount)
    {
        RpcTakeDamage(damageAmount);
    }

    [ClientRpc]
    public void RpcTakeDamage(int damageAmount)
    {
        if (currentHealth == 0) { return; }
        currentHealth = Mathf.Max(currentHealth - damageAmount, 0);
        if (currentHealth == 0) { Die(); }
        UpdatePlayerHealthOnClients(currentHealth);
    }


   
    [Server]
    public void UpdatePlayerHealthOnClients(float newHealthValue)
    {
        RpcUpdateHealth(newHealthValue);
    }

    [ClientRpc]
    public void RpcUpdateHealth(float newHealth)
    {
        if (!NetworkServer.active)
        {
            Debug.LogError("Sunucu aktif değil");
            return;
        }
        if (isLocalPlayer)
        {
            currentHealth = newHealth;
        }
    }

   

    [Server]
    public void Die()
    {
       
        RpcDie();
    }

    [ClientRpc]
    public void RpcDie()
    {
        isDead = true;
        _animator.SetBool("Die",true);
        NetworkServer.Destroy(gameObject);
        //SpectateWorldWhileDead();
        //HideGameObjectsUponDeath();
    }
    
    #region AfterUI
    bool DisplayHealthPoint(float _health, int pointNumber)
    {
        return ((pointNumber * 10) >= _health);
    }
    void Respawn()
    {
        currentHealth = maxHealth;
        // Oyuncunun başlangıç pozisyonuna veya belirlediğiniz bir noktaya geri dönmesini sağlayabilirsiniz.
    }
    
    private void HideGameObjectsUponDeath()
    {
        foreach (var item in disableUponDeath)
        {
            item.SetActive(false);
        }
    }
    
    private void ShowGameObjectsUponRespawn()
    {
        foreach (var item in disableUponDeath)
        {
            item.SetActive(true);
        }
    }
    
    private void SpectateWorldWhileDead()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        //Kamera ayarlarını değiştir. disabling the character controller component attached to my player
    }

    #endregion
   
  
}
