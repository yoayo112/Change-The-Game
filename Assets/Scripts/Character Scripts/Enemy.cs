using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public float armorMin;
    public float armorMax;

    public int attackPowerMin;
    public int attackPowerMax;

    public int healPowerMin;
    public int healPowerMax;

    public int healthMin;
    public int healthMax;

    public int energyMin;
    public int energyMax;

    public int speedMin;
    public int speedMax;

    public float Get_ArmorMin() => armorMin;
    public float Get_ArmorMax() => armorMax;

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

    public override void Set_Starting_Stats()
    {
        Set_Ranges();

        Set_CharacterType(CharacterType.enemy);
        Set_Armor(UnityEngine.Random.Range(armorMin, armorMax));
        Set_AttackPower(UnityEngine.Random.Range(attackPowerMin, attackPowerMax));
        Set_HealPower(UnityEngine.Random.Range(healPowerMin, healPowerMax));
        Set_MaxHealth(UnityEngine.Random.Range(healthMin, healthMax));
        Set_MaxEnergy(UnityEngine.Random.Range(energyMin, energyMax));
        Set_Speed(UnityEngine.Random.Range(speedMin, speedMax));

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
