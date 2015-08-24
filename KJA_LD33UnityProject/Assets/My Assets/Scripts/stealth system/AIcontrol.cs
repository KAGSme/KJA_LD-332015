using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum alertStatus
{
	//default
	calm,
	//saw enemy
	spotted,
	//run when on low hp
	flee,
	//move to area
	inspect,
	//watch person inspect
	watch,
    //everyone knows
    alert,
	//oh god, please help she's eating my soul, stop eating my soul, i need it for stuff
	assist
}

public enum charType
{
	guard,
	villager
}

public class AIcontrol : MonoBehaviour, CharMotor.DamageReceiver, Vision.Receiver
{

	public int Health = 100;
	public alertStatus curState;
	public CircleCollider2D alertOtherCol;
	float startTime;
	public float inspectToAlarmTime;
	public float inspectToCalmTime;
	public List<PatrolMarker> patrolRoute;
	int i = 0;  //worst variable naming of all time
	Vector3 lootAtPoint;
	public charType type;
	//public Vector3 safeZone;
    public bool isStationary;
    public bool randomPatrol = false; //jim -- was too lazy to make several patrol paths

    public LayerMask HostileMask;

    public bool Hostile = false;

    public static float MaxVisRange = 0;
    static CharMotor player;
    Vector2 LSpottedPos;
   /* public float attackRateOfFire;
    public int Damage;
	public float range;
	bool canAttack;
	float attackTime; */

	[System.Serializable]
	public class Attack
	{
		public float Range = 1;
		public float RoF = 1;
		public int Damage = 1;
		public float Duration = 0.3f;
		[HideInInspector]
		public float LastAttack;
		[HideInInspector]
		public CharMotor LastTarget;
	};

	public List<Attack> Attacks;

	Attack CurAttack;


	
	static stealthSounds audio;

	CharMotor Threat;
	[HideInInspector]
	public CharMotor Mtr;
	Vision Vis;
	Repulsor Rep;
	Animation Anim;
    LineRenderer LnRndr;

    public float ThreatLevel = 0;
    float LSpotted;
    bool Spotted;


    float DefSpeed;
    bool canRegen = true;
    float regenCoolDown;

	void Awake()
	{
		//player = GameObject.Find("Monster Token");
		player = FindObjectOfType<PlayerController>().GetComponent<CharMotor>();
		audio = FindObjectOfType<stealthSounds>().GetComponent<stealthSounds>();
        Mtr = GetComponent<CharMotor>();
        Anim = GetComponentInChildren<Animation>();
        Vis = GetComponentInChildren<Vision>();
        Rep = GetComponent<Repulsor>();
        LnRndr = GetComponentInChildren<LineRenderer>();
        Vis.Recv = this;
        MaxVisRange = Mathf.Max(MaxVisRange, Vis.Radius);
	}

	void Start()
	{

		DefSpeed = Mtr.Speed;
		if (randomPatrol)
		{
			i = Random.Range(0, patrolRoute.Count);
		}
	}
	void Update()
	{

        if(!Spotted) {
            ThreatLevel -= Time.deltaTime;
        } else {
            ThreatLevel = Mathf.Max(0, ThreatLevel);
            ThreatLevel += Time.deltaTime;

            LSpotted = Time.deltaTime;
            LSpottedPos = player.Trnsfrm.position; 
            Spotted = false;
        }

        LnRndr.enabled = false;

        if(CurAttack != null) {
            // DesVec = Vector2.zero;
            if(Time.fixedTime - CurAttack.LastAttack > CurAttack.Duration) {
                //Debug.Log("dmg " + CurAttack.Damage);
                if(Mtr.Target == CurAttack.LastTarget && Mtr.Target != null) Mtr.Target.applyDamage(CurAttack.Damage, Mtr);
                CurAttack = null;
                Vis.enabled = true;
            } 
        }

		Rep.enabled = true;
		Mtr.Speed = DefSpeed;
		switch (curState)
		{
			case alertStatus.calm:
				if (isStationary == false)
				{
					Patrol();
				}
				if (isStationary == true)
				{
					//Debug.Log("stat 0");
					stationary();
				}
				break;
			case alertStatus.spotted:
				spotted();
				/*//Debug.Log("countdown time: " + (Time.time - startTime));
				if (Time.time - startTime > inspectToAlarmTime)
				{
					//Debug.Log("countdown done");
					//startTime = Time.time;
					changeStatus(alertStatus.alert);
				}
				else
				{

                    /*
					Vector2 direction = lootAtPoint - transform.position;
					RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, 100, LayerMask.GetMask("Player"));
					Debug.DrawLine(transform.position, lootAtPoint, Color.red);
					if (!hit)
					{
						//Debug.Log("no hit");
						changeStatus(alertStatus.inspect);
					}
					else
					{
						//Debug.Log("hit" + hit.collider.gameObject.name);
					}* /
				} */
				break;
			case alertStatus.inspect:
			    inspect();
			/*	//Debug.Log("countdown time: " + (Time.time - startTime));
				if (Time.time - startTime > inspectToCalmTime)
				{
					//Debug.Log("countdown done");
					changeStatus(alertStatus.calm);
					alertOtherCol.enabled = false;
				}*/
				break; 
			case alertStatus.watch:
				//watch();
				if (Time.time - startTime > inspectToCalmTime)
				{
					//Debug.Log("countdown done");
					changeStatus(alertStatus.calm);
					alertOtherCol.enabled = false;
				}
				break;
			case alertStatus.alert:
				alert();
				break;
			case alertStatus.flee:
				regenHP();
				break;
            case alertStatus.assist:
                assist();
                break;
			default:
				break;
		}
	}

