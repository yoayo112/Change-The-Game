using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct StatsStruct
{
    public float armor;
    public int attackPower;
    public int healPower;
    public int maxHealth;
    public int currentHealth;
    public int maxEnergy;
    public int currentEnergy;
    public int speed;

    public StatsStruct(float armor_, int attackPower_, int healPower_, int maxHealth_, int maxEnergy_, int speed_)
    {
        armor         = armor_;
        attackPower   = attackPower_;
        healPower     = healPower_;
        maxHealth     = maxHealth_;
        currentHealth = maxHealth_;
        maxEnergy     = maxEnergy_;
        currentEnergy = maxEnergy_;
        speed         = speed_;
    }

    public override string ToString()
    {
        string msg_ = "-----------------------------------\n";
        msg_ += $"Armor: {armor}\n";
        msg_ += $"AttackPower: {attackPower}\n";
        msg_ += $"HealPower: {healPower}\n";
        msg_ += $"MaxHealth: {maxHealth}\n";
        msg_ += $"CurrentHealth: {currentHealth}\n";
        msg_ += $"MaxEnergy: {maxEnergy}\n";
        msg_ += $"CurrentEnergy: {currentEnergy}\n";
        msg_ += $"Speed: {speed}\n";
        msg_ += "---------------------------------------------";

        return msg_;
    }
}