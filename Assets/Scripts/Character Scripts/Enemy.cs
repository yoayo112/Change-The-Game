/*
Project: Change the Game
File: Character.cs
Date Created: March 03, 2024
Author(s): Sean Thornton
Info:
Enemy combat controller and stats. Extend this class to make specific enemies.

*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    private float armorMin;
    private float armorMax;

    private int attackPowerMin;
    private int attackPowerMax;

    private int healPowerMin;
    private int healPowerMax;

    private int healthMin;
    private int healthMax;

    private int energyMin;
    private int energyMax;

    private int speedMin;
    private int speedMax;

    //------------------------------------------------------------------------------------
    // Accessors
    //------------------------------------------------------------------------------------

    private float Get_ArmorMin() => armorMin;
    private float Get_ArmorMax() => armorMax;

    public int Get_AttackPowerMin() => attackPowerMin;
    public int Get_AttackPowerMax() => attackPowerMax;

    public int Get_HealPowerMin() => healPowerMin;
    public int Get_HealPowerMax() => healPowerMax;

    public int Get_HealthMin() => healthMin;
    public int Get_HealthMax() => healthMax;

    public int Get_EnergyMin() => energyMin;
    public int Get_EnergyMax() => energyMax;

    public int Get_SpeedMin() => speedMin;
    public int Get_SpeedMax() => speedMax;

    //------------------------------------------------------------------------------------
    // Mutators
    //------------------------------------------------------------------------------------

    public void Set_ArmorMin(float armorMin_) => armorMin = armorMin_;
    public void Set_ArmorMax(float armorMax_) => armorMax = armorMax_;

    public void Set_AttackPowerMin(int attackPowerMin_) => attackPowerMin = attackPowerMin_;
    public void Set_AttackPowerMax(int attackPowerMax_) => attackPowerMax = attackPowerMax_;

    public void Set_HealPowerMin(int healPowerMin_) => healPowerMin = healPowerMin_;
    public void Set_HealPowerMax(int healPowerMax_) => healPowerMax = healPowerMax_;

    public void Set_HealthMin(int healthMin_) => healthMin = healthMin_;
    public void Set_HealthMax(int healthMax_) => healthMax = healthMax_;

    public void Set_EnergyMin(int energyMin_) => energyMin = energyMin_;
    public void Set_EnergyMax(int energyMax_) => energyMax = energyMax_;

    public void Set_SpeedMin(int speedMin_) => speedMin = speedMin_;
    public void Set_SpeedMax(int speedMax_) => speedMax = speedMax_;


    //Overridden method in Character.cs. This is called when the Enemy is initialized by the CombatController.
    //Sets starting stats based off of ranges. Ranges are set in Set_Ranges() which must be defined in the subclass.
    //See _ENEMY_TEMPLATE.cs for a default example of this.
    public override void Set_Starting_Stats()
    {
        Set_Ranges();

        Set_CharacterType(CharacterType.enemy);
        Set_Armor(UnityEngine.Random.Range(armorMin, armorMax));
        Set_AttackPower(UnityEngine.Random.Range(attackPowerMin, attackPowerMax + 1));
        Set_HealPower(UnityEngine.Random.Range(healPowerMin, healPowerMax + 1));
        Set_MaxHealth(UnityEngine.Random.Range(healthMin, healthMax + 1));
        Set_MaxEnergy(UnityEngine.Random.Range(energyMin, energyMax + 1));
        Set_Speed(UnityEngine.Random.Range(speedMin, speedMax + 1));

        Set_CurrentHealth(Get_MaxHealth());
        Set_CurrentEnergy(Get_MaxEnergy());
    }

    //----------------------------------------------------------------------------
    // Stubs for subclass methods. Override these when you extend the class
    //----------------------------------------------------------------------------
    public virtual void Set_Ranges()
    {
        return;
    }
}
