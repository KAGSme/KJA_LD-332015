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

public class AIcontrol : MonoBehaviour {

	public alertStatus curState;
	public CircleCollider2D alertOtherCol;
	float startTime;
	public float inspectToAlarmTime;
	public List<PatrolMarker> patrolRoute;
	int i = 0;
	Vector3 tempPlayerPos;

	void Update()
	{
		switch (curState)
		{
			case alertStatus.calm:
				Patrol();
				break;
			case alertStatus.spotted:
				spotted();
				break;
			case alertStatus.inspect:
				inspect();
				break;
			case alertStatus.watch:
				watch();
				break;
			case alertStatus.alert:
				alert();
				break;
			default:
				break;
		}
	}

	void Patrol()
	{
		
		this.GetComponent<CharMotor>().setTarget(patrolRoute[i].position);
		if (this.transform.position.x > patrolRoute[i].position.x - 0.1 && this.transform.position.x < patrolRoute[i].position.x + 0.1)
		{
			if (this.transform.position.y > patrolRoute[i].position.y - 0.1 && this.transform.position.y < patrolRoute[i].position.y + 0.1)
			{
				if (i+1 == patrolRoute.Count)
				{
					Debug.Log("change: full rotation");
					i = 0;
				}
				else
				{
					Debug.Log("change: next point");
					i++;
				}
				this.GetComponent<CharMotor>().setTarget(patrolRoute[i].position);
			}
		}
		//Debug.Log(i);
	}

	void spotted()
	{
		//stop walking
		this.GetComponent<CharMotor>().setTarget(new Vector2(this.transform.position.x, this.transform.position.y));
		//look at player
		var angle = Mathf.Atan2(tempPlayerPos.y, tempPlayerPos.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
		//if looking at time is longer go to alert status
		if (Time.time - startTime > inspectToAlarmTime)
		{
			changeStatus(alertStatus.alert);
		}
	}

	void inspect()
	{
		//alert other enemys
		//move to players last know location
		//inpect area 
		//resume patrol
		curState = alertStatus.calm;
	}

	void watch()
	{
		//watch target 
	}

	void alert()
	{

	}
	
	public void changeStatus(alertStatus newStatus)
	{
		curState = newStatus;
		Debug.Log("new state: " + newStatus);
		if (newStatus == alertStatus.calm)
		{
			Patrol();
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
		Debug.Log("new state: " + newStatus);
		watch();
	}

	void alertOthers()
	{
		if (alertOtherCol.enabled == false)
		{
			alertOtherCol.enabled = true;
		}
		else
		{
			alertOtherCol.enabled = false;
		}
	}

	public void seePlayer(Vector3 playerPos)
	{
		changeStatus(alertStatus.spotted);
		startTime = Time.time;
	}

	public void lostPlayer()
	{
		changeStatus(alertStatus.inspect);
	}

}
