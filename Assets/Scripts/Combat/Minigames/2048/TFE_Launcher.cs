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
    private float depth;
    private Vector3 origin;
    public float speed = 15f;
    public float mass = 5;
    // Start is called before the first frame update
    void Start()
    {
        origin = spawnPoint.transform.position;
        depth = wall.transform.position.z - origin.z;
        depth = depth / 1.5f;
    }

    // Update is called once per frame
    void Update()
    {
        //find and look at mouse.
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, depth+0.1f);
        RaycastHit hit;
        Physics.Raycast(mgOverlay.ScreenToWorldPoint(mousePos),mgOverlay.transform.forward, out hit, depth+0.1f);
        Vector3 target = new Vector3(hit.point.x, hit.point.y-origin.y, hit.point.z);
        launcher.transform.LookAt(target);
        //spawnPoint.transform.LookAt(target);

        //onclick
        if (Input.GetMouseButtonDown(0))
        {
            //create a cube with mass and physics at the given spawn point.
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = spawnPoint.transform.position;
            Rigidbody rb = cube.AddComponent<Rigidbody>();
            rb.mass = mass;

            //ok time to math this out. Hello trig.

            /**
             * first, find the height of the arc given y which is the height of the mouse on the wall.
             * see discord for visual work. 
             * basically, we can mirror a triangle over the y axis, and since we know the length of A (the distance of the full arc), 
             * and also that the wall at y is HALF that same length, we can find the angle theta with height = y, length = depth/2.
             * Now, with theta, we can now apply this information to the initial triangle, with original depth to find height.
             * 
             * so we find the height of the triangle given a cross section from the point on a wall (mouse position y)
             * **/
            Debug.Log(target);
            float theta = Mathf.Tan((target.y) / (depth / 2));
            float height = Mathf.Tan(theta) * depth;

            /**
             * second, now that we have found the height, we need to find a new angle, phi, such that with a known initial velocity, gravity, 
             * and arc height, a projectile fired at angle phi will hit the wall target height (mouse position y).
             * google says the angle of a projectile arc is the arcsin of the square root of 2 x gravity x arc height, divided by initial velocity.
             * **/

            float phi = Mathf.Asin(Mathf.Sqrt(2f * 9.81f * height) / speed);
            Debug.Log(phi);
            /**
             * Lastly, now that we have the force applied to the projectile at angle phi, we simply need this in terms of x velocity, y velocity and z velocity,
             * since the rigidbody object uses vector3's to operate.
             *
             * A google search reveals that force vectors follow trig rules, 
             * so the force of x is the square root of the force of y sqared plus the force of z squared, etc.
             * 
             * lets just use sohcahtoa
             ***/
            float fy = Mathf.Cos(phi) * speed;
            float fz = Mathf.Sin(phi) * speed;

            Vector3 force = new Vector3(0,fy, fz);
            rb.AddForce(force, ForceMode.Impulse);

        }

    }
}
