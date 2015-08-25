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
    public float invisibleTimer = 6;
    float invisibleInit;
    Color color;
    public float kaboomRadius = 5;
    public int kaboomDmg = 50;
    public LayerMask enemyLayers;
    public GameObject kaboomParticles;

    AudioSource audio;
    public AudioClip healing, acidSpit1, acidSpit2, invisibility, kaboom;

	// Use this for initialization
	void Start () {
        pData = gameObject.GetComponent<PlayerData>();
        acidSpitReady = false;
        mainLine = GetComponent<LineRenderer>();
        ResetLine();
        invisibleInit = invisibleTimer;
        audio = GetComponent<AudioSource>();
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
            audio.PlayOneShot(healing);
        }
        if (Input.GetButtonDown("Ability2") && pData.Stamina >= abilityCost[1] && pData.Mana >= abilityCost[1])
        {
            if (!acidSpitReady && acidSpitPrefab != null)
            {
                acidSpitReady = true;
                pData.IncreaseMana(-abilityCost[1]);
                pData.IncreaseStamina(-abilityCost[1]);
                audio.PlayOneShot(acidSpit1);
            }
        }
        if (Input.GetButtonDown("Ability3") && pData.Stamina >= abilityCost[2] && pData.Mana >= abilityCost[2] && !pData.isInvisible)
        {
            var charMotors = FindObjectsOfType<CharMotor>();
            foreach (var ch in charMotors)
            {
                if (ch.Target != null && ch.Target.gameObject.tag == "Player") ch.Target = null;
            }
            invisibleTimer = invisibleInit;
            pData.isInvisible = true;
            pData.IncreaseMana(-abilityCost[2]);
            pData.IncreaseStamina(-abilityCost[2]);
            audio.PlayOneShot(invisibility);
        }
        if (pData.isInvisible)
        {
            if ((invisibleTimer -= Time.deltaTime) <= 0) pData.isInvisible = false;
        }
        if (Input.GetButtonDown("Ability4") && pData.Mana >= abilityCost[3] && pData.Stamina >= abilityCost[3])
        {
            pData.IncreaseMana(-abilityCost[3]);
            pData.IncreaseStamina(-abilityCost[3]);
            audio.PlayOneShot(kaboom);
            var particles = (GameObject)Instantiate(kaboomParticles, transform.position, Quaternion.identity);
            var colls = Physics2D.OverlapCircleAll(transform.position, kaboomRadius, enemyLayers);
            foreach (var c in colls)
            {
                if (c.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    c.gameObject.GetComponent<CharMotor>().applyDamage(kaboomDmg, GetComponent<CharMotor>());
                }
                if (c.gameObject.layer == LayerMask.NameToLayer("villager"))
                {
                    c.gameObject.GetComponent<CharMotor>().applyDamage(-kaboomDmg, GetComponent<CharMotor>());
                }
            }
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
                    audio.PlayOneShot(acidSpit2);
                    ResetLine();
                    var mouseDirection = position - transform.position;
                    var angle = Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg;

                    GameObject acidSpitClone = (GameObject)Instantiate(acidSpitPrefab, transform.position, Quaternion.AngleAxis(angle - 90, Vector3.forward));
                    Physics2D.IgnoreCollision(acidSpitClone.GetComponent<Collider2D>(), GetComponent<Collider2D>());
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
        mainLine.SetPosition(0, new Vector3 (transform.position.x, transform.position.y, -5));
        mainLine.SetPosition(1, new Vector3(destination.x, destination.y, -5));
    }

    void ResetLine()
    {
        mainLine.SetPosition(0, transform.position);
        mainLine.SetPosition(1, transform.position);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, kaboomRadius);
    }
}
