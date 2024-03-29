﻿using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace JonathanDefraiteur.SimpleSceneManager.Editor
{
	public class SsmWindow : EditorWindow {
		
		private static SsmWindow instance;
		private static Vector2 buildScrollPosition = Vector2.zero;
		private static Vector2 otherScrollPosition = Vector2.zero;
		
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
			// Debug.Log($"{Screen.width} / {Screen.height}");
			EnabledSceneGUI();
		}

		#region Enabled Scene

		private void EnabledSceneGUI()
		{
			var activeScene = EditorSceneManager.GetActiveScene();
			var scenes = EditorBuildSettings.scenes;
			
			// Build scenes
			BuildScenesTitleGUI();
			
			if (scenes.Length > 0) {
				buildScrollPosition = EditorGUILayout.BeginScrollView(buildScrollPosition, GetBuildScenesHeightOption(scenes.Length));
				SceneListGUI(ref scenes);
				EditorGUILayout.EndScrollView();
			}
			else {
				SsmGUI.NoSceneInBuildSettings();
			}
			
			// Other scenes
			OtherScenesTitleGUI();

			otherScrollPosition = EditorGUILayout.BeginScrollView(otherScrollPosition, GUILayout.ExpandHeight(true));
			UnusedScenesGUI(ref scenes);
			GUILayout.EndScrollView();
			
			EditorBuildSettings.scenes = scenes;
		}

		private GUILayoutOption GetBuildScenesHeightOption(int _sceneCount, int _lineHeight = 21)
		{
			int height = _sceneCount * _lineHeight;
			return height > Screen.height / 2 ? GUILayout.ExpandHeight(true) : GUILayout.Height(height);
		}

		private void BuildScenesTitleGUI()
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Scenes in build");
			GUILayout.FlexibleSpace();
			SsmGUI.BuildSettingsButton();
			EditorGUILayout.EndHorizontal();
		}

		private void OtherScenesTitleGUI()
		{
			GUILayout.Label("Others in project");
		}

		private void SceneListGUI(ref EditorBuildSettingsScene[] scenes)
		{
			EditorGUILayout.BeginVertical();
			int buildIndex = 0;
			for (int i = 0; i < scenes.Length; i++) {
				SceneGUI(ref scenes, i, ref buildIndex);
			}
			EditorGUILayout.EndVertical();
		}

		private void SceneGUI(ref EditorBuildSettingsScene[] context, int index, ref int buildIndex)
		{
			EditorBuildSettingsScene scene = context[index];
			
			Color gc = GUI.color;
			EditorGUILayout.BeginHorizontal();
			
			// Is active ?
			GUI.color = SsmUtility.IsActive(scene.path) ? Color.yellow : gc;
			// Select Scene
			SsmGUI.SelectButton(scene.path);
			
			GUI.color = gc;
			// Remove Scene
			if (SsmGUI.Button(SsmContent.BtnRemove)) {
				SsmAction.RemoveSceneInBuild(scene.path, ref context);
			}
			
			// Is active ?
            GUI.color = SsmUtility.IsActive(scene.path) ? Color.yellow : gc;
            
            // Enable toggle / Build index
			var content = SsmContent.BtnIndex.Content;
			content.text = scene.enabled ? buildIndex.ToString() : "";
			if (GUILayout.Button(content, SsmContent.BtnIndex.Style, SsmContent.BtnIndex.Width)) {
				SsmAction.ToggleSceneEnabling(ref scene);
			}
			buildIndex = scene.enabled ? buildIndex + 1 : buildIndex;
			
			// Button to open it (Label)
			SsmGUI.LabelButton(scene.path);
			
			GUI.color = gc;
			// Play button
			SsmGUI.PlayModeButton(scene.path);
			SsmGUI.PlayButton(scene.path);
			
			EditorGUILayout.EndHorizontal();
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

		private void UnusedScenesGUI(ref EditorBuildSettingsScene[] usedScenes)
		{
			string[] usedSceneGUIDs = usedScenes.Select(scene => scene.guid.ToString()).ToArray();
			string[] sceneGUIDs = AssetDatabase.FindAssets("t:scene");
			string[] scenePaths = sceneGUIDs
				.Where(guid => !usedSceneGUIDs.Contains(guid))
				.Select(AssetDatabase.GUIDToAssetPath)
				.ToArray();
			
			EditorGUILayout.BeginVertical();
			for (int i = 0; i < scenePaths.Length; i++) {

				if(SsmSettings.settings.pathsToIgnore.Any(x => scenePaths[i].StartsWith(x)))
					continue;
				
				UnusedSceneGUI(scenePaths[i], ref usedScenes);
			}
			EditorGUILayout.EndVertical();
		}
		
		private void UnusedSceneGUI(string scenePath, ref EditorBuildSettingsScene[] context) {
			Color gc = GUI.color;
			EditorGUILayout.BeginHorizontal();
			
			// Is active ?
			GUI.color = SsmUtility.IsActive(scenePath) ? Color.yellow : gc;
			// Select Scene
			SsmGUI.SelectButton(scenePath);
			
			// Add Scene
			GUI.color = gc;
			if (SsmGUI.Button(SsmContent.BtnAdd)) {
				SsmAction.AddSceneInBuild(scenePath, ref context);
			}
			
			// Is active ?
			GUI.color = SsmUtility.IsActive(scenePath) ? Color.yellow : gc;
			// Button to open it (Label)
			SsmGUI.LabelButton(scenePath);
			GUI.color = gc;

			// Play button
			SsmGUI.PlayModeButton(scenePath);
			SsmGUI.PlayButton(scenePath);
			
			EditorGUILayout.EndHorizontal();
		}

		#endregion
	}
}
