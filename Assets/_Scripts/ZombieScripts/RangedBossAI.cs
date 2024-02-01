using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Pathfinding;

public class RangedBossAI : NetworkBehaviour
{

    private Transform currentTarget;
    private Transform closestPlayer;
    private Seeker seeker;
    private CharacterController controller;
    private Path path;
    
    public float speed = 1;
    public float nextWaypointDistance = 3;
    private int currentWaypoint = 0;
    public float repathRate = 0.5f;
    private float lastRepath = float.NegativeInfinity;
    public bool reachedEndOfPath;
    private float lastAttackTime;
    private bool isAttacking = false;
    public float attackDuration = 1f;
    
    public GameObject Zombie;
    
    [SyncVar]
    public ZombieState currentState;
    [SyncVar]
    private ZombieState previousState;
    
    public enum ZombieState {
        Chase,
        Attack1,
        Attack2,
        Attack3,
        Dead
    }

    [SyncVar]
    public int zombieHealth = 100;
    
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
            // Reset the waypoint counter so that we start to move towards the first point in the path
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
        
        ZombieMove();
    }

    private void ZombieMove()
    {
        currentTarget = FindClosestPlayer();

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
        
        transform.forward = currentTarget.transform.position - transform.position;

        switch (currentState)
        {
            case ZombieState.Chase:

                if (currentTarget != null)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
                    
                    if (distanceToTarget < 10f)
                    {
                        int randomAttack = Random.Range(1, 4);
                        
                        switch (randomAttack)
                        {
                            case 1:
                                SetState(ZombieState.Attack1);
                                break;
                            case 2:
                                SetState(ZombieState.Attack2);
                                break;
                            case 3:
                                SetState(ZombieState.Attack3);
                                break;
                        }
                    }
                }
                break;

            case ZombieState.Attack1:
            case ZombieState.Attack2:
            case ZombieState.Attack3:
                
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
        NetworkServer.Destroy(Zombie);
        Destroy(Zombie);
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
    
    [Command]
    public void CmdTakeDamage(int damage) {
        zombieHealth -= damage;
        if (zombieHealth <= 0) {
            SetState(ZombieState.Dead);
        }
    }
    
    public void SetState(ZombieState newState)
    {
        currentState = newState;
    }
}
