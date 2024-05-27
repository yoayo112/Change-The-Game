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
using System;
using UnityEngine;

public class Priestess : Player
{
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
    //Overridden Starting Method
    //----------------------------------------------------------------
    public override void Set_Starting_Stats()
    {
        _firstRound = true;
        Set_StatsStruct(_priestessStats);
        Set_CharacterType(CharacterType.player);
    }

    //----------------------------------------------------------------
    //Overridden minigame cycle 
    //----------------------------------------------------------------
    
    //Called on button click. Wrapper for minigame coroutine.
    public virtual void Minigame_Button()
    {
        _title = "Priestess Minigame";
        _trigger = "Attack";
        _actionState = "BAKED Combat-Attack";
        _targetType = CharacterType.both;
        StartCoroutine(Minigame_Cycle());
    }

    //this determines the order of events regarding a minigame cycle specific to the priestess.
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

    //called by the minigame cycle, specific to the priestess to allow for different spell types.
    public override IEnumerator Resulting_Action(MinigameBase _script)
    {
        switch(_script.Get_Action_Type())
        {
            case 0:
                //dps
                Attack_Characters(CharacterType.enemy, _targets.ToArray(), _effectiveness);
                break;
            case 1:
                //health
                Heal_Characters(CharacterType.player, _targets.ToArray(), _effectiveness);
                break;
            case 2:
                //buff/debuff
                int oldVal = Get_AttackPower();
                int modVal = (int)Math.Round(_priestessStats.attackPower * (_effectiveness));
                Modify_AttackPower(0 - modVal);
                Debug.Log("Priestess buffs attack power from " + oldVal + " -TO- " + Get_AttackPower());
                //TODO: add complex behaviour
                break;
        }
        yield return null;
    }
}
