using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody2D))]
public class AcidSpit : MonoBehaviour {

    Vector3 destination;
    Rigidbody2D rb;
    public float speed;


	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}
	
	void FixedUpdate () {
        rb.AddRelativeForce(new Vector2(0, speed));
	}

    void Update() 
    {
        if (Mathf.Abs(Vector3.Magnitude(destination - transform.position)) < 4.0f)
        {
            Explode();
        }
    }

    public void SetDestination(Vector3 setDestination)
    {
        destination = setDestination;
    }

    void Explode()
    {

    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag != "Player")
        {
            Explode();
        }
    }
}
