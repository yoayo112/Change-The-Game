/*
Project: Change the Game
File: CombatEventManager.cs
Date Created: March 01, 2024
Author(s): Elijah Theander

Info:
Defines and handles custom C# delegates and events for combat scenes.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatEventManager : MonoBehaviour
{
    //Characters invoke this when they attack, other characters intercept.
    public delegate void OnDamage(CharacterType type, int[] indexes, float damage);
    public static event OnDamage onDamage;

    //Characters invoke this when they heal, other characters intercept.
    public delegate void OnHeal(CharacterType type, int[] indexes, float healing);
    public static event OnHeal onHeal;

    //CombatController invokes this to tell characters who is up.
    public delegate void OnStartTurn(int turnPosition);
    public static event OnStartTurn onStartTurn;

    //Characters invoke this to tell CombatController they are done
    public delegate void OnEndTurn();
    public static event OnEndTurn onEndTurn;

    //----------------------------------------------------------------
    // Methods that invoke
    //----------------------------------------------------------------

    public static void Deal_Damage(CharacterType type, int[] indexes, float damage)
    {
        onDamage?.Invoke(type, indexes, damage);
    }

    public static void Heal_Damage(CharacterType type, int[] indexes, float healing)
    {
        onHeal?.Invoke(type, indexes, healing);
    }
    public static void Start_Turn(int turnPosition_)
    {
        onStartTurn?.Invoke(turnPosition_);
    }
    public static void End_Turn()
    {
        onEndTurn?.Invoke();
    }


}
