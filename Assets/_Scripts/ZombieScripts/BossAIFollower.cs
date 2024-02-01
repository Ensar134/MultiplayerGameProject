using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Mirror;

public class BossAIFollower : NetworkBehaviour, IDamageableInterface
{
    public Transform aiTarget;
    public BossAI.ZombieState currentState;
    public float bossHealt = 100f;
    public BossAI bossAI;
    
    private Animator _animator;
    private bool _isDead;

    public ParticleSystem smashParticle;
    public ParticleSystem jumpAttackParticle;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isServer == false) return;
        
        if (aiTarget == null) return;
        
        transform.position = new Vector3(aiTarget.position.x, 0, aiTarget.position.z);
        
        transform.rotation = aiTarget.rotation;
        
        if (aiTarget != null) {
            BossAI zombieAI = aiTarget.GetComponent<BossAI>();
            if (zombieAI != null) {
                currentState = zombieAI.currentState;
            }
        }

        switch (currentState)
        {
            case BossAI.ZombieState.Chase:
                
                _animator.SetBool("Attack1",false);
                _animator.SetBool("Attack2",false);
                _animator.SetBool("JumpAttack",false);
                
                break;

            case BossAI.ZombieState.Attack1:
                
                //smash attack
                _animator.SetBool("Attack1", true);
                _animator.SetBool("Attack2",false);
                _animator.SetBool("JumpAttack",false);

                break;

            case BossAI.ZombieState.Attack2:

                //punch attack
                _animator.SetBool("Attack1", false);
                _animator.SetBool("Attack2",true);
                _animator.SetBool("JumpAttack",false);
                
                break;

            case BossAI.ZombieState.JumpAttack:
                
                _animator.SetBool("Attack1", false);
                _animator.SetBool("Attack2",false);
                _animator.SetBool("JumpAttack",true);

                break;
            
            case BossAI.ZombieState.Dead:
                
                _animator.SetBool("IsDead", true);
                Destroy(gameObject,5f);
                
                break;
        }
    }
    
    public void TakeDamage(int damage)
    {
        bossHealt -= damage;
        
        if(bossHealt <= 0f)
        {
            bossAI.SetState(BossAI.ZombieState.Dead);
            bossAI.ZombieDead();
            _isDead = true;
        }
    }
    
    [ClientRpc]
    public void SmashAttack()
    {
        smashParticle.transform.position = gameObject.transform.position;
        smashParticle.transform.forward = gameObject.transform.forward;
        smashParticle.transform.SetParent(null, true);
        smashParticle.Play();
    }
    
    [ClientRpc]
    public void JumpAttack()
    {
        jumpAttackParticle.transform.position = new Vector3(gameObject.transform.position.x, 0.2f, gameObject.transform.position.z);
        jumpAttackParticle.transform.forward = gameObject.transform.forward;
        jumpAttackParticle.transform.SetParent(null, true);
        jumpAttackParticle.Play();
    }
}
