using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GridShooter : MonoBehaviour
{
    //-----------------------------------------------------------------------------------
    //  General Game class members
    //-----------------------------------------------------------------------------------
    public GameObject gridSprite; // Reference to rope grid prefab
    private GameObject grid; // This instances rope grid.
    public TMP_Text hitsDisplay;
    public TMP_Text effectivenessDisplay;
    private Dictionary<int,Vector3> gridPositions = new Dictionary<int,Vector3>(); // Vector3 positions for grid
    private int totalTargets;
    private int hitTargets; //How many targets player has hit
    private int missedTargets; //How many targets missed to old age
    private float minigameEffectiveness;
    private bool gameRunning = false;
    //-----------------------------------------------------------------------------------
    //  Target specific class members
    //-----------------------------------------------------------------------------------
    public GameObject targetSprite; //Reference to the target prefab
    private TargetGrid targets; //Object that holds information about game targets.
    public float targetSpawnRate = 5f; // How often targets within a group can spawn
    public float groupSpawnRate = 1f; // How often a group can start spawning
    private bool isSpawning = false; // Is a target group spawning right now?
    public float maxTargetTime = 2f; // How long a target lasts before dissapearing.
    private int[][] targetGroup; // Vector holding target groups

    //------------------------------------------------------------------------------------
    //  Weapon specific class members
    //------------------------------------------------------------------------------------
    public GameObject reticleSprite; // Reference to reticle prefab
    private GameObject reticle; // This instances reticle.
    public GameObject bangSprite; //Reference to Bang! prefab
    public TMP_Text ammoDisplay; // UI element for current ammo
    public Slider reloadProgressBar; // UI progress bar for reload
    private int aimPos; // Current Grid the reticle is at.
    private int currentAmmo; // current ammo.
    public int maxAmmo = 6; // maximum ammo.
    public float reloadTime = 2f; // How long to reload.
    private float reloadProgress = 0f; 
    private bool isReloading = false; // Reload coroutine running?

    void Start() // Called before first frame update.
    {
        SetupGrid();
        SetupTargetGroup();


        aimPos = 5;
        SetAim(aimPos);

        targets = new TargetGrid(maxTargetTime, targetSprite,gridPositions,reticleSprite.transform.rotation);
        missedTargets = 0;
        hitTargets = 0;
        totalTargets = 0;
        minigameEffectiveness = 0f;

        currentAmmo = maxAmmo;
        SetAmmoDisplay();

        Time.timeScale = 1.0f;
        gameRunning = false;
    }

    void Update() //Called once per frame.
    {
        if(gameRunning)
        {
            if(!isSpawning)
            {
                StartCoroutine(CreateTargetGroup(targetGroup));
            }

            if(isReloading)
            {
                reloadProgress += Time.deltaTime;
                reloadProgressBar.value = reloadProgress/reloadTime;
            }

            AgeTargets();
            HandleInputs();
        }
    }

    //--------------------------------------------------------------------------------------------
    // Mutators
    //--------------------------------------------------------------------------------------------
    private void SetAim(int pos)
    {
        reticle.transform.position = gridPositions[pos];
    }

    private void SetAmmoDisplay()
    {
        ammoDisplay.text = currentAmmo + "/" + maxAmmo;
    }

    private void SetHitsDisplay()
    {
        hitsDisplay.text = "Hits: " + hitTargets;
    }

    private void SetEffectivenessDisplay()
    {
        effectivenessDisplay.text = "Effectiveness: " + minigameEffectiveness;
    }

    //---------------------------------------------------------------------------------------------
    //  Event Subscriptions
    //---------------------------------------------------------------------------------------------
    public void OnTimeOver()
    {
        StopAllCoroutines();
        isSpawning = false;
        isReloading = false;
        gameRunning = false;
        targets.KillAll();
        minigameEffectiveness = (float)hitTargets/(float)totalTargets;
        SetEffectivenessDisplay();

        ResetState();
    }
    public void OnStartClicked()
    {
        Debug.Log("Start clicked entered.");
        if(!gameRunning)
        {
            hitTargets = 0;
            SetHitsDisplay();
            minigameEffectiveness = 0f;
            gameRunning = true;
            Debug.Log("Start Clicked ending");
        }
    }
    //--------------------------------------------------------------------------------------------
    //  General Game Methods
    //--------------------------------------------------------------------------------------------
    private void AgeTargets()
    {
        for(int pos = 1; pos <= 9; pos++)
        {
            if(targets.HasTarget(pos))
            {
                targets.AddTime(pos,Time.deltaTime);

                if(targets.TooOld(pos)) //Target is dying of old age.
                {
                    missedTargets++;
                    targets.KillTarget(pos);
                }
            }
        }
    }

    IEnumerator CreateTargetGroup(int[][] targetGroup)
    {
        isSpawning = true;
        Debug.Log("Spawning Target Group...");

        foreach(int[] group in targetGroup)
        {
            foreach(int pos in group)
            {
                targets.CreateTarget(pos);
                totalTargets++;
                yield return new WaitForSeconds(targetSpawnRate);
            }
            
            yield return new WaitForSeconds(groupSpawnRate);
        }

        isSpawning = false;
        Debug.Log("Group Spawning Finished!");
    }

    
    void FireWeapon()
    {
        currentAmmo--;
        SetAmmoDisplay();
        GameObject clone = Instantiate(bangSprite,gridPositions[aimPos],transform.rotation);
        Destroy(clone, 0.1f);

        if(targets.HasTarget(aimPos))
        {
            hitTargets++;
            SetHitsDisplay();
            targets.KillTarget(aimPos);
        }
    }
    IEnumerator Reload()
    {
        isReloading = true;
        currentAmmo = 0;
        SetAmmoDisplay();
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        SetAmmoDisplay();
        isReloading = false;
        reloadProgress = 0f;
        reloadProgressBar.value = 0f;
    }

    private void ResetState()
    {
        aimPos = 5;
        SetAim(aimPos);

        currentAmmo = maxAmmo;
        SetAmmoDisplay();
        missedTargets = 0;
        totalTargets = 0;
    }

    //----------------------------------------------------------------------
    //  Input Handling
    //----------------------------------------------------------------------
    void HandleInputs()
    {
        if(Input.GetButtonDown("GridRight"))
        {
            UpdateAimHorizontal("R");
        }
        if(Input.GetButtonDown("GridLeft"))
        {
            UpdateAimHorizontal("L");
        }
        if(Input.GetButtonDown("GridUp"))
        {
            UpdateAimVertical("U");
        }
        if(Input.GetButtonDown("GridDown"))
        {
            UpdateAimVertical("D");
        }
        if(Input.GetButtonDown("GridReload"))
        {
            if(!isReloading)
            {
                if(currentAmmo < maxAmmo)
                {
                    StartCoroutine(Reload());
                }
            }
        }
        if(Input.GetButtonDown("GridFire"))
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
    private void UpdateAimHorizontal(string dir)
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
        SetAim(aimPos);
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
        SetAim(aimPos);
    }
    
    //--------------------------------------------------------------------------
    //  Initial setup
    //--------------------------------------------------------------------------
    private void SetupGrid()
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

        grid = Instantiate(gridSprite, gridPositions[5], transform.rotation);
        reticle = Instantiate(reticleSprite, gridPositions[5], transform.rotation);
    }
    private void SetupTargetGroup()
    {
        targetGroup = new int[][]
        {
            new int[]{1,2,3},
            new int[]{4,5,6},
            new int[]{7,8,9},
            new int[]{1,5,9},
            new int[]{7,5,3}
        };
    }
}
