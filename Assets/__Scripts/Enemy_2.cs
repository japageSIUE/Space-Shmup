using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_2 : Enemy
{
    [Header("Enemy_2 Inscribed Fields")]
    public float lifeTime = 10;
    //Enemy_2 uses a sine wave to modify a 2-point linear interpolation
    [Tooltip("Determines how much the sine wave will ease the interpolation")]
    public float sinEccentricity = 0.6f;
    public AnimationCurve rotCurve;

    [Header("Enemy_2 Private fields")]
    [SerializeField] private float birthTime; //interpolation start time
    [SerializeField] private Vector3 p0, p1; // Lerp_points

    private Quaternion baseRotation;

    // Start is called before the first frame update
    void Start()
    {
        //pick any point on the left side of the screen
        p0 = Vector3.zero;
        p0.x = -bndCheck.camWidth + bndCheck.radius;
        p0.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        //pick any point on the right side of the screen
        p1 = Vector3.zero;
        p1.x = bndCheck.camWidth + bndCheck.radius;
        p1.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        //possibly swap sides
        if(Random.value > 0.5f)
        {
            //setting the .x of each point to it's negative will move it to
            // the other side of the screen
            p0.x *= -1;
            p1.x *= -1;
        }

        //set the birthTime to the current time
        birthTime = Time.time;

        // set up the inital ship rotation
        transform.position = p0;
        transform.LookAt(p1, Vector3.back);
        baseRotation = transform.rotation;
    }

    public override void Move()
    {
        //linear interpolations wokr based on a u value between 0 & 1
        float u = (Time.time - birthTime) / lifeTime;

        // if u>1, then it has been longer than lifeTime since birthTime
        if(u > 1)
        {
            //this Enemy_2 has finished it's life
            Destroy(this.gameObject);
            return;
        }

        // use the AnimationCurve to set the rotation about Y
        float shipRot = rotCurve.Evaluate(u) * 360;
        // if(p0.x > p1.x) shipRot = -shipRot;
        // transform.rotation = Quaternion.Euler(0, shipRot, 0);
        transform.rotation = baseRotation * Quaternion.Euler(-shipRot, 0, 0);

        //adjust u by adding a U curve based on a sine wave
        u = u + sinEccentricity * (Mathf.Sin(u * Mathf.PI * 2));

        //interpolate the two linear interpolation points
        pos = (1-u) * p0 + u * p1;

        //note that Enemy_2 does NOT call the base.Move() method
    }
}
