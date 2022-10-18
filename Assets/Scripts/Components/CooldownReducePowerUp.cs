using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Powerup that reduces shooting cooldown of the player
public class CooldownReducePowerUp : PowerUp {
    // Component parameters

    /// Amount by which player's cooldown is divided
    public float divider;

    // Implementation of `PowerUp`

    public override void OnBegin(Player player) {
	player.maxCooldown /= divider;
    }

    public override void OnEnd(Player player) {
	player.maxCooldown *= divider;
    }
}
