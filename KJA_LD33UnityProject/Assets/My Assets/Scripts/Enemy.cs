using UnityEngine;
using System.Collections;

public class Enemy : CharMotor {

    
    void Start() {
        base.Start();
        Target = FindObjectOfType<PlayerController>().Motor;

    }

    void Update() {

        //setTarget(FindObjectOfType<PlayerController>().transform.position);

        base.Update();
    }
}
