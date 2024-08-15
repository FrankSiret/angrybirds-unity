using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour {

    [SerializeField] private AudioClip[] _hitClips;

    private AudioSource _audioSource;
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

        // Debug.Log($"MOVING : {_collided && GameManager.instance.NoBodyMoving(true)}");

        if (!_sent && _collided && GameManager.instance.NoBodyMoving(true) ) {
            _sent = true;
            Camera.main.GetComponent<Camara>().ResetPosition();
            Destroy(gameObject, 2);
        }

        // if bird is off the scene then destroy it
        if (!_sent && transform.position.y < -6 && GameManager.instance.NoBodyMoving(false)) {
            _sent = true;
            Camera.main.GetComponent<Camara>().ResetPosition();
            Destroy(gameObject, 2);
        }
    }
}
