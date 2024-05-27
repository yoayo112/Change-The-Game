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
    //Overridden minigame trigger
    //----------------------------------------------------------------
    //Called on button click. Wrapper for minigame coroutine.
    public virtual void Minigame_Button()
    {
        _title = "Priestess Minigame";
        _trigger = "Attack";
        _actionState = "BAKED Combat-Attack";
        _targetType = CharacterType.both; 
        StartCoroutine(Begin_Targeting(1));
    }

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
    //Overridden Starting Method
    //----------------------------------------------------------------
    public override IEnumerator Resulting_Action(MinigameBase _script)
    {
        switch(_script.Get_Action_Type())
        {
            case 0:
                //health
                Heal_Characters(CharacterType.player, _targets.ToArray(), _effectiveness);
                break;
            case 1:
                //dps
                Attack_Characters(CharacterType.enemy, _targets.ToArray(), _effectiveness);
                break;
            case 2:
                //buff/debuff
                int modVal = (int)Math.Round(_priestessStats.attackPower * (_effectiveness));
                Modify_AttackPower(0 - modVal);
                //TODO: add complex behaviour
                break;
        }
        yield return null;
    }
}
