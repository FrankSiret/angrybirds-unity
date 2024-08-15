using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour {

    [SerializeField] private AudioClip[] _hitClips;

    private AudioSource _audioSource;
    public GameObject destroyEffect;

    private bool _collided;

    private bool _sent;

    // get collided
    public bool GetCollided() {
        return _collided;
    }

    void Awake() {
        _collided = false;
        _sent = false;
        _audioSource = GetComponent<AudioSource>();
    }

    public void Release() {
        PathPoints.instance.Clear();
        StartCoroutine(CreatePathPoints());
    }

    IEnumerator CreatePathPoints() {
        while (true) {
            if (_collided) break;
            PathPoints.instance.CreateCurrentPathPoint(transform.position);
            yield return new WaitForSeconds(PathPoints.instance.timeInterval);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (!_collided || collision.relativeVelocity.magnitude > 3) {
            SoundManager.instance.PlayRandomClip(_hitClips, _audioSource);
        }
        _collided = true;
    }

    void LateUpdate() {

        Rigidbody2D bird = GetComponent<Rigidbody2D>();
        if(bird == null) return;

        if (!_sent && _collided && GameManager.instance.NoBodyMoving(true) ) {
            _sent = true;
            StartCoroutine(ResetPositionCoroutine());
        }

        // if bird is off the scene then destroy it
        if (!_sent && transform.position.y < -6 && GameManager.instance.NoBodyMoving(false)) {
            _sent = true;
            StartCoroutine(ResetPositionCoroutine());
        }
    }

    IEnumerator ResetPositionCoroutine() {
        yield return new WaitForSeconds(2f);
        Instantiate(destroyEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
        if(GameManager.instance.HasEnoughShots()) {
            SlingShot slingShot = FindObjectOfType<SlingShot>();
            if(slingShot != null) slingShot.CreateBird();
        }
    }
}
