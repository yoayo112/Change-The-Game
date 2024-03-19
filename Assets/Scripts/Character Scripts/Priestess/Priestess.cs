/*
Project: Change the Game
File: Priestess.cs
Date Created: March 18, 2024
Author(s): Sky Vercauteren
Info:

Holds priestes's stats and handles combat gui functions.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Priestess : Player
{
    //unexposed vars.
    private Camera _main;
    private Camera _overlay;
    private GameObject _typer;
    //---------------------------------------------------------------
    //Permanent stats (if we want to balance the cowboy differently)
    //---------------------------------------------------------------
    protected StatsStruct _priestessStats = new StatsStruct(
        50,   //armor
        20,   //attack power
        20,   //heal power
        100,  //max health
        100,  //max energy
        50   //speed
    );


    //----------------------------------------------------------------
    //Action Methods
    //----------------------------------------------------------------
    //wrapper function for the coroutine below called by button click
    public void Attack()
    {
        //ask player to target enemy (TODO)
        base._targets = new int[] { 0 };

        //display minigame
        StartCoroutine(Minigame());
    }

    //This whole thing is juist aseries of waits until the minigame has started and finieshed.
    private IEnumerator Minigame()
    {
        //display minigame
        _typer = GameObject.Find("Priestess Minigame");
        _main = GameObject.Find("Main Camera").GetComponent<Camera>();
        _overlay = _typer.GetComponentInChildren<Camera>();
        _main.GetUniversalAdditionalCameraData().cameraStack.Add(_overlay);
        Find_Canvas("CombatGUI").gameObject.SetActive(false);

        //broadcast start
        _typer.GetComponentInChildren<MiniGameTimer>().Start_Countdown();

        //wait for gameplay
        yield return new WaitWhile(() => !_typer.GetComponentInChildren<TypingGame>().Get_isRunning());

        //wait for minigame to finish
        yield return new WaitWhile(() => _typer.GetComponentInChildren<TypingGame>().Get_isRunning());

        //then get the effictiveness and animate
        _effectiveness = _typer.GetComponentInChildren<TypingGame>().Get_effectiveness();
        _main.GetUniversalAdditionalCameraData().cameraStack.Remove(_overlay);
        yield return Animate_Attack("BAKED Combat-Attack");

        //Broadcast the event
        Attack_Characters(CharacterType.enemy, _targets, _effectiveness);

        //End the turn
        _executingTurn = false;
    }

    //----------------------------------------------------------------
    //Overridden Starting Method
    //----------------------------------------------------------------
    public override void Set_Starting_Stats()
    {
        Set_StatsStruct(_priestessStats);
        Set_CharacterType(CharacterType.player);
    }
}
