using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor
{
	public class SceneShortcutWindow : EditorWindow {
	private const string playModeSceneToResetKey = "playModeSceneToReset";
	
	private static SceneShortcutWindow instance;
	private static GUIStyle playModeSceneButtonStyle;
	private static Color playModeSceneButtonColor;
	private static GUIContent playModeSceneButtonContent;
	
	public static bool IsOpen => instance != null;

	static SceneShortcutWindow() {
		EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
	}

	[MenuItem("Window/Simple Scene Manager", false, 10000)]
	public static void Init() {
		// Get existing open window or if none, make a new one:
		instance = GetWindow<SceneShortcutWindow>();
		instance.Show();
		instance.titleContent = new GUIContent("Scenes");
	}

	private void OnEnable() {
		instance = this;
	}

	private void OnGUI() {
		if (playModeSceneButtonStyle == null) {
			playModeSceneButtonStyle = new GUIStyle(EditorStyles.miniButtonMid);
			playModeSceneButtonStyle.fontStyle = FontStyle.Bold;
			playModeSceneButtonColor = Color.cyan;
			playModeSceneButtonContent = new GUIContent("»", "Start from this scene when play");
		}
		
		EnabledSceneGUI();
	}

	void EnabledSceneGUI() {

		var scenes = EditorBuildSettings.scenes;
		
		EditorGUILayout.BeginVertical();
		for (int i = 0; i < scenes.Length; i++) {
			SceneGUI(ref scenes[i]);
		}
		EditorGUILayout.EndVertical();

		EditorBuildSettings.scenes = scenes;
	}

	void SceneGUI(ref EditorBuildSettingsScene scene) {
		Color gc = GUI.color;
		EditorGUILayout.BeginHorizontal();
		
		// Enable toggle
		if (GUILayout.Button(scene.enabled ? "✔" : "", EditorStyles.miniButtonLeft, GUILayout.Width(20f)))
			ToogleSceneEnabling(ref scene);
		
		// Button to open it (Label)
		if (GUILayout.Button(SceneFileName(scene), EditorStyles.miniButtonMid))
			OpenScene(scene);
		
		// Play button
		GUI.color = IsScenePlayedAtStart(scene) ? playModeSceneButtonColor : gc;
		if (GUILayout.Button(playModeSceneButtonContent, playModeSceneButtonStyle, GUILayout.Width(20f))) {
			TogglePlayModeStartScene(scene);
		}
		GUI.color = gc;
		if (GUILayout.Button("►", EditorStyles.miniButtonRight, GUILayout.Width(20f))) {
			PlayScene(scene);
		}
		
		EditorGUILayout.EndHorizontal();
		GUI.color = gc;
	}

	static string SceneFileName(EditorBuildSettingsScene scene) {
		return SceneFileName(scene.path);
	}

	static string SceneFileName(string path) {
		// Remove extension
		string pathWithoutExtension = path.Split('.')[0];
		var pathParts = pathWithoutExtension.Split(new []{'/', '\\'});
		return pathParts[pathParts.Length - 1];
	}

	static void ToogleSceneEnabling(ref EditorBuildSettingsScene scene) {
		scene.enabled = !scene.enabled;
	}

	static void OpenScene(EditorBuildSettingsScene scene) {
		if (EditorApplication.isPlayingOrWillChangePlaymode) return;
		
		List<Scene> scenesToSave = new List<Scene>();
		for (int i = 0; i < EditorSceneManager.sceneCount; i++) {
			Scene sceneAt = EditorSceneManager.GetSceneAt(i);
			if (sceneAt.isDirty) {
				scenesToSave.Add(sceneAt);
			}
		}
		if (EditorSceneManager.SaveModifiedScenesIfUserWantsTo(scenesToSave.ToArray())) {
			EditorSceneManager.OpenScene(scene.path);
		}
	}

	static void PlayScene(EditorBuildSettingsScene scene) {
		if (IsScenePlayedAtStart(scene)) {
			EditorApplication.isPlaying = true;
			return;
		}

		if (EditorSceneManager.playModeStartScene == null) {
			EditorPrefs.SetString(playModeSceneToResetKey, "null");
		}
		else {
			EditorPrefs.SetString(playModeSceneToResetKey, EditorSceneManager.playModeStartScene.name);
		}
		SetPlayModeStartScene(scene);
		EditorApplication.isPlaying = true;
	}

	static bool IsScenePlayedAtStart(EditorBuildSettingsScene scene) {
		return EditorSceneManager.playModeStartScene != null
		       && EditorSceneManager.playModeStartScene.name == SceneFileName(scene);
	}

	static void TogglePlayModeStartScene(EditorBuildSettingsScene scene) {
		if (IsScenePlayedAtStart(scene)) {
			EditorSceneManager.playModeStartScene = null;
		}
		else {
			SetPlayModeStartScene(scene);
		}
	}
	
	static void SetPlayModeStartScene(EditorBuildSettingsScene scene)
	{
		SceneAsset myWantedStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);
		if (myWantedStartScene != null) {
			EditorSceneManager.playModeStartScene = myWantedStartScene;
		}
		else {
			Debug.Log("Could not find scene " + scene.path);
		}
	}

	[MenuItem("Edit/Play First Scene %#&p", false, 149)]
	static void PlayFirstScene() {
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
		
		PlayScene(scene);
	}

	private static void OnPlayModeStateChanged(PlayModeStateChange state) {
		if (state != PlayModeStateChange.EnteredPlayMode) return;
		if (!EditorPrefs.HasKey(playModeSceneToResetKey)) return;

		string playModeSceneToReset = EditorPrefs.GetString(playModeSceneToResetKey);
		if (playModeSceneToReset == "null") {
			EditorSceneManager.playModeStartScene = null;
		}
		else {
			foreach (EditorBuildSettingsScene settingsScene in EditorBuildSettings.scenes) {
				if (playModeSceneToReset != SceneFileName(settingsScene)) continue;

				var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(settingsScene.path);
				EditorSceneManager.playModeStartScene = sceneAsset;
				break;
			}
		}
		EditorPrefs.DeleteKey(playModeSceneToResetKey);

		if (IsOpen) {
			instance.Repaint();
		}
	}
}

}
