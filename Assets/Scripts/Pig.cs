using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Pig : MonoBehaviour
{
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _damageThreshold;
    private float _currentHealth;
    public GameObject deathEffect;

    public GameObject pointEffect;

    [SerializeField] private AudioClip _deathClip;

    private Animator _animator;

    private bool _died = false;

    public void Awake() {
        _currentHealth = _maxHealth;
        _animator = GetComponent<Animator>();
    }

    public void TakeDamage(float damage) {
        _currentHealth -= damage;

        if(_currentHealth < _maxHealth / 2) {
            _animator.SetBool("LowHealth", true);
        }

        if(!_died && _currentHealth <= 0) {
            _died = true;
            Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {

        SceneTransition sceneTransition = GetComponent<SceneTransition>();
        
        if(sceneTransition != null) {
            return;
        }

        float velocity = collision.relativeVelocity.magnitude;

        if(velocity > _damageThreshold) {
            TakeDamage(velocity);
        }
    }

    private void Die() {
        GameManager.instance.RemovePig(this);
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        Instantiate(pointEffect, transform.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(_deathClip, transform.position);
        Destroy(gameObject);
    }

}
