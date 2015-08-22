using UnityEngine;
using System.Collections;

public class AlertOtherCollider : MonoBehaviour {

	public AIcontrol attatchedEnemy;

	void OnTriggerEnter2D(Collider2D other)
	{
		Debug.Log("enter " + other.name);

		if (other.gameObject.tag == "Enemy")
		{
			other.GetComponent<AIcontrol>().changeStatus(alertStatus.watch, this.gameObject);
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		Debug.Log("enter " + other.name);

		if (other.gameObject.tag == "Enemy")
		{
			other.GetComponent<AIcontrol>().changeStatus(alertStatus.calm);
		}
	}
}
