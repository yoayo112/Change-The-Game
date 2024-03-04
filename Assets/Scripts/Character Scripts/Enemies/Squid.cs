using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squid : Enemy
{
    private const string NAME = "Squid";

    //Modify these values to define the ranges of the enemy stats.
    private const float ARMOR_MIN = 0.0f;
    private const float ARMOR_MAX = 0.5f;
    private const int ATTACK_POWER_MIN = 12;
    private const int ATTACK_POWER_MAX = 15;
    private const int HEAL_POWER_MIN = 0;
    private const int HEAL_POWER_MAX = 0;
    private const int HEALTH_MIN = 50;
    private const int HEALTH_MAX = 70;
    private const int ENERGY_MIN = 0;
    private const int ENERGY_MAX = 0;
    private const int SPEED_MIN = 50;
    private const int SPEED_MAX = 150;

    public override void Execute_Turn()
    {
        int playerCount_ = CombatController.players.Count;
        int randomTarget_ = UnityEngine.Random.Range(0, playerCount_);
        int target_;

        for (int i = 0; i < playerCount_; i++)
        {
            target_ = (randomTarget_ + i) % playerCount_;
            if (CombatController.players[target_].Is_Alive())
            {
                break;
            }
        }

        Attack_Enemy(target_, 0.0f);
    }

    public override void Set_Ranges()
    {
        Set_Name(NAME);

        Set_ArmorMin(ARMOR_MIN);
        Set_ArmorMax(ARMOR_MAX);

        Set_AttackPowerMin(ATTACK_POWER_MIN);
        Set_AttackPowerMax(ATTACK_POWER_MAX);

        Set_HealPowerMin(HEAL_POWER_MIN);
        Set_HealPowerMax(HEAL_POWER_MAX);

        Set_HealthMin(HEALTH_MIN);
        Set_HealthMax(HEALTH_MAX);

        Set_EnergyMin(ENERGY_MIN);
        Set_EnergyMax(ENERGY_MAX);

        Set_SpeedMin(SPEED_MIN);
        Set_SpeedMax(SPEED_MAX);
    }
}
