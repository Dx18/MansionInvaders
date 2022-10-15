using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Controls ghoup of ghosts. Tasks and responsibilities:
///
/// 1. Spawning ghosts;
/// 2. Updating `GhostGroup` instance;
public class GhostCoordinator : MonoBehaviour {
    // Component parameters

    /// Min X coordinate of ghosts with min possible X
    public float boundLeft;
    /// Initial Y coordinate of ghosts with max possible Y
    public float boundTop;
    /// Width of "line" on which ghosts are moving
    public float lineWidth;
    /// Number of ghosts horizontally and vertically
    public Vector2Int ghostCount;
    /// Speed of ghosts
    public float speed;
    /// Lower bound of shooting cooldown
    public float maxCooldownLower;
    /// Upper bound of shooting cooldown
    public float maxCooldownUpper;

    /// Ghost prefab
    public GameObject ghost;

    // Events

    /// Invoked when a ghost dies
    public event Action<int, int> onGhostDeath;
    /// Invoked when a ghost reaches map's bottom bound or touches the
    /// player
    public event Action onGhostBoundReach;

    // Group state

    /// Current ghost group
    GhostGroup ghostGroup;

    // Unity component messages

    void Start() {
	ghostGroup = new GhostGroup(boundLeft, boundTop, lineWidth,
				    ghostCount, speed,
				    maxCooldownLower,
				    maxCooldownUpper);
	ghostGroup.onGhostDeath += onGhostDeath;

	// Spawning ghosts
	for (int x = 0; x < ghostCount.x; ++x) {
	    for (int y = 0; y < ghostCount.y; ++y) {
		Ghost instance = GameObject.Instantiate(ghost, transform)
		    .GetComponent<Ghost>();

		instance.Init(ghostGroup, x, y);
		instance.transform.position = ghostGroup.CalculatePosition(x, y);
		instance.onGhostBoundReach += onGhostBoundReach;
	    }
	}
    }

    void Update() {
	ghostGroup.Update(Time.deltaTime);
    }
}
