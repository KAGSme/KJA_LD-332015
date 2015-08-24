using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody2D))]
public class AcidSpit : MonoBehaviour {

    Vector3 destination;
    Rigidbody2D rb;
    public float speed;
    public GameObject puddlePrefab;


	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}
	
	void FixedUpdate () {
        rb.AddRelativeForce(new Vector2(0, speed));
        Mathf.Clamp(rb.velocity.magnitude, 0, speed);
	}

    void Update() 
    {
        var distance = Vector3.Distance(new Vector3(destination.x, destination.y, 0), transform.position);
        if (distance < 3f)
        {
            Debug.Log("spit destination reached");
            Explode();
        }
        
    }

    public void SetDestination(Vector3 setDestination)
    {
        destination = setDestination;
        Debug.Log(setDestination);
    }

    void Explode()
    {
        GameObject puddle = (GameObject)Instantiate(puddlePrefab, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag != "Player")
        {
            Explode();
        }
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawLine(destination, transform.position);
    }
}
