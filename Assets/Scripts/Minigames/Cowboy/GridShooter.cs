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
    [Header("General Members")]
    public GameObject gridSprite; // Reference to rope grid prefab
    private GameObject _grid; // This instances rope grid.
    private Transform _parent;
    public TMP_Text hitsText;
    public TMP_Text timerText;
    private Dictionary<int,Vector3> _gridPositions = new Dictionary<int,Vector3>(); // Vector3 positions for grid
    private Vector3 _spriteScale = new Vector3(45f,45f,1f);
    private int _totalTargets;
    private int _hitTargets; //How many targets player has hit
    private int _missedTargets; //How many targets missed to old age
    private float _minigameEffectiveness;
    private bool _gameRunning = false;

    public InkSplatter inkSplatter;

    private enum _MoveDirections
    {
        up,
        down,
        left,
        right
    }

    //-----------------------------------------------------------------------------------
    //  Target specific class members
    //-----------------------------------------------------------------------------------
    [Header("Target Members")]
    public GameObject targetSprite; //Reference to the target prefab
    private TargetGrid _targets; //Object that holds information about game targets.
    [Header("Target Spawn Settings")]
    public float targetSpawnRate = 5f; // How often targets within a group can spawn
    public float groupSpawnRate = 1f; // How often a group can start spawning
    private bool _isSpawning = false; // Is a target group spawning right now?
    public float maxTargetTime = 2f; // How long a target lasts before dissapearing.
    private int[][] _targetGroup; // Vector holding target groups

    //------------------------------------------------------------------------------------
    //  Weapon specific class members
    //------------------------------------------------------------------------------------
    [Header("Weapon Members")]
    public GameObject reticleSprite; // Reference to reticle prefab
    private GameObject _reticle; // This instances reticle.
    public GameObject bangSprite; //Reference to Bang! prefab
    public TMP_Text ammoText; // UI element for current ammo
    public Slider reloadProgressBar; // UI progress bar for reload
    [Header("Weapon Settings")]
    private int _aimPos; // Current Grid the reticle is at.
    private int _currentAmmo; // current ammo.
    public int maxAmmo = 6; // maximum ammo.
    public float reloadTime = 2f; // How long to reload.
    private float _reloadProgress = 0f; 
    private bool _isReloading = false; // Reload coroutine running?

    //-------------------------------------------------------------------------------------
    //  Audio handling
    //-------------------------------------------------------------------------------------
    [Header("Audio Members and Settings")]
    public AudioSource audioSource;
    public AudioClip[] audioClipArray;
    public float volume = 0.5f;


    void OnEnable()
    {
        GridShooterEventManager.onStart += Start_Minigame;
        GridShooterEventManager.onUpdateTimer += Update_Timer;
        GridShooterEventManager.onTimeOver += Time_Over;
    }
    void OnDisable()
    {
        GridShooterEventManager.onStart -= Start_Minigame;
        GridShooterEventManager.onUpdateTimer -= Update_Timer;
        GridShooterEventManager.onTimeOver -= Time_Over;
    }
    void Start() // Called before first frame update.
    {
        _parent = transform.parent;
        Setup_Grid();
        Setup_Target_Group();


        _aimPos = 5;
        Set_Aim(_aimPos);

        _targets = new TargetGrid(maxTargetTime,_parent,targetSprite,_spriteScale,_gridPositions,reticleSprite.transform.rotation);
        _missedTargets = 0;
        _hitTargets = 0;
        _totalTargets = 0;
        _minigameEffectiveness = 0f;

        _currentAmmo = maxAmmo;
        Set_Ammo_Display();

        Time.timeScale = 1.0f;
        _gameRunning = false;
    }

    void Update() //Called once per frame.
    {
        if(_gameRunning)
        {
            if(!_isSpawning)
            {
                StartCoroutine(Create_Target_Group(_targetGroup));
            }

            if(_isReloading)
            {
                _reloadProgress += Time.deltaTime;
                reloadProgressBar.value = _reloadProgress/reloadTime;
            }

            Age_Targets();
            Handle_Inputs();
        }
    }

    //--------------------------------------------------------------------------------------------
    // Mutators
    //--------------------------------------------------------------------------------------------
    private void Set_Aim(int pos_)
    {
        _reticle.transform.position = _gridPositions[pos_];
    }

    private void Set_Ammo_Display()
    {
        ammoText.text = _currentAmmo + "/" + maxAmmo;
    }

    private void Set_Hits_Text()
    {
        hitsText.text = "Hits: " + _hitTargets;
    }

    private void Set_Timer_Text(int seconds_)
    {
        timerText.text = $"{seconds_}";
    }


    //---------------------------------------------------------------------------------------------
    //  Event Subscriptions
    //---------------------------------------------------------------------------------------------
    public void Time_Over()
    {
        StopAllCoroutines();
        _isSpawning = false;
        _isReloading = false;
        _gameRunning = false;
        _targets.Kill_All();
        _minigameEffectiveness = (float)_hitTargets/(float)_totalTargets;

        Reset_State();
    }
    public void Update_Timer(int seconds_)
    {
        Set_Timer_Text(seconds_);
    }
    public void Start_Minigame()
    {
        if(!_gameRunning)
        {
            _hitTargets = 0;
            Set_Hits_Text();
            _minigameEffectiveness = 0f;
            _gameRunning = true;
        }
    }
    //--------------------------------------------------------------------------------------------
    //  General Game Methods
    //--------------------------------------------------------------------------------------------
    private void Age_Targets()
    {
        for(int pos_ = 1; pos_ <= 9; pos_++)
        {
            if(_targets.Has_Target(pos_))
            {
                _targets.Add_Time(pos_,Time.deltaTime);

                if(_targets.Is_Too_Old(pos_)) //Target is dying of old age.
                {
                    _missedTargets++;
                    _targets.Kill_Target(pos_);
                }
            }
        }
    }

    IEnumerator Create_Target_Group(int[][] targetGroup_)
    {
        _isSpawning = true;

        foreach(int[] group_ in targetGroup_)
        {
            foreach(int pos_ in group_)
            {
                _targets.Create_Target(pos_);
                _totalTargets++;
                yield return new WaitForSeconds(targetSpawnRate);
            }
            
            yield return new WaitForSeconds(groupSpawnRate);
        }

        _isSpawning = false;
    }

    
    void Fire_Weapon()
    {
        _currentAmmo--;
        Set_Ammo_Display();
        audioSource.PlayOneShot(audioClipArray[0],volume);
        GameObject clone = Instantiate(bangSprite,_gridPositions[_aimPos],transform.rotation);
        clone.transform.SetParent(_parent);
        clone.transform.localScale = _spriteScale;
        Destroy(clone, 0.1f);

        if(_targets.Has_Target(_aimPos))
        {
            _hitTargets++;
            Set_Hits_Text();
            _targets.Kill_Target(_aimPos);
        }
    }
    IEnumerator Reload()
    {
        _isReloading = true;
        _currentAmmo = 0;
        Set_Ammo_Display();
        audioSource.PlayOneShot(audioClipArray[1], volume);
        yield return new WaitForSeconds(reloadTime);
        _currentAmmo = maxAmmo;
        Set_Ammo_Display();
        _isReloading = false;
        _reloadProgress = 0f;
        reloadProgressBar.value = 0f;
    }

    private void Reset_State()
    {
        _aimPos = 5;
        Set_Aim(_aimPos);

        _currentAmmo = maxAmmo;
        Set_Ammo_Display();
        _missedTargets = 0;
        _totalTargets = 0;
    }

    //----------------------------------------------------------------------
    //  Input Handling
    //----------------------------------------------------------------------
    void Handle_Inputs()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            inkSplatter.Splat();
        }

        if (Input.GetButtonDown("GridRight"))
        {
            Update_Aim_Horizontal(_MoveDirections.right);
        }
        if(Input.GetButtonDown("GridLeft"))
        {
            Update_Aim_Horizontal(_MoveDirections.left);
        }
        if(Input.GetButtonDown("GridUp"))
        {
            Update_Aim_Vertical(_MoveDirections.up);
        }
        if(Input.GetButtonDown("GridDown"))
        {
            Update_Aim_Vertical(_MoveDirections.down);
        }
        if(Input.GetButtonDown("GridReload"))
        {
            if(!_isReloading)
            {
                if(_currentAmmo < maxAmmo)
                {
                    StartCoroutine(Reload());
                }
            }
        }
        if(Input.GetButtonDown("GridFire"))
        {
            if(_currentAmmo > 0)
            {
                Fire_Weapon();
            }
            else
            {
                audioSource.PlayOneShot(audioClipArray[2], volume);
                if(!_isReloading)
                {
                    StartCoroutine(Reload());
                }
            }
        }
    }
    private void Update_Aim_Horizontal(_MoveDirections dir_)
    {
        if(dir_ == _MoveDirections.right)
        {
            switch(_aimPos)
            {
                case 3:
                    _aimPos = 1;
                    break;
                case 6:
                    _aimPos = 4;
                    break;
                case 9:
                    _aimPos = 7;
                    break;
                default:
                    _aimPos++;
                    break;
            }
        }
        else if(dir_ == _MoveDirections.left )
        {
            switch(_aimPos)
            {
                case 1:
                    _aimPos = 3;
                    break;
                case 4:
                    _aimPos = 6;
                    break;
                case 7:
                    _aimPos = 9;
                    break;
                default:
                    _aimPos--;
                    break;
            }
        }
        Set_Aim(_aimPos);
    }
    void Update_Aim_Vertical(_MoveDirections dir_)
    {
        if(dir_ == _MoveDirections.up)
        {
            switch(_aimPos)
            {
                case 1: 
                    _aimPos = 7;
                    break;
                case 2:
                    _aimPos = 8;
                    break;
                case 3:
                    _aimPos = 9;
                    break;
                default:
                    _aimPos = _aimPos - 3;
                    break;
            }
        }
        else if(dir_ == _MoveDirections.down)
        {
            switch(_aimPos)
            {
                case 7:
                    _aimPos = 1;
                    break;
                case 8:
                    _aimPos = 2;
                    break;
                case 9:
                    _aimPos = 3;
                    break;
                default:
                    _aimPos = _aimPos + 3;
                    break;
            }
        }
        Set_Aim(_aimPos);
    }
    
    //--------------------------------------------------------------------------
    //  Initial setup
    //--------------------------------------------------------------------------
    private void Setup_Grid()
    {
        //Top Row
        _gridPositions.Add(1, new Vector3(-2.9f, 2.9f,0f));
        _gridPositions.Add(2, new Vector3(0f,    2.9f,0f));
        _gridPositions.Add(3, new Vector3(2.9f,  2.9f,0f));
        //Middle Row
        _gridPositions.Add(4, new Vector3(-2.9f, 0f,0f));
        _gridPositions.Add(5, new Vector3(0f,    0f,0f));
        _gridPositions.Add(6, new Vector3(2.9f,  0f,0f));
        //Bottom Row
        _gridPositions.Add(7, new Vector3(-2.9f, -2.9f,0f));
        _gridPositions.Add(8, new Vector3(0f,    -2.9f,0f));
        _gridPositions.Add(9, new Vector3(2.9f,  -2.9f,0f));

        _grid = Instantiate(gridSprite, _gridPositions[5], transform.rotation);
        _grid.transform.SetParent(_parent);
        _grid.transform.localScale = _spriteScale;

        Debug.Log($"Grid Parent: {_grid.transform.parent}");

        _reticle = Instantiate(reticleSprite, _gridPositions[5], transform.rotation);
        _reticle.transform.SetParent(_parent);
        _reticle.transform.localScale = _spriteScale;
        
    }
    private void Setup_Target_Group()
    {
        _targetGroup = new int[][]
        {
            new int[]{1,2,3},
            new int[]{4,5,6},
            new int[]{7,8,9},
            new int[]{1,5,9},
            new int[]{7,5,3}
        };
    }
}
