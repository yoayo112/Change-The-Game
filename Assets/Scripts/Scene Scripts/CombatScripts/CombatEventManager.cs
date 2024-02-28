using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatEventManager : MonoBehaviour
{
    //On Attack Event for taking damage.
    public delegate void OnDamage(int[] indexes, float damage);
    public static event OnDamage onDamage;

    public static void Take_Damage(int [] indexes, float damage)
    {
        onDamage?.Invoke(indexes, damage);
    }
}
