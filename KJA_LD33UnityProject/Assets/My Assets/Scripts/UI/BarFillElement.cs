using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum UIBars { hp, mana, stamina };

public class BarFillElement : MonoBehaviour {

    PlayerData pData;
    Image barImage;
    public UIBars barType;

    void Start()
    {
        barImage = GetComponent<Image>();
        pData = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerData>();
    }

    void Update()
    {
        switch (barType)
        {
            case UIBars.hp:
                barImage.fillAmount = pData.Hp / pData.maxhp;
                break;
            case UIBars.mana:
                barImage.fillAmount = pData.Mana / pData.maxMana;
                break;
            case UIBars.stamina:
                barImage.fillAmount = pData.Stamina / pData.maxStamina;
                break;
        }
    }
}
