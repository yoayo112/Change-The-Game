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
        StartCoroutine(Begin_Targeting(1));
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
