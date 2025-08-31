#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class SceneSwitcherMenu : EditorWindow
{
    private static SceneSwitcherMenu window;

    [MenuItem("Scene Switcher/Open Window")]
    public static void ShowWindow()
    {
        window = GetWindow<SceneSwitcherMenu>("Scene Switcher");
    }

    private void OnGUI()
    {
        GUILayout.Label("Scenes in Build Settings", EditorStyles.boldLabel);

        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (GUILayout.Button(System.IO.Path.GetFileNameWithoutExtension(scene.path)))
            {
                SwitchScene(scene.path);
            }
        }
    }

    private void SwitchScene(string scenePath)
    {
        if (EditorApplication.isPlaying)
        {
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorSceneManager.OpenScene(scenePath);
        }
    }
}
#endif
