using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeTownPortal : MonoBehaviour
{
    public GameObject portal;
    public void GTFO()
    {
        portal.SetActive(true);
    }
}
