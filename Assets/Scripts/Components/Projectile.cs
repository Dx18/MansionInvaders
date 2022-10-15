using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Projectile shot either by a ghost or the player. Tasks and
/// responsibilities:
///
/// 1. Moving;
/// 2. Self-destruction
public class Projectile : MonoBehaviour {
    // Component parameters

    /// Velocity of projectile
    public Vector2 velocity;
    /// Tag of current projectile's enemy. Used to determine when
    /// current projectile needs to be destroyed
    public string targetTag;

    // Unity component messages

    void Start() {
	GetComponent<Rigidbody2D>().velocity = new Vector3(
	    velocity.x,
	    velocity.y
	);
    }

    void OnTriggerEnter2D(Collider2D other) {
	if (other.tag == "ProjectileBound" || other.tag == targetTag) {
	    Destroy(gameObject);
	}
    }
}
