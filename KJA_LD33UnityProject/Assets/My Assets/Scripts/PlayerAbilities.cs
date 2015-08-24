using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerData))]
public class PlayerAbilities : MonoBehaviour {

    PlayerData pData;
    [Range(1,99)]
    public int[] abilityCost = new int[4];
    int ability1HealthRegen = 30;
    public GameObject acidSpitPrefab;
    [Range(1,300)]
    public float acidSpitRange = 100;
    bool acidSpitReady;
    LineRenderer mainLine;
    public float invisibleTimer;
    float invisibleInit;
    Color color;
    

	// Use this for initialization
	void Start () {
        pData = gameObject.GetComponent<PlayerData>();
        acidSpitReady = false;
        mainLine = GetComponent<LineRenderer>();
        ResetLine();
        invisibleInit = invisibleTimer;
	}
	
	// Update is called once per frame
	void Update () {
        IsInvisible();
        AcidSpit();

        if (Input.GetButtonDown("Ability1") && pData.Stamina >= abilityCost[0] && pData.Mana >= abilityCost[0])
        {
            pData.IncreaseHP(ability1HealthRegen);
            pData.IncreaseMana(-abilityCost[0]);
            pData.IncreaseStamina(-abilityCost[0]);
        }
        if (Input.GetButtonDown("Ability2") && pData.Stamina >= abilityCost[1] && pData.Mana >= abilityCost[1])
        {
            if (!acidSpitReady && acidSpitPrefab != null)
            {
                acidSpitReady = true;
                pData.IncreaseMana(-abilityCost[1]);
                pData.IncreaseStamina(-abilityCost[1]);
            }
        }
        if (Input.GetButtonDown("Ability3") && pData.Stamina >= abilityCost[2] && pData.Mana >= abilityCost[2] && !pData.isInvisible)
        {
            invisibleTimer = invisibleInit;
            pData.isInvisible = true;
            pData.IncreaseMana(-abilityCost[2]);
            pData.IncreaseStamina(-abilityCost[2]);
        }
        if (pData.isInvisible)
        {
            if ((invisibleTimer -= Time.deltaTime) <= 0) pData.isInvisible = false;
        }
        if (Input.GetButtonDown("Ability3") &&  pData.Mana >= abilityCost[2])
        {

        }
	}

    void AcidSpit()
    {
        if (acidSpitReady)
        {
            var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var distance = Vector3.Distance(new Vector3(position.x, position.y, 0), transform.position);
            DrawLine(new Vector3(position.x, position.y, 0));
            mainLine.SetColors(Color.red, Color.red);

            if (distance < acidSpitRange)
            {
                mainLine.SetColors(Color.green, Color.green);
                if (Input.GetButtonDown("Fire2"))
                {
                    ResetLine();
                    var mouseDirection = position - transform.position;
                    var angle = Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg;

                    GameObject acidSpitClone = (GameObject)Instantiate(acidSpitPrefab, transform.position, Quaternion.AngleAxis(angle - 90, Vector3.forward));
                    acidSpitClone.GetComponent<AcidSpit>().SetDestination(position);
                    acidSpitReady = false;

                }
            }
        }
    }

    void IsInvisible()
    {
        if (pData.isInvisible)
        { 
            GetComponent<CharMotor>().RotObj.gameObject.GetComponent<SpriteRenderer>().color = new Color(0.6f,0.6f,1,0.5f);
            gameObject.layer = LayerMask.NameToLayer("PlayerInvisible");
        }
        if (!pData.isInvisible) 
        {
            GetComponent<CharMotor>().RotObj.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            gameObject.layer = LayerMask.NameToLayer("Player");
        }
    }

    void DrawLine(Vector3 destination)
    {
        mainLine.SetPosition(0, transform.position);
        mainLine.SetPosition(1, destination);
    }

    void ResetLine()
    {
        mainLine.SetPosition(0, transform.position);
        mainLine.SetPosition(1, transform.position);
    }
}
