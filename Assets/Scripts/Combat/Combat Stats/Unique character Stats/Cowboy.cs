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
using UnityEngine.Rendering.Universal;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.XR;

public class Cowboy : Player
{
    public ParentConstraint GunConstraint;

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
    //Overridden minigame trigger
    //----------------------------------------------------------------
    //Called on button click. Wrapper for minigame coroutine.
    public override void Minigame_Button()
    {
        _title = "Cowboy Minigame";
        _trigger = "Attack";
        _actionState = "combat_attack";
        _targetType = CharacterType.enemy;
        StartCoroutine(Minigame_Cycle());
    }

    //---------------------------------------------------------------
    //Overridden Minigame
    //so that cowboy's gun weights to his hand at the right time in animating,
    //then weights back to holster parent constraint
    //----------------------------------------------------------------

    public override IEnumerator Run_Minigame()
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

        //reparent the gun from the holster to the hand
        Reparent("to hand");

        //animate and wait for animation to finish
        Animator animator = gameObject.GetComponentInChildren<Animator>();
        yield return GlobalService.AnimWait(animator, _trigger, _actionState); //TODO: how to better sync attack anim with hurt anim??

        //re holster gun
        Reparent("to holster");
    }

    //reparents the gun from hand to holster and vice versa
    private void Reparent(string direction)
    {
        List<ConstraintSource> constraints = new List<ConstraintSource>();
        GunConstraint.GetSources(constraints);
        ConstraintSource holster = constraints[0];
        ConstraintSource hand = constraints[1];

        if (direction == "to hand")
        {
            holster.weight = 0;
            hand.weight = 1;
        }
        else if(direction == "to holster")
        {
            holster.weight = 1;
            hand.weight = 0;
        }

        constraints = new List<ConstraintSource>
        {
            holster,
            hand
        };
        GunConstraint.SetSources(constraints);
    }

    //----------------------------------------------------------------
    //Overridden Starting Method
    //----------------------------------------------------------------
    public override void Set_Starting_Stats()
    {
        _firstRound = true;
        Set_StatsStruct(_cowboyStats);
        Set_CharacterType(CharacterType.player);
    }
}
