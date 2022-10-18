using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Rendering-related things placed as camera component
public class CameraRendering : MonoBehaviour {
    // Component parameters

    /// Material with lighting shader
    public Material material;

    // Unity component messages

    void OnPreRender() {
	// Screen clearing. Reason: viewport of camera is modified
	// (see `CameraScaling` script) for the lighting shader
	// applied more easily. Thus, pixels outside of viewport must
	// be cleared explicitly
	GL.Clear(true, true, Color.black);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest) {
	// Applying lighting shader
	Graphics.Blit(src, dest, material);
    }
}
