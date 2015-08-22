﻿using UnityEngine;
using System.Collections;


public class CharMotor : MonoBehaviour {

    [HideInInspector] public Transform Trnsfrm;

    Rigidbody2D Bdy;

    public 
    NavMesh NavMsh;

    public float Speed = 4;
    public NavMesh.Path Path;
    public int CurNodeI;
    NavMesh.Node CurNode;

    public int MaxSmoothIter = -1;
    public float PathRadius = 1.0f;

    //two targeting modes..
    public CharMotor Target;

    Vector2 TargetP;
    NavMesh.Node TargetNode;


    Vector2 ValidPos; 

    public void setTarget( Vector2 at ) {
        var n = NavMsh.findNode(at, TargetNode);
        if(n == null) return;
        Target = null;
        TargetNode = n;
        TargetP = at;
    }

    void Awake() {
        Trnsfrm = transform;
        Bdy = GetComponent<Rigidbody2D>();
    }
	void Start () {
//        Target = FindObjectOfType<PlayerController>();
        NavMsh = FindObjectOfType<NavMesh>();
        TargetNode = CurNode = NavMsh.findNode(Trnsfrm.position, CurNode);
        TargetP = ValidPos = Trnsfrm.position;
    }



    // adjusts the corner by pathing radius of the AI - ie go around corners not through them
    Vector2 smoothHelper( Vector2 edge, Vector2 at, bool flip  ) {  //todo think of better name...
        Vector2 ret = edge, vec = edge - at;
        if(flip) vec = -vec;
        vec = Vector3.Cross(vec, Vector3.forward);  //lazy..
        vec.Normalize();
        //Debug.DrawLine(ret, ret + vec * PathRadius, Color.grey);
        ret += vec * PathRadius;
        return ret;
    }


	void Update () {

        if( NavMsh == null ) return;
        CurNode = NavMsh.findNode(Trnsfrm.position, CurNode);

        if(Target != null) {
            TargetP = Target.Trnsfrm.position;
            TargetNode = Target.CurNode;
        }

        Vector2 tPos = TargetP, cPos = Trnsfrm.position;
        //Debug.DrawLine(tPos, cPos); 

        // if target moved much - need to recalculate path
        if(Path != null) 
            if((Path.Smooth[0].P - tPos).sqrMagnitude > 0.25f) Path = null;
        

        if(Path == null) {
            Path = NavMsh.getPath(cPos, tPos, TargetNode);

            if(Path != null) {
                CurNodeI = Path.Smooth.Count - 2;
                ValidPos = Trnsfrm.position;
            //    Debug.Log("pc  " + Path.Smooth.Count + "  cn " + CurNode);
            } else {
                CurNodeI = -1;
          //      Debug.Log("path fail");
            }
        }


        ///funnel!
        //  basicly recursivlely look at edge we must travers to get to next node until we find a corner in our way - then move thataway
        //  no corner in the way means just go towards target
        Vector2 vec, cnrA, cnrB;
        if(Path != null) {
            for(; CurNodeI > 0; CurNodeI--) {  //path is backwards - because .... reasons
                //  tPos = Path.Smooth[CurNode].P;                
                    ///

                if(Util.sign(Path.Smooth[CurNodeI].E2, cPos, Path.Smooth[CurNodeI].E1) < 0) {
                    // if  so  then we passed through an edge - advance our place along the path
                    //  -done awkard way because fo how things were refactored - not neatening because it won't be more efficent - and might somehow implode

                    //CurNode = Path.Smooth[CurNodeI].;///todo 
                   
                    continue;
                }

                // the arc defined between cnrA < cPos > cnrB  is the range of current valid directions 
                cnrA = smoothHelper(Path.Smooth[CurNodeI].E1, cPos, true);
                cnrB = smoothHelper(Path.Smooth[CurNodeI].E2, cPos, false);

                float sgn = -1;
                //  int msi = MaxSmoothIter;
                for(int ci = CurNodeI - 1; ; ci--) { 

                    if(ci <= 0) break;  //reached end of path
                    
                    // only one of these will be different from current corners   -- this would be an obvious place to optimise (todo)
                    Vector2 nCnrA = smoothHelper(Path.Smooth[ci].E1, cPos, true);
                    Vector2 nCnrB = smoothHelper(Path.Smooth[ci].E2, cPos, false);


                    //new corner may refine our current valid arc 
                    //  it may refine it so far that the angle of the arc becomes 0 - ie a direction - cnrA . cnrB  would be  exactly the same direction  - if so then we are done here
                    if((nCnrA - cnrA).sqrMagnitude > (nCnrB - cnrB).sqrMagnitude) {
                         //Debug.DrawLine(cPos, fnlA2, Color.black);
                        sgn = Util.sign(nCnrA, cPos, cnrA);
                        if(Util.sign(nCnrA, cPos, cnrA) > 0) {

                            if(Util.sign(cnrB, cPos, nCnrA) < 0) {
                                tPos = cnrB;
                                //  Debug.Log("breakb");
                                break;
                            }
                            cnrA = nCnrA;
                        }
                    } else {
                        //Debug.DrawLine(cPos, fnlB2, Color.black);
                        sgn = Util.sign(cnrB, cPos, nCnrB);
                        if(Util.sign(cnrB, cPos, nCnrB) > 0) {
                            if(Util.sign(cnrB, cPos, nCnrA) < 0) {
                                    tPos = cnrA;
                                //   Debug.Log("breaka");
                                    break;
                            }
                            cnrB = nCnrB;
                        }
                    }
                    // if( msi-- == 0 ) break;
                }

                if(Util.sign(cnrB, cPos, tPos) < 0) tPos = cnrB;
                if(Util.sign(tPos, cPos, cnrA) < 0) tPos = cnrA;

                /*//   Debug.Log("sgn  " + sgn);
                Debug.DrawLine(cPos, fnlA, Color.green);
                Debug.DrawLine(cPos, fnlB, Color.red);
                Debug.DrawLine(cPos, tPos, Color.white); */
                break;
            }

        } else {
            if(NavMsh.findNode(cPos) != TargetNode) //fallen off map somehow  ... used to happen when colliders dind match map and also current node wasn't clamped    -- try and move back to last valid position
                tPos = ValidPos;
        }

        vec = tPos - cPos;
        var mag = vec.magnitude;
        if(mag > 0.1f) {
            vec /= mag;

            Bdy.velocity = vec * Speed;
        } else Bdy.velocity = Vector2.zero;
    }


    // wave manager needs to know how many enemies are active
    void OnEnable() {
    //    FindObjectOfType<WaveMan>().enemySpawned();
    //    Debug.Log("AI " + name + " :: Enable ");
    }
    void OnDisable() {
      //  Debug.Log("AI " + name + " :: Disenable ");
        //var wm = FindObjectOfType<WaveMan>();
        //if( wm != null ) wm.enemyDied();

    }

    void OnDrawGizmos() {

        if(Path != null) {
           //*
            Gizmos.color = Color.grey;
            NavMesh.SmoothNode prev = null;
            foreach( NavMesh.SmoothNode sn in Path.Smooth ) {
                if(prev != null) {
                    Gizmos.DrawLine(prev.P, sn.P);
                }
                prev = sn;
            }//*/
        }
    }
}
