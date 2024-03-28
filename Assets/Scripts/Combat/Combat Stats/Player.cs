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
    protected float _effectiveness;
    protected CharacterType _targetType;
    private bool _click;
    private Vector3 _clickLocation;

    //minigame vars
    private Camera _main;
    private Camera _overlay;
    protected string _title;
    private GameObject _minigame;
    private string _mgPrefab;
    private MinigameBase _mgScript;

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
        Set_StatsStruct(_permanentStats);
        Set_CharacterType(CharacterType.player);
    }

    //---------------------------------------------------------------
    //Overriden Execute_Turn Method to show player GUI
    //---------------------------------------------------------------
    public override void Execute_Turn()
    {
        GlobalService.Find_Canvas_In_Children(gameObject, "CombatGUI").gameObject.SetActive(true);
    }

    //----------------------------------------------------------------
    //Minigame and Targeting System
    //----------------------------------------------------------------

    //Called on button click. Wrapper for minigame coroutine.
    //PLZ OVERRIDE ME: for game title, animation state, and trigger for each character!!
    //This won't work with these empty strings!
    public virtual void Minigame_Button()
    {
        _title = "";
        _trigger = "";
        _actionState = "";
        _targetType = CharacterType.enemy;
        StartCoroutine(Begin_Targeting(1));
    }

    //wait until all targets are selected then run the minigame
    public virtual IEnumerator Begin_Targeting(int numberOfTargets_)
    {
        //find game / init variables
        _minigame = GameObject.Find(_title);
        _mgScript = _minigame.GetComponentInChildren<MinigameBase>();
        _mgPrefab = _mgScript.gameObject.name;
        _targets = new List<int>(0);
        string targetTag = _targetType == CharacterType.enemy ? "Enemy" : "Party";

        //hide combat gui.
        GlobalService.Find_Canvas_In_Children(gameObject, "CombatGUI").gameObject.SetActive(false);
        //show targeting gui
        _mgScript.targeting_GUI.gameObject.SetActive(true);

        for (int i=0; i< numberOfTargets_; i++)
        {
            _click = false;
            yield return Select_Target(targetTag);
        }

        //TODO - Remove targetability from objects?

        //Let the targets hang for a moment.
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(Run_Minigame());
    }

    //called when a clickable target is raycast and clicked
    public virtual IEnumerator Select_Target(string tag)
    {
        //wait for click on targetable object
        bool selected = false;
        GameObject selectedObject = new GameObject();
        int selectedIndex = -99;

        while(!selected)
        {
            yield return new WaitWhile(() => !_click);

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(_clickLocation);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                Debug.Log("Something was clicked");
                if(hit.collider.gameObject.CompareTag(tag))
                {
                    Debug.Log("It was an acceptable target");
                    //hide targeting gui
                    _mgScript.targeting_GUI.gameObject.SetActive(false);
                    //target selected object
                    selectedObject = hit.collider.gameObject;
                    selectedIndex = selectedObject.GetComponentInChildren<Character>().Get_Position();
                    selected = true;

                    if (!_targets.Contains(selectedIndex))
                    {
                        // spawn cursor at targetted location
                        GameObject target = Instantiate(Resources.Load<GameObject>("CombatTarget"));
                        Vector3 selectedPosition = selectedObject.transform.position;
                        target.transform.position = new Vector3(selectedPosition.x + 1, selectedPosition.y + 2, selectedPosition.z);
                        //refocus player on new target
                        Vector3 newTarget = new Vector3(selectedPosition.x, transform.position.y, selectedPosition.z);
                        transform.LookAt(newTarget, Vector3.up);
                        // add targeted object to list
                        _targets.Add(selectedIndex);
                    }
                }
                else
                {
                    //it wasnt a target so wait for the next click
                    _click = false;
                }
            }else{
                //nothing was clicked
                _click = false;
            }
        }
    }


    //Use me to trigger and run gameplay, 
    //or override me for a specific/unique minigame.
    public virtual IEnumerator Run_Minigame()
    {
        //display minigame
        _main = GameObject.Find("Main Camera").GetComponent<Camera>();
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

        //Broadcast the event
        //TODO: can we make the character action methods into delegates?
        // -> This way, a function could be passed as an arg on button click and called here,
        //    I.e. minigames could trigger and broadcast other actions, not just attack.
        Attack_Characters(CharacterType.enemy, _targets.ToArray(), _effectiveness);

        //TODO - Remove all cursors

        //End the turn
        _executingTurn = false;
    }

    //-----------------------------------------------------------------
    // Writing currentHealth and currentEnergy values back into
    // "permanent" stats
    //-----------------------------------------------------------------

    public void End_Combat()
    {
        _permanentStats.currentHealth = Get_CurrentHealth();
        _permanentStats.currentEnergy = Get_CurrentEnergy();
    }
}
