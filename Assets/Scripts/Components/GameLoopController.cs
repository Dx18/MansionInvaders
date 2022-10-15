using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Controls game loop. Knows about `UIController`, `Player` and
/// `GhostCoordinator`. Tasks and responsibilities:
///
/// 1. Observing UI for the request to start new session;
/// 2. Spawning player and ghost coordinator when the new session is
///    started;
/// 3. Tracking score;
/// 4. Spawning new waves of ghosts if current one is cleared by the
///    player
public class GameLoopController : MonoBehaviour {
    // Component parameters

    /// Object with `UIController` script
    public UIController uiController;
    /// Object with `Player` script
    public Player player;
    /// Object with `GhostCoordinator` script
    public GhostCoordinator ghostCoordinator;

    // State

    /// True if and only if play session is running
    bool isGameRunning;
    /// Current score
    int score;
    /// Number of alive ghosts
    int aliveCount;

    // Instances of player and ghosts

    /// Instance of player in current play session
    Player playerInstance;
    /// Instance of ghost coordinator in current play session
    GhostCoordinator ghostCoordinatorInstance;

    // Unity component messages

    void Start() {
	uiController.onGameStart += () => {
            // Handling the request to start session
	    if (!isGameRunning) {
		isGameRunning = true;

		SpawnPlayer();
		SpawnGhostGroup();

		uiController.OnPlayerSpawn(playerInstance);
	    }
	};
    }

    // Private section

    /// Spawns player
    void SpawnPlayer() {
	playerInstance = GameObject.Instantiate(player)
	    .GetComponent<Player>();
		
	playerInstance.onHealthChange += (int newHealth) => {
	    if (newHealth == 0) {
		ResetGame();
	    }
	};
    }

    /// Spawns ghost group
    void SpawnGhostGroup() {
	ghostCoordinatorInstance = GameObject.Instantiate(ghostCoordinator)
	    .GetComponent<GhostCoordinator>();

	Vector2Int ghostCount = ghostCoordinatorInstance.ghostCount;
	aliveCount = ghostCount.x * ghostCount.y;

	ghostCoordinatorInstance.onGhostDeath += (int x, int y) => {
	    ++score;
	    --aliveCount;
	    uiController.OnScoreUpdate(score);

	    if (aliveCount == 0) {
		SpawnGhostGroup();
	    }
	};
	ghostCoordinatorInstance.onGhostBoundReach += () => {
	    ResetGame();
	};
    }

    /// Stops play session when it is lost
    void ResetGame() {
	if (!isGameRunning) {
	    return;
	}

	Destroy(playerInstance.gameObject);
	Destroy(ghostCoordinatorInstance.gameObject);
	foreach (Projectile projectile in FindObjectsOfType<Projectile>()) {
	    Destroy(projectile.gameObject);
	}
	foreach (PowerUp powerUp in FindObjectsOfType<PowerUp>()) {
	    Destroy(powerUp.gameObject);
	}
	isGameRunning = false;
	uiController.OnPlayerDeath(score);
	score = 0;
    }
}
