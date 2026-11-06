using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(BlinkColorOnHit))]
public class EnemyShield : MonoBehaviour
{
    [Header("Inscribed")]
    public float health = 10;

    private List<EnemyShield> protectors = new List<EnemyShield>();
    private BlinkColorOnHit blinker;

    // Start is called before the first frame update
    void Start()
    {
        blinker = GetComponent<BlinkColorOnHit>();
        blinker.ignoreOnCollisionEnter = true; //finna give me an error

        if(transform.parent == null) return;
        EnemyShield shieldParent = transform.parent.GetComponent<EnemyShield>();
        if(shieldParent != null)
        {
            shieldParent.AddProtector(this);
        }
    }

    ///<summary>
    /// Called by another EnemyShield to join the protectors of this EnemyShield
    /// </summary>
    /// <param name = "shieldChild">The EnemyShield that will portect this</param>
    public void AddProtector(EnemyShield shieldChild)
    {
        protectors.Add(shieldChild);
    }

    ///<summary>
    /// Shortcut for gameObject.activeInHierarchy and gameObject.SetActive()
    /// </summary>
    public bool isActive
    {
        get { return gameObject.activeInHierarchy; }
        private set { gameObject.SetActive(value); }
    }

    /// <summary>
    /// called by Enemy_4.OnCollisionEnter() & parent's EnemyShields.TakeDamage()
    ///  to distribute damage to EnemyShield protectors
    /// </summary>
    /// <param name = "dmg"> the amount of damage to be handled </param>
    /// <returns> Any damage not handled by this shield</returns>
    public float TakeDamage(float dmg)
    {
        // can we pass damage to a protector EnemyShield?
        foreach(EnemyShield es in protectors)
        {
            if(es.isActive)
            {
                dmg = es.TakeDamage(dmg);
                // if all damage was handled, return 0 damage
                if(dmg == 0) return 0;
            }
        }

        //if the code gets here, this EnemyShield will blicnk and take damage
        //make the blinker blink
        blinker.SetColors();

        health -= dmg;
        if(health <= 0)
        {
            //deactivate this EnemyShield GameObject
            isActive = false;
            //return any damage that was not absorbed by this EnemyShield
            return -health;
        }

        return 0;
    }
}
