using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoundsCheck))]
public class PowerUp : MonoBehaviour
{
    [Header("Inscribed")]
    //this is an unusal but handy use of Vector2s
    [Tooltip("x holds a min value and y a max value for a Random.Range() call")]
    public Vector2 rotMinMax = new Vector2(15,90);
    [Tooltip("Ditto the first tooltip")]
    public Vector2 driftMinMax = new Vector2(.25f, 2);
    public float lifeTime = 10; // powerup will exist for # seconds
    public float fadeTime = 4; // then fades over # seconds

    [Header("Dynamic")]
    public eWeaponType type; // type of powerup
    public GameObject cube; //reference to the powercube child
    public TextMesh letter; //reference to the textmesh
    public Vector3 rotPerSecond; //euler rotation speed for powercube
    public float birthTime; // the Time.time this was instantiated

    private Rigidbody rigid;
    private BoundsCheck bndCheck;
    private Material cubeMat;

    void Awake()
    {
        //find the cube reference (there's only a single child)
        cube = transform.GetChild(0).gameObject;
        //find the TextMesh and other components
        letter = GetComponent<TextMesh>();
        rigid = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();
        cubeMat = cube.GetComponent<Renderer>().material;

        //set a random velocity
        Vector3 vel = Random.onUnitSphere; // get random XYZ velocity
        vel.z = 0; // flatten the vel to the XY plane
        vel.Normalize(); // normalizing a Vector3 sets it's length to 1m

        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        rigid.velocity = vel;

        //set the rotation of this PowerUp GameObject to R: [ 0, 0, 0 ]
        transform.rotation = Quaternion.identity;
        // Quaternion.identity is equal to no roationtion

        //randomize rotPerSecond for PowerCube using rotMinMax x & y
        rotPerSecond = new Vector3(Random.Range(rotMinMax[0], rotMinMax[1]),
                                    Random.Range(rotMinMax[0], rotMinMax[1]),
                                    Random.Range(rotMinMax[0], rotMinMax[1]) );

        birthTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);

        //fade out the powerup over time
        //given the default values, a powerup will exist for ten seconds
        // then fade out over 4
        float u = (Time.time - (birthTime + lifeTime)) / fadeTime;
        //if u >= 1, destroy this powerup
        if(u >= 1)
        {
            Destroy(this.gameObject);
            return;
        }
        //if u>0, decrease the opacity (i.e alpha) of the powercube & letter
        if(u > 0)
        {
            Color c = cubeMat.color;
            c.a = 1f - u; // set the alpha of powercube to 1-u
            cubeMat.color = c;
            //fade the letter too, just not as much
            c = letter.color;
            c.a = 1f - (u * 0.5f); // set the alpha of the letter to 1-(u/2)
            letter.color = c;
        }

        if(!bndCheck.isOnScreen)
        {
            //if the powerup has drifted entirely off screen, destroy it
            Destroy(this.gameObject);
        }
    }

    // | Another line pulled from the book that gives a compiler error 
    // |  for some reason. Seems to work just fine anyways.
    // |  It's supposed to ensure SetType() is called any time type is 
    // V  set by a class other than PowerUp.
    //public eWeaponType type { get { return _type; } set { SetType(value); } }

    public void SetType(eWeaponType wt)
    {
        //grab the WeaponDefinition from Main
        WeaponDefinition def = Main.GET_WEAPON_DEFINITION(wt);
        cubeMat.color = def.powerUpColor; // set the color of powercube
        //letter.color = def.color; //we could colorize the letter too
        letter.text = def.letter; //set the letter that is shown
        type = wt; //finally, actually set the type
    }

    ///<summary>
    /// this function is called by the Hero class when a PowerUp is collected
    /// </summary>
    /// <param name = "target"> the GameObject absorbing this PowerUp</param>
    public void AbsorbedBy(GameObject target)
    {
        Destroy(this.gameObject);
    }
}  
