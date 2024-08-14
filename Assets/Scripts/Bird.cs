using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour {
    private bool _collided;

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
        _collided = true;
    }

    void LateUpdate() {

        Rigidbody2D bird = GetComponent<Rigidbody2D>();
        if(bird == null) return;

        if (_collided && noBodyMoving(false) ) {
            Camera.main.GetComponent<Camara>().ResetPosition();
            Destroy(gameObject, 2);
        }

        // if bird is off the scene then destroy it
        if (transform.position.y < -6 && noBodyMoving(true)) {
            Camera.main.GetComponent<Camara>().ResetPosition();
            Destroy(gameObject, 2);
        }
    }

    public float getAccelerate() {
        Rigidbody2D bird = GetComponent<Rigidbody2D>();
        return bird.velocity.magnitude;
    }

    public bool noBodyMoving(bool noCheckBird) {
        Rigidbody2D bird = GetComponent<Rigidbody2D>();
        Pig[] pigs = FindObjectsOfType<Pig>();
        Glass[] glasses = FindObjectsOfType<Glass>();

        for(int i = 0; i < pigs.Length; i++) {
            if(pigs[i].GetComponent<Rigidbody2D>().velocity.magnitude >= 0.1f) {
                Debug.Log("pig velocity: " + pigs[i].GetComponent<Rigidbody2D>().velocity.magnitude);
                return false;
            }
        }
        for(int i = 0; i < glasses.Length; i++) {
            if(glasses[i].GetComponent<Rigidbody2D>().velocity.magnitude >= 0.1f) {
                Debug.Log("glass velocity: " + glasses[i].GetComponent<Rigidbody2D>().velocity.magnitude);
                return false;
            }
        }
        if (!noCheckBird && bird != null)
            Debug.Log("bird velocity: " + bird.velocity.magnitude);
        return noCheckBird || bird == null || bird.velocity.magnitude < 0.1;
    }
}
