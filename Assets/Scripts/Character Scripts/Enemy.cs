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
    public int level = 1;

    private int armorMin;
    private int armorMax;

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

    public int Get_Level() => level;

    public int Get_ArmorMin() => armorMin;
    public int Get_ArmorMax() => armorMax;

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

    public void Set_Level(int level_) => level = level_;

    public void Set_ArmorMin(int armorMin_) => armorMin = armorMin_;
    public void Set_ArmorMax(int armorMax_) => armorMax = armorMax_;

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
        Set_Armor(Random.Range(armorMin, armorMax + 1));
        Set_AttackPower(Random.Range(attackPowerMin, attackPowerMax + 1));
        Set_HealPower(Random.Range(healPowerMin, healPowerMax + 1));
        Set_MaxHealth(Random.Range(healthMin, healthMax + 1));
        Set_MaxEnergy(Random.Range(energyMin, energyMax + 1));
        Set_Speed(Random.Range(speedMin, speedMax + 1));

        Multiply_Stats();

        Set_CurrentHealth(Get_MaxHealth());
        Set_CurrentEnergy(Get_MaxEnergy());
    }

    public void Multiply_Stats()
    {
        float mult_ = Get_Level_Multiplier();

        Set_Armor((int) (Get_Armor() * mult_));
        Set_AttackPower((int) (Get_AttackPower() * mult_));
        Set_HealPower((int) (Get_HealPower() * mult_));
        Set_MaxHealth((int) (Get_MaxHealth() * mult_));
        Set_MaxEnergy((int) (Get_MaxEnergy() * mult_));
        Set_Speed((int) (Get_Speed() * mult_));
    }

    //----------------------------------------------------------------------------
    // Stubs for subclass methods. Override these when you extend the class
    //----------------------------------------------------------------------------
    public virtual void Set_Ranges()
    {
        return;
    }

    public virtual float Get_Level_Multiplier()
    {
        return 0.9f + (0.1f * level);
    }
}
