using UnityEngine;
using System.Collections;

public class VillageStatus : MonoBehaviour {

    [HideInInspector]
    public static VillageStatus vStatus = null;

    int villagerCount = 0;
    int deathCount = 0;

    public int VillagerCount
    {
        get { return villagerCount; }
    }

    public int IncreaseDeatchCount
    {
        set
        {
            deathCount += value;
            if (deathCount == villagerCount) { 
                //fail state
            }
        }
    }

	// Use this for initialization
	void Start () {
        if (vStatus != null) Destroy(this.gameObject);
        else vStatus = this;

        var villagers = FindObjectsOfType<CharMotor>();
        foreach (var v in villagers)
        {
            if (v.gameObject.layer == LayerMask.NameToLayer("villager"))
            {
                villagerCount++;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	}
}
