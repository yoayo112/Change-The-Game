using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TFE_Launcher : MonoBehaviour
{
    public GameObject spawnPoint;
    public GameObject launcher;
    public GameObject background;
    public float depth = 20;
    public float speed = 15f;
    public float mass = 5;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //find and look at mouse.
        Vector3 target = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, depth)); 
        launcher.transform.LookAt(target);
        spawnPoint.transform.LookAt(target);

        //onclick
        if (Input.GetMouseButtonDown(0))
        {
            //create a cube with mass and physics at the given spawn point.
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = spawnPoint.transform.position;
            Rigidbody rb = cube.AddComponent<Rigidbody>();
            rb.mass = mass;

            //calculate the arc based on the initial velocity, distance and gravity.
            float kin = (9.81f * depth) / (speed * speed);
            float angle = 0.5f * Mathf.Asin(kin);
            //convert the angle found with arcsin to x and y coordinated for the appplication of force using sin and cosign.
            angle *= Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * speed;
            float y = Mathf.Sin(angle) * speed;
            rb.AddForce(new Vector3(0,y,x), ForceMode.Impulse);

        }

    }
}
