using UnityEngine;
using System.Collections;
using System.Reflection;



public class Vision : MonoBehaviour {

    public float Radius = 6.0f;
    public float Cone = 0.5f;
    public float Freq = 0.5f;
    public LayerMask Layers, ObstacleLayers;

    public interface Receiver {
       void spotted(CharMotor cm);
    }
   // public GameObject Recv;

    public Receiver Recv;

    public CharMotor Motor;
    float Timer;


    /*
  public static T Cast<T>(object o)
        {
            return (T)o;
        } 

Then invoke this using reflection:

        MethodInfo castMethod = this.GetType().GetMethod("Cast").MakeGenericMethod(t);
        object castedObject = castMethod.Invoke(null, new object[] { ob
     * 
     
    public static Receiver cast<T>(object o) {
        return (Receiver)(T)o;
    }*/
    void Awake() {
        if(Motor == null) {
            Motor = GetComponent<CharMotor>();
            if(Motor == null) GetComponentInParent<CharMotor>();
        }
        Timer = Freq;

        /*
               foreach( Component c in Recv.GetComponents<Component>() ) {
            var t = c.GetType();
            if(typeof(Receiver).IsAssignableFrom(t) ) {
                MethodInfo castMethod = this.GetType().GetMethod("cast").MakeGenericMethod(t);
                object castedObject = castMethod.Invoke(null, new object[] { c });
                _Recv = (Receiver)castedObject;
                //Debug.Log(" castedObject " + castedObject + "  " + castedObject.GetType() + " t " + t + "   " + typeof(Receiver).IsAssignableFrom(t));
                break;
            }
        */
        //_Recv.spotted(Motor);
       // Debug.Log(" _Recv " + _Recv);
    }
    void Start() {

    }



    void Update() {

        if((Timer -= Time.deltaTime) < 0) {
            Timer = Freq;



            var cols = Physics2D.OverlapCircleAll(Motor.Trnsfrm.position, Radius, Layers);
            foreach(var c in cols) {
                var mtr = c.gameObject.GetComponent<CharMotor>();
                if(mtr == null) continue; //err?

                Vector2 vec = (Motor.Trnsfrm.position - mtr.Trnsfrm.position);// *Frc;
                var mag = vec.magnitude; vec /= mag;
                if(Vector2.Dot(Motor.Trnsfrm.up, vec.normalized) < Cone) continue;

                var hit = Physics2D.Raycast(Motor.Trnsfrm.position, -vec, mag, ObstacleLayers);
                //Debug.DrawLine(Motor.Trnsfrm.position, (Vector2)Motor.Trnsfrm.position - vec * mag);
                //Debug.Log("hit " + hit + "  hit " + hit.collider);

                if(hit.collider != null) continue;
                

                Recv.spotted(mtr);
            }

        }
    }

    void OnDrawGizmos() {

        Gizmos.color = Color.cyan;
        var p =transform.position;
        Gizmos.DrawWireSphere( p, Radius );
        Gizmos.DrawLine(p, p + -transform.up * (Radius + 2));

        var vec = new Vector2(Mathf.Sqrt(1 - Cone * Cone), Cone);
        Gizmos.DrawLine(p, p + -transform.up * (Radius + 2));
        Gizmos.DrawLine(p, p + -transform.TransformDirection(vec) * (Radius + 1) );
        vec.x = -vec.x;
        Gizmos.DrawLine(p, p + -transform.TransformDirection(vec) * (Radius + 1));
    }
}
