using UnityEngine;
using System.Collections;

public class Enemy : CharMotor, Vision.Receiver {


    Vision Vis;

    void Start() {
        base.Start();
        //Target = FindObjectOfType<PlayerController>().Motor;

        Vis = GetComponentInChildren<Vision>();
        Vis.Recv = this;

        targetWP();
    }

    void targetWP() {
        Target = null;
        for(int i = 100; ; ) {
            setTarget(EnemyWP.getP());
            if(TargetNode != null) break;
            if(i-- <= 0) {
                Debug.LogError(" can't find valid EnemyWP ");
                break;
            }
        }
    }

    void Update() {

        //setTarget(FindObjectOfType<PlayerController>().transform.position);

        if(Target != null) {


        } else {
            if(((Vector2)Trnsfrm.position - TargetP).sqrMagnitude < 1.0f) {
                targetWP();
            }
        }


        base.Update();
    }

    void Vision.Receiver.spotted(CharMotor mtr) {
       
        Target = mtr;
        Vis.enabled = false;
    }

    //void OnDrawGizmos
}
