using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VillageStatus : MonoBehaviour {

    [HideInInspector]
    public static VillageStatus vStatus = null;

    int villagerCount = 0;
    int deathCount = 0;
    Text text;

    public int VillagerCount
    {
        get { return villagerCount; }
    }

    public void IncreaseDeathCount()
    {
            deathCount++;
            Debug.Log(deathCount);
            if (deathCount == villagerCount) 
        {
            Application.LoadLevel("main menu");
        }
    }

	// Use this for initialization
	void Awake () {
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

        text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        var living = villagerCount - deathCount;
        text.text = living.ToString() + " Villagers left";
	}
}
