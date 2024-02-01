using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ZombieAttack : MonoBehaviour
{
    public LayerMask breakableWallLayer;
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Damage Player");
            
            var damageable = other.gameObject.GetComponentInParent<IDamageableInterface>();
            if(damageable != null)
            {
                damageable.TakeDamage(10);
            }
        }
        
        if (other.gameObject.layer == 12)
        {
            Debug.Log("Damage Wall");
            
            Duvar wall = other.gameObject.GetComponentInParent<Duvar>();
            wall.Test();
        }
    }
}
