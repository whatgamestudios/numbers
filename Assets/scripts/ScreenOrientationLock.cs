using UnityEngine;


/**
 * Lock orientation to portrait only.
 */
public class ScreenOrientationLock : MonoBehaviour {

    void Start() {
        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = true;
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.orientation = ScreenOrientation.AutoRotation;
    }
}
