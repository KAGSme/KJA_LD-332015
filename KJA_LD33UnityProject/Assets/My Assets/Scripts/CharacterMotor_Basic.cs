using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMotor_Basic : MonoBehaviour {

    public float acceleration = 100;
    public float maxSpeed = 100;
    public float deadzone = 3f;
    Rigidbody2D rigidbodyThis;
    Vector3 mouseDirection;

    CharMotor Motor;

	// Use this for initialization
	void Awake() {
        Motor = GetComponent<CharMotor>();
        rigidbodyThis = GetComponent<Rigidbody2D>();
	}

    // Update is called once per frame
    void Update()
    {
        var position = Camera.main.WorldToScreenPoint(transform.position);
        mouseDirection = Input.mousePosition - position;
    }
	
	
	void FixedUpdate () {
        if (DeadzoneCheck())
        {
            MovementForce(acceleration);
        }
	}

    //applies a circular deadzone to the character
    bool DeadzoneCheck()
    {
        Vector3 direction = new Vector3(Mathf.Abs(Camera.main.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x),
            Mathf.Abs(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - transform.position.y),
        0);

        if (direction.magnitude > deadzone)
        {
            return true;
        }
        else return false;
    }

    //handles movement
    void MovementForce(float acc)
    {
        if (Input.GetButton("Fire1"))
        {
            ObjectLookAtMouse();
            //rigidbodyThis.AddRelativeForce(new Vector2(0, acc));
       
      /*  if (rigidbodyThis.velocity.magnitude > maxSpeed)
        {
            rigidbodyThis.velocity = rigidbodyThis.velocity.normalized * maxSpeed;
        }*/
           
            var mr = Camera.main.ScreenPointToRay(Input.mousePosition);
            float intr;
            if(new Plane(Vector3.back, 0).Raycast(mr, out intr)) {
                Motor.setTarget( mr.GetPoint( intr ) );
            }
        }
    }

    //Orients camera to face cursor
    void ObjectLookAtMouse()
    {
        var angle = Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle -90, Vector3.forward);
    }
}
