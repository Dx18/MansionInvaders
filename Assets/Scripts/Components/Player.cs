using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Player. Tasks and responsibilities:
///
/// 1. Moving;
/// 2. Shooting (cooldown updating, projectile spawning);
/// 3. Managing animations;
/// 4. Storing and updating current health
public class Player : MonoBehaviour {
    // Component parameters

    /// Speed of player
    public float speed;
    /// Shot cooldown
    public float maxCooldown;
    /// Projectile prefab
    public GameObject projectile;
    /// Current health
    public int health;

    // Events

    /// Invoked when player's health is changed
    public event Action<int> onHealthChange;

    // Shot cooldown

    /// Current shot cooldown
    float cooldown;

    // Components

    Rigidbody2D playerRigidbody;
    Animator playerAnimator;

    // API for UIController

    /// Returns ratio between current shot cooldown and initial shot
    /// cooldown
    public float GetCooldownFraction() {
	return Mathf.Min(cooldown / maxCooldown, 1);
    }

    // Unity component messages

    void Start() {
	playerRigidbody = GetComponent<Rigidbody2D>();
	playerAnimator = GetComponent<Animator>();
    }

    void Update() {
	// Moving
	float direction = Input.GetAxisRaw("Horizontal");
	playerRigidbody.velocity = new Vector3(direction * speed, 0, 0);

	// Updating cooldown
	cooldown = Mathf.Max(cooldown - Time.deltaTime, 0);

	// Shooting
	if (Input.GetButtonDown("Fire") && cooldown == 0) {
	    cooldown = maxCooldown;
	    GameObject.Instantiate(projectile, transform.position, Quaternion.identity);
	}

	// Controlling animations
	if (direction < 0) {
	    playerAnimator.Play("MovingLeft");
	} else if (direction > 0) {
	    playerAnimator.Play("MovingRight");
	} else {
	    playerAnimator.Play("Idle");
	}
    }

    void OnTriggerEnter2D(Collider2D other) {
	if (other.tag == "GhostProjectile" && health > 0) {
	    --health;
	    onHealthChange(health);
	}
    }

    // Private section

    /// Resets shot cooldown
    void Reset() {
	cooldown = maxCooldown;
    }
}