	//walk from point to point
	//done
	void Patrol()
	{

        if(Health <= 75 ) {
            changeStatus(alertStatus.flee);
            return;
        }
		//Debug.Log("patrol");
		Mtr.setTarget(patrolRoute[i].position);
		float leeway = 1.2f; //jim - bbigger leeway = stuck less
		if (Mtr.Trnsfrm.position.x > patrolRoute[i].position.x - leeway && Mtr.Trnsfrm.position.x < patrolRoute[i].position.x + leeway)
		{
			if (Mtr.Trnsfrm.position.y > patrolRoute[i].position.y - leeway && Mtr.Trnsfrm.position.y < patrolRoute[i].position.y + leeway)
			{

				if (randomPatrol)
				{
					i = Random.Range(0, patrolRoute.Count);
				}
				else
				{
					if (i + 1 == patrolRoute.Count)
					{
						//Debug.Log("change: full rotation");
						i = 0;
					}
					else
					{
						//Debug.Log("change: next point");
						i++;
					}
				}
				Mtr.setTarget(patrolRoute[i].position);
			}
		}
		//Debug.Log(i);
	}

	void stationary() //jim - wtf..? alex - its when they stand still they need to return to there original position.
	{
		//Debug.Log("stat 1");
		if (Mtr.Trnsfrm.position.x > patrolRoute[0].position.x - 0.1 && Mtr.Trnsfrm.position.x < patrolRoute[0].position.x + 0.1)
		{
			//Debug.Log("stat 2");
			if (Mtr.Trnsfrm.position.y > patrolRoute[0].position.y - 0.1 && Mtr.Trnsfrm.position.y < patrolRoute[0].position.y + 0.1)
			{
				//Debug.Log("stat 3");
				if (Mtr.Trnsfrm.rotation.z != patrolRoute[0].lookDirection.z)
				{
					//Debug.Log("stat 4");
					Vector3 vectorToTarget = patrolRoute[0].lookDirection - Mtr.Trnsfrm.position;
					float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
					Quaternion q = Quaternion.AngleAxis(angle - 180, Vector3.forward);
					transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 10);
				}
			}
		}
	}

	//if can see player either go to alert or investigate
	void spotted()
	{
		
        Mtr.Target = player; //look at player
        Mtr.Speed = 0; //stop walking

        if(Vis.check(player.Trnsfrm.position)) {
            LnRndr.enabled = true;
            LnRndr.SetPosition(0, Mtr.Trnsfrm.position + Vector3.back);
            LnRndr.SetPosition(1, player.Trnsfrm.position + Vector3.back);

            Spotted = true;
            if(ThreatLevel > inspectToAlarmTime)  {
                Threat = player;
                changeStatus(alertStatus.alert);
            }
        } else if ( Time.fixedTime - LSpotted  > 0.5f )  {
            changeStatus(alertStatus.inspect);
        }
			
/*		Vector3 vectorToTarget = lootAtPoint - Mtr.Trnsfrm.position;
		float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
		Quaternion q = Quaternion.AngleAxis(angle - 180, Vector3.forward);
		transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 10);
 * */
		//if looking at time is longer go to alert status	
	}

	//look at the last know position for the player
	//done
	void inspect()
	{
		//alert other enemys
		//alertOtherCol.enabled = true;
		//move to players last know location
        Mtr.setTarget(LSpottedPos);

        if(Vis.check(player.Trnsfrm.position)) {
            LnRndr.enabled = true;
            LnRndr.SetPosition(0, Mtr.Trnsfrm.position + Vector3.back);
            LnRndr.SetPosition(1, player.Trnsfrm.position + Vector3.back);

            Spotted = true;
            if(ThreatLevel > inspectToAlarmTime) {
                Threat = player;
                changeStatus(alertStatus.alert);
            }

        } else if(ThreatLevel < -2.0f ) {
            changeStatus(alertStatus.calm);
        }
//        Mtr.Targe;
	}

