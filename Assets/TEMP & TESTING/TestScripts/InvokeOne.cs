using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageEvent: UnityEvent<int,float>
{

}
public class InvokeOne : MonoBehaviour
{
    public DamageEvent m_Damage;
    // Start is called before the first frame update
    void Start()
    {
        if(m_Damage == null)
        {
            m_Damage = new DamageEvent();
            m_Damage.AddListener(DamageTaken);
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_Damage.Invoke(1,2.4f);
    }

    void DamageTaken(int pos, float damageVal)
    {
        Debug.Log("Position " + pos + "being dealt " + damageVal + "Damage");
    }
}
