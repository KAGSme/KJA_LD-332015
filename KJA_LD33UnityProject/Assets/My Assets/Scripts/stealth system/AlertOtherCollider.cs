using UnityEngine;
using System.Collections;

public class AlertOtherCollider : MonoBehaviour {

	public AIcontrol attatchedEnemy;
/*
	void OnTriggerStay2D(Collider2D other)
	{
		//Debug.Log("call " + other.name);
		if (other.gameObject.tag == "Enemy")
		{
			Debug.Log("call " + other.name);
			if (attatchedEnemy.getStatus() == alertStatus.alert)
			{
				other.GetComponent<AIcontrol>().changeStatus(alertStatus.alert);
			}
			else
			{
				other.GetComponent<AIcontrol>().changeStatus(alertStatus.watch, this.gameObject);
			}		
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{	
		if (other.gameObject.tag == "Enemy")
		{
			Debug.Log("no call " + other.name);
			other.GetComponent<AIcontrol>().changeStatus(alertStatus.calm);
		}
	} */
}
