using UnityEngine;
using System.Collections;

public class AlertOtherCollider : MonoBehaviour {

	public AIcontrol attatchedEnemy;

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Enemy")
		{
			Debug.Log("enter " + other.name);
			other.GetComponent<AIcontrol>().changeStatus(alertStatus.watch, this.gameObject);
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{	
		if (other.gameObject.tag == "Enemy")
		{
			Debug.Log("enter " + other.name);
			other.GetComponent<AIcontrol>().changeStatus(alertStatus.calm);
		}
	}
}
