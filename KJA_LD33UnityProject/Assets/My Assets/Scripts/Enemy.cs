using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : CharMotor, Vision.Receiver, CharMotor.DamageReceiver {


    public float SpeedWander = 2;
    public float SpeedChase = 3;

    [System.Serializable]
    public class Attack {
        public float Range = 1;
        public float RoF = 1;
        public int Damage = 1;
        public float Duration = 0.3f;
        [HideInInspector] public float LastAttack;
        [HideInInspector] public CharMotor LastTarget;
    };

    public List<Attack> Attacks;

    Attack CurAttack;

    Vision Vis;
    Repulsor Rep;

    static WaveMan WavMn;

    protected void Start() {

        if(WavMn == null) WavMn = FindObjectOfType<WaveMan>();
        WavMn.enemySpawned();
        Speed = SpeedWander;
        base.Start();
        //Target = FindObjectOfType<PlayerController>().Motor;

        Rep = GetComponent<Repulsor>();
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

    protected void Update() {

        //setTarget(FindObjectOfType<PlayerController>().transform.position);
        Rep.enabled = true;
        if( CurAttack != null ) {
           // DesVec = Vector2.zero;
            if(Time.fixedTime - CurAttack.LastAttack > CurAttack.Duration) {
                //Debug.Log("dmg " + CurAttack.Damage);

                if(Target == CurAttack.LastTarget && Target != null) Target.applyDamage(CurAttack.Damage,this);
                CurAttack = null;
                Vis.enabled = true;

            } else Rep.enabled = false;
        } else if(Target != null) {
            Speed = SpeedWander;
            var vec = ((Vector2)Target.Trnsfrm.position - (Vector2)Trnsfrm.position);
            var mag = vec.magnitude; vec /= mag;
            //Debug.Log("mag " + mag + "  dt " + Vector2.Dot(vec, -RotObj.transform.up));
            if(Vector2.Dot(vec, -RotObj.transform.up) > 0.8f) {
                bool inRange = true;
                foreach(var a in Attacks) {
                    if(mag > a.Range) {
                        inRange = false;
                        continue;
                    }
                    if( Time.fixedTime - a.LastAttack > a.Duration + a.RoF) {
                        CurAttack = a;
                        var anim = RotObj.GetComponent<Animation>();
                        if(anim != null) anim.Play();
                        a.LastAttack = Time.fixedTime;
                        a.LastTarget = Target;
                        Speed = 0;
                        Vis.enabled = false;
                        break;
                    }
                }
                if(inRange) {
                    Speed = 0;
                    Rep.enabled = false;
                }
            }

        } else {
            Speed = SpeedWander;
            if(((Vector2)Trnsfrm.position - TargetP).sqrMagnitude < 1.0f) {
                targetWP();
            }
        }


        base.Update();
    }


    public void spotted(CharMotor mtr) {
        
        if(mtr == Target) return;
        if(Target != null) {
            if((Target.Trnsfrm.position - Trnsfrm.position).sqrMagnitude < (mtr.Trnsfrm.position - Trnsfrm.position).sqrMagnitude)
                return;
        }
        Target = mtr;
        //Vis.enabled = false;
        //if( CurAttack ==null ) Speed = SpeedChase;
    }

    //void OnDrawGizmos

    public int Health = 20;
    public void recvDamage(int dmg, CharMotor src) {
        //if(this == null) return;
        if((Health -= dmg) <= 0) {
            WavMn.enemyDied();
            Destroy(gameObject);
        }  else if(src != null) spotted(src);
    }
}
