using UnityEngine;
using System.Collections;

public class PlayerData : MonoBehaviour {

    int hp;
    [Range(1,600)]
    public int maxhp;
    int mana;
    [Range(1, 600)]
    public int maxMana;
    float stamina;
    [Range(1, 600)]
    public float maxStamina;
    [Range(1, 600)]
    public float staminaRegen;

    public int Hp { get { return hp; } }
    public int Mana { get { return mana; } }
    public float Stamina { get { return stamina; } }

	// Use this for initialization
	void Start () {
        hp = maxhp;
        mana = 0;
        stamina = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (stamina < maxStamina)
        {
            stamina += Time.deltaTime * staminaRegen;
        }
	}

     public void IncreaseHP(int increase)
    {
        hp += increase;
    }

     public void IncreaseMana(int increase)
     {
        mana += increase;
    }
     public void IncreaseStamina(int increase)
    {
        stamina += increase;
    }
}
