using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // enables the loading and reloading of scenes

public class Main : MonoBehaviour
{
    static private Main S; // a private singleton for main
    static private Dictionary<eWeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Inscribed")]
    public bool spawnEnemies = true;
    public GameObject[] prefabEnemies; //array of enemy prefabs
    public float enemySpawnPerSecond = 0.5f;
    public float enemyInsetDefault = 1.5f;
    public float gameRestartDelay = 2;
    public GameObject prefabPowerUp;
    public WeaponDefinition[] weaponDefinitions;
    public eWeaponType[] powerUpFrequency = new eWeaponType[] {
                            eWeaponType.blaster, eWeaponType.blaster,
                            eWeaponType.spread, eWeaponType.shield };
    
    private BoundsCheck bndCheck;

    void Awake()
    {
        S = this;
        // set the bndCheck to reference the 
        // BoundsCheck component on this GameObject
        bndCheck = GetComponent<BoundsCheck>();

        //Invoke SpawnEnemy() once (in 2 seconds based on base values)
        Invoke(nameof(SpawnEnemy), 1f/enemySpawnPerSecond);

        //a generic dictionary with eWeaponType as the key
        WEAP_DICT = new Dictionary<eWeaponType, WeaponDefinition>();
        foreach(WeaponDefinition def in weaponDefinitions)
        {
            WEAP_DICT[def.type] = def;
        }
    }

    public void SpawnEnemy()
    {
        //if spawnEnemies is false, skip to the next invoke of SpawnEnemy()
        if(!spawnEnemies)
        {
            Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond );
            return;
        }
        //pick a random enemy prefab to instantiate
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

        //position the enemy above the screen with a random x postion
        float enemyInset = enemyInsetDefault;
        if(go.GetComponent<BoundsCheck>() != null)
        {
            enemyInset = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }
    
    
        //set the initial position for the spawned enemy
        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyInset;
        float xMax = bndCheck.camWidth - enemyInset;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyInset;
        go.transform.position = pos;

        //invoke SpawnEnemy() again
        Invoke(nameof(SpawnEnemy), 1f/enemySpawnPerSecond);
    }

    void DelayedRestart()
    {
        //invoke the Restart() method in gameRestartDelay seconds
        Invoke(nameof(Restart), gameRestartDelay);
    }

    void Restart()
    {
        //reload __Scene_0 to restart the game
        //"__Scene_0" below starts with 2 underscores and ends with a zero
        SceneManager.LoadScene("__Scene_0");
    }

    static public void HERO_DIED()
    {
        S.DelayedRestart();
    }

    ///<summary>
    /// static function that gets a weapon definition for the WEAP_DICT static
    /// protected field of the Main class.
    /// </summary>
    /// <returns>the WeaponDefinition, or if there is no WeaponDefinition with
    /// the eWeaponType passed in, returns a new WeaponDefinition with a
    /// eWeaponType of eWeaponType.none </returns>
    ///<param name="wt">the eWeaponType of the desired WeaponDefinition</param>
    static public WeaponDefinition GET_WEAPON_DEFINITION(eWeaponType wt)
    {
        if(WEAP_DICT.ContainsKey(wt))
        {
            return(WEAP_DICT[wt]);
        }
        //if no entry of the correct type exists in WEAP_DICT, return a new
        // WeaponDefinition with a type of eWeaponType.none (the default value)
        return(new WeaponDefinition());
    }

    /// <summary>
    /// called by an enemy ship whenever it is detryoed. it sometimes creates a
    /// powerup in place of the destryoed ship
    /// </summary>
    /// <param name = "e"> the enemy that was destroyed </param>
    static public void SHIP_DESTROYED(Enemy e)
    {
        //potentially generate a powerup
        if(Random.value <= e.powerUpDropChance)
        {
            //choose a powerup from the possibilities in powerup frequency
            int ndx = Random.Range(0, S.powerUpFrequency.Length);
            eWeaponType pUpType = S.powerUpFrequency[ndx];

            //spawn a powerup
            GameObject go = Instantiate<GameObject>(S.prefabPowerUp);
            PowerUp pUp = go.GetComponent<PowerUp>();
            //set it to the proper WeaponType
            pUp.SetType(pUpType);

            //set it to the position of the destryoed ship
            pUp.transform.position = e.transform.position;
        }
    }
}
