﻿using UnityEngine;
using System.Collections;

public class PlayerData : MonoBehaviour, CharMotor.DamageReceiver {

    float hp;
    [Range(1,600)]
    public int maxhp;
    float mana;
    [Range(1, 600)]
    public int maxMana;
    float stamina;
    [Range(1, 600)]
    public float maxStamina;
    [Range(1, 600)]
    public float staminaRegen;

    public float Hp { get { return hp; } }
    public float Mana { get { return mana; } }
    public float Stamina { get { return stamina; } }

	// Use this for initialization
	void Start () {
        hp = maxhp/2;
        mana = 100;
        stamina = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (stamina < maxStamina)
        {
            stamina += Time.deltaTime * staminaRegen;
        }
        hp = Mathf.Clamp(hp, 0, maxhp);
        mana = Mathf.Clamp(mana, 0, maxMana);
        stamina = Mathf.Clamp(stamina, 0, maxStamina);
	}

     public void IncreaseHP(int increase)
    {
        hp = Mathf.Clamp(hp+increase, 0, maxhp);
    }

     public void IncreaseMana(int increase)
     {
         mana = Mathf.Clamp(mana+increase, 0, maxMana);
    }
     public void IncreaseStamina(int increase)
    {
        stamina += increase;
    }

     public void recvDamage(int dmg, CharMotor src) {
         IncreaseHP(-dmg);
     }
}
