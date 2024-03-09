/*
Project: Change the Game
File: InkSplatter.cs
Date Created: March 04, 2024
Author(s): Sean Thornton
Info:

Script that controls ink splatter effect

    TO USE: Add as a component to the object that will be splattered.
            Use inkSplatter.Splat() to cause a splat.
            Use Remove_Splats() to remove all splats.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkSplatter : MonoBehaviour
{
    public float duration = 3.0f;       //Time splat will stay on screen
                                        //for duration < 0: Object will stay until scene ends or is removed by Remove_Splats

    public float slideLength = 10f;     //How far the splats will slide down the y axis
    public float startOpacity = 1f;     //The starting opacity

    public float xRange = 88.8f;        //How far in the x direction the random splat may be placed
    public float yRange = 50f;          //How far in the y direction

    public bool followParent = false;   //Will the splats follow the object as it moves?

    private GameObject[] splatPrefabs;  //Ink Prefabs

    private Vector3 _objectPos;         //Object position
    private Quaternion _objectRot;      //Object rotation
    private int _sortingOrder;

    // Start is called before the first frame update
    void Start()
    {
        splatPrefabs = Resources.LoadAll<GameObject>("Prefabs/InkSplatter"); //Load prefabs
        _objectPos = gameObject.transform.position;
        _objectRot = gameObject.transform.rotation;
        _sortingOrder = gameObject.GetComponent<Canvas>().sortingOrder + 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (followParent)
            Update_Position();
    }

    private void Update_Position() 
    {
        _objectPos = this.gameObject.transform.position;
        _objectRot = this.gameObject.transform.rotation;
    }

    public void Splat()
    {
        StartCoroutine(Make_Splat());
    }

    public void Splat(int val_)
    {
        for (int i = 0; i < val_; i++)
            Splat();
    }

    public IEnumerator Make_Splat()
    { 
        float deltaX_ = Random.Range(-xRange, xRange);
        float deltaY_ = Random.Range(-yRange, yRange);
        Vector3 delta_ = new Vector3(deltaX_, deltaY_, 0f);
        Vector3 originalPos_ = _objectPos + delta_;
        Quaternion originalRot_ = _objectRot;

        int rand_ = Random.Range(0, splatPrefabs.Length);

        GameObject splat_ = Instantiate(splatPrefabs[rand_], originalPos_, originalRot_);

        splat_.tag = "splat";

        if (duration < 0f && !followParent) //Splat stays and does not move. Object must be destroyed elsewhere.
            yield break;

        SpriteRenderer sr_ = splat_.GetComponent<SpriteRenderer>();
        sr_.sortingOrder = _sortingOrder;

        float timeAlive_ = 0f;
        float opacity_;
        float percentage_;

        while (duration < 0f)   //Splat stays and follows it's parent. Object must be destroyed elsewhere.
        {
            if (splat_ == null) //End loop if splat_ is destroyed.
                break;

            originalPos_ = _objectPos + delta_;
            originalRot_ = _objectRot;
            splat_.transform.position = originalPos_;
            splat_.transform.rotation = originalRot_;

            yield return null;
        }

        while (timeAlive_ < duration)   //For duration > 0, the splat stays for duration seconds and slides down the y axis in that time over slideLength.                                       
        {
            if (followParent)
            {
                originalPos_ = _objectPos + delta_;
                originalRot_ = _objectRot;
            }
            percentage_ = timeAlive_ / duration;
            opacity_ = startOpacity * (1 - percentage_);

            sr_.color = new Color(1f, 1f, 1f, opacity_);
            splat_.transform.position = originalPos_ + new Vector3(0, -slideLength * percentage_, 0);
            splat_.transform.rotation = originalRot_;
            yield return null;
            timeAlive_ += Time.deltaTime;
        }

        if (splat_ != null)         //If splat still exists, destroy it.
            Destroy(splat_);  
    }

    public void Remove_Splats()
    {
        GameObject[] splats_ = GameObject.FindGameObjectsWithTag("splat");
        for (int i = 0; i < splats_.Length; i++)
        {
            Destroy(splats_[i]);
        }
    }
}
