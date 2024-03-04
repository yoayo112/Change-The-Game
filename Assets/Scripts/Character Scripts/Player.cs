using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    //Starting Values for when combat starts. We will revert to these at combat end.
    //Basically, only using these dictionaries to allow specific stats to be mutable 
    //while in combat, but not outside of combat.
    private Dictionary<string, int> _startingStats = new Dictionary<string, int>()
    {
        {"attackPower", 0},
        {"healPower", 0},
        {"maxHealth", 0},
        {"maxEnergy", 0},
        {"speed", 0}
    };

    private float _startingArmor = 0f;

    //These values can be changed during combat.
    private Dictionary<string, int> _combatStats = new Dictionary<string, int>();
    private float _combatArmor = 0f;

    //------------------------------------------------------------------------------------
    // Stat Modification methods
    //------------------------------------------------------------------------------------
    public void Modify_Armor(float modVal_)
    {
        //do whatever calulation makes sense here.
        float modified_ = _combatArmor;

        modified_ -= modVal_;

        _combatArmor = modified_;
        Set_Armor(_combatArmor);
    }

    public void Modify_AttackPower(int modVal_)
    {
        int modified_ = _combatStats["attackPower"];

        modified_ -= modVal_;

        _combatStats["attackPower"] = modified_;
        Set_AttackPower(modified_);
    }

    public void Modify_HealPower(int modVal_)
    {
        int modified_ = _combatStats["healPower"];

        modified_ -= modVal_;

        _combatStats["healPower"] = modified_;
        Set_HealPower(modified_);
    }

    public void Modify_MaxHealth(int modVal_)
    {
        int modified_ = _combatStats["maxHealth"];

        modified_ -= modVal_;

        _combatStats["maxHealth"] = modified_;
        Set_MaxHealth(modified_);
    }

    public void Modify_MaxEnergy(int modVal_)
    {
        int modified_ = _combatStats["maxEnergy"];

        modified_ -= modVal_;

        _combatStats["maxEnergy"] = modified_;
        Set_MaxEnergy(modified_);
    }

    public void Modify_Speed(int modVal_)
    {
        int modified_ = _combatStats["speed"];

        modified_ -= modVal_;

        _combatStats["speed"] = modified_;
        Set_Speed(modified_);
    }

    //----------------------------------------------------------------
    //Overridden Starting Method
    //----------------------------------------------------------------
    public override void Set_Starting_Stats()
    {
        _combatStats = _startingStats;
        _combatArmor = _startingArmor;
        Set_CharacterType(CharacterType.player);

        Set_Armor(_combatArmor);
        Set_AttackPower(_combatStats["attackPower"]);
        Set_HealPower(_combatStats["healPower"]);
        Set_MaxHealth(_combatStats["maxHealth"]);
        Set_MaxEnergy(_combatStats["maxEnergy"]);
        Set_Speed(_combatStats["speed"]);
    }
}
