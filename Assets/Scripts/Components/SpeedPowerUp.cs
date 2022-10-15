using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Powerup that increases player's movement speed
public class SpeedPowerUp : PowerUp {
    // Component parameters

    /// Amount by which player's speed is multiplied
    public float multiplier;

    // Implementation of `PowerUp`
    
    public override void OnBegin(Player player) {
	player.speed *= multiplier;
    }

    public override void OnEnd(Player player) {
	player.speed /= multiplier;
    }
}
