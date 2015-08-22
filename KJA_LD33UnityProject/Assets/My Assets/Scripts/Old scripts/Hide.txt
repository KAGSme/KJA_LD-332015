using UnityEngine;
using System.Collections;

public class Hide : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerCharacter_Health>().IsInvisible = true;
            other.gameObject.GetComponentInChildren<Renderer>().material.color = new Color32(137, 137, 137,137);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerCharacter_Health>().IsInvisible = false;
            other.gameObject.GetComponentInChildren<Renderer>().material.color = new Color32(255, 255, 255, 255);
        }
    }
}
