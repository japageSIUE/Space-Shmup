using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoundsCheck))]
public class Enemy : MonoBehaviour
{
    [Header("Inscribed")]
    public float speed = 10f; // the movement speed is 10m/s
    public float fireRate = 0.3f; // seconds/shot (unused)
    public float health = 10; // damage needed to destory this enemy
    public int score = 100; // points earned for destryoing this guy
    public float powerUpDropChance = 1f; // chance to drop a powerup

    public WeaponTime weaponTime;
    public ScoreCounter scoreCounter;

    protected bool calledShipDestroyed = false;
    protected BoundsCheck bndCheck;

    void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        //find the gameobject in hierarchy
        GameObject scoreGO = GameObject.Find("Score");
        GameObject weapGO = GameObject.Find("WeaponTime");
        // then the script for it
        scoreCounter = scoreGO.GetComponent<ScoreCounter>();
        weaponTime = weapGO.GetComponent<WeaponTime>();
    }

    // this is a property: a method that acts like a field
    public Vector3 pos
    {
        get{ return this.transform.position; }
        set{ this.transform.position = value; }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        // check wheteher this enemy has gone off the botom of the screen
        if(bndCheck.LocIs(BoundsCheck.eScreenLocs.offDown))
        {
            Destroy(gameObject);
        }
        
        // if(!bndCheck.isOnScreen)
        // {
        //     if(pos.y < bndCheck.camHeight - bndCheck.radius)
        //     {
        //         // we're off the bottom so destroy this guy
        //         Destroy(gameObject);
        //     }
        // }
    }

    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    // void OnCollisionEnter(Collision coll)
    // {
    //     GameObject otherGO = coll.gameObject;
    //     ProjectileHero p = otherGO.GetComponent<ProjectileHero>();
    //     if(p != null)
    //     {
    //         Destroy(otherGO); //destroy the projectile
    //         Destroy(gameObject); // destroy this enemy gameobjct
    //     }
    //     else
    //     {
    //         Debug.Log("Enemy hit by non-ProjectileHero: " + otherGO.name);
    //     }
    // }

    void OnCollisionEnter(Collision coll)
    {
        GameObject otherGO = coll.gameObject;
        //check for collisons with ProjectileHero
        ProjectileHero p = otherGO.GetComponent<ProjectileHero>();
        if(p != null)
        {
            // only damage this enemy when it's on screen
            if(bndCheck.isOnScreen)
            {
                //get the samage amount from Main WEAP_DICT
                //mmm samage
                health -= Main.GET_WEAPON_DEFINITION(p.type).damageOnHit;
                if(health <= 0)
                {
                    // tell main this ship was destroyed
                    if(!calledShipDestroyed)
                    {
                        calledShipDestroyed = true;
                        Main.SHIP_DESTROYED(this);
                        scoreCounter.score += (this.score * weaponTime.weaponMult);
                    }
                    Destroy(this.gameObject);
                }
            }
            //Destroy the ProjectileHero regardless
            Destroy(otherGO);
        }
        else
        {
            print("Enemy hit by non-ProjectileHero: " + otherGO.name);
        }
    }
}
