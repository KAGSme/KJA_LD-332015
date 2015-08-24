﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WPTemplate<T> : MonoBehaviour {

    public float Radius = 1;
    static List<WPTemplate<T>> Wps = new List<WPTemplate<T>>();

    Transform Trnsfrm;

    void Awake() {
        Trnsfrm = transform;
    }

    void OnEnable() {
        Wps.Add(this);
    }
    void OnDisnable() {
        Wps.Remove(this);
    }

    Vector2 _getP() {
        return (Vector2)Trnsfrm.position + Random.insideUnitCircle * Radius;
    }

    public static Vector2 getP() {
        return Wps[Random.Range(0, Wps.Count)]._getP();
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}
