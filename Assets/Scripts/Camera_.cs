using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_ : MonoBehaviour
{
    [SerializeField] private float xCameraStop;

    private Vector3 _initialPosition;

    void Start()
    {
        _initialPosition = transform.position;
    }

    void LateUpdate()
    {
        Bird bird = FindObjectOfType<Bird>();
        if(bird == null) return;

        Transform birdTransform = bird.transform;

        if (birdTransform.position.x > transform.position.x && birdTransform.position.x < xCameraStop) {
            transform.position = new Vector3(birdTransform.position.x, transform.position.y, transform.position.z);
        }
    }

    public void ResetPosition() {
        StartCoroutine(ResetPositionCoroutine());
    }

    IEnumerator ResetPositionCoroutine() {
        yield return new WaitForSeconds(2f);
        transform.position = _initialPosition;
        if(GameManager.instance.HasEnoughShots()) {
            SlingShot slingShot = FindObjectOfType<SlingShot>();
            if(slingShot != null) slingShot.CreateBird();
        }
    }
}
