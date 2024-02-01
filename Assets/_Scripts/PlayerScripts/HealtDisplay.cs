using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class HealtDisplay : NetworkBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private Canvas healthCanvas;
    public float lerpSpeed = 5.0f;


    
    private void Awake()
    {
        playerHealth.ClientOnHealthUpdated += HandleHealtUpdated;
    }

    private void OnDestroy()
    {
        playerHealth.ClientOnHealthUpdated -= HandleHealtUpdated;
    }

    

    private void  HandleHealtUpdated(int currentHealth, int maxHealth)
    {
        float targetFillAmount = 1.0f - (float)currentHealth / maxHealth;
        healthBarImage.fillAmount = Mathf.Lerp(healthBarImage.fillAmount, targetFillAmount, lerpSpeed * Time.deltaTime);

    }
    public void OnEnable()
    {
        if (isLocalPlayer)
        {
            healthCanvas.gameObject.SetActive(true);
            healthCanvas.gameObject.transform.SetParent(null);
            
        }
        
    }
   
    
  
}
