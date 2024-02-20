using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test : MonoBehaviour
{

    public float targetSpawnRate = 5f; // A target should spawn every x seconds.
    public float maxTargetTime = 2f; // How long a target lasts before dissapearing.
    public GameObject target; //Reference to the target prefab
    private float targetSpawnCooldown; // Counter for spawning targets
    private int missedTargets; //How many targets have died to being around too long.
    private int hitTargets; //How many targets player has hit
    private Dictionary<int,Vector3> gridPositions = new Dictionary<int,Vector3>(); // Vector3 positions for grid
    private Dictionary<int,GameObject> currentTargets = new Dictionary<int,GameObject>(); //Contains target instances
    private Dictionary<int,bool> hasTarget = new Dictionary<int,bool>(); // Does grid at spot have target?
    private Dictionary<int,float> timeAlive = new Dictionary<int,float>();// how long has target been up
    private int aimPos; // Current Grid the red selection box is on.

    private int currentAmmo;
    public int maxAmmo = 6;
    public float reloadTime = 2f;
    private bool isReloading = false;

    // Start is called before the first frame update
    void Start()
    {
        //Top Row
        gridPositions.Add(1, new Vector3(-2.9f, 2.9f,0f));
        gridPositions.Add(2, new Vector3(0f,    2.9f,0f));
        gridPositions.Add(3, new Vector3(2.9f,  2.9f,0f));
        //Middle Row
        gridPositions.Add(4, new Vector3(-2.9f, 0f,0f));
        gridPositions.Add(5, new Vector3(0f,    0f,0f));
        gridPositions.Add(6, new Vector3(2.9f,  0f,0f));
        //Bottom Row
        gridPositions.Add(7, new Vector3(-2.9f, -2.9f,0f));
        gridPositions.Add(8, new Vector3(0f,    -2.9f,0f));
        gridPositions.Add(9, new Vector3(2.9f,  -2.9f,0f));

        aimPos = 5;
        transform.position = gridPositions[aimPos];
        targetSpawnCooldown = targetSpawnRate; //Start off by spawning a target.

        for(int i = 1; i <= 9; i++) //Instantiate dicts.
        {
            hasTarget.Add(i,false);
            timeAlive.Add(i,0f);
        }

        missedTargets = 0;
        hitTargets = 0;

        currentAmmo = maxAmmo;

        Time.timeScale = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(targetSpawnCooldown < targetSpawnRate)
        {
            targetSpawnCooldown += Time.deltaTime;
        }
        else
        {
            targetSpawnCooldown = 0f;
            CreateTarget();
        }

        UpdateTargets();
    }

    void UpdateTargets()
    {
        for(int i = 1; i <= 9; i++)
        {
            if(hasTarget[i])
            {
                timeAlive[i] += Time.deltaTime;

                if(timeAlive[i] > maxTargetTime) //Target is dying of old age.
                {
                    missedTargets++;
                    Debug.Log("Miss: " + missedTargets);
                    KillTarget(i);
                }
            }
        }
    }
    void KillTarget(int pos)
    {
        timeAlive[pos] = 0f;
        hasTarget[pos] = false;
        Destroy(currentTargets[pos]);
    }
    void CreateTarget()
    {
        int rand = Random.Range(1,10);

        if(!hasTarget[rand])
        {
            currentTargets[rand] = Instantiate(target,gridPositions[rand],transform.rotation);
            hasTarget[rand] = true;
            timeAlive[rand] = 0f;
        }
    }

    void UpdateAimHorizontal(string dir)
    {
        if(dir == "R")
        {
            switch(aimPos)
            {
                case 3:
                    aimPos = 1;
                    break;
                case 6:
                    aimPos = 4;
                    break;
                case 9:
                    aimPos = 7;
                    break;
                default:
                    aimPos++;
                    break;
            }
        }
        else if(dir == "L")
        {
            switch(aimPos)
            {
                case 1:
                    aimPos = 3;
                    break;
                case 4:
                    aimPos = 6;
                    break;
                case 7:
                    aimPos = 9;
                    break;
                default:
                    aimPos--;
                    break;
            }
        }
        transform.position = gridPositions[aimPos];
    }
    void UpdateAimVertical(string dir)
    {
        if(dir == "U")
        {
            switch(aimPos)
            {
                case 1: 
                    aimPos = 7;
                    break;
                case 2:
                    aimPos = 8;
                    break;
                case 3:
                    aimPos = 9;
                    break;
                default:
                    aimPos = aimPos - 3;
                    break;
            }
        }
        else if(dir == "D")
        {
            switch(aimPos)
            {
                case 7:
                    aimPos = 1;
                    break;
                case 8:
                    aimPos = 2;
                    break;
                case 9:
                    aimPos = 3;
                    break;
                default:
                    aimPos = aimPos + 3;
                    break;
            }
        }
        transform.position = gridPositions[aimPos];
    }
    void FireWeapon()
    {
        currentAmmo--;

        if(hasTarget[aimPos])
        {
            hitTargets++;
            Debug.Log("Hits: " + hitTargets);
            KillTarget(aimPos);
        }
    }

    //Create a coroutine for Reloading
    IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
    }

    public void OnRight()
    {
        UpdateAimHorizontal("R");
    }
    public void OnLeft()
    {
        UpdateAimHorizontal("L");
    }
    public void OnUp()
    {
        UpdateAimVertical("U");
    }
    public void OnDown()
    {
        UpdateAimVertical("D");
    }
    public void OnReload()
    {
        if(!isReloading)
        {
            StartCoroutine(Reload());
        }
    }
    public void OnFire()
    {
        if(currentAmmo > 0)
        {
            FireWeapon();
        }
        else
        {
            if(!isReloading)
            {
                StartCoroutine(Reload());
            }
        }
    }


}
