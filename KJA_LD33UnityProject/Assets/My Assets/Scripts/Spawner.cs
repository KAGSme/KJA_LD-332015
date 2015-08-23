using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {

    public float SpawnRate = 2;
    //public Transform SpawnTarget;
    float SpawnTimer;

    public float Radius = 1;
    List<Enemy> Spawning = new List<Enemy>();

    Transform Trnsfrm;
    WaveMan WMan;

    void Awake() {

        SpawnTimer = SpawnRate;
        Trnsfrm = transform;
        WMan = FindObjectOfType<WaveMan>();
    }
    void Start() {
        WMan.registerSpawner(SpawnRate);
    }

    void Update() {

        if((SpawnTimer -= Time.deltaTime) < 0.0f) {

            //if( Physics2D.CircleCast(....   - todo check are is clear

            SpawnTimer += SpawnRate;

            var fab = WMan.getSpawn(ref SpawnTimer, SpawnRate);
            if(fab != null) {


                var bot = (Instantiate(fab, getP(), Quaternion.identity) as GameObject).GetComponent<Enemy>();
                //Spawning.Add(bot);
                //bot.enabled = false;
              //  bot.rigidbody2D.isKinematic = true;
            }
        }
    }

    Vector2 getP() {
        return (Vector2)Trnsfrm.position + Random.insideUnitCircle * Radius;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}
