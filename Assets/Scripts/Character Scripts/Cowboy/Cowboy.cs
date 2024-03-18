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
    private int[] _targets;
    //---------------------------------------------------------------
    //Permanent stats (if we want to balance the cowboy differently)
    //---------------------------------------------------------------
    protected StatsStruct _cowboyStats = new StatsStruct(
        50,   //armor
        20,   //attack power
        20,   //heal power
        100,  //max health
        100,  //max energy
        200   //speed
    );


    //----------------------------------------------------------------
    //Action Methods
    //----------------------------------------------------------------
    public void Attack()
    {
        //ask player to target enemy (TODO)
        _targets = new int[]{ 0 };

        //display minigame
        StartCoroutine(Minigame_CountDown());
    }

    //This whole thing is juist aseries of waits until the minigame has started and finieshed.
    private IEnumerator Minigame_CountDown()
    {
        //display minigame
        _shooter = GameObject.Find("Cowboy Minigame");
        _main = GameObject.Find("Main Camera").GetComponent<Camera>();
        _overlay = _shooter.GetComponentInChildren<Camera>();
        _main.GetUniversalAdditionalCameraData().cameraStack.Add(_overlay);
        _shooter.GetComponentInChildren<MiniGameTimer>().Start_Countdown();
        yield return new WaitWhile(() => !_shooter.GetComponentInChildren<GridShooterController>().Get_gameRunning());
        StartCoroutine(Minigame());
    }
    private IEnumerator Minigame()
    {
        //hide the combat gui
        Find_Canvas("CombatGUI").gameObject.SetActive(false);
        //wait for minigame to finish
        yield return new WaitWhile(() => _shooter.GetComponentInChildren<GridShooterController>().Get_gameRunning());

        //then get the effictiveness and broadcast the attack event.
        float effectiveness_ = _shooter.GetComponentInChildren<GridShooterController>().Get_Effectiveness();
        Attack_Characters(CharacterType.enemy, _targets, effectiveness_);
        _main.GetUniversalAdditionalCameraData().cameraStack.Remove(_overlay);
        

        //and of course animate
        gameObject.GetComponentInChildren<Animator>().SetTrigger("Attack");
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
