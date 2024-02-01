using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Pathfinding;

public class BossAI : NetworkBehaviour
{
    private Transform currentTarget;
    private Transform closestPlayer;
    private Seeker seeker;
    private CharacterController controller;    
    private Path path;
    
    public float speed = 1;
    public float nextWaypointDistance = 3;
    public float repathRate = 0.5f;
    
    private int currentWaypoint = 0;
    private float lastRepath = float.NegativeInfinity;
    private bool isAttacking = false;
    private float lastAttackTime;
    
    public bool reachedEndOfPath;
    public float attackDuration = 1f;

    [SyncVar]
    public ZombieState currentState;
    [SyncVar]
    private ZombieState previousState;
    
    public GameObject Zombie;
    
    public enum ZombieState {
        Chase,
        Attack1,
        Attack2,
        JumpAttack,
        Dead
    }

    [SyncVar]
    public int zombieHealth = 50;
    

    void Start() {
        
        controller = GetComponent<CharacterController>();
        seeker = GetComponent<Seeker>();
        seeker.pathCallback += OnPathComplete;

        SetState(ZombieState.Chase);
        

    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    public void OnDisable () {
        if (seeker != null)
        {
            seeker.pathCallback -= OnPathComplete;
        }
    }
    
    
    private void FixedUpdate()
    {
        if (!isServer) return;

        currentTarget = FindClosestPlayer();
        
        ZombieMove();
    }

    private void ZombieMove()
    {
        
        if (currentState == ZombieState.Dead) return;

        if (zombieHealth <= 0)
        {
            SetState(ZombieState.Dead);
            return;
        }
        
        if (Time.time - lastAttackTime > attackDuration)
        {
            isAttacking = false;
            SetState(ZombieState.Chase);
        }
        
        if (Time.time > lastRepath + repathRate && seeker.IsDone())
        {
            lastRepath = Time.time;

            if (currentTarget != null && !isAttacking)
            {
                seeker.StartPath(transform.position, currentTarget.position, OnPathComplete);
            }
        }

        switch (currentState)
        {
            case ZombieState.Chase:
    
                if (currentTarget != null)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

                    if (distanceToTarget > 17f)
                    {
                        SetState(ZombieState.JumpAttack);
                    }
                    
                    if (distanceToTarget < 3f)
                    {
                        int randomAttack = Random.Range(1, 3);
                        switch (randomAttack)
                        {
                            case 1:
                                SetState(ZombieState.Attack1);
                                break;
                            case 2:
                                SetState(ZombieState.Attack2);
                                break;
                        }
                    }
                }
                break;
    
            case ZombieState.Attack1:
            case ZombieState.Attack2:
            case ZombieState.JumpAttack:
                if (previousState != currentState && !isAttacking) 
                {
                    StartAttack();
                }
                break;
        }
    }

    private void StartAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
    }
    
    [ClientRpc]
    public void ZombieDead()
    {
        if (isServer)
        {
            NetworkServer.Destroy(Zombie);
        }
    }
    
    private Transform FindClosestPlayer()
    {
        float closestDistance = float.MaxValue;

        foreach (Transform playerTransform in PlayerManager.ClientTransforms)
        {
            Vector3 playerPosition = playerTransform.position;
            float distance = Vector3.Distance(transform.position, playerPosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = playerTransform;
            }
        }

        return closestPlayer;
    }
    
    public void SetState(ZombieState newState)
    {
        previousState = currentState;
        currentState = newState;
    }
    
    [Command]
    public void CmdTakeDamage(int damage) {
        zombieHealth -= damage;
        if (zombieHealth <= 0) {
            SetState(ZombieState.Dead);
        }
    }    
}
