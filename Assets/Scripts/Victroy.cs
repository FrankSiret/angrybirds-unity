using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Victroy : MonoBehaviour
{
    
    private void OnMouseDown() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