/*	//watch the person who went to investigate. if they die then go on alert 
	//done
	void watch()
	{
		//stop moving
		Mtr.setTarget(new Vector2(Mtr.Trnsfrm.position.x, Mtr.Trnsfrm.position.y));
		//watch target 
		Vector3 vectorToTarget = lootAtPoint - transform.position;
		float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
		Quaternion q = Quaternion.AngleAxis(angle - 180, Vector3.forward);
		transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 10);
	}
    */
    public class DuplicateKeyComparer<K> : IComparer<K> where K : System.IComparable {
        public int Compare(K x, K y) {
            int result = x.CompareTo(y);
            if(result == 0) return 1;
            else return -result;  // (-) invert 
        }
    }

    int LowHealth = 70;
    float ShoutTimer;
	void alert()
	{
        if(Health <= LowHealth )
		{
			changeStatus(alertStatus.flee);
            return;
		}
		if (type == charType.villager)
		{
            if(((Vector2)Mtr.Trnsfrm.position - Mtr.TargetP).sqrMagnitude < 0.75f) {
                if(Time.fixedTime - LSpotted > 5.0f)
                    changeStatus(alertStatus.calm);
            }
            //Debug.Log("go to safe zone");
           // Mtr.setTarget(SafeZoneWP.getP());
		}
		else if (type == charType.guard)
		{
           // Debug.Log(" alert " + Threat);
            if( Threat == null || ThreatLevel < -3.0f ) {
                changeStatus( alertStatus.calm ); 
                return;
            }

			Mtr.Target = Threat;

			var vec = ((Vector2)Threat.Trnsfrm.position - (Vector2)Mtr.Trnsfrm.position);
			var mag = vec.magnitude; vec /= mag;
			//Debug.Log("mag " + mag + "  dt " + Vector2.Dot(vec, -RotObj.transform.up));
			if (Vector2.Dot(vec, -Mtr.RotObj.transform.up) > 0.8f)
			{
				bool inRange = true;
				foreach (var a in Attacks)
				{
					if (mag > a.Range)
					{
						inRange = false;
						continue;
					}
					if (Time.fixedTime - a.LastAttack > a.Duration + a.RoF)
					{
						CurAttack = a;
						if (Anim != null) Anim.Play();
						a.LastAttack = Time.fixedTime;
						a.LastTarget = Threat;

						Vis.enabled = false;
						break;
					}
				}
				if (inRange)
				{
					Mtr.Speed = 0;
					Rep.enabled = false;
				}
			}

            if(CurAttack == null && (Time.fixedTime - ShoutTimer > 1.5f ) ) {
                LowHealth = 75;

                SortedList<int, AIcontrol> allies = new SortedList<int, AIcontrol>(new DuplicateKeyComparer<int>());
                foreach( var c in Physics2D.OverlapCircleAll( Mtr.Trnsfrm.position, 15,  1 << gameObject.layer ) ) {
                    var ai = c.GetComponent<AIcontrol>();
                    if( ai.type == charType.villager ) continue;
                    if(ai.curState == alertStatus.calm || (ai.curState == alertStatus.flee && ai.Health > 50)) {
                        /*ai.Mtr.setTarget(Mtr.Trnsfrm.position);
                        ai.changeStatus(alertStatus.assist);
                        LowHealth = Mathf.Max( 50, LowHealth- 5 );
                        ai.LowHealth = Mathf.Min( ai.LowHealth, ai.Health );
                       // Debug.Log("help"); */
                        allies.Add(ai.Health, ai);
                    } else if(ai.curState == alertStatus.alert) LowHealth = Mathf.Max(50, LowHealth - 5);
                }
                //if( allies.Count == 0 ) return;
                //Debug.Log(" allies " + allies.Count );
                foreach(var e in allies) {
                    var ai = e.Value;

                    if(ai.Health < LowHealth) break;

                    ai.Mtr.setTarget(Mtr.Trnsfrm.position);
                    ai.changeStatus(alertStatus.assist);
                    LowHealth = Mathf.Max(50, LowHealth - 5);
                    ai.LowHealth = Mathf.Min(ai.LowHealth, LowHealth);
                }
            }

			/*if(canAttack == false) {
				if(Time.time - attackTime > attackRateOfFire) {
					canAttack = true;
				}
			}
			//follow and attack player
			Mtr.setTarget(player.transform.position);
			if (Vector2.Distance(Mtr.Trnsfrm.position, player.transform.position) <= range && canAttack == true)
			{
				//deal damage
				player.GetComponent<PlayerData>().IncreaseHP(Damage);
				canAttack = false;
			}
			*/
		}
	}

    void assist() {
        if(((Vector2)Mtr.Trnsfrm.position - Mtr.TargetP).sqrMagnitude < 0.75f)
            changeStatus(alertStatus.calm);
    }
	public void ensureStatus(alertStatus newStatus)
	{
		if (curState != newStatus) changeStatus(newStatus);
	}
	public void changeStatus(alertStatus newStatus)
	{

		curState = newStatus;
		Debug.Log("new state: " + newStatus);
		if (curState == alertStatus.calm)
		{
			audio.playCalm();
			//canAttack = true;
			if (isStationary == true)
			{
				Mtr.setTarget(patrolRoute[0].position);
			}
		}
		else if (newStatus == alertStatus.spotted)
		{
			audio.playSpotted();
			//spotted();
		}
		else if (newStatus == alertStatus.inspect)
		{
			//inspect();
		}
		else if (newStatus == alertStatus.alert)
		{

            if(type == charType.villager) {
                //Debug.Log("go to safe zone");
                Mtr.setTarget(SafeZoneWP.getP());
            }

            if(Threat == player) {
                Hostile = true;
                Vis.Layers = HostileMask;
                GetComponentInChildren<SpriteRenderer>().color = Color.red;
            }
			audio.playAlarm();
			//alert();
		}
		else if (newStatus == alertStatus.flee)
		{
            Mtr.setTarget(SafeZoneWP.getP());
		}
	}

	/*public void changeStatus(alertStatus newStatus, GameObject watchThis)
	{
		curState = newStatus;
		startTime = Time.time;
		//Debug.Log("new state: " + newStatus);
		watch();
	}*/

	public alertStatus getStatus()
	{
		return curState;
	}

	public void seePlayer(Transform playerPos)
	{

		if (curState == alertStatus.calm)
		{
			startTime = Time.time;
			lootAtPoint = playerPos.position;
			//Debug.Log(lootAtPoint);
			changeStatus(alertStatus.spotted);
		}
		else if (curState == alertStatus.inspect || curState == alertStatus.watch)
		{
			startTime = Time.time;
			lootAtPoint = playerPos.position;
			//Debug.Log(lootAtPoint);
			changeStatus(alertStatus.alert);

			newThreat(player);
		}
	}

