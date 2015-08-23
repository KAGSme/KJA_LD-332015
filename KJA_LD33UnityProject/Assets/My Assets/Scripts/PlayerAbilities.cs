using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerData))]
public class PlayerAbilities : MonoBehaviour {

    PlayerData pData;
    [Range(1,99)]
    public int[] abilityCost = new int[4];
    int ability1HealthRegen = 30;

	// Use this for initialization
	void Start () {
        pData = gameObject.GetComponent<PlayerData>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButton("Ability1") && pData.Stamina >= abilityCost[0] && pData.Mana >= abilityCost[0])
        {
            pData.IncreaseHP(ability1HealthRegen);
            pData.IncreaseMana(-abilityCost[0]);
            pData.IncreaseStamina(-abilityCost[0]);
        }
        if (Input.GetButton("Ability2") && pData.Stamina >= abilityCost[1] && pData.Mana >= abilityCost[1])
        {

        }
        if (Input.GetButton("Ability3") && pData.Stamina >= abilityCost[2] && pData.Mana >= abilityCost[2])
        {

        }
        if (Input.GetButton("Ability3") &&  pData.Mana >= abilityCost[2])
        {

        }
	}
}
