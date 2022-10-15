using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Camera scaling component
public class CameraScaling : MonoBehaviour {
    // Component parameters

    /// Sprite which camera makes fit the window
    public SpriteRenderer targetToFit;

    // Components
    
    Camera orthographicCamera;

    void Start() {
        orthographicCamera = GetComponent<Camera>();
    }

    void Update() {
	float targetWidth = targetToFit.bounds.size.x;
	float targetHeight = targetToFit.bounds.size.y;
	float screenWidth = Screen.width;
	float screenHeight = Screen.height;

        float targetAspect = targetWidth / targetHeight;
	float screenAspect = screenWidth / screenHeight;

	if (targetAspect >= screenAspect) {
	    // Target's visible width matches window width
	    orthographicCamera.rect = new Rect(
		0, 0.5f * (1 - screenAspect / targetAspect),
		1, screenAspect / targetAspect
	    );
	} else {
	    // Target's visible height matches window height
	    orthographicCamera.rect = new Rect(
		0.5f * (1 - targetAspect / screenAspect), 0,
		targetAspect / screenAspect, 1
	    );
	}
    }
}
