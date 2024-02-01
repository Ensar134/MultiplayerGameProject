using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Examples.Basic;
using Pathfinding;
using Telepathy;

public class ZombieAI : NetworkBehaviour {
    
    private Transform currentTarget;
    private Transform closestPlayer;
    private Seeker seeker;
    private CharacterController controller;
    
    private Path path;
    public float speed = 2;
    public float nextWaypointDistance = 3;
    private int currentWaypoint = 0;
    public float repathRate = 0.5f;
    private float lastRepath = float.NegativeInfinity;
    public bool reachedEndOfPath;
    private float lastAttackTime;
    private bool isAttacking = false;
    public float attackDuration = 1f;
    
    public GameObject Zombie;

    [Header("Wall AI Test Properties")]
    public LayerMask breakableWallLayer;
    public float attackDistance = 1f;
    
    
    [SyncVar]
    public ZombieState currentState;
    [SyncVar]
    private ZombieState previousState;
    
    public enum ZombieState {
        Chase,
        Attack,
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
        
        currentTarget = FindClosestPlayer();
        
        ZombieMove();
    }
    
    private void CheckForBreakableWall(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2f, breakableWallLayer))
        { 
            Duvar wall = hit.collider.GetComponent<Duvar>();
            if (wall != null)
            {
                Debug.Log("Kırılabilir bir duvar bulundu!");
                SetState(ZombieState.Attack);
            }
        }
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
                    
                    if(distanceToTarget < 1.3f) { 

                        SetState(ZombieState.Attack);
                    }

                    else
                    {
                        if (path != null && currentWaypoint < path.vectorPath.Count)
                        {
                            Vector3 directionToNextWaypoint = (path.vectorPath[currentWaypoint] - transform.position).normalized;
                            CheckForBreakableWall(directionToNextWaypoint);
                        }
                    }
                }
                break;

            case ZombieState.Attack:
                
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
            Spawner.Instance.ZombieKilled();
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
}