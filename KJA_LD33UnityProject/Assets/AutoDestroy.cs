using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour {

    public float timeToDestroy;

    void Update()
    {
        if ((timeToDestroy -= Time.deltaTime) <= 0) Destroy(this.gameObject);
    }
}
