using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ENEMY_TEMPLATE : Enemy //Replace "ENEMY_TEMPLATE" with your class name.
{
    //Modify these values to define the ranges of the enemy stats.
    private const float ARMOR_MIN = 0.0f;
    private const float ARMOR_MAX = 0.0f;
    private const int ATTACK_POWER_MIN = 0;
    private const int ATTACK_POWER_MAX = 0;
    private const int HEAL_POWER_MIN = 0;
    private const int HEAL_POWER_MAX = 0;
    private const int HEALTH_MIN = 0;
    private const int HEALTH_MAX = 0;
    private const int ENERGY_MIN = 0;
    private const int ENERGY_MAX = 0;
    private const int SPEED_MIN = 0;
    private const int SPEED_MAX = 0;

    /* This is where the enemy does its action on its turn.
     * Use CombatController.players and .enemies and .turnNumber for combat info
     * The target positions of players characters will be in the range [0, CombatController.players.Count())
     * The target positions of enemy characters will be in the range [CombatController.MAX_PLAYERS, CombatController.MAX_PLAYERS + CombatController.enemies.Count]
     * 
     * Use - Attack_Enemies(int[] targets_, float effectiveness_) to attack multiple targets
     *       Attack_Enemy(int target_) to attack a single target
     *       Take_Damage(float damage_) to take self damage
     *       Heal_Characters(int[] targets_, float effectiveness_) to heal multiple targets
     *       Heal_Character(int target_, effeictness_) to heal a single target
     *       Take_Healing(float healing_) to heal self
    */
    public override void Execute_Turn()
    {
        /*
         * Example turn, attacking random player that is alive
         * 

        int playerCount_ = CombatController.players.Count;
        int randomTarget_ = UnityEngine.Random.Range(0, playerCount_);
        int target_ = -1;

        for (int i = 0; i < playerCount_; i++)
        {
            target_ = (randomTarget_ + i) % playerCount_;
            if (CombatController.players[target_].Is_Alive())
            {
                break;
            }
        }

        Attack_Enemy(target_, 0.0f);
        */
    }

    // Sets the stats ranges for the enemy when it is initialized
    // Leave as is unless you have special logic to determine ranges.
    public override void Set_Ranges()
    {
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
