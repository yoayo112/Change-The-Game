using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCamLookAt : MonoBehaviour
{

    public Transform cowboyPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cowboyPos = cowboyPrefab.position;
        Vector3 camObjPos = transform.position;
        Quaternion lookRotation = Quaternion.LookRotation((cowboyPos-camObjPos).normalized);

        transform.rotation = lookRotation;
    }
}
