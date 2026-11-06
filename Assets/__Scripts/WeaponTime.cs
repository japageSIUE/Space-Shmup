using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WeaponTime : MonoBehaviour
{
    [Header("Dynamic")]
    public Hero hero;
    public int weaponMult;
    private Text weaponText;

    // Start is called before the first frame update
    void Start()
    {
        weaponText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if(hero.weapons[0].type == eWeaponType.blaster)
        {
            weaponText.text = "It's Blastin time!";
            weaponMult = 1;
        }
        if(hero.weapons[0].type == eWeaponType.spread)
        {
            weaponText.text = "It's Spreader time!";
            weaponMult = 1;
        }
        if(hero.weapons[0].type == eWeaponType.luigi)
        {
            weaponText.text = "It's Luigi time! x2 Score";
            weaponMult = 2;
        }
        if(hero == null)
        {
            weaponText.text = "It's time to stop.";
        }
    }
}
