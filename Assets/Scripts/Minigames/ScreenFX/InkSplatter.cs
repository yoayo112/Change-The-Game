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
    public float duration = 3.0f;

    public float slideLength = 10f;
    public float startOpacity = 1f;

    public float xRange = 88.8f;
    public float yRange = 50f;

    public bool followParent = false;

    private GameObject[] splatPrefabs;

    private Vector3 _objectPos;
    private Quaternion _objectRot;

    // Start is called before the first frame update
    void Start()
    {
        splatPrefabs = Resources.LoadAll<GameObject>("Prefabs/InkSplatter");
        _objectPos = this.gameObject.transform.position;
        _objectRot = this.gameObject.transform.rotation;
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

        float timeAlive_ = 0f;
        float opacity_;
        float percentage_;

        while (duration < 0f)   //Splat stays and follows it's parent. Object must be destroyed elsewhere.
        {
            if (followParent)
            {
                originalPos_ = _objectPos + delta_;
                originalRot_ = _objectRot;
            }

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
