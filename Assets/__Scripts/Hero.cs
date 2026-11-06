using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S { get; private set; } // singleton property

    [Header("Inscribed")]
    // these fields control the movement of the ship
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;
    public Weapon[] weapons;

    [Header("Dynamic")] [Range(0,4)] [SerializeField]
    private float _shieldLevel = 1; // underscore is reminder this is private
    [Tooltip("This field holds a reference to the last triggering GameObject")]
    private GameObject lastTriggerGo = null;
    //declare a new delegate type WeaponFireDelegate
    public delegate void WeaponFireDelegate();
    // create a WeaponFireDelegate event named fireEvent
    public event WeaponFireDelegate fireEvent;

    //stuff for score
    public ScoreCounter scoreCounter;
    public WeaponTime weaponTime;


    void Awake()
    {
        if(S == null)
        {
            S = this; // set the singleton only if it's null
        }
        else
        {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
        }
        //fireEvent += TempFire;

        //reset the weapons to start _Hero with one blaster
        ClearWeapons();
        weapons[0].SetType(eWeaponType.blaster); //this needs to be blaster

        //just stuff for score
        //find the gameobject in hierarchy
        GameObject scoreGO = GameObject.Find("Score");
        GameObject weapGO = GameObject.Find("WeaponTime");
        // then the script for it
        scoreCounter = scoreGO.GetComponent<ScoreCounter>();
        weaponTime = weapGO.GetComponent<WeaponTime>();
    }


    // Update is called once per frame
    void Update()
    {
        // pull in information from the Input class
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");

        // Change transform.position based on the axes
        Vector3 pos = transform.position;
        pos.x += hAxis * speed * Time.deltaTime;
        pos.y += vAxis * speed * Time.deltaTime;
        transform.position = pos;

        // rotate the ship to make it feel more dynamic
        transform.rotation = Quaternion.Euler(vAxis * pitchMult, hAxis * rollMult, 0);  

        // allow the ship to fire
        // if(Input.GetKeyDown(KeyCode.Space))
        // {
        //     TempFire();
        // } 

        // use the fireEvent to fire Weapons when the space bar is pressed
        if(Input.GetAxis("Jump") == 1 && fireEvent != null)
        {
            fireEvent();
        }
    }

    // void TempFire()
    // {
    //     GameObject projGO = Instantiate<GameObject>(projectilePrefab);
    //     projGO.transform.position = transform.position;
    //     Rigidbody rigidB = projGO.GetComponent<Rigidbody>();
    //     //rigidB.velocity = Vector3.up * projectileSpeed;

    //     ProjectileHero proj = projGO.GetComponent<ProjectileHero>();
    //     proj.type = eWeaponType.blaster;
    //     float tSpeed = Main.GET_WEAPON_DEFINITION(proj.type).velocity;
    //     rigidB.velocity = Vector3.up * tSpeed;
    // }

    void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        //Debug.Log("Shield trigger hit by: " + go.gameObject.name);

        // make sure it's not the same triggering go as last time
        if(go == lastTriggerGo) return;
        lastTriggerGo = go;

        Enemy enemy = go.GetComponent<Enemy>();
        PowerUp pUp = go.GetComponent<PowerUp>();
        if(enemy != null) //if the shield was triggered by an enemy
        {
            shieldLevel--; //decrease the shield level by 1
            Destroy(go); // and destroy the enemy
        }
        else if(pUp != null) //if the shield hit a powerup
        {
            AbsorbPowerUp(pUp); // absorb the powerup
        }
        else
        {
            Debug.LogWarning("Shield trigger hit by non-enemy: " + go.name);
        }
    }

    public void AbsorbPowerUp(PowerUp pUp)
    {
        Debug.Log("Absorbed PowerUp: " + pUp.type);
        switch(pUp.type)
        {
            //this is no longer empty :)
            case eWeaponType.shield:
                if(shieldLevel >= 4)
                {
                    scoreCounter.score += (300 * weaponTime.weaponMult);
                }
                shieldLevel++;
                break;

            default:
                if(pUp.type == weapons[0].type) // if it is the same type
                {
                    Weapon weap = GetEmptyWeaponSlot();
                    if(weap != null)
                    {
                        //set it to the pUp.type
                        weap.SetType(pUp.type);
                    }
                    else
                    {
                        scoreCounter.score += (150 * weaponTime.weaponMult);
                    }
                }
                else // if this is a different weapon type
                {
                    ClearWeapons();
                    weapons[0].SetType(pUp.type);
                }
                break;
        }
        pUp.AbsorbedBy(this.gameObject);
    }

    public float shieldLevel
    {
        get { return(_shieldLevel); }
        private set
        {
            _shieldLevel = Mathf.Min(value, 4);
            // if the shield is going to be set to less than zero
            if(value < 0)
            {
                Destroy(this.gameObject); // destroy the hero
                Main.HERO_DIED();
            }
        }
    }

    /// <summary>
    /// finds the first empty weapon slot (i.e. type = none) and returns
    /// it.
    /// </summary>
    /// <returns> the first empty Weapon slot or null if none are empty </returns>
    Weapon GetEmptyWeaponSlot()
    {
        for(int i = 0; i < weapons.Length; i++)
        {
            if(weapons[i].type == eWeaponType.none)
            {
                return(weapons[i]);
            }
        }
        return(null);
    }

    ///<summary>
    /// sets the type of all weapon slots to none
    /// </summary
    void ClearWeapons()
    {
        foreach(Weapon w in weapons)
        {
            w.SetType(eWeaponType.none);
        }
    }
}
