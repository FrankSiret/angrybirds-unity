using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pig : MonoBehaviour
{
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _damageThreshold;
    private float _currentHealth;
    public GameObject deathEffect;

    public void Awake() {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(float damage) {
        _currentHealth -= damage;
        Debug.Log($"health {_currentHealth}");
        if(_currentHealth <= 0) {
            Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {

        SceneTransition sceneTransition = GetComponent<SceneTransition>();
        Debug.Log($"sceneTransition {sceneTransition}");
        
        if(sceneTransition != null) {
            Debug.Log("sceneTransition not Started");
            return;
        }

        float velocity = collision.relativeVelocity.magnitude;

        Debug.Log($"velocity {velocity}");

        if(velocity > _damageThreshold) {
            TakeDamage(velocity);
        }
    }

    private void Die() {
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject, 0.1f);
    }

}
