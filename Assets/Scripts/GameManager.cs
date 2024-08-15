using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] public int MaxNumberOfShots = 3;

    private int _usedNumberOfShots;

    private IconHandle _iconHandler;

    private List<Pig> _pigs = new List<Pig>();

    [SerializeField] private GameObject sceneTransition;

    [SerializeField] private Canvas _canvas;

    [SerializeField] private GameObject _restartScreenObject;

    [SerializeField] private SlingShot _slingShot;

    [Header("Scripts")]
    [SerializeField] private CameraManager _cameraManager;

    private bool _ending = false;
    private bool _started = false;

    private int _points = 0;

    public void Start() {
        _points = 0;
        _restartScreenObject.SetActive(false);
        Instantiate(sceneTransition, Vector3.zero, Quaternion.identity);
    }

    public bool Started {
        get {
            return _started;
        }
        set {
            _started = value;
        }
    }

    public bool IsFinish() {
        return _ending;
    }

    private void Update() {
        Debug.Log("Points: " + _points);
    } 

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        _iconHandler = FindObjectOfType<IconHandle>();

        Pig[] pigs = FindObjectsOfType<Pig>();
        for(int i = 0; i < pigs.Length; i++) {
            _pigs.Add(pigs[i]);
        }
    }

    public void UseShot() {
        _usedNumberOfShots++;
        _iconHandler.UseShot(_usedNumberOfShots);
    }

    public bool HasEnoughShots() {
        return _usedNumberOfShots < MaxNumberOfShots;
    }

    public bool NoBodyMoving(bool checkBird) {
        Rigidbody2D bird = GetComponent<Rigidbody2D>();
        Pig[] pigs = FindObjectsOfType<Pig>();
        Glass[] glasses = FindObjectsOfType<Glass>();

        for(int i = 0; i < pigs.Length; i++) {
            if(pigs[i].GetComponent<Rigidbody2D>().velocity.magnitude >= 0.1f) {
                return false;
            }
        }
        for(int i = 0; i < glasses.Length; i++) {
            if(glasses[i].GetComponent<Rigidbody2D>().velocity.magnitude >= 0.1f) {
                return false;
            }
        }
        return !checkBird || bird == null || bird.velocity.magnitude < 0.1f;
    }

    private void LateUpdate() {
        if(!_ending && _usedNumberOfShots == MaxNumberOfShots) {
            bool collided = true;
            Bird bird = FindObjectOfType<Bird>();
            if(bird != null) {
                collided = bird.GetCollided();
            }
            bool noBodyMoving = NoBodyMoving(true);
            
            if(noBodyMoving && collided) {
                _ending = true;
                StartCoroutine(CheckAfterWaitTime());
            }
        }
    }

    private IEnumerator CheckAfterWaitTime() {
        yield return new WaitForSeconds(3f);
        
        if(_pigs.Count == 0) {
            WinGame();
        } else {
            LoseGame();
        }
    }

    public void RemovePig(Pig pig) {
        bool removed = _pigs.Remove(pig);
        if(removed) {
            _points += 5000;
            CheckForAllDeadPig();
        }
    }

    private void CheckForAllDeadPig() {
        if(_pigs.Count == 0) {
            _ending = true;
            StartCoroutine(WinWaitTime());
        }
    }

    private IEnumerator WinWaitTime() {
        yield return new WaitForSeconds(3f);
        WinGame();
    }

    public void EnabledSlingShot(bool enabled) {
        if(!_started && enabled) {
            return;
        }
        if(_ending && enabled) {
            return;
        }
        _slingShot.enabled = enabled;
    }

    #region Win/Lose

    public void WinGame() {
        Debug.Log("You win!");
        EnabledSlingShot(false);
        
        _cameraManager.SwitchToIdleCamera();

        _restartScreenObject.SetActive(true);
    }
    
    public void LoseGame() {
        Debug.Log("You lose!");
        EnabledSlingShot(false);
        
        _cameraManager.SwitchToIdleCamera();

        _restartScreenObject.SetActive(true);
    }

    public void RestartGame() {
        Debug.Log("Restart game!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion
}
