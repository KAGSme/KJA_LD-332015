using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

enum EnemyState { PATROLLING, CHASING, SEARCHING};

public class EnemyAI : MonoBehaviour {
    
    
    public Transform[] patrolWayPoints;
    public float patrolSpeed = 10f;
    public float chaseSpeed = 20f;
    public float chaseTime = 5f;
    public float patrolWaitTime = 1f;
    public float searchWaitTime = 5f;
    public bool patrolLoop;
    public bool justLooksForPlayer;
    public int damageToPlayer;
    public float turnSpeed = 8;


    private EnemyLineofSight elSight;
    private int wayPointIndex = 0;
    private int wayPointIterator = 1;
    private Transform TargetDestination;
    private EnemyState enemyState = EnemyState.PATROLLING;
    private Rigidbody2D rigidbodyThis;
    private Vector3 originalPosition;

	// Use this for initialization
	void Awake () {
        wayPointIndex = 0;
        rigidbodyThis = gameObject.GetComponent<Rigidbody2D>();
        elSight = GetComponentInChildren<EnemyLineofSight>();
        originalPosition = transform.position;
	}

    void Start()
    {
        elSight.Rend.material.color = new Color32(160, 255, 170, 122);
        elSight.Rend.sortingLayerName = "MiddleGround";
    }

	// Update is called once per frame
	void Update () {
        switch(enemyState){
            case (EnemyState.PATROLLING):
                Patrolling();
                break;
            case (EnemyState.CHASING):
                Chasing();
                break;
            case (EnemyState.SEARCHING):
                Searching();
                break;
        }
	}

    float patrolWaitTimer = 0;
    void Patrolling()
    {
        if (elSight.PlayerInSight && !elSight.IsInvisible)
        {
            elSight.Rend.material.color = new Color32(255, 160, 160, 122);
            enemyState = EnemyState.CHASING;
            GameControl_MAIN.gameControl.PlayChaseMusic();
        }
        if (justLooksForPlayer)
        {
            TargetDestination = patrolWayPoints[wayPointIndex];
            patrolWaitTimer += Time.deltaTime;
            if (patrolWaitTimer >= patrolWaitTime)
            {
                patrolWaitTimer = 0;
                if (patrolWayPoints.Length != 1)
                {
                    if (wayPointIndex == (patrolWayPoints.Length - 1) && patrolLoop)
                    {
                        wayPointIndex = -1;
                    }
                    if (wayPointIndex == (patrolWayPoints.Length - 1) && !patrolLoop)
                    {
                        wayPointIterator = -1;
                    }
                    if (wayPointIndex == 0 && !patrolLoop)
                    {
                        wayPointIterator = 1;
                    }
                    wayPointIndex += wayPointIterator;
                }
            }
            LooksAtSimple(TargetDestination.position);
        }
        else
        {
            TargetDestination = patrolWayPoints[wayPointIndex];
            if (DestinationCloseDistanceCheck(TargetDestination.position))
            {
                patrolWaitTimer += Time.deltaTime;
                if (patrolWaitTimer >= patrolWaitTime)
                {
                    patrolWaitTimer = 0;
                    if (patrolWayPoints.Length != 1)
                    {
                        if (wayPointIndex == (patrolWayPoints.Length - 1) && patrolLoop)
                        {
                            wayPointIndex = -1;
                        }
                        if (wayPointIndex == (patrolWayPoints.Length - 1) && !patrolLoop)
                        {
                            wayPointIterator = -1;
                        }
                        if (wayPointIndex == 0 && !patrolLoop)
                        {
                            wayPointIterator = 1;
                        }
                        wayPointIndex += wayPointIterator;
                    }
                }
            }
            else
            {
                MoveTowardsSimple(TargetDestination.position, patrolSpeed);
            }
        }
    }

    float chaseTimer = 0;
    void Chasing()
    {
        MoveTowardsSimple(elSight.Player.transform.position, chaseSpeed);
        if (!elSight.PlayerInSight)
        {
            chaseTimer += Time.deltaTime;
            if (chaseTimer > chaseTime)
            {
                chaseTimer = 0;
                elSight.Rend.material.color = new Color32(255, 255, 160, 122);
                enemyState = EnemyState.SEARCHING;
            }
        }
        if (elSight.IsInvisible)
        {
            chaseTimer = 0;
            elSight.Rend.material.color = new Color32(255, 255, 160, 122);
            enemyState = EnemyState.SEARCHING;
        }
    }
    float searchWaitTimer = 0;
    void Searching()
    {
        if (!elSight.PlayerInSight || elSight.IsInvisible)
        {
            searchWaitTimer += Time.deltaTime;
            if (searchWaitTimer > searchWaitTime)
            {
                if (justLooksForPlayer)
                {
                    if (!DestinationCloseDistanceCheck(originalPosition))
                    {
                        MoveTowardsSimple(originalPosition, patrolSpeed);
                    }
                    else
                    {
                        searchWaitTimer = 0;
                        enemyState = EnemyState.PATROLLING;
                        GameControl_MAIN.gameControl.PlayMainMusic();
                        elSight.Rend.material.color = new Color32(160, 255, 170, 122);
                    }
                }
                else
                {
                    searchWaitTimer = 0;
                    enemyState = EnemyState.PATROLLING;
                    GameControl_MAIN.gameControl.PlayMainMusic();
                    elSight.Rend.material.color = new Color32(160, 255, 170, 122);
                }
            }
        }
        else 
        {
            elSight.Rend.material.color =  new Color32(255, 160, 160, 122);
            enemyState = EnemyState.CHASING; 
        }
    }

    void MoveTowardsSimple(Vector3 destination, float speed)
    {
        if (!GameControl_MAIN.gameControl.isPaused)
        {
            var direction = destination - transform.position;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation =  Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), 5 * Time.deltaTime);

            rigidbodyThis.AddRelativeForce(new Vector2(speed, 0));
        }
    }

    void LooksAtSimple(Vector3 destination)
    {
        var direction = destination - transform.position;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), turnSpeed * Time.deltaTime);
    }

    bool DestinationCloseDistanceCheck(Vector3 TargetDestination)
    {
        if (Mathf.Abs(Vector3.Magnitude(TargetDestination - transform.position)) < 4.0f)
        {
            return true;
        }
        else return false;
    }

    void OnDrawGizmosSelected() 
    {
        if (patrolWayPoints[0] != null)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < patrolWayPoints.Length; i++)
            {
                if (i != patrolWayPoints.Length - 1)
                {
                    Gizmos.DrawLine(patrolWayPoints[i].position, patrolWayPoints[i + 1].position);
                }
                if (i == patrolWayPoints.Length - 1 && patrolLoop)
                {
                    Gizmos.DrawLine(patrolWayPoints[i].position, patrolWayPoints[0].position);
                }
            }
        }
    }

    void OnCollisionStay2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Player" && enemyState == EnemyState.CHASING)
        {
            coll.gameObject.GetComponent<PlayerCharacter_Health>().HealthDecrease(damageToPlayer);
        }
    }
}
