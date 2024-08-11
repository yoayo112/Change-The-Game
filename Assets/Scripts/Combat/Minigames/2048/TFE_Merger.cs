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
                GameObject newBubble = Instantiate(bubbleResource);
                newBubble.transform.position = request.position;

                //remove pending request
                mergeRequests.Remove(request.objects[0]);
                pendingMerges.Remove(request);

                //delete both objects
                Destroy(request.objects[0]);
                Destroy(request.objects[1]);
            }
            else //if they cant be mergered, just remove the request
            {
                //remove pending request
                mergeRequests.Remove(request.objects[0]);
                pendingMerges.Remove(request);
            }
        }
    }
}
