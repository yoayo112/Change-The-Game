using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructTesting : MonoBehaviour
{
    public StatsStruct test1;
    public StatsStruct test2;
    // Start is called before the first frame update
    void Start()
    {
        test1 = new StatsStruct(0.1f, 20,20, 100, 100, 50);
        test2 = test1;

        Debug.Log("Test 1");
        Debug.Log(test1);

        Debug.Log("Test 2");
        Debug.Log(test2);

        test1.armor = 4.0f;
        test2.armor = 2.0f;

        Debug.Log("Test 1 now:");
        Debug.Log(test1);

        Debug.Log("Test 2 now:");
        Debug.Log(test2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
