using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SlingShot : MonoBehaviour {

    [Header("Line Renderers")]
    [SerializeField] private LineRenderer[] _lineRenderers;

    [Header("Transforms References")]
    [SerializeField] private Transform[] stickPositions;
    [SerializeField] private Transform _centerPosition;
    [SerializeField] private Transform _initialPosition;
    [SerializeField] private Transform _elasticTransform;

    [SerializeField] private Vector3 _idlePosition;

    [Header("Slingshot Stats")]
    [SerializeField] private float _maxDistance;
    [SerializeField] private float _shotForce;
    [SerializeField] private float _bottomBoundary;
    [SerializeField] private float _elasticDivider = 1.2f;
    [SerializeField] private AnimationCurve _elasticCurve;

    [Header("Bird")]
    [SerializeField] private GameObject _birdPrefab;
    [SerializeField] private float _birdPositionOffset;
    
    [Header("Sounds")]
    [SerializeField] private AudioClip[] _elasticPulledClips;
    [SerializeField] private AudioClip[] _elasticReleaseClips;
    [SerializeField] private AudioSource _audioSource;

    [Header("Scripts")]
    [SerializeField] private CameraManager _cameraManager;

    private Rigidbody2D _bird;
    private Collider2D _birdCollider;
    private GameObject _instantiateBird;
    private bool _isMouseDown;

    void Awake() {
        _audioSource = GetComponent<AudioSource>();
    }

    void Start() {
        _lineRenderers[0].positionCount = 2;
        _lineRenderers[1].positionCount = 2;
        _lineRenderers[0].SetPosition(0, stickPositions[0].position);
        _lineRenderers[1].SetPosition(0, stickPositions[1].position);

        CreateBird();
    }

    public void CreateBird() {
        _elasticTransform.DOComplete();
        _instantiateBird = Instantiate(_birdPrefab);
        _bird = _instantiateBird.GetComponent<Rigidbody2D>();
        _birdCollider = _bird.GetComponent<Collider2D>();
        _birdCollider.enabled = false;

        _bird.isKinematic = true;

        _idlePosition = _initialPosition.position;
        SetLines(_idlePosition);

        GameManager.instance.EnabledSlingShot(true);

        _cameraManager.SwitchToIdleCamera();
    }

    void Update() {
        if (_isMouseDown) {

            // play sound repeatedly
            if (!_audioSource.isPlaying) {
                SoundManager.instance.PlayRandomClip(_elasticPulledClips, _audioSource);
            }

            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10;

            _idlePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            _idlePosition = _centerPosition.position + Vector3.ClampMagnitude(_idlePosition - _centerPosition.position, _maxDistance);

            _idlePosition = ClampBoundary(_idlePosition);

            SetLines(_idlePosition);

            if (_birdCollider) {
                _birdCollider.enabled = true;
            }
        } else {
            _idlePosition = _initialPosition.position;
        }
    }

    private void OnMouseDown() {
        _isMouseDown = true;
        _cameraManager.SwitchToFollowCamera(_instantiateBird.transform);
    }

    private void OnMouseUp() {
        _isMouseDown = false;
        if(_bird == null) return;
        Shoot();
        _idlePosition = _initialPosition.position;
    }

    void Shoot() {

        GameManager.instance.UseShot();

        SoundManager.instance.PlayRandomClip(_elasticReleaseClips, _audioSource);
        
        _bird.isKinematic = false;
        Vector3 birdForce = (_idlePosition - _centerPosition.position) * _shotForce * -1;
        _bird.velocity = birdForce;

        _bird.GetComponent<Bird>().Release();

        _bird = null;
        _birdCollider = null;

        AnimateSlingShot();

        GameManager.instance.EnabledSlingShot(false);
    }

    void SetLines(Vector3 position) {
        _lineRenderers[0].SetPosition(1, position);
        _lineRenderers[1].SetPosition(1, position);

        if (_bird) {
            Vector3 dir = position - _centerPosition.position;
            _bird.transform.position = position + dir.normalized * _birdPositionOffset;
            _bird.transform.right = -dir.normalized;
        }
    }

    Vector3 ClampBoundary(Vector3 vector) {
        vector.y = Mathf.Clamp(vector.y, _bottomBoundary, 1000);
        return vector;
    }

    #region Animate SlingShot

    private void AnimateSlingShot() {
        _elasticTransform.position = _lineRenderers[0].GetPosition(1);
        float dist = Vector2.Distance(_elasticTransform.position, _centerPosition.position);
        float time = dist / _elasticDivider;
        _elasticTransform.DOMove(_centerPosition.position, time).SetEase(_elasticCurve);
        StartCoroutine(AnimateSlingShotLines(_elasticTransform, time));
    }

    private IEnumerator AnimateSlingShotLines(Transform trans, float time) {
        float elapsedTime = 0f;
        while(elapsedTime < time) {
            elapsedTime += Time.deltaTime;
            SetLines(trans.position);
            yield return null;
        }
    } 

    #endregion
}
