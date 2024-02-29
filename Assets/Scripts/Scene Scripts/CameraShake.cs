using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public IEnumerator Shake(float duration_, float magnitude_)
    {
        Vector3 originalPos_ = transform.localPosition;

        float elapsed_ = 0.0f;

        while (elapsed_ < duration_)
        {
            float x = Random.Range(-1f, 1f) * magnitude_;
            float y = Random.Range(-1f, 1f) * magnitude_;

            transform.localPosition = new Vector3(originalPos_.x + x, originalPos_.y + y, originalPos_.z);

            elapsed_ += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos_;
    }
}
