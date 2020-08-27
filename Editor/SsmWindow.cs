using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JonathanDefraiteur.SimpleSceneManager.Editor
{
	public class SsmWindow : EditorWindow {
		private const float buttonsWidth = 20f;
		
		private static SsmWindow instance;
		private static GUIStyle sceneButtonStyle;
		private static GUIStyle playModeSceneButtonStyle;
		private static Color playModeSceneButtonColor;
		private static GUIContent playModeSceneButtonContent;
		private static GUIContent playSceneButtonContent;
		private static GUIStyle addSceneButtonStyle;
		private static GUIContent addSceneButtonContent;
		
		public static bool IsOpen => instance != null;

		static SsmWindow() {
			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
		}

		[MenuItem("Window/Simple Scene Manager", false, 10000)]
		public static void Init() {
			// Get existing open window or if none, make a new one:
			instance = GetWindow<SsmWindow>();
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
			Debug.Log($"Active Scene: {activeScene.path}");
			var scenes = EditorBuildSettings.scenes;

			if (scenes.Length > 0) {
				SceneListGUI(ref scenes);
			}
			else {
				SsmGUI.NoSceneInBuildSettings();
			}

			BuildSettingsShortcutGUI();
			
			UnusedScenesGUI(ref scenes);

			EditorBuildSettings.scenes = scenes;
		}

		void BuildSettingsShortcutGUI()
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			SsmGUI.BuildSettingsButton();
			EditorGUILayout.EndHorizontal();
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
				SsmAction.ToggleSceneEnabling(ref scene);
			
			GUILayout.Label(scene.enabled ? index.ToString() : "", EditorStyles.miniButtonMid, GUILayout.Width(buttonsWidth));
			index = scene.enabled ? index + 1 : index;
			
			// Button to open it (Label)
			if (GUILayout.Button(SsmUtility.SceneGUIContent(scene), sceneButtonStyle))
				SsmAction.OpenScene(scene.path);
			
			// Play button
			GUI.color = SsmUtility.IsScenePlayedAtStart(scene.path) ? playModeSceneButtonColor : gc;
			if (GUILayout.Button(playModeSceneButtonContent, playModeSceneButtonStyle, GUILayout.Width(buttonsWidth))) {
				SsmAction.TogglePlayModeStartScene(scene.path);
			}
			GUI.color = gc;
			if (GUILayout.Button(playSceneButtonContent, EditorStyles.miniButtonRight, GUILayout.Width(buttonsWidth))) {
				SsmAction.PlayScene(scene.path);
			}
			
			EditorGUILayout.EndHorizontal();
			GUI.color = gc;
		}

		private static void OnPlayModeStateChanged(PlayModeStateChange state) {
			if (state != PlayModeStateChange.EnteredPlayMode) return;
			if (!EditorPrefs.HasKey(SsmUtility.PlayModeSceneToResetKey)) return;

			string playModeSceneToReset = EditorPrefs.GetString(SsmUtility.PlayModeSceneToResetKey);
			if (playModeSceneToReset == "null") {
				EditorSceneManager.playModeStartScene = null;
			}
			else {
				foreach (EditorBuildSettingsScene settingsScene in EditorBuildSettings.scenes) {
					if (playModeSceneToReset != SsmUtility.SceneFileName(settingsScene)) continue;

					var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(settingsScene.path);
					EditorSceneManager.playModeStartScene = sceneAsset;
					break;
				}
			}
			EditorPrefs.DeleteKey(SsmUtility.PlayModeSceneToResetKey);

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
			if (GUILayout.Button(SsmUtility.SceneGUIContent(scenePath), sceneButtonStyle)) {
				SsmAction.OpenScene(scenePath);
			}

			// Play button
			//GUI.color = IsScenePlayedAtStart(scene) ? playModeSceneButtonColor : gc;
			if (GUILayout.Button("", playModeSceneButtonStyle, GUILayout.Width(buttonsWidth))) {
			//	TogglePlayModeStartScene(scene);
			}
			//GUI.color = gc;
			if (GUILayout.Button(playSceneButtonContent, EditorStyles.miniButtonRight, GUILayout.Width(buttonsWidth))) {
				SsmAction.PlayScene(scenePath);
			}
			
			EditorGUILayout.EndHorizontal();
			GUI.color = gc;
		}

		#endregion
	}
}
