using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// -In this we setup the waves that make up the level, can specify what enemies and how many per wave, breather between waves and spawn rate.
/// One neat thing about this is the use of an AnimationCurve so the spawn rate can be easily varied over the course of the wave which is great
/// for tweaking the difficulty of the waves while still keeping a high degree of randomness.
/// Also handles the wave notifier gui - and what to do after last wave.
/// 
/// 
/// Code is not particulary fancy..
/// </summary>
public class WaveMan : MonoBehaviour {

    [System.Serializable]
    public class Entry {
        public GameObject Prefab;
        public int Count;
    };

    [System.Serializable]
    public class Wave {
        public float PreWaveBreather = 3.0f;
        public List<Entry> Enemies;
        public AnimationCurve SpawnCurve;
        public float GlobalWaveRate = 1.0f;
    };
    public List<Wave> Waves;

    //public SpriteRenderer WaveGui;
    //public List<Sprite> WaveUIs;

    public int NextScene = 0;
   // public GameObject Boss;


    int CurWave = -1;
    float Delay = 0;
    float TotalRate = 0;
    float Elap, Rate;

    List<GameObject> Pool = new List<GameObject>();

    public void nextScene() {
        StartCoroutine(_nextScene());
    }

    IEnumerator _nextScene() {
        yield return new WaitForSeconds(1.0f );
        Application.LoadLevel(NextScene);
    }

    void nextWave( ) {
        Debug.Log("Next wave ");
        Pool.Clear();
        if(CurWave + 1 >= Waves.Count) {
           // if(Boss) Boss.SetActive(true);
            //else 
            nextScene();
            return;
        }
        CurWave++;
        Wave w  = Waves[CurWave];
        Delay = w.PreWaveBreather;
        foreach(Entry e in w.Enemies) {
            for( int i = e.Count; i-- !=0; ) Pool.Add(e.Prefab);
        }
       // float eps = w.TotalWaveTime / (float) Pool.Count;
        float avg = 0;
        for(int i = Pool.Count;i-- !=0;) {
            float sample = eval(  (float)i / (float) Pool.Count );
      //      Debug.Log(sample );
            avg += sample;
        }

        avg /= (float)Pool.Count;
     //    Debug.Log("avg  "+avg);
        Rate = avg/ w.GlobalWaveRate;
        Elap = 0;


       // WaveGui.enabled = true;
      //  WaveGui.color = Color.white;
     //   WaveGui.sprite = WaveUIs[CurWave];
    }
    void Awake() {
        nextWave();
    }

    /*
    void OnGUI() {
        string label;
        if(Delay > -1) label = "Wave " + (CurWave + 1) + " in : " + 0.1f*(float)((int)(Mathf.Max(Delay, 0)*10.0f)) ; 
        else if( CurWave+1 <=Waves.Count ) {
            label = "Wave " + (CurWave + 1) + " Enemies remaining : " + (Pool.Count + ActiveEnemies)  +"    "+Elap;
        } else return;
        

        GUI.Label(new Rect(30, 30, 200, 50), label );
    }    */
    void Update() {
        Delay -= Time.deltaTime;
        Elap += Time.deltaTime;

        if(Delay > -1) {
            if(Delay < 0) {
                var col = Color.white; col.a = 1 + Delay; col.a *= col.a;
              //  WaveGui.color = col;
            }
        } else {
          //  WaveGui.enabled = false;
        }

    }

    float eval(float a ) {
        Wave w = Waves[CurWave];
        return (2.0f - w.SpawnCurve.Evaluate(a / w.SpawnCurve[w.SpawnCurve.length - 1].time));
    }
    public GameObject getSpawn( ref float next, float mod ) {

        if(Delay > 0 ) { next = Delay; return null; }
        if( Pool.Count == 0 ) { next = 1.0f; return null; }

        Wave w  = Waves[CurWave];
        next = mod * Rate * eval(Elap);

        int pi = Random.Range(0,Pool.Count);
        var ret = Pool[ pi];
        Pool.RemoveAt(pi);
       // ActiveEnemies++;
        return ret;
    }

    public void enemySpawned() {
        ActiveEnemies++;
    }
    public void enemyDied() {
        ActiveEnemies--;
        if(ActiveEnemies == 0 && Pool.Count == 0) {
            nextWave();
        }
    }

    int ActiveEnemies = 0;
    public void registerSpawner(float rate) {
        TotalRate += rate;
    }

}
