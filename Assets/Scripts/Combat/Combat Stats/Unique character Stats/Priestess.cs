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
        _targetType = CharacterType.enemy; //TODO: Add versatility in who priestess can target
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
}
