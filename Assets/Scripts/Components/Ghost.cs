using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

/// Controls ghost. Tasks and responsibilities:
///
/// 1. Updating position;
/// 2. Spawning projectiles;
/// 3. Spawning powerups;
/// 4. Notifying subscribers about reaching map's bottom bound or
///    touching the player
public class Ghost : MonoBehaviour {
    // Component parameters

    /// Projectile prefab
    public GameObject projectile;
    /// Probability of prefab drop
    public float powerUpProbability;
    /// Array of available powerup prefabs
    public GameObject[] powerUps;

    // Events

    /// Invoked when ghost reaches map's bottom bound or touches the
    /// player
    public event Action onGhostBoundReach;

    // Inner state

    /// Current ghost group
    GhostGroup ghostGroup;
    /// Column of current ghost
    int indexX;
    /// Row of current ghost
    int indexY;

    // Components

    Rigidbody2D ghostRigidbody;

    // Initialization (called by GhostCoordinator after instantiating)

    /// Initializes current ghost explicitly
    public void Init(GhostGroup ghostGroup, int indexX, int indexY) {
	this.ghostGroup = ghostGroup;
	this.indexX = indexX;
	this.indexY = indexY;
    }

    // Unity component messages

    void Start() {
        ghostRigidbody = GetComponent<Rigidbody2D>();
    }

    void Update() {
	ghostRigidbody.MovePosition(ghostGroup.CalculatePosition(indexX, indexY));

	if (ghostGroup.ShouldShoot(indexX, indexY)) {
	    ghostGroup.OnShot();
	    GameObject.Instantiate(projectile, transform.position, Quaternion.identity);
	}
    }

    void OnTriggerEnter2D(Collider2D other) {
	if (other.tag == "PlayerProjectile") {
	    ghostGroup.OnDeath(indexX, indexY);

	    // Attempt to drop powerup on death
	    if (Random.value <= powerUpProbability &&
		ghostGroup.OnPowerUpDropAttempt()) {
		int index = Random.Range(0, powerUps.Length);
		PowerUp instance = GameObject.Instantiate(
		    powerUps[index],
		    transform.position,
		    Quaternion.identity
		).GetComponent<PowerUp>();
		instance.Init(ghostGroup);
	    }

	    Destroy(gameObject);
	} else if (other.tag == "GhostBound" ||
		   other.tag == "Player") {
	    onGhostBoundReach();
	}
    }
}
