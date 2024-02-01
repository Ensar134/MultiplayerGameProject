using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.VFX;

public class PatlayanAIFollower : NetworkBehaviour, IDamageableInterface
{
    public Transform aiTarget;
    public PatlayanZombieAI.ZombieState currentState;
    private Animator _animator;
    public ParticleSystem explosionParticle;
    public PatlayanZombieAI patlayanZombieAI;
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

        if (aiTarget != null) 
        {
            if (patlayanZombieAI != null) 
            {
                currentState = patlayanZombieAI.currentState;
            }
        }

        CheckStates();
    }

    private void CheckStates()
    {
        if (currentState == PatlayanZombieAI.ZombieState.Explode)
        {
            ActivateParticleAndDestroy();
        }

        if (currentState == PatlayanZombieAI.ZombieState.Chase)
        {
            _animator.SetBool("IsAttacking", false);
        }
        
        if (currentState == PatlayanZombieAI.ZombieState.Dead)
        {
            _animator.SetBool("IsDead", true);
            Destroy(gameObject,3f);
        }
    }
    
    public void ActivateParticleAndDestroy()
    {
        explosionParticle.gameObject.SetActive(true);
        _animator.SetBool("IsDead", true);
        Destroy(gameObject, 0.5f);
    }
    
    public void TakeDamage(int damage)
    {
        dummyHealth -= damage;

        if(dummyHealth <= 0f)
        {
            patlayanZombieAI.SetState(PatlayanZombieAI.ZombieState.Explode);
            patlayanZombieAI.ZombieDead();
            _isDead = true;
        }

        RpcBloodEffect(); 
    }
    
    private void RpcBloodEffect()
    {
        GameObject bloodVfx = Instantiate(_bloodVfxEffect, transform.position, Quaternion.identity);
        Destroy(bloodVfx, 2f);
    }
}