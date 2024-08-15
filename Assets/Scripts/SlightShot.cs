using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SlightShot : MonoBehaviour {
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

    
    [Header("Sounds")]
    [SerializeField] private AudioClip[] _elasticPulledClips;
    [SerializeField] private AudioClip[] _elasticReleaseClips;

    private AudioSource _audioSource;

    void Awake() {
        _audioSource = GetComponent<AudioSource>();
    }

    void Start() {
        lineRenderers[0].positionCount = 2;
        lineRenderers[1].positionCount = 2;
        lineRenderers[0].SetPosition(0, stickPositions[0].position);
        lineRenderers[1].SetPosition(0, stickPositions[1].position);

        CreateBird();
    }

    public void CreateBird() {
        bird = Instantiate(birdPrefab).GetComponent<Rigidbody2D>();
        birdCollider = bird.GetComponent<Collider2D>();
        birdCollider.enabled = false;

        bird.isKinematic = true;

        currentPosition = initialPosition.position;
        SetLines(currentPosition);

        GameManager.instance.EnabledSlightShot(true);
    }

    void Update() {
        if (isMouseDown) {

            // play sound repeatedly
            if (!_audioSource.isPlaying) {
                SoundManager.instance.PlayRandomClip(_elasticPulledClips, _audioSource);
            }

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
            currentPosition = initialPosition.position;
        }
    }

    private void OnMouseDown() {
        isMouseDown = true;
        // SoundManager.instance.PlayClip(_elasticPulledClip, _audioSource);
    }

    private void OnMouseUp() {
        isMouseDown = false;
        if(bird == null) return;
        Shoot();
        currentPosition = initialPosition.position;
    }

    void Shoot() {

        GameManager.instance.UseShot();

        SoundManager.instance.PlayRandomClip(_elasticReleaseClips, _audioSource);
        
        bird.isKinematic = false;
        Vector3 birdForce = (currentPosition - center.position) * force * -1;
        bird.velocity = birdForce;

        bird.GetComponent<Bird>().Release();

        bird = null;
        birdCollider = null;

        SetLines(center.position);

        GameManager.instance.EnabledSlightShot(false);
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
}
