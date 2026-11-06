using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<summary>
/// this is an enum of the various possible weapon types.
/// it also includes a shield type to allow a shield powerup
/// items marked [NI] are no implemented in the book
/// </summary>
public enum eWeaponType
{
    none, //default
    blaster, // simple blastem
    spread, // the shottie
    luigi, //like from mario (kinda)
    NI_missile, // oh this one's also metroid [NI]
    NI_laser, // DOT [NI]
    shield // raise sheildLevel
}

///<summary>
/// TheWeaponDefintion class allows you to set the properties of a 
/// specific weapon in the Inspector. the Main class has an array of 
/// WeaponDefinitions that make this possible
/// </summary>
[System.Serializable]
public class WeaponDefinition
{
    public eWeaponType type = eWeaponType.none;
    [Tooltip("Letter to show on the PowerUp cube")]
    public string letter;
    [Tooltip("Color of the PowerUp cube")]
    public Color powerUpColor = Color.white;
    [Tooltip("Prefab of Weapon model that is attached to the Player Ship")]
    public GameObject weaponModelPrefab;
    [Tooltip("prefab of the projectile that is fired")]
    public GameObject projectilePrefab;
    [Tooltip("Color of the fired projectile")]
    public Color projectileColor = Color.white;
    [Tooltip("Damage caused when a single projectile hits an enemy")]
    public float damageOnHit = 0;
    [Tooltip("Damage caused per second by the laser [NI]")]
    public float damagePerSec = 0;
    [Tooltip("Seconds to delay between shots")]
    public float delayBetweenShots = 0;
    [Tooltip("velocity of idnividual projectiles")]
    public float velocity = 100;
}
public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;

    [Header("Dynamic")]
    [SerializeField]
    [Tooltip("Setting this manually whilem playing does not work properly")]
    private eWeaponType _type = eWeaponType.none;
    public WeaponDefinition def;
    public float nextShotTime; //time the weapon will fire next

    private GameObject weaponModel;
    private Transform shotPointTrans;

    void Start()
    {
        // set up PROJECTILE_ANCHOR if it has not already been done
        if(PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }

        shotPointTrans = transform.GetChild(0);
        //call SetType() for the default _type set in the inspector
        SetType(_type);

        //find the fireEvent of a Hero componnent in the hierarchy
        Hero hero = GetComponentInParent<Hero>();
        if(hero != null) hero.fireEvent += Fire;
    }

    public eWeaponType type
    {
        get { return(_type); }
        set { SetType(value); }
    }

    public void SetType(eWeaponType wt)
    {
        _type = wt;
        if(type == eWeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true);
        }
        //get the WeaponDefinition for this type from Main
        def = Main.GET_WEAPON_DEFINITION(_type);
        //destroy any old model and then attach a model for this weapon
        if(weaponModel != null) Destroy(weaponModel);
        weaponModel = Instantiate<GameObject>(def.weaponModelPrefab, transform);
        weaponModel.transform.localPosition = Vector3.zero;
        weaponModel.transform.localScale = Vector3.one;

        nextShotTime = 0; //you can fire immmediately after _type is set
    }

    private void Fire()
    {
        //if this.gameObject is inactive, return
        if(!gameObject.activeInHierarchy) return;
        //if it hasn't been enough time between shots, return
        if(Time.time < nextShotTime) return;        

        ProjectileHero p;
        Vector3 vel = Vector3.up * def.velocity;

        switch(type)
        {
            case eWeaponType.blaster:
                p = MakeProjectile();
                p.vel = vel;
                break;

            case eWeaponType.spread:
                p = MakeProjectile();
                p.vel = vel;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p.vel = p.transform.rotation * vel;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p.vel = p.transform.rotation * vel;
                break;

            case eWeaponType.luigi:
                p = MakeProjectile();
                p.vel = vel;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(90, Vector3.back);
                p.vel = p.transform.rotation * vel;
                break;


        }
    }

    private ProjectileHero MakeProjectile()
    {
        GameObject go;
        go = Instantiate<GameObject>(def.projectilePrefab, PROJECTILE_ANCHOR);
        ProjectileHero p = go.GetComponent<ProjectileHero>();

        Vector3 pos = shotPointTrans.position;
        pos.z = 0;
        p.transform.position = pos;

        p.type = type;
        nextShotTime = Time.time + def.delayBetweenShots;
        return(p);
    }
}
