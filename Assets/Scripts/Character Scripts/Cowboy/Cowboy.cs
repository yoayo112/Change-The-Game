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
        Debug.Log("Click");
        //ask player to target enemy (TODO)
        int[] targets_;
        targets_ = new int[]{ 0 };

        //display minigame
        StartCoroutine(Minigame(targets_));
    }

    public IEnumerator Minigame(int[] targets_)
    {
        //display minigame
        GameObject shooter = GameObject.Find("Cowboy Minigame");
        Camera main = GameObject.Find("Main Camera").GetComponent<Camera>();
        Camera overlay = shooter.GetComponentInChildren<Camera>();
        main.GetUniversalAdditionalCameraData().cameraStack.Add(overlay);
        shooter.GetComponentInChildren<MiniGameTimer>().Start_Countdown();

        //wait for minigame
        yield return new WaitWhile(() => shooter.GetComponent<GridShooterController>().Get_gameRunning());

        //then get the effictiveness and broadcast the attack event.
        float effectiveness_ = shooter.GetComponent<GridShooterController>().Get_Effectiveness();
        Attack_Characters(CharacterType.enemy, targets_, effectiveness_);
        main.GetUniversalAdditionalCameraData().cameraStack.Remove(overlay);
        Find_Canvas("CombatGUI").gameObject.SetActive(false);

        //and of course animate
        gameObject.GetComponent<Animator>().SetTrigger("Attack");
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
