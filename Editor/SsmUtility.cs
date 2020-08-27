using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace JonathanDefraiteur.SimpleSceneManager.Editor
{
    public class SsmUtility
    {
        public const string PlayModeSceneToResetKey = "playModeSceneToReset";

        public static bool IsScenePlayedAtStart(string scenePath) {
            return EditorSceneManager.playModeStartScene != null
                   && EditorSceneManager.playModeStartScene.name == SceneFileName(scenePath);
        }
        
        public static string SceneFileName(EditorBuildSettingsScene scene) {
            return SceneFileName(scene.path);
        }

        public static string SceneFileName(string path) {
            // Remove extension
            string pathWithoutExtension = path.Split('.')[0];
            string[] pathParts = pathWithoutExtension.Split(new []{'/', '\\'});
            return pathParts[pathParts.Length - 1];
        }

        public static GUIContent SceneGUIContent(EditorBuildSettingsScene scene)
        {
            return SceneGUIContent(scene.path);
        }
        public static GUIContent SceneGUIContent(string path)
        {
            return new GUIContent(SceneFileName(path), path);
        }
    }
}
