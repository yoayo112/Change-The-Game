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
    private float depth; // the distance between the canon and the wall
    private float height; // the height of the WALL, not the arc.
    private Vector3 origin; // the location of the canon
    private float velocity = 17.98f;
    public float mass = 5;
    // Start is called before the first frame update
    void Start()
    {
        origin = spawnPoint.transform.position;
        depth = wall.transform.position.z - origin.z;
        Debug.Log("wall:" + wall.transform.position.z + " - origin " + origin.z + " = depth " + depth);
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
            Debug.Log("target.y " + target.y + " - origin.y " + origin.y + " = " + (target.y - origin.y));
            //create a cube with mass and physics at the given spawn point.
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = spawnPoint.transform.position;
            Rigidbody rb = cube.AddComponent<Rigidbody>();
            rb.mass = mass;
            float v = velocity + ((4.45f * y) / height);

            //ok time to math this out. Hello trig.

            //the relationship between target.y (or just y) and the total length of the arc is
            //R = d + ((d * y)/h
            float r = depth + ((depth*y)/height);
            Debug.Log("d + (dy/h) = "+r);

            //theta = arcsin(rg/v^2)
            float theta = Mathf.Asin((r * 9.81f) / (v * v))/2;
            Debug.Log("th = (asin((r*9.81)/(v^2))/2) = " + theta);
            //theta = theta * Mathf.Rad2Deg;
            Debug.Log("as radians: " + theta);

            float fz = Mathf.Cos(theta) * v;
            float fy = Mathf.Sin(theta) * v;
            Debug.Log("fy= " + fy + " ,fz= " + fz);

            Vector3 force = new Vector3(0,fy, fz);
            rb.AddForce(force, ForceMode.Impulse);

        }

    }
}
