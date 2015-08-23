using UnityEngine;
using System.Collections;

public class Repulsor : MonoBehaviour {

    public float Radius = 1.0f;
   /* public float Frc = 1;
    public float MaxFrc = 1; */
    public float Freq = 0.5f; 
    public LayerMask Layers;

    Vector2 VelAdd;
    
    CharMotor Motor;

    float Timer;

    void Awake() {
        Motor = GetComponent<CharMotor>();
        Timer = Freq;
    }

   /* void addV(Vector2 v) {
        VelAdd += v;
        if(VelAdd.sqrMagnitude > MaxFrc * MaxFrc) {
            VelAdd = VelAdd.normalized * MaxFrc;
        }
    }*/
    void Update() {
        
       // if( (Timer -= Time.deltaTime ) > 0 ) return;
        if((Timer -= Time.deltaTime) < 0) {
            Timer = Freq;
            var lyr = gameObject.layer;
            gameObject.layer = 31;

            VelAdd = Vector2.zero;

            var cols = Physics2D.OverlapCircleAll(Motor.Trnsfrm.position, Radius, Layers);
            foreach(var c in cols) {
                var rep = c.gameObject.GetComponent<Repulsor>();
                if(rep == null) continue;

                Vector2 add = (Motor.Trnsfrm.position - rep.Motor.Trnsfrm.position).normalized;// *Frc;
                // rep.addV(-add);
                // addV(add);
                VelAdd += add;
                // Debug.Log("rep?");
            }
            VelAdd.Normalize();
            gameObject.layer = lyr;
        }

        if(VelAdd == Vector2.zero ) return; //no ep required
        Motor.DesVec.Normalize();
        var dot = Vector2.Dot(Motor.DesVec, VelAdd );
        if(dot < -0.7f) {
            Motor.DesVec = (Motor.DesVec + Vector2.Lerp(VelAdd, (Vector2)Vector3.Cross(Vector3.back, VelAdd), (dot + 0.7f) * 4.0f) ).normalized;
        } else  
            Motor.DesVec = (Motor.DesVec + VelAdd).normalized;
    }

   /* void FixedUpdate() {
        if( VelAdd.sqrMagnitude > 0 ) {

            var dot = Vector2.Dot( Motor.Bdy.velocity.normalized, VelAdd.normalized );
            if( dot < -0.7f ) {
                Motor.Bdy.velocity += Vector2.Lerp(VelAdd, (Vector2)Vector3.Cross(Vector3.back, VelAdd), (dot + 0.7f) *4.0f );
            } else 
                Motor.Bdy.velocity += VelAdd;
            
            if(Motor.Bdy.velocity.sqrMagnitude > Motor.Speed * Motor.Speed) {

                Motor.Bdy.velocity = Motor.Bdy.velocity.normalized * Motor.Speed;
            }

            VelAdd *= 0.9f;
        }
       
    }*/
    void OnDrawGizmos() {

        Gizmos.DrawLine(transform.position, (Vector2)transform.position + VelAdd );
    }

}
