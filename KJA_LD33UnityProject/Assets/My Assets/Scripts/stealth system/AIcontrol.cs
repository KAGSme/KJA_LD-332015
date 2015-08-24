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
	alert
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
	public Vector3 safeZone;
	public bool isStationary;
	public bool randomPatrol = false; //jim -- was too lazy to make several patrol paths
	static CharMotor player;
	/* public float attackRateOfFire;
	 public int Damage;
	 public float range;
	 bool canAttack;
	 float attackTime; */
	//stuff to regen HP
	bool canRegen;
	float regenCoolDown;
	

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


	public AudioClip audioCalm;
	public AudioClip audioSpotted;
	public AudioClip audioAlarm;
	AudioSource audio;

	CharMotor Threat;
	[HideInInspector]
	public CharMotor Mtr;
	Vision Vis;
	Repulsor Rep;
	Animation Anim;

	float DefSpeed;

	void Awake()
	{
		//player = GameObject.Find("Monster Token");
		player = FindObjectOfType<PlayerController>().GetComponent<CharMotor>();
		audio = GetComponent<AudioSource>();
		Mtr = GetComponent<CharMotor>();
		Anim = GetComponentInChildren<Animation>();
		Vis = GetComponentInChildren<Vision>();
		Rep = GetComponent<Repulsor>();
		Vis.Recv = this;
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

		if (CurAttack != null)
		{
			// DesVec = Vector2.zero;
			if (Time.fixedTime - CurAttack.LastAttack > CurAttack.Duration)
			{
				//Debug.Log("dmg " + CurAttack.Damage);
				if (Mtr.Target == CurAttack.LastTarget && Mtr.Target != null) Mtr.Target.applyDamage(CurAttack.Damage, Mtr);
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
				//spotted();
				//Debug.Log("countdown time: " + (Time.time - startTime));
				if (Time.time - startTime > inspectToAlarmTime)
				{
					//Debug.Log("countdown done");
					//startTime = Time.time;
					changeStatus(alertStatus.alert);
				}
				else
				{
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
					}
				}
				break;
			case alertStatus.inspect:
				//inspect();
				//Debug.Log("countdown time: " + (Time.time - startTime));
				if (Time.time - startTime > inspectToCalmTime)
				{
					//Debug.Log("countdown done");
					changeStatus(alertStatus.calm);
					alertOtherCol.enabled = false;
				}
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
			default:
				break;
		}
	}

	//walk from point to point
	//done
	void Patrol()
	{
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

	void stationary() //jim - wtf..?
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
		//stop walking
		Mtr.setTarget(new Vector2(Mtr.Trnsfrm.position.x, Mtr.Trnsfrm.position.y));
		//look at player
		Debug.Log(lootAtPoint);
		Vector3 vectorToTarget = lootAtPoint - Mtr.Trnsfrm.position;
		float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
		Quaternion q = Quaternion.AngleAxis(angle - 180, Vector3.forward);
		transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 10);
		//if looking at time is longer go to alert status	
	}

	//look at the last know position for the player
	//done
	void inspect()
	{
		//alert other enemys
		alertOtherCol.enabled = true;
		//move to players last know location
		Mtr.setTarget(lootAtPoint);
	}

	//watch the person who went to investigate. if they die then go on alert 
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


	void alert()
	{
		if (Health < 50)
		{
			changeStatus(alertStatus.flee);
		}
		if (type == charType.villager)
		{
			//Debug.Log("go to safe zone");
			Mtr.setTarget(safeZone);
		}
		else if (type == charType.guard)
		{
			
			// Debug.Log(" alert " + Threat);
			if (Threat == null)
			{
				changeStatus(alertStatus.calm);
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
					Rep.enabled = true;
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

	public void ensureStatus(alertStatus newStatus)
	{
		if (curState != newStatus) changeStatus(newStatus);
	}
	public void changeStatus(alertStatus newStatus)
	{

		curState = newStatus;
		//Debug.Log("new state: " + newStatus);
		if (curState == alertStatus.calm)
		{
			audio.clip = audioCalm;
			audio.Play();
			//canAttack = true;
			if (isStationary == true)
			{
				Mtr.setTarget(patrolRoute[0].position);
			}
		}
		else if (newStatus == alertStatus.spotted)
		{
			audio.clip = audioSpotted;
			audio.Play();
			spotted();
		}
		else if (newStatus == alertStatus.inspect)
		{
			inspect();
		}
		else if (newStatus == alertStatus.alert)
		{
			audio.clip = audioAlarm;
			audio.Play();
			alert();
		}
		else if (newStatus == alertStatus.flee)
		{
			Mtr.setTarget(safeZone);
		}
	}

	public void changeStatus(alertStatus newStatus, GameObject watchThis)
	{
		curState = newStatus;
		startTime = Time.time;
		Debug.Log("new state: " + newStatus);
		watch();
	}

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

	public void lostPlayer()
	{
		if (curState == alertStatus.spotted)
		{
			changeStatus(alertStatus.inspect);
		}
		else if (curState == alertStatus.alert)
		{
			changeStatus(alertStatus.inspect);
		}
	}


	public void recvDamage(int dmg, CharMotor src)
	{
		// if(this == null) return;
		if ((Health -= dmg) <= 0) Destroy(gameObject);
		else spotted(src);
	}


	void newThreat(CharMotor mtr)
	{
		if (mtr == Threat) return;

		if (Threat != null)
		{
			if ((Threat.Trnsfrm.position - Mtr.Trnsfrm.position).sqrMagnitude < (Mtr.Trnsfrm.position - mtr.Trnsfrm.position).sqrMagnitude)
				return;
		}
		Threat = mtr;
	}
	public void spotted(CharMotor mtr)
	{ //for enemies
		//Debug.Log("spotted " + mtr );

		newThreat(mtr);
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
		else if (Time.time - regenCoolDown > 1)
		{
			canRegen = true;
		}
	}
}
