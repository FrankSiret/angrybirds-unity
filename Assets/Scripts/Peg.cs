using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Peg : MonoBehaviour {
    public LineRenderer[] lineRenderers;
    public Transform[] stickPositions;
    public Transform center;
    public Transform initialPosition;

    public Vector3 currentPosition;

    public float maxLength;

    public float bottomBoundary;

    bool isMouseDown;

    public GameObject birdPrefab;

    public float birdPositionOffset;

    Rigidbody2D bird;
    Collider2D birdCollider;

    public float force;

    public GameObject victory;

    private bool _won = false;

    // find all enemies in the scene
    private int enemiesAlive {
        get {
            return FindObjectsOfType<Pig>().Length;
        }
    }

    void Start() {
        lineRenderers[0].positionCount = 2;
        lineRenderers[1].positionCount = 2;
        lineRenderers[0].SetPosition(0, stickPositions[0].position);
        lineRenderers[1].SetPosition(0, stickPositions[1].position);

        CreateBird();
    }

    void CreateBird() {
        bird = Instantiate(birdPrefab).GetComponent<Rigidbody2D>();
        birdCollider = bird.GetComponent<Collider2D>();
        birdCollider.enabled = false;

        bird.isKinematic = true;

        ResetLines();
    }

    void Update() {
        if (isMouseDown) {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10;

            currentPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            currentPosition = center.position + Vector3.ClampMagnitude(currentPosition - center.position, maxLength);

            currentPosition = ClampBoundary(currentPosition);

            SetLines(currentPosition);

            if (birdCollider) {
                birdCollider.enabled = true;
            }
        } else {
            ResetLines();
        }

        if(!_won && enemiesAlive <= 0) {
            Debug.Log("You win!");
            _won = true;
        }

        if(_won) {
            Bird bird = FindAnyObjectByType<Bird>();
            bool noBodyMoving = bird == null || bird.noBodyMoving(false);
            if(noBodyMoving) {
                StartCoroutine(WaitAndReload());
            }
        }
    }

    private void OnMouseDown() {
        isMouseDown = true;
    }

    private void OnMouseUp() {
        isMouseDown = false;
        Shoot();
        currentPosition = initialPosition.position;
    }

    void Shoot() {
        bird.isKinematic = false;
        Vector3 birdForce = (currentPosition - center.position) * force * -1;
        bird.velocity = birdForce;

        bird.GetComponent<Bird>().Release();

        bird = null;
        birdCollider = null;
        Invoke("CreateBird", 2);
    }

    void ResetLines() {
        currentPosition = initialPosition.position;
        SetLines(currentPosition);
    }

    void SetLines(Vector3 position) {
        lineRenderers[0].SetPosition(1, position);
        lineRenderers[1].SetPosition(1, position);

        if (bird) {
            Vector3 dir = position - center.position;
            bird.transform.position = position + dir.normalized * birdPositionOffset;
            bird.transform.right = -dir.normalized;
        }
    }

    Vector3 ClampBoundary(Vector3 vector) {
        vector.y = Mathf.Clamp(vector.y, bottomBoundary, 1000);
        return vector;
    }

    IEnumerator WaitAndReload() {
        yield return new WaitForSeconds(3);
        Instantiate(victory, new Vector3(-0.52f, 2.96f, 0), Quaternion.identity);
    }
}
