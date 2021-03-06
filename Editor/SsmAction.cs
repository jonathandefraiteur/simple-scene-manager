using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JonathanDefraiteur.SimpleSceneManager.Editor
{
    public class SsmAction
    {
        #region Play
        
        public static void PlayScene(string scenePath) {
            if (SsmUtility.IsScenePlayedAtStart(scenePath)) {
                EditorApplication.isPlaying = true;
                return;
            }

            if (EditorSceneManager.playModeStartScene == null) {
                EditorPrefs.SetString(SsmUtility.PlayModeSceneToResetKey, "null");
            }
            else {
                EditorPrefs.SetString(SsmUtility.PlayModeSceneToResetKey, EditorSceneManager.playModeStartScene.name);
            }
            SetPlayModeStartScene(scenePath);
            EditorApplication.isPlaying = true;
        }

        [MenuItem("Edit/Play First Scene %#&p", false, 155)]
        public static void PlayFirstScene() {
            // Get the first scene
            EditorBuildSettingsScene scene = null;
            foreach (EditorBuildSettingsScene buildScene in EditorBuildSettings.scenes) {
                if (!buildScene.enabled)
                    continue;
                scene = buildScene;
                break;
            }
            if (scene == null) {
                Debug.LogError("No scene enabled in build settings");
                return;
            }
			
            PlayScene(scene.path);
        }
        
        #endregion

        #region Play Mode Start Scene

        public static void SetPlayModeStartScene(string scenePath)
        {
            SceneAsset myWantedStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            if (myWantedStartScene == null) {
                Debug.Log("Could not find scene " + scenePath);
                return;
            }
            EditorSceneManager.playModeStartScene = myWantedStartScene;
        }

        public static void TogglePlayModeStartScene(string scenePath) {
            if (SsmUtility.IsScenePlayedAtStart(scenePath))
                EditorSceneManager.playModeStartScene = null;
            else
                SetPlayModeStartScene(scenePath);
        }

        #endregion

        #region Scene in build actions

        public static void ToggleSceneEnabling(ref EditorBuildSettingsScene scene) {
            scene.enabled = !scene.enabled;
        }

        public static void AddSceneInBuild(string scenePath, ref EditorBuildSettingsScene[] context)
        {
            var scenes = new List<EditorBuildSettingsScene>(context);
            scenes.Add(new EditorBuildSettingsScene(scenePath, true));
            context = scenes.ToArray();
        }

        public static void RemoveSceneInBuild(string scenePath, ref EditorBuildSettingsScene[] context)
        {
            context = context
                .Where(_s => _s.path != scenePath)
                .ToArray();
        }

        #endregion

        #region Scene common actions

        public static void OpenScene(string scenePath) {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
			
            List<Scene> scenesToSave = new List<Scene>();
            for (int i = 0; i < EditorSceneManager.sceneCount; i++) {
                Scene sceneAt = EditorSceneManager.GetSceneAt(i);
                if (sceneAt.isDirty) {
                    scenesToSave.Add(sceneAt);
                }
            }
            if (EditorSceneManager.SaveModifiedScenesIfUserWantsTo(scenesToSave.ToArray())) {
                EditorSceneManager.OpenScene(scenePath);
            }
        }

        public static void SelectScene(string scenePath)
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(scenePath);
        }

        #endregion
    }
}
