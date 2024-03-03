using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TO USE:  1.  Select your camera you want to shake in scene hiearchy
//          2.  In inspector, Add Component > CameraShake
//          3.  In your scene script, create public field for CameraShake object. e.g. "public CameraShake cameraShakeObject;"
//          4.  In your scene script, call the Shake method with StartCoroutine(cameraShakeObject.Shake(duration, magnitude))
//          5.  Add the camera to the public field created in the script in inspector

public class CameraShake : MonoBehaviour
{
    public IEnumerator Shake(float duration_, float magnitude_)
    {
        Vector3 originalPos_ = transform.localPosition;

        float speed_ = 0.01f;
        float elapsed_ = 0.0f;

        while (elapsed_ < duration_)
        {
            float x = Random.Range(-1f, 1f) * magnitude_;
            float y = Random.Range(-1f, 1f) * magnitude_;

            transform.localPosition = new Vector3(originalPos_.x + x, originalPos_.y + y, originalPos_.z);

            elapsed_ += Time.deltaTime + speed_;

            yield return new WaitForSeconds(speed_);
            //yield return null;
        }

        transform.localPosition = originalPos_;
    }
}
