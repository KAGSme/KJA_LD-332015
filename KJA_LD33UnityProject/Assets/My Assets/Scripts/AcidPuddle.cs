using UnityEngine;
using System.Collections;

public class AcidPuddle : MonoBehaviour {

    public float timeToDestroy;
    public float radius;
    public int damage;
    float timerInit;
    Color color;
    //public LayerMask enemyLayers;


	// Use this for initialization
	void Start () {
        timerInit = timeToDestroy;
	}

	// Update is called once per frame
	void Update () {
        FadeAway();
        if ((timeToDestroy -= Time.deltaTime) <= 0) Destroy(this.gameObject);

        var colls = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (var coll in colls)
        {
            if (coll.gameObject.tag == "Enemy")
            {
                coll.gameObject.GetComponent<CharMotor>().applyDamage(damage, null);
                Debug.Log(coll.gameObject.name);
            }
        }
	}
   
    void FadeAway()
    {
        GetComponent<SpriteRenderer>().color = color;
        color = new Color(1, 1, 1, timeToDestroy / timerInit);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
