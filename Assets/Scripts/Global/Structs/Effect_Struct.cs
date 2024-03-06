using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct StatusEffect
{
    public StatusType type;
    public int level;
    public int turnsLeft;


    public StatusEffect(StatusType type_, int level_, int turns_)
    {
        type = type_;
        level = level_;
        turnsLeft = turns_;
    }
}