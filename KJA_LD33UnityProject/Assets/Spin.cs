using UnityEngine;
using System.Collections;

public class Spin : MonoBehaviour {

    float timerInit;
    Color color;
    public float timeToDestroy = 0.3f;
    static Spin waypoint;

	// Use this for initialization
	void Start () {
        if (waypoint != null)
        {
            Destroy(gameObject);
        }
        else waypoint = this;

        timerInit = timeToDestroy;

	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetButton("Fire1"))
        {
            var mr = Camera.main.ScreenPointToRay(Input.mousePosition);
            float intr;
            if(new Plane(Vector3.back, 0).Raycast(mr, out intr)) {
                var p = mr.GetPoint(intr);
                transform.position = p;
            }
        }
        if ((timeToDestroy -= Time.deltaTime) <= 0) Destroy(this.gameObject);
        transform.Rotate(new Vector3(0, 0, 100 * Time.deltaTime));
        GetComponent<SpriteRenderer>().color = color;
        color = new Color(1, 1, 1, timeToDestroy / timerInit);
	}
}
