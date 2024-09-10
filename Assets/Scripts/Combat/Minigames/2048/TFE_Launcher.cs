/*
Project: Change the Game
File: TFE_Launcher.cs
Date Created: Aug 10, 2024
Author(s): Sky Vercauteren, Sean Thornton
Info:

handles trigonometry of launching a projectile in the 2048 minigame
*/

using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class TFE_Launcher : MonoBehaviour
{
    public GameObject spawnPoint;
    public GameObject launcher;
    public GameObject wall;
    public Camera mgOverlay;
    private float depth; // the distance between the cannon and the wall
    private float height; // the height of the WALL, not the arc.
    private Vector3 origin; // the location of the cannon
    public float velocity = 19;
    public float mass = 5;
    // Start is called before the first frame update
    void Start()
    {
        origin = spawnPoint.transform.position;
        depth = wall.transform.position.z - origin.z;
        height = 10;
    }

    // Update is called once per frame
    void Update()
    {
        //find and look at mouse.
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, depth+0.1f);
        Vector3 target = mgOverlay.ScreenToWorldPoint(mousePos);
        launcher.transform.LookAt(target);
        //spawnPoint.transform.LookAt(target);

        //onclick
        if (Input.GetMouseButtonDown(0))
        {
            float y = target.y - origin.y;
            float x = target.x - origin.x;
            //create a cube with mass and physics at the given spawn point.
            GameObject projectile = Instantiate(Resources.Load<GameObject>("2048Bubble"));
            //GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Cube);
            projectile.transform.position = spawnPoint.transform.position;
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.mass = mass;
            

            //ok time to math this out. Hello trig.

            //the relationship between target.y (or just y) and the total length of the arc is
            //R = d + ((d * y)/h
            float r = depth + ((depth*y)/height);

            //This apporximation works!
            // - theta = arcsin(rg/v^2)
            //float v = velocity + ((4.45f * y) / height);
            //float theta = Mathf.Asin((r * 9.81f) / (v * v))/2;

            //Lets try seansters monster equation - :(
            //float v = velocity;
            //float k = (2 * 9.81f * y) / (v * v);
            //float bigNumerator = (0 - 0.125f) * Mathf.Sqrt((16 * (y * y)) + (8 * y) + 1) * ((0 - ((64 * (y - 1)) / (((4 * y) + 1) * ((4 * y) + 1)))) + (64 / ((4 * y) + 1)) - (512 / (((4 * y) + 1) * ((4 * y) + 1))));
            //float bigTerm = 0.5f * Mathf.Sqrt((bigNumerator / Mathf.Sqrt(3 - (4 * y))) - (((8 * y) - 2) / ((4 * y) + 1)) + ((2 * ((4 * y) - 1)) / ((4 * y) + 1)) + (32 / (((4 * y) + 1) * ((4 * y) + 1))));
            //float atan = (Mathf.Sqrt(3 - (4 * y)) / Mathf.Sqrt((16 * (y * y)) + (8 * y) + 1)) + bigTerm + (2 / (4 * y) + 1);
            //float theta = 2 * Mathf.PI - (2 * Mathf.Atan(atan));

            //Seans chat GPT solution
            float v = velocity;
            float v2 = (v * v);
            float sqrtTerm = Mathf.Sqrt((v2 * v2) - ((9.81f * 9.81f) * (depth * depth)) - (2 * 9.81f * v2 * y));
            float theta = Mathf.Atan((v2 - sqrtTerm)/(9.81f*depth));

            // - phi = tan(distance x/depth)
            float phi = Mathf.Tan(x / (1.5f*depth));


            float fz = Mathf.Cos(theta) * v;
            float fy = Mathf.Sin(theta) * v;

            float fx = Mathf.Sin(phi) * v;

            Vector3 force = new Vector3(fx, fy, fz);
            rb.AddForce(force, ForceMode.Impulse);

        }

    }
}