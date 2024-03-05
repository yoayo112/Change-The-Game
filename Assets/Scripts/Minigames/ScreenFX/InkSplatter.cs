/*
Project: Change the Game
File: InkSplatter.cs
Date Created: March 04, 2024
Author(s): Sean Thornton
Info:

Script that controls ink splatter effect
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkSplatter : MonoBehaviour
{
    public Canvas canvas;
    public GameObject[] splats;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Check_Input();
    }

    void Check_Input()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("SPLAT");
            Make_Splat();
        }
    }


    public void Make_Splat()
    {
        float randX_ = Random.Range(-88.8f, 88.8f);
        float randY_ = Random.Range(-50.0f, 50.0f);

        UnityEngine.Object.Instantiate(splats[Random.Range(0, splats.Length)], new Vector3(randX_, randY_, 0.0f), canvas.transform.rotation);
    }
}
