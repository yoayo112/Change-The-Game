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
    public float xRange = 88.8f;
    public float yRange = 50f;
    
    public float opacity = .5f;

    public Canvas canvas;
    public GameObject[] splats;

    private Vector3 _canvasPosition;
    private Quaternion _canvasRotation;


    // Start is called before the first frame update
    void Start()
    {
        splats = Resources.LoadAll<GameObject>("Prefabs/InkSplatter");
        _canvasPosition = canvas.transform.position;
        _canvasRotation = canvas.transform.rotation;
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
        SpriteRenderer sr_;

        float deltaX_ = Random.Range(-xRange, xRange);
        float deltaY_ = Random.Range(-yRange, yRange);
        Vector3 delta_ = new Vector3(deltaX_, deltaY_, 0f);

        Vector3 splatPosition_ = _canvasPosition + delta_;

        int rand_ = Random.Range(0, splats.Length);

        GameObject splat_ = UnityEngine.GameObject.Instantiate(splats[rand_], splatPosition_, _canvasRotation);
        sr_ = splat_.GetComponent<SpriteRenderer>();
        sr_.color = new Color(1f, 1f, 1f, opacity);
    }
}
