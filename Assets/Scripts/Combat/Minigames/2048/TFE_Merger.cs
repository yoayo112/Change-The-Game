/*
Project: Change the Game
File: TFE_Merger.cs
Date Created: Aug 10, 2024
Author(s): Sky Vercauteren
Info:

handles merges between collisions in the 2048 minigame. Attached to the gravity table
*/


using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class TFE_Merger : MonoBehaviour
{
    private Dictionary<GameObject,GameObject> mergeRequests;
    private List<(Vector3, GameObject[])> pendingMerges;

    // Start is called before the first frame update
    void Start()
    {
        mergeRequests = new Dictionary<GameObject,GameObject>();
        pendingMerges = new List<(Vector3, GameObject[])>();
    }

    // Update is called once per frame
    void Update()
    {
        if(pendingMerges.Count>0)
        {
            merge();
        }
    }

    public void addMergeRequest(GameObject requester, GameObject partner, Vector3 position)
    {
        //if the dictionary has either party in either slot then let the prior merge resolve first.
        //i.e. if there is already a request pending, dont merge!
        if (!mergeRequests.ContainsKey(requester) && !mergeRequests.ContainsKey(partner) && !mergeRequests.ContainsValue(requester) && !mergeRequests.ContainsValue(partner))
        {
            //neither party has a merge request pending
            //add a new request.
            mergeRequests.Add(requester, partner);
            pendingMerges.Add((position, new GameObject[2] { requester, partner}));
        }
    }

    private void merge()
    {
        //we need to check again that its not empty to make sure that there wasnt another call resolving when the frame updated and called this again
        if(pendingMerges.Count > 0)
        {
            (Vector3 position, GameObject[] objects) request = pendingMerges[0];

            //instantiate a new object at the position

            //Will need to switch value sum to find this resource
            int sum = request.objects[0].GetComponent<TFE_Projectile>().value * 2;
            Debug.Log(sum);
            GameObject bubbleResource = null;
            switch(sum)
            {
                case 4:
                    bubbleResource = Resources.Load<GameObject>("TFE_bubble_4");
                    break;
                case 8:
                    bubbleResource = Resources.Load<GameObject>("TFE_bubble_8");
                    break;
                case 16:
                    bubbleResource = Resources.Load<GameObject>("TFE_bubble_16");
                    break;
                case 32:
                    bubbleResource = Resources.Load<GameObject>("TFE_bubble_32");
                    break;
                case 64:
                    bubbleResource = Resources.Load<GameObject>("TFE_bubble_64");
                    break;
                case 128:
                    bubbleResource = Resources.Load<GameObject>("TFE_bubble_128");
                    break;
                case 256:
                    bubbleResource = Resources.Load<GameObject>("TFE_bubble_256");
                    break;
                case 512:
                    bubbleResource = Resources.Load<GameObject>("TFE_bubble_512");
                    break;
                case 1024:
                    bubbleResource = Resources.Load<GameObject>("TFE_bubble_1024");
                    break;
                case 2048:
                    bubbleResource = Resources.Load<GameObject>("TFE_bubble_2048");
                    break;

            }
            if(bubbleResource != null)
            {
                //Cover both objects with animation
                StartCoroutine(animateMerge(request.objects, bubbleResource));


                //remove pending request
                mergeRequests.Remove(request.objects[0]);
                pendingMerges.Remove(request);

            }
            else //if they cant be mergered, just remove the request
            {
                //remove pending request
                mergeRequests.Remove(request.objects[0]);
                pendingMerges.Remove(request);
            }
        }
    }

    private IEnumerator animateMerge(GameObject[] spheres, GameObject bubbleResource)
    {
        //get both spawn points.
        Vector3 P1 = spheres[0].transform.position;
        Vector3 P3 = spheres[1].transform.position;

        //spawn and position animation prefab
        GameObject mergeAnimate = Instantiate(Resources.Load<GameObject>("MergeAnimation"));
        mergeAnimate.transform.position = P1;
        GameObject pointA = mergeAnimate.GetComponentInChildren<TFE_MergeAnimation>().pointA.gameObject;
        GameObject pointB = mergeAnimate.GetComponentInChildren<TFE_MergeAnimation>().pointB.gameObject;

        //scale prefab to correct size
        mergeAnimate.transform.localScale = new Vector3(
            spheres[0].transform.localScale.x * mergeAnimate.transform.localScale.x, 
            spheres[0].transform.localScale.y * mergeAnimate.transform.localScale.y,
            spheres[0].transform.localScale.z * mergeAnimate.transform.localScale.z);

        //change prefab materiel
        pointA.GetComponent<SkinnedMeshRenderer>().material = spheres[0].gameObject.GetComponent<MeshRenderer>().materials[0];
        pointB.GetComponent<SkinnedMeshRenderer>().material = spheres[0].gameObject.GetComponent<MeshRenderer>().materials[0];

        Vector3 P2 = pointB.transform.position;
        //For this trig we are using 3 points, P1, P2, P3 in 3d space, (x,y,z)
        //I am going to define some coordinate differences below, since each will be used 1-2x when finding segment lengths
        float X1 = Mathf.Abs(P3.x - P1.x);     float Y1 = Mathf.Abs(P3.y - P1.y);     float Z1 = Mathf.Abs(P3.z - P1.z);
        float X2 = Mathf.Abs(P2.x - P1.x);     float Y2 = Mathf.Abs(P2.y - P1.y);     float Z2 = Mathf.Abs(P2.z - P1.z);
        float X3 = Mathf.Abs(P3.x - P2.x);     float Y3 = Mathf.Abs(P3.y - P2.y);     float Z3 = Mathf.Abs(P3.z - P2.z);

        //Now I can define some segment lengths as:
        float A = Mathf.Sqrt((Y1 * Y1) + (Z1 * Z1));
        float B = Mathf.Sqrt((Y2 * Y2) + (Z2 * Z2));
        float C = Mathf.Sqrt((Y3 * Y3) + (Z3 * Z3));

        float D = Mathf.Sqrt((X1 * X1) + (Z1 * Z1));
        float E = Mathf.Sqrt((X2 * X2) + (Z2 * Z2));
        float F = Mathf.Sqrt((X3 * X3) + (Z3 * Z3));
        //finally, I can find the angle I need to rotate the shape over the z axis as:
        float theta = Mathf.Acos(((A * A) + (B * B) - (C * C)) / (2 * A * B));
        theta *= Mathf.Rad2Deg;
        //and the angle I need to rotate the sape over the y axis as:
        float phi = Mathf.Acos(((D*D)+(E*E)-(F*F))/(2*D*E));
        phi *= Mathf.Rad2Deg;

        //adjust prefab to cover both spheres
        //Vector3 targetLine = P1 - P2; //find the vector representing the distance between the original objects
        //now we need to rotate the whole object until the line between the animated sphered == this target line
        mergeAnimate.transform.Rotate(new Vector3(0, phi, theta));

        //Ok lastly, it,s not spawning the new object in the correct place. So I am going to find the center of the animation object.
        //i.e. spawn new bubble halfway between sphere a and sphere b
        Vector3 centerLength = P1 - P3;
        float centerLength_x = centerLength.x / 2;
        float centerLength_y = centerLength.y / 2;
        float centerLength_z = centerLength.z / 2;
        Vector3 center = new Vector3(
            P1.x + centerLength_x,
            P1.y,
            P1.z + centerLength_z);


        //delete both objects
        Destroy(spheres[0]);
        Destroy(spheres[1]);

        //start->wait for animation to finish
        yield return GlobalService.AnimWait(mergeAnimate.GetComponentInChildren<Animator>(), "Merge", "Seperate");

        //add resulting bubble
        GameObject newBubble = Instantiate(bubbleResource);
        newBubble.transform.position = center;

        //delete animation
        Destroy(mergeAnimate);
        yield return null;
    }
}
