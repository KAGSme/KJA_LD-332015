using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerData))]
public class PlayerAbilities : MonoBehaviour {

    PlayerData pdata;
    [Range(1,99)]
    public int[] abilityCost = new int[4];
    int ability1HealthRegen;

	// Use this for initialization
	void Start () {
        this.gameObject.GetComponent<PlayerData>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButton("Ability1") && pdata.Stamina >= abilityCost[0] && pdata.Mana >= abilityCost[0])
        {
            pdata.IncreaseHP(ability1HealthRegen);
        }
        if (Input.GetButton("Ability2") && pdata.Stamina >= abilityCost[1] && pdata.Mana >= abilityCost[1])
        {

        }
        if (Input.GetButton("Ability3") && pdata.Stamina >= abilityCost[2] && pdata.Mana >= abilityCost[2])
        {

        }
        if (Input.GetButton("Ability3") &&  pdata.Mana >= abilityCost[2])
        {

        }
	}
}
