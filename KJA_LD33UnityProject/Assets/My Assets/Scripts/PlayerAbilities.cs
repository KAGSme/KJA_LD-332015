using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerData))]
public class PlayerAbilities : MonoBehaviour {

    PlayerData pData;
    [Range(1,99)]
    public int[] abilityCost = new int[4];
    int ability1HealthRegen = 30;
    public GameObject acidSpitPrefab;
    bool acidSpitReady;

	// Use this for initialization
	void Start () {
        pData = gameObject.GetComponent<PlayerData>();
        acidSpitReady = false;
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
            if (!acidSpitReady && acidSpitPrefab != null)
            {
                acidSpitReady = true;
            }
        }
        if (Input.GetButton("Ability3") && pData.Stamina >= abilityCost[2] && pData.Mana >= abilityCost[2])
        {

        }
        if (Input.GetButton("Ability3") &&  pData.Mana >= abilityCost[2])
        {

        }

        if (acidSpitReady)
        {
            if (Input.GetButtonDown("Fire2"))
            {

                var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var mouseDirection = position - transform.position;
                var angle = Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg;

                GameObject acidSpitClone = (GameObject)Instantiate(acidSpitPrefab, transform.position, Quaternion.AngleAxis(angle - 90, Vector3.forward));
                acidSpitPrefab.GetComponent<AcidSpit>().SetDestination(position);

                pData.IncreaseMana(-abilityCost[1]);
                pData.IncreaseStamina(-abilityCost[1]);
                acidSpitReady = false;
            }
        }
	}
}
