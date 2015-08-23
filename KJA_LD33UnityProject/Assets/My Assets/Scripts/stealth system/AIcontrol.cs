using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum alertStatus
	{
		//default
		calm,
		//saw enemy
		spotted,
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

public class AIcontrol : MonoBehaviour {

	public alertStatus curState;
	public CircleCollider2D alertOtherCol;
	float startTime;
	public float inspectToAlarmTime;
	public float inspectToCalmTime;
	public List<PatrolMarker> patrolRoute;
	int i = 0;
	Vector3 lootAtPoint;
	public charType type;
	public Vector3 safeZone;
	public bool stationary;

	void Update()
	{
		switch (curState)
		{
			case alertStatus.calm:
				if (stationary == false)
				{
					Patrol();
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
						Debug.Log("no hit");
						changeStatus(alertStatus.inspect);
					}
					else
					{
						Debug.Log("hit" + hit.collider.gameObject.name);
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
			default:
				break;
		}
	}

	//walk from point to point
	//done
	void Patrol()
	{
		//Debug.Log("patrol");
		this.GetComponent<CharMotor>().setTarget(patrolRoute[i].position);
		if (this.transform.position.x > patrolRoute[i].position.x - 0.1 && this.transform.position.x < patrolRoute[i].position.x + 0.1)
		{
			if (this.transform.position.y > patrolRoute[i].position.y - 0.1 && this.transform.position.y < patrolRoute[i].position.y + 0.1)
			{
				if (i+1 == patrolRoute.Count)
				{
					//Debug.Log("change: full rotation");
					i = 0;
				}
				else
				{
					//Debug.Log("change: next point");
					i++;
				}
				this.GetComponent<CharMotor>().setTarget(patrolRoute[i].position);
			}
		}
		//Debug.Log(i);
	}

	//if can see player either go to alert or investigate

	void spotted()
	{   
		//stop walking
		GetComponent<CharMotor>().setTarget(new Vector2(this.transform.position.x, this.transform.position.y));
		//look at player
		Debug.Log(lootAtPoint);
		Vector3 vectorToTarget = lootAtPoint - this.transform.position;
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
		this.GetComponent<CharMotor>().setTarget(lootAtPoint);	
	}

	//watch the person who went to investigate. if they die then go on alert 
	//done
	void watch()
	{	
		//stop moving
		this.GetComponent<CharMotor>().setTarget(new Vector2(this.transform.position.x, this.transform.position.y));
		//watch target 
		Vector3 vectorToTarget = lootAtPoint - transform.position;
		float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
		Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
		transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 10);
	}

	void alert()
	{
		if (type == charType.villager)
		{
			//Debug.Log("go to safe zone");
			this.GetComponent<CharMotor>().setTarget(safeZone);
		}
		else if (type == charType.guard)
		{
			//follow and attack player
		}
	}
	
	public void changeStatus(alertStatus newStatus)
	{
		curState = newStatus;
		Debug.Log("new state: " + newStatus);
		if (curState == alertStatus.calm)
		{
			if (stationary == true)
			{
				this.GetComponent<CharMotor>().setTarget(patrolRoute[0].position);
			}
		}
		else if (newStatus == alertStatus.spotted)
		{
			spotted();
		}
		else if (newStatus == alertStatus.inspect)
		{
			inspect();
		}
		else if (newStatus == alertStatus.alert)
		{
			alert();
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

}
