// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
// [ExecuteInEditMode]
[RequireComponent(typeof(Camera))]

public class CameraAutoAdjust : MonoBehaviour {

    // Set this to the in-world distance between the left & right edges of your scene.
    public float sceneWidth = 800;

    Camera _camera;

    public float orthographicSize = 5.3f;

    public float aspect = 1.5f;

    void Start() {
//        _camera = GetComponent<Camera>();

        // Camera.main.projectionMatrix = Matrix4x4.Ortho(
        //     -orthographicSize * aspect, 
        //     orthographicSize * aspect, 
        //     -orthographicSize, orthographicSize, 
        //     GetComponent<Camera>().nearClipPlane, 
        //     GetComponent<Camera>().farClipPlane);
    }

    // Adjust the camera's height so the desired scene width fits in view
    // even if the screen/window size changes dynamically.
//     void Update() {
// //        Debug.Log("Screen width: " + Screen.width + ", height: " + Screen.height);

//         float unitsPerPixel = sceneWidth / Screen.width;

//         float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;

//         _camera.orthographicSize = desiredHalfHeight;
//     }
}
