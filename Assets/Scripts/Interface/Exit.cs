using UnityEngine;

public class Exit : MonoBehaviour {
    public void ExitProgram() {
        Application.Quit();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; 
        #endif
    }
}