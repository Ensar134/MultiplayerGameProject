using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.VFX;

public class AIFollower : NetworkBehaviour, IDamageableInterface
{
    public Transform aiTarget;
    public ZombieAI.ZombieState currentState;
    private Animator _animator;
    public GameObject attackCollider;
    public ZombieAI zombieAI;
    private bool _isDead;

    [SyncVar]
    public float dummyHealth = 100f;
    
    [SerializeField] GameObject _bloodVfxEffect;
    
    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isServer == false) return;

        if (_isDead == false)
        {
            transform.position = new Vector3(aiTarget.position.x, 0f, aiTarget.position.z);
            transform.rotation = aiTarget.rotation;
        }
        
        if (aiTarget != null) {
            
            if (zombieAI != null) {
                currentState = zombieAI.currentState;
            }
        }
        
        if (currentState == ZombieAI.ZombieState.Dead)
        {
            _animator.SetBool("IsDead", true);
            Destroy(gameObject,5f);
        }

        if (currentState == ZombieAI.ZombieState.Attack)
        {
            _animator.SetBool("IsAttacking", true);
        }

        if (currentState == ZombieAI.ZombieState.Chase)
        {
            _animator.SetBool("IsAttacking", false);
        }
    }
    
    public void TakeDamage(int damage)
    {
        dummyHealth -= damage;

        if(dummyHealth <= 0f)
        {
            zombieAI.SetState(ZombieAI.ZombieState.Dead);
            zombieAI.ZombieDead();
            _isDead = true;
        }

        RpcBloodEffect(); 
    }
    
    private void RpcBloodEffect()
    {
        GameObject bloodVfx = Instantiate(_bloodVfxEffect, transform.position, Quaternion.identity);
        Destroy(bloodVfx, 2f);
    }
    
    public void OpenAttackCollider()
    {
        attackCollider.SetActive(true);
    }
    
    public void CloseAttackCollider()
    {
        attackCollider.SetActive(false);
    }
}
