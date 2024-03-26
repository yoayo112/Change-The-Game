/*
Project: Change the Game
File: Cowboy.cs
Date Created: March 17, 2024
Author(s): Sky Vercauteren
Info:

Holds cowboy's stats and handles combat gui functions.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Cowboy : Player
{
    //unexposed vars.
    private Camera _main;
    private Camera _overlay;
    private GameObject _shooter;
    //---------------------------------------------------------------
    //Permanent stats (if we want to balance the cowboy differently)
    //---------------------------------------------------------------
    protected StatsStruct _cowboyStats = new StatsStruct(
        50,   //armor
        20,   //attack power
        20,   //heal power
        100,  //max health
        100,  //max energy
        80   //speed
    );


    //----------------------------------------------------------------
    //Action Methods
    //----------------------------------------------------------------
    
    //wrapper function for the coroutine below called by button click
    public void Attack()
    {
        //ask player to target enemy (TODO)
        base._targets = new int[] { Random.Range(0, 2) };

        StartCoroutine(Minigame());
    }

    public IEnumerator Minigame()
    {
        //display minigame
        _shooter = GameObject.Find("Cowboy Minigame");
        _main = GameObject.Find("Main Camera").GetComponent<Camera>();
        _overlay = _shooter.GetComponentInChildren<Camera>();
        _main.GetUniversalAdditionalCameraData().cameraStack.Add(_overlay);
        _shooter.GetComponentInChildren<MiniGameTimer>().Start_Countdown();
        GlobalService.Find_Canvas_In_Children(gameObject, "CombatGUI").gameObject.SetActive(false);

        //waiting for shooter MG to start
        yield return new WaitWhile(() => !_shooter.GetComponentInChildren<GridShooterController>().Get_gameRunning());

        //waiting for shooter MG to finish
        yield return new WaitWhile(() => _shooter.GetComponentInChildren<GridShooterController>().Get_gameRunning());

        //get effectiveness and animate
        float _effectiveness = _shooter.GetComponentInChildren<GridShooterController>().Get_Effectiveness();
        _main.GetUniversalAdditionalCameraData().cameraStack.Remove(_overlay);
        transform.parent.GetComponentInChildren<CowboyAction>().GetOut();

        Animator animator = gameObject.GetComponentInChildren<Animator>();
        yield return GlobalService.AnimWait(animator, "Attack", "combat_attack");

        transform.parent.GetComponentInChildren<CowboyAction>().PutAway();
        //send the event
        Attack_Characters(CharacterType.enemy, _targets, _effectiveness);
        
        //finally tells dear ol' grandma Character.cs she can call End_Turn()
        _executingTurn = false;
    }

    //----------------------------------------------------------------
    //Overridden Starting Method
    //----------------------------------------------------------------
    public override void Set_Starting_Stats()
    {
        Set_StatsStruct(_cowboyStats);
        Set_CharacterType(CharacterType.player);
    }
}
