using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Powerup that can be picked up by the player. Tasks and
/// responsibilities:
///
/// 1. Moving;
/// 2. Modifying necessary player abilities;
/// 3. Self-destruction
public abstract class PowerUp : MonoBehaviour {
    // Component parameters

    /// Velocity of powerup
    public Vector2 velocity;
    /// Duration of powerup effect if it is caught by the player
    public float duration;

    // Inner state

    /// Current ghost group
    GhostGroup ghostGroup;

    // Components

    Rigidbody2D powerUpRigidbody;
    SpriteRenderer powerUpSpriteRenderer;

    // Initialization (called by Ghost after instantiating)

    /// Initializes current powerup explicitly
    public void Init(GhostGroup ghostGroup) {
	this.ghostGroup = ghostGroup;
    }

    // Actions current power up does

    /// Invoked when current powerup starts its functioning
    public abstract void OnBegin(Player player);

    /// Invoked when current powerup ends its functioning
    public abstract void OnEnd(Player player);

    // Unity component messages

    void Start() {
	powerUpRigidbody = GetComponent<Rigidbody2D>();
	powerUpSpriteRenderer = GetComponent<SpriteRenderer>();

	powerUpRigidbody.velocity = new Vector3(
	    velocity.x,
	    velocity.y
	);
    }

    void OnTriggerEnter2D(Collider2D other) {
	if (other.tag == "ProjectileBound") {
	    ghostGroup.OnPowerUpDestruction();
	    Destroy(gameObject);
	} else if (other.tag == "Player") {
	    Player player = other.GetComponent<Player>();
	    OnBegin(player);
	    StartCoroutine(DelayOnEnd(player));
	}
    }

    // Private section

    IEnumerator DelayOnEnd(Player player) {
	powerUpRigidbody.simulated = false;
	powerUpSpriteRenderer.enabled = false;
	
	yield return new WaitForSeconds(duration);

	OnEnd(player);
	ghostGroup.OnPowerUpDestruction();
	Destroy(gameObject);
    }
}
