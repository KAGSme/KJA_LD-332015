using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbilityAvaliable : MonoBehaviour {

    PlayerAbilities pAbilities;
    PlayerData pData;
    public int abilityNo;
    Text text;

    void Start()
    {
        pData = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerData>();
        pAbilities = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAbilities>();
        text = GetComponent<Text>();
    }

    void Update()
    {
        if (pData.Stamina < pAbilities.abilityCost[abilityNo])
        {
            text.color = new Color(0.6f, 0.6f, 0.6f);
        }
        else if (pData.Stamina >= pAbilities.abilityCost[abilityNo])
        {
            text.color = new Color(1f,1f,0.5f);
        }
    }
}