/*	public void lostPlayer()
	{
		if (curState == alertStatus.spotted)
		{
			changeStatus(alertStatus.inspect);
		}
		else if (curState == alertStatus.alert)
		{
			changeStatus(alertStatus.inspect);
		}
	} */


	public void recvDamage(int dmg, CharMotor src)
	{

		// if(this == null) return;
        if ((Health -= dmg) <= 0)
        {
            Debug.Log("dead villager");
            VillageStatus.vStatus.IncreaseDeathCount();
            Destroy(gameObject);
        }
        else if (src != null) spotted(src);
	}


	void newThreat(CharMotor mtr)
	{
		if (mtr == Threat) return;

        if(Threat != null) {
            if((Threat.Trnsfrm.position - Mtr.Trnsfrm.position).sqrMagnitude < (Mtr.Trnsfrm.position - mtr.Trnsfrm.position).sqrMagnitude)
                return;
        }
        Threat = mtr;
    }
    public void spotted(CharMotor mtr) { //for enemies
     //   Debug.Log("spotted " + mtr );

		newThreat(mtr);
        Spotted = true;
        if(Threat == mtr && mtr == player && !Hostile ) {
            ThreatLevel = Mathf.Max( ThreatLevel+ 0.5f, 0 );
            if( curState != alertStatus.spotted && curState != alertStatus.inspect ) 
                changeStatus(alertStatus.spotted);
            return;
        }		
        ensureStatus(alertStatus.alert);        
    }


	void regenHP()
	{   
        
		if (Health >= 100)
		{
			changeStatus(alertStatus.calm);
		}
		else if (canRegen == true)
		{
			regenCoolDown = Time.time;
			Health++;
			canRegen = false;
		}
		else if (Time.time - regenCoolDown > 0.5 )
		{
			canRegen = true;
		}
	}
	public void check(CharMotor plyr) {

        if(Hostile) return;
        if(curState == alertStatus.calm
            || (curState == alertStatus.alert
                && (Threat == null || (plyr.Trnsfrm.position - Mtr.Trnsfrm.position).sqrMagnitude < (Threat.Trnsfrm.position - Mtr.Trnsfrm.position).sqrMagnitude))) {

            if( Vis.check(plyr.Trnsfrm.position) ) {
                Spotted = true;
                //if(curState == alertStatus.alert) 
                ThreatLevel = 0;
                changeStatus(alertStatus.spotted);
            }
        }



    }

}
