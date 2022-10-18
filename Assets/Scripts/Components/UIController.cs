using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// Controls UI. Knows about `Player`. Tasks and responsibilities:
///
/// 1. Managing GUI;
/// 2. Controlling play session start
public class UIController : MonoBehaviour {
    // Component parameters

    /// Image with player's current health indication
    public Image healthIndicator;
    /// Image with player's current shot cooldown indication
    public Image chargeIndicator;
    /// Text field for messages to the player. Used for:
    ///
    /// 1. Telling player to press any button to start session;
    /// 2. Telling player about his/her death and his/her score
    public Text messageText;
    public Text scoreText;

    /// Sprites used for health indication
    public Sprite[] healthIndicatorSprites;
    /// Sprites used for shot cooldown indication
    public Sprite[] chargeIndicatorSprites;

    /// Initial message to the player
    [Multiline]
    public string initialMessage;
    /// Message shown to the player in case of death. Must be a string
    /// suitable for formatting (session score is substituted)
    [Multiline]
    public string deathMessage;

    // Events

    /// Invoked when the start is requested
    public event Action onGameStart;

    // Player

    /// Player in the current session
    Player player;

    // API for GameLoopController

    /// Invoked when session is being started and player is being
    /// spawned
    public void OnPlayerSpawn(Player player) {
	this.player = player;

	this.player.onHealthChange += (int newHealth) => {
	    healthIndicator.sprite = healthIndicatorSprites[newHealth];
	};

	healthIndicator.sprite = healthIndicatorSprites[healthIndicatorSprites.Length - 1];
	healthIndicator.enabled = true;
	chargeIndicator.enabled = true;
	
	messageText.text = "";
	scoreText.text = "0";
    }

    /// Invoked when player dies
    public void OnPlayerDeath(int score) {
	messageText.text = String.Format(deathMessage, score);
	healthIndicator.enabled = false;
	chargeIndicator.enabled = false;
	scoreText.text = "";
    }

    /// Invoked when score is updated
    public void OnScoreUpdate(int newScore) {
	scoreText.text = newScore.ToString();
    }

    // Unity component messages

    void Start() {
	messageText.text = initialMessage;
	scoreText.text = "";

	healthIndicator.enabled = false;
	chargeIndicator.enabled = false;
    }

    void Update() {
	if (player != null) {
	    // Play session is running. Updating charge indication
	    float fraction = 1 - player.GetCooldownFraction();
	    int maxSpriteIndex = chargeIndicatorSprites.Length - 1;
	    chargeIndicator.sprite = chargeIndicatorSprites[
		(int) (maxSpriteIndex * fraction)
	    ];
	} else {
	    // Play session is not running. Checking for the start
	    // request
	    if (Input.anyKeyDown) {
		onGameStart();
	    }
	}
    }
}
