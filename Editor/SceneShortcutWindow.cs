using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JonathanDefraiteur.Editor
{
	public class SceneShortcutWindow : EditorWindow {
		private const string playModeSceneToResetKey = "playModeSceneToReset";
		private const float buttonsWidth = 20f;
		
		private static SceneShortcutWindow instance;
		private static GUIStyle sceneButtonStyle;
		private static GUIStyle playModeSceneButtonStyle;
		private static Color playModeSceneButtonColor;
		private static GUIContent playModeSceneButtonContent;
		private static GUIContent playSceneButtonContent;
		private static GUIStyle addSceneButtonStyle;
		private static GUIContent addSceneButtonContent;
		
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
				sceneButtonStyle = new GUIStyle(EditorStyles.miniButtonMid);
				sceneButtonStyle.alignment = TextAnchor.MiddleLeft;
				
				playModeSceneButtonStyle = new GUIStyle(EditorStyles.miniButtonMid);
				playModeSceneButtonStyle.fontStyle = FontStyle.Bold;
				playModeSceneButtonColor = Color.cyan;
				playModeSceneButtonContent = new GUIContent("»", "Start from this scene when play");
				
				playSceneButtonContent = new GUIContent("►", "Play this scene");
				
				addSceneButtonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
				addSceneButtonStyle.fontStyle = FontStyle.Bold;
				addSceneButtonContent = new GUIContent("+", "Add this scene to the build");
			}
			
			EnabledSceneGUI();
		}

		#region Enabled Scene
		
		void EnabledSceneGUI()
		{
			var activeScene = EditorSceneManager.GetActiveScene();
			Debug.Log(activeScene.path);
			var scenes = EditorBuildSettings.scenes;

			if (scenes.Length > 0) {
				SceneListGUI(ref scenes);
			}
			else {
				NoSceneGUI();
			}

			BuildSettingsShortcutGUI();
			
			UnusedScenesGUI(ref scenes);

			EditorBuildSettings.scenes = scenes;
		}

		void BuildSettingsShortcutGUI()
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Build Settings...")) {
				EditorApplication.ExecuteMenuItem("File/Build Settings...");
			}
			EditorGUILayout.EndHorizontal();
		}

		void NoSceneGUI()
		{
			EditorGUILayout.BeginVertical();
			EditorGUILayout.HelpBox("No scenes in BuildSettings", MessageType.Info);
			EditorGUILayout.EndVertical();
		}

		void SceneListGUI(ref EditorBuildSettingsScene[] scenes)
		{
			EditorGUILayout.BeginVertical();
			int buildIndex = 0;
			for (int i = 0; i < scenes.Length; i++) {
				SceneGUI(ref scenes[i], ref buildIndex);
			}
			EditorGUILayout.EndVertical();
		}

		void SceneGUI(ref EditorBuildSettingsScene scene, ref int index) {
			Color gc = GUI.color;
			EditorGUILayout.BeginHorizontal();
			
			// Enable toggle
			if (GUILayout.Button(scene.enabled ? "✔" : "", EditorStyles.miniButtonLeft, GUILayout.Width(buttonsWidth)))
				ToogleSceneEnabling(ref scene);
			
			GUILayout.Label(scene.enabled ? index.ToString() : "", EditorStyles.miniButtonMid, GUILayout.Width(buttonsWidth));
			index = scene.enabled ? index + 1 : index;
			
			// Button to open it (Label)
			if (GUILayout.Button(SceneGUIContent(scene), sceneButtonStyle))
				OpenScene(scene.path);
			
			// Play button
			GUI.color = IsScenePlayedAtStart(scene.path) ? playModeSceneButtonColor : gc;
			if (GUILayout.Button(playModeSceneButtonContent, playModeSceneButtonStyle, GUILayout.Width(buttonsWidth))) {
				TogglePlayModeStartScene(scene.path);
			}
			GUI.color = gc;
			if (GUILayout.Button(playSceneButtonContent, EditorStyles.miniButtonRight, GUILayout.Width(buttonsWidth))) {
				PlayScene(scene.path);
			}
			
			EditorGUILayout.EndHorizontal();
			GUI.color = gc;
		}

		static void ToogleSceneEnabling(ref EditorBuildSettingsScene scene) {
			scene.enabled = !scene.enabled;
		}

		static void OpenScene(string scenePath) {
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

		static void PlayScene(string scenePath) {
			if (IsScenePlayedAtStart(scenePath)) {
				EditorApplication.isPlaying = true;
				return;
			}

			if (EditorSceneManager.playModeStartScene == null) {
				EditorPrefs.SetString(playModeSceneToResetKey, "null");
			}
			else {
				EditorPrefs.SetString(playModeSceneToResetKey, EditorSceneManager.playModeStartScene.name);
			}
			SetPlayModeStartScene(scenePath);
			EditorApplication.isPlaying = true;
		}

		static bool IsScenePlayedAtStart(string scenePath) {
			return EditorSceneManager.playModeStartScene != null
			       && EditorSceneManager.playModeStartScene.name == SceneFileName(scenePath);
		}

		static void TogglePlayModeStartScene(string scenePath) {
			if (IsScenePlayedAtStart(scenePath)) {
				EditorSceneManager.playModeStartScene = null;
			}
			else {
				SetPlayModeStartScene(scenePath);
			}
		}
		
		static void SetPlayModeStartScene(string scenePath)
		{
			SceneAsset myWantedStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
			if (myWantedStartScene != null) {
				EditorSceneManager.playModeStartScene = myWantedStartScene;
			}
			else {
				Debug.Log("Could not find scene " + scenePath);
			}
		}

		[MenuItem("Edit/Play First Scene %#&p", false, 155)]
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
			
			PlayScene(scene.path);
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
		
		#endregion

		#region Global Scenes

		void UnusedScenesGUI(ref EditorBuildSettingsScene[] usedScenes)
		{
			string[] usedSceneGUIDs = usedScenes.Select(scene => scene.guid.ToString()).ToArray();
			string[] sceneGUIDs = AssetDatabase.FindAssets("t:scene");
			string[] scenePaths = sceneGUIDs
				.Where(guid => !usedSceneGUIDs.Contains(guid))
				.Select(AssetDatabase.GUIDToAssetPath)
				.ToArray();
			
			EditorGUILayout.BeginVertical();
			for (int i = 0; i < scenePaths.Length; i++) {
				UnusedSceneGUI(ref scenePaths[i]);
			}
			EditorGUILayout.EndVertical();
		}
		
		void UnusedSceneGUI(ref string scenePath) {
			Color gc = GUI.color;
			EditorGUILayout.BeginHorizontal();
			
			// Enable toggle

			if (GUILayout.Button(addSceneButtonContent, addSceneButtonStyle, GUILayout.Width(buttonsWidth))) {
			//	ToogleSceneEnabling(ref scene);
			}
			
			
			// Button to open it (Label)
			if (GUILayout.Button(SceneGUIContent(scenePath), sceneButtonStyle)) {
				OpenScene(scenePath);
			}

			// Play button
			//GUI.color = IsScenePlayedAtStart(scene) ? playModeSceneButtonColor : gc;
			if (GUILayout.Button("", playModeSceneButtonStyle, GUILayout.Width(buttonsWidth))) {
			//	TogglePlayModeStartScene(scene);
			}
			//GUI.color = gc;
			if (GUILayout.Button(playSceneButtonContent, EditorStyles.miniButtonRight, GUILayout.Width(buttonsWidth))) {
				PlayScene(scenePath);
			}
			
			EditorGUILayout.EndHorizontal();
			GUI.color = gc;
		}

		#endregion

		#region Utility

		private static string SceneFileName(EditorBuildSettingsScene scene) {
			return SceneFileName(scene.path);
		}

		private static string SceneFileName(string path) {
			// Remove extension
			string pathWithoutExtension = path.Split('.')[0];
			string[] pathParts = pathWithoutExtension.Split(new []{'/', '\\'});
			return pathParts[pathParts.Length - 1];
		}

		static GUIContent SceneGUIContent(EditorBuildSettingsScene scene)
		{
			return SceneGUIContent(scene.path);
		}
		static GUIContent SceneGUIContent(string path)
		{
			return new GUIContent(SceneFileName(path), path);
		}

		#endregion
	}
}
