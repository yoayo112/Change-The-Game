using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeHandler : MonoBehaviour
{
    //exposed
    public float delayTime = 1f;
    [Header("Geographic Objects")]
    [Help("Use: \n - Drag the portal object inside a doorway or location to trigger the scene. \n - Type the name of the scene you are going to. \n - Type the name of the spawn object *in the destination scene* you want to start from.", UnityEditor.MessageType.Info)]
    public GameObject portal;
    public string goTo;
    public string spawnName;

    //unexposed
    private Animator animator_;
    private Transform body_;
    static string newSpawn = "";
    // Start is called before the first frame update
    void Start()
    {
        animator_ = GameObject.Find("transition").GetComponent<Animator>();
        body_ = GameObject.Find("Cowboy_body").GetComponent<Transform>();
        if (newSpawn != "")
        {
            body_.root.GetComponent<Transform>().position = GameObject.Find(newSpawn).GetComponent<Transform>().position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //conditionlogic for scene transitions.
        if (portal.GetComponent<Collider>().bounds.Contains(body_.position))
        {
            StartCoroutine(fadeOut(goTo));
        }
    }

    IEnumerator fadeOut(string name)
    {
        animator_.SetTrigger("fade-out");
        newSpawn = spawnName;
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene(name);
    }
}

