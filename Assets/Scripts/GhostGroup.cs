using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

/// Group of moving ghosts. Explanation of moving of the top-left
/// ghost (begins at point S):
///
///                  boundLeft     boundLeft + lineWidth
///                      |                   |
///                      V                   V
///
/// boundTop ----------> S>------------------V
///                                          |
///                                          |
/// boundTOp - 1 ------> V-------------------<
///                      |
///                      |
/// boundTop - 2 ------> >-------------------V
///                                          |
///                                          |
/// boundTop - 3 ------> V-------------------<
///                      |
///                     ...
public class GhostGroup {
    // Events

    /// Invoked on ghost death
    public event Action<int, int> onGhostDeath;

    // Configuration

    /// Min X coordinate of ghosts with min possible X
    float boundLeft;
    /// Initial Y coordinate of ghosts with max possible Y
    float boundTop;
    /// Width of "line" on which ghosts are moving
    float lineWidth;
    /// Number of ghosts horizontally and vertically
    Vector2Int ghostCount;
    /// Speed of ghosts
    float speed;
    /// Lower bound of shooting cooldown
    float maxCooldownLower;
    /// Upper bound of shooting cooldown
    float maxCooldownUpper;

    // Movement progress

    /// Distance passed by each alive ghost
    float progress;

    // Dead ghosts tracking

    /// Dead (inactive) ghosts
    bool[,] isDead;
    /// Number of alive (active) ghosts
    int aliveCount;

    // Shooting control

    /// Column in which ghost will shoot next
    int nextShotColumn;
    /// Time before shot
    float shotCooldown;

    // Powerup control

    /// True if and only if there is no powerup either falling or
    /// currently used by player
    bool powerUpEnabled;

    // Initialization

    public GhostGroup(float boundLeft, float boundTop, float lineWidth,
		      Vector2Int ghostCount, float speed,
		      float maxCooldownLower, float maxCooldownUpper) {
	this.boundLeft = boundLeft;
	this.boundTop = boundTop;
	this.lineWidth = lineWidth;
	this.ghostCount = ghostCount;
	this.speed = speed;
	this.maxCooldownLower = maxCooldownLower;
	this.maxCooldownUpper = maxCooldownUpper;

	progress = 0;

	isDead = new bool[ghostCount.x, ghostCount.y];
	aliveCount = ghostCount.x * ghostCount.y;

	ResetShotState();

	powerUpEnabled = true;
    }

    // API for GhostCoordinator

    /// Updates ghost group
    public void Update(float delta) {
	progress += speed * delta;
	shotCooldown = Mathf.Max(shotCooldown - delta, 0);
    }

    // API for Ghost

    /// Calculates position of given ghost
    public Vector2 CalculatePosition(int indexX, int indexY) {
	return new Vector2(boundLeft + indexX, boundTop - indexY) +
	    CalculateRelativePosition();
    }

    /// Invoked when given ghost dies
    public void OnDeath(int indexX, int indexY) {
	if (!isDead[indexX, indexY]) {
	    isDead[indexX, indexY] = true;

	    if (!IsColumnAlive(indexX)) {
		ResetShotState();
	    }

	    onGhostDeath(indexX, indexY);
	}
    }

    /// Returns true if and only if given ghost is allowed to shoot.
    /// Depends on:
    ///
    /// 1. Whether given ghost is in the correct column;
    /// 2. Whether given ghost is the first in the column;
    /// 3. Current value of shot cooldown
    public bool ShouldShoot(int indexX, int indexY) {
	return nextShotColumn == indexX && shotCooldown == 0 &&
	    IsFirstInColumn(indexX, indexY);
    }

    /// Invoked when ghost shots. Does not check the
    /// permission. Resets shot state
    public void OnShot() {
	ResetShotState();
    }

    /// Invoked when ghosts attempts to drop a powerup. If it is
    /// possible then disables current powerup drop ability and
    /// returns true. Otherwise returns false
    public bool OnPowerUpDropAttempt() {
	if (powerUpEnabled) {
	    powerUpEnabled = false;
	    return true;
	}

	return false;
    }

    // API for PowerUp

    /// Invoked when powerup is destroyed: either by reaching map
    /// bound or by being used up by the player
    public void OnPowerUpDestruction() {
	powerUpEnabled = true;
    }

    // Private section

    /// Calculates position of a ghost relative to its starting point
    Vector2 CalculateRelativePosition() {
	float localProgress = progress % (2 * lineWidth + 2);
	int periodIndex = (int) (progress / (2 * lineWidth + 2));

	if (localProgress <= lineWidth) {
	    return new Vector2(localProgress, -periodIndex * 2);
	} else if (localProgress <= lineWidth + 1) {
	    return new Vector2(lineWidth, -periodIndex * 2 - localProgress + lineWidth);
	} else if (localProgress <= 2 * lineWidth + 1) {
	    return new Vector2(2 * lineWidth + 1 - localProgress, -periodIndex * 2 - 1);
	} else {
	    return new Vector2(0, -periodIndex * 2 - localProgress + 2 * lineWidth);
	}
    }

    /// Returns true if and only if given ghost is alive and is the
    /// first in its column
    bool IsFirstInColumn(int indexX, int indexY) {
	if (isDead[indexX, indexY]) {
	    return false;
	}

	for (int i = indexY + 1; i < ghostCount.y; ++i) {
	    if (!isDead[indexX, i]) {
		return false;
	    }
	}

	return true;
    }

    /// Resets shot state
    void ResetShotState() {
	int[] aliveColumns = GetAliveColumns();

	if (aliveColumns.Length == 0) {
	    return;
	}

	nextShotColumn = aliveColumns[Random.Range(0, aliveColumns.Length)];
	shotCooldown = Random.Range(maxCooldownLower, maxCooldownUpper);
    }

    /// Returns true if and only if given column is "alive", that is
    /// contains an alive ghost
    bool IsColumnAlive(int x) {
	for (int y = 0; y < ghostCount.y; ++y) {
	    if (!isDead[x, y]) {
		return true;
	    }
	}
	return false;
    }

    /// Returns array of "alive" columns
    int[] GetAliveColumns() {
	int count = 0;
	for (int x = 0; x < ghostCount.x; ++x) {
	    if (IsColumnAlive(x)) {
		++count;
	    }
	}

	int[] result = new int[count];
	int current = 0;
	for (int x = 0; x < ghostCount.x; ++x) {
	    for (int y = 0; y < ghostCount.y; ++y) {
		if (!isDead[x, y]) {
		    result[current] = x;
		    ++current;
		    break;
		}
	    }
	}

	return result;
    }
}
