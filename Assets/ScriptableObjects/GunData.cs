using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.ShootingSystem;
using UnityEngine;
[CreateAssetMenu(fileName = "Gun", menuName = "Weapon/Gun")]

public class GunData : ScriptableObject
{
   [Header("Info")] 
   public new string name;
   [SerializeField] public Bullet _bulletPrefab ;
   [SerializeField] public int _bulletID ;

   [Header("Shooting")]
   public float damage;
   public float MaxTime; // bullet ;=> max time
   public float bulletSpeed;
  
   
   [Header("Reloading")] 
   public int currentAmmo; 
   public int maxAmmo;
   public float fireRate; // bullet => delay
   public float reloadTime;
  
  
} 
    



