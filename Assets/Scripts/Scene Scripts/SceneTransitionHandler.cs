using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeHandler : MonoBehaviour
{
    //exposed
    public float delayTime = 1f;
    public GameObject door;
    public string goTo;

    //unexposed
    private Animator animator;
    private Transform body;
    // Start is called before the first frame update
    void Start()
    {
        animator = GameObject.Find("transition").GetComponent<Animator>();
        body = GameObject.Find("Cowboy_body").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        //conditionlogic for scene transitions.
        if (door.GetComponent<Collider>().bounds.Contains(body.position))
        {
            StartCoroutine(fadeOut(goTo));
        }
    }

    IEnumerator fadeOut(string name)
    {

        animator.SetTrigger("fade-out");
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene(name);
    }
}

