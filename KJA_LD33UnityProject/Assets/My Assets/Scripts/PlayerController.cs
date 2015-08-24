using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour {

    float acceleration = 100;
    float maxSpeed = 100;
    public float deadzone = 3f;
    Rigidbody2D rigidbodyThis;
    Vector3 mouseDirection;

    [HideInInspector] 
    public CharMotor Motor;
    float DefSpeed;

    [System.Serializable]
    public class Attack {
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

	// Use this for initialization
	void Awake() {
        Motor = GetComponent<CharMotor>();
        rigidbodyThis = GetComponent<Rigidbody2D>();

        DefSpeed = Motor.Speed;
	}

    // Update is called once per frame
    void Update()
    {
        var position = Camera.main.WorldToScreenPoint(transform.position);
        mouseDirection = Input.mousePosition - position;


        Motor.Speed = DefSpeed;
        if(CurAttack != null) {
            // DesVec = Vector2.zero;
            if(Time.fixedTime - CurAttack.LastAttack > CurAttack.Duration) {
                //Debug.Log("dmg " + CurAttack.Damage);

                if(Motor.Target == CurAttack.LastTarget && Motor.Target != null) {


                    var ai = Motor.Target.GetComponent<AIcontrol>();
                    if(ai != null) {
                        GetComponent<PlayerData>().IncreaseMana(100);
                        Destroy(ai.gameObject);
                    } else Motor.Target.applyDamage(CurAttack.Damage, Motor);
                }
                CurAttack = null;
            } else Motor.Speed = 0;
        } else {



            if(Input.GetButton("Fire1")) {
                var mr = Camera.main.ScreenPointToRay(Input.mousePosition);
                float intr;
                if(new Plane(Vector3.back, 0).Raycast(mr, out intr)) {
                    var p = mr.GetPoint(intr);
                    CharMotor trgt = null;

                    var col = Physics2D.OverlapPoint(p);
                    if(col && (trgt = col.GetComponent<CharMotor>()) != null) {
                        Motor.Target = trgt;
                    } else
                        Motor.setTarget(p);
                }
            }

            if(Motor.Target != null) {
                var vec = ((Vector2)Motor.Target.Trnsfrm.position - (Vector2)Motor.Trnsfrm.position);
                var mag = vec.magnitude; vec /= mag;
                //Debug.Log("mag " + mag + "  dt " + Vector2.Dot(vec, -RotObj.transform.up));
                if(Vector2.Dot(vec, -Motor.RotObj.transform.up) > 0.8f) {
                    bool inRange = true;
                    foreach(var a in Attacks) {
                        if(mag > a.Range) {
                            inRange = false;
                            continue;
                        }
                        if(Time.fixedTime - a.LastAttack > a.Duration + a.RoF) {
                            CurAttack = a;
                            //var anim = RotObj.GetComponent<Animation>();
                            //if(anim != null) anim.Play();
                            a.LastAttack = Time.fixedTime;
                            a.LastTarget = Motor.Target;

                            var ai = Motor.Target.GetComponent<AIcontrol>();
                            if(ai != null) {
                                ai.Mtr.enabled = false;
                                ai.enabled = false;
                                ai.Mtr.Bdy.velocity = Vector2.zero;
                                a.LastAttack += 1.0f;
                            }
                            break;
                        }
                    }
                    if(inRange) {
                        Motor.Speed = 0;
                    }
                }

            }
        }
    }
	
    /*
	
	void FixedUpdate () {
        if (DeadzoneCheck())
        {
            MovementForce(acceleration);
        }
	} */

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
            //ObjectLookAtMouse();
            //rigidbodyThis.AddRelativeForce(new Vector2(0, acc));
       
      /*  if (rigidbodyThis.velocity.magnitude > maxSpeed)
        {
            rigidbodyThis.velocity = rigidbodyThis.velocity.normalized * maxSpeed;
        }*/
           


           
        }
    }

    //Orients camera to face cursor
    void ObjectLookAtMouse()
    {
        var angle = Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }
}
