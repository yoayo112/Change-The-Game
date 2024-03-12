/*
Project: Change the Game
File: CameraShake.cs
Date Created: Februrary 28, 2024
Author(s): Sean Thornton
Info:

Script to add simple camera shake effect to a camera object.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float magnitude = 1f;
    public float duration = 1f;
    public float frequency = 30f;

    public void Shake()
    {
        StartCoroutine(ShakeCamera());
    }

    public IEnumerator ShakeCamera()
    {
        Vector3 originalPos_ = transform.localPosition;

        float wait_ = 1f / frequency;
        float elapsed_ = 0.0f;

        while (elapsed_ < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPos_.x + x, originalPos_.y + y, originalPos_.z);

            elapsed_ += Time.deltaTime + wait_;

            yield return new WaitForSeconds(wait_);
            //yield return null;
        }

        transform.localPosition = originalPos_;
    }
}
