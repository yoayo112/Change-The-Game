/*
Project: Change the Game
File: Character.cs
Date Created: March 01, 2024
Author(s): Elijah Theander, Sean Thornton, Sky Vercauteren
Info:
player Stats script that stores stats about players,
handles whether or not a character is living, and processes
outgoing and incoming damage with events. extends Character.cs

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Player : Character
{
    //Starting Values for when combat starts. We will revert to these at combat end.
    //Basically, only using these dictionaries to allow specific stats to be mutable 
    //while in combat, but not outside of combat.
    protected StatsStruct _permanentStats = new StatsStruct(
        50, //armor
        20,   //attack power
        20,   //heal power
        100,  //max health
        100,  //max energy
        100  //speed
    );

    //temp storage for each turn
    protected List<int> _targets;
    private GameObject _currentArrow;
    private int _selectedIndex;
    protected bool _targetIsConfirmed;
    protected float _effectiveness;
    protected CharacterType _targetType;
    private bool _click;
    private Vector3 _clickLocation;
    private List<GameObject> _cursors;

    //minigame vars
    protected Camera _main;
    protected Camera _overlay;
    protected string _title;
    protected GameObject _minigame;
    protected string _mgPrefab;
    protected MinigameBase _mgScript;
    protected GameObject _targetingGUI;
    protected Button _confirmTarget;
    protected bool _firstRound = true;

    //Animation Vars
    protected string _trigger;
    protected string _actionState;

    //------------------------------------------------------------------------------------
    // Methods for updating starting stats(permanent stats, update on level up or such.)
    //------------------------------------------------------------------------------------
    public void Set_PermanentArmor(int val_) => _permanentStats.armor = val_;
    public void Set_PermanentAttackPower(int val_) => _permanentStats.attackPower = val_;
    public void Set_PermanentHealPower(int val_) => _permanentStats.healPower = val_;
    public void Set_PermanentMaxHealth(int val_) => _permanentStats.maxHealth = val_;
    public void Set_PermanentMaxEnergy(int val_) => _permanentStats.maxEnergy = val_;
    public void Set_PermanentSpeed(int val_) => _permanentStats.speed = val_;
    public void Set_PermanentStats(int armor_, int ap_, int hp_, int maxHealth_, int maxEnergy_, int speed_)
    {
        Set_PermanentArmor(armor_);
        Set_PermanentAttackPower(ap_);
        Set_PermanentHealPower(hp_);
        Set_PermanentMaxEnergy(maxEnergy_);
        Set_PermanentMaxHealth(maxHealth_);
        Set_PermanentSpeed(speed_);
    }
    

    //------------------------------------------------------------------------------------
    //Update
    //------------------------------------------------------------------------------------
    void Update()
    {
        //currently the only thing update is needed for is to check for target clicks
        if(!_click)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _click = true;
                _clickLocation = Input.mousePosition;
            }
        }
    }

    //------------------------------------------------------------------------------------
    // Stat Modification methods (temporary modifications to stats during combat.)
    //------------------------------------------------------------------------------------
    public void Modify_Armor(int modVal_)
    {
        //do whatever calulation makes sense here.
        int modified_ = Get_Armor();

        modified_ -= modVal_;

        Set_Armor(modified_);
    }

    public void Modify_AttackPower(int modVal_)
    {
        int modified_ = Get_AttackPower();

        modified_ -= modVal_;

        Set_AttackPower(modified_);
    }

    public void Modify_HealPower(int modVal_)
    {
        int modified_ = Get_HealPower();

        modified_ -= modVal_;

        Set_HealPower(modified_);
    }

    public void Modify_MaxHealth(int modVal_)
    {
        int modified_ = Get_MaxHealth();

        modified_ -= modVal_;

        Set_MaxHealth(modified_);
    }

    public void Modify_MaxEnergy(int modVal_)
    {
        int modified_ = Get_MaxEnergy();

        modified_ -= modVal_;

        Set_MaxEnergy(modified_);
    }

    public void Modify_Speed(int modVal_)
    {
        int modified_ = Get_Speed();

        modified_ -= modVal_;

        Set_Speed(modified_);
    }

    //----------------------------------------------------------------
    //Overridden Starting Method
    //----------------------------------------------------------------
    public override void Set_Starting_Stats()
    {
        _firstRound = true;
        Set_StatsStruct(_permanentStats);
        Set_CharacterType(CharacterType.player);
    }

    //---------------------------------------------------------------
    //Overriden Execute_Turn Method to show player GUI
    //---------------------------------------------------------------
    public override void Execute_Turn()
    {
        Canvas gui = GlobalService.Find_Canvas_In_Children(gameObject, "CombatGUI");
        gui.GetComponent<GraphicRaycaster>().enabled = true;
        gui.gameObject.SetActive(true);
    }

    //-----------------------------------------------------------------
    // Writing currentHealth and currentEnergy values back into
    // "permanent" stats
    //-----------------------------------------------------------------

    public void End_Combat()
    {
        _permanentStats.currentHealth = Get_CurrentHealth();
        _permanentStats.currentEnergy = Get_CurrentEnergy();
        _firstRound = true;
    }

    //----------------------------------------------------------------
    //Minigame Event Cycle
    //----------------------------------------------------------------

    //Called on button click. Wrapper for minigame coroutine.
    //PLZ OVERRIDE ME: for game title, animation state, and trigger for each character!!
    //This won't work with these empty strings!
    public virtual void Minigame_Button()
    {
        _title = "";
        _trigger = "";
        _actionState = "";
        _targetIsConfirmed = false;
        _targetType = CharacterType.enemy; //TODO: You should be able to target non enemies too?
        StartCoroutine(Minigame_Cycle());
    }

    //this determines the order of events regarding a minigame cycle. Override for unique behavior
    public virtual IEnumerator Minigame_Cycle()
    {
        //begin targeting -> (select target -> confirm target)
        yield return Begin_Targeting(1);


        //display the minigame -> (start countdown -> initialize -> wait for gameplay -> animate)
        yield return Run_Minigame();

        //broadcast an event corresponding with the appropriate result of the minigame
        yield return Resulting_Action(_mgScript);

        //cleanup all minigame stuff
        yield return Finish_Minigame();

        //End the turn
        _executingTurn = false;
    }

    //----------------------------------------------------------------
    //Targeting
    //----------------------------------------------------------------

    //Manage combat gui, targeting gui, and target selection/
    public virtual IEnumerator Begin_Targeting(int numberOfTargets_)
    {
        //find game / init variables
        if (_firstRound || _minigame == null)
        {
            _minigame = GameObject.Find(_title);
            _firstRound = false;
        }
        else { _minigame.SetActive(true); }
        _mgScript = _minigame.GetComponentInChildren<MinigameBase>();
        _mgPrefab = _mgScript.gameObject.name;
        _cursors = new List<GameObject>();
        _targetingGUI = _mgScript.targeting_GUI.gameObject;
        _confirmTarget = _targetingGUI.GetComponentInChildren<Button>();
        _confirmTarget.onClick.AddListener(Confirm_Target); //TODO: for some reason you have to click twice?
        _targets = new List<int>(0);
        string targetTag = string.Empty;
        switch (_targetType)
        {
            case CharacterType.enemy: targetTag = "Enemy"; break;
            case CharacterType.player: targetTag = "Player"; break;
            case CharacterType.both: targetTag = "both"; break;
        }

        //hide combat gui.
        _minigame.SetActive(false);
        Canvas gui = GlobalService.Find_Canvas_In_Children(gameObject, "CombatGUI");
        gui.GetComponent<GraphicRaycaster>().enabled = false;
        gui.gameObject.SetActive(false);
       
        //show targeting gui but not the confirm button
        _confirmTarget.gameObject.SetActive(false);
        _targetingGUI.gameObject.SetActive(true);

        //wait for targets to be selected
        for (int i = 0; i < numberOfTargets_; i++)
        {
            _click = false;
            yield return Select_Target(targetTag);
        }
        
        //then hide targeting gui
        _targetingGUI.gameObject.SetActive(false);


        //TODO - Remove targetability from objects?

        //Let the targets hang for a moment.
        yield return new WaitForSeconds(0.5f);
    }

    //raycasts an object/click and assigns a target if its acceptable.
    public virtual IEnumerator Select_Target(string tag)
    {
        //empty target
        _currentArrow = null;
        bool selected = false;
        _targetIsConfirmed = false;
        GameObject selectedObject; //= new GameObject();
        _selectedIndex = -99;

        //wait for raycast click.
        while (!selected || !_targetIsConfirmed)
        {
            yield return new WaitWhile(() => !_click);

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(_clickLocation);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                Debug.Log("Something was clicked");
                //Debug.Log(hit.collider.gameObject.tag);

                //Determine if it was appropriate
                bool acceptable_ = false;
                if (tag == "both")
                {
                    acceptable_ = (hit.collider.gameObject.CompareTag("Enemy") || hit.collider.gameObject.CompareTag("Player"));
                }
                else
                {
                    acceptable_ = hit.collider.gameObject.CompareTag(tag);
                }

                //continue with raycasting the target
                if (acceptable_)
                {
                    Debug.Log("It was an acceptable target");

                    //target selected object
                    selectedObject = hit.collider.gameObject;
                    _selectedIndex = selectedObject.GetComponentInChildren<Character>().Get_Position();
                    selected = true;

                    // spawn cursor at targetted location
                    if (_currentArrow == null)
                    {
                        _currentArrow = Instantiate(Resources.Load<GameObject>("CombatTarget"));
                    }

                    Vector3 selectedPosition = selectedObject.transform.position;
                    _currentArrow.transform.position = new Vector3(selectedPosition.x + 1, selectedPosition.y + 2, selectedPosition.z);
                   
                    //refocus player on new target
                    Vector3 newTarget = new Vector3(selectedPosition.x, transform.position.y, selectedPosition.z);
                    transform.LookAt(newTarget, Vector3.up);

                    //refocus camera on new target
                    combatShot.LookAt = selectedObject.transform;

                    //show confirm button
                    _confirmTarget.gameObject.SetActive(true);

                    _click = false; //exit the block and wait for confirm or target change.
                }
                else
                {
                    //it wasnt a target so wait for the next click
                    _click = false;
                }
            }
            else
            {
                //nothing was clicked
                _click = false;
            }
        }
    }

    //called on click of "confirm" in the targeting gui.
    public void Confirm_Target()
    {
        if (!_targets.Contains(_selectedIndex) && _selectedIndex != -99 && _currentArrow != null)
        {
            //confirm
            _targetIsConfirmed = true;
            
            // add targeted object to list
            _targets.Add(_selectedIndex);
            _cursors.Add(_currentArrow);
        }
    }



    //----------------------------------------------------------------
    //  !! MINIGAME !!
    //----------------------------------------------------------------


    //Use me to trigger and run gameplay, 
    //or override me for a specific/unique minigame.
    public virtual IEnumerator Run_Minigame()
    {
        //display minigame
        _minigame.SetActive(true);
        //_main = GameObject.Find("Main Camera").GetComponent<Camera>();
        _main = GlobalService.Get_Camera_Brain().GetComponent<Camera>();
        _overlay = _minigame.GetComponentInChildren<Camera>();
        _main.GetUniversalAdditionalCameraData().cameraStack.Add(_overlay);

        //fade in
        Animator mg_animator = GlobalService.Find_Canvas_In_Children(_minigame, _mgPrefab).gameObject.GetComponent<Animator>();
        yield return GlobalService.AnimWait(mg_animator, "fade", "Fade In");

        //broadcast start
        _minigame.GetComponentInChildren<MiniGameTimer>().Start_Countdown();

        //wait for gameplay to start
        yield return new WaitWhile(() => !_minigame.GetComponentInChildren<MinigameBase>().Get_isRunning());

        //wait for minigame to finish
        yield return new WaitWhile(() => _minigame.GetComponentInChildren<MinigameBase>().Get_isRunning());
        //fade out
        yield return GlobalService.AnimWait(mg_animator, "fade", "Fade Out");

        //then get the effictiveness
        _effectiveness = _minigame.GetComponentInChildren<MinigameBase>().Get_effectiveness();
        _main.GetUniversalAdditionalCameraData().cameraStack.Remove(_overlay);
        //animate and wait for animation to finish
        Animator animator = gameObject.GetComponentInChildren<Animator>();
        yield return GlobalService.AnimWait(animator, _trigger, _actionState); //TODO: how to better sync attack anim with hurt anim??
    }

    //overload me if a characters minigame needs to be more complex than just "attack"
    public virtual IEnumerator Resulting_Action(MinigameBase _script)
    {
        //Broadcast the event
        //TODO: can we make the character action methods into delegates?
        // -> This way, a function could be passed as an arg on button click and called here,
        //    I.e. minigames could trigger and broadcast other actions, not just attack.
        Attack_Characters(CharacterType.enemy, _targets.ToArray(), _effectiveness);
        yield return null;
    }

    //reset targeting and base minigame variables
    public virtual IEnumerator Finish_Minigame()
    {

        for (int i = 0; i < _cursors.Count; i++)
        {
            Destroy(_cursors[i]);
        }
        _cursors = new List<GameObject>();
        _currentArrow = null;
        _targets = new List<int>();
        _selectedIndex = -99;
        _targetIsConfirmed = false;
        _effectiveness = 0;
        _click = false;
        _minigame.SetActive(false); //I think this is where our gui click problems are coming from?
        //BUT ALSO - If this is false, the GameObject.Find() won't work in the begin_targeting() method.

        yield return null;
    }
}
