using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RangedBossAIFollower : NetworkBehaviour, IDamageableInterface
{
    public Transform aiTarget;
    public RangedBossAI.ZombieState currentState;
    public RangedBossAI rangedBossAI;
    
    private Animator _animator;
    private bool _isDead;

    public float bossHealt = 100f;
    public ParticleSystem rangedAttack1Particle;
    public ParticleSystem rangedAttack2Particle;
    
    public Transform rangedAttackSpawn;

    
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
            RangedBossAI zombieAI = aiTarget.GetComponent<RangedBossAI>();
            if (zombieAI != null) {
                currentState = zombieAI.currentState;
            }
        }

        switch (currentState)
        {
            case RangedBossAI.ZombieState.Chase:
                
                _animator.SetBool("Attack1",false);
                _animator.SetBool("Attack2",false);
                _animator.SetBool("Attack3",false);

                break;

            case RangedBossAI.ZombieState.Attack1:

                _animator.SetBool("Attack1", true);
                _animator.SetBool("Attack2",false);
                _animator.SetBool("Attack3",false);

                break;

            case RangedBossAI.ZombieState.Attack2:

                _animator.SetBool("Attack1", false);
                _animator.SetBool("Attack2",true);
                _animator.SetBool("Attack3",false);

                break;
            
            case RangedBossAI.ZombieState.Attack3:

                _animator.SetBool("Attack1", false);
                _animator.SetBool("Attack2",false);
                _animator.SetBool("Attack3",true);

                break;
            
            case RangedBossAI.ZombieState.Dead:
                
                _animator.SetBool("IsDead", true);
                
                break;
        }
    }
    
    public void TakeDamage(int damage)
    {
        bossHealt -= damage;
        
        if(bossHealt <= 0f)
        {
            rangedBossAI.SetState(RangedBossAI.ZombieState.Dead);
            rangedBossAI.ZombieDead();
            _isDead = true;
        }
    }
    
    [ClientRpc]
    public void RangedAttack1()
    {
        var transform1 = rangedAttack1Particle.transform;
        transform1.position = rangedAttackSpawn.transform.position;
        transform1.forward = gameObject.transform.forward;
        rangedAttack1Particle.GetComponent<Rigidbody>().velocity = transform1.forward * 10f;
        rangedAttack1Particle.transform.SetParent(null, true);
        rangedAttack1Particle.Play();
    }
    
    [ClientRpc]
    public void RangedAttack2()
    {
        var transform2 = rangedAttack2Particle.transform;
        transform2.position = rangedAttackSpawn.transform.position;
        transform2.forward = gameObject.transform.forward;
        rangedAttack2Particle.GetComponent<Rigidbody>().velocity = transform2.forward * 10f;
        rangedAttack2Particle.transform.SetParent(null, true);
        rangedAttack2Particle.Play();
    }
}
