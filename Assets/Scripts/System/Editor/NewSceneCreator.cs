using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewSceneCreator : EditorWindow
{
    protected string _newSceneName;
    protected string _presetName;
    protected readonly GUIContent _NameContent = new GUIContent("New Scene Name");
    protected readonly GUIContent _presetContent = new GUIContent("Preset Scene");

    [MenuItem("Tools/CreateTemplateScene")]
    static void Init()
    {
        NewSceneCreator window = GetWindow<NewSceneCreator>();
        window.Show();
        window._newSceneName = "NewScene";  //ここが作られるシーンの名前（初期）
        window._presetName = "PresetScene";
    }

    void OnGUI()
    {
        _newSceneName = EditorGUILayout.TextField(_NameContent, _newSceneName);
        _presetName = EditorGUILayout.TextField(_presetContent, _presetName);


        if (GUILayout.Button("Create"))
            CheckAndCreateScene(_presetName);
    }

    protected void CheckAndCreateScene(string presetName)
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogWarning("Cannot create scenes while in play mode.  Exit play mode first.");
            return;
        }

        if (string.IsNullOrEmpty(_newSceneName))
        {
            Debug.LogWarning("Please enter a scene name before creating a scene.");
            return;
        }

        Scene currentActiveScene = SceneManager.GetActiveScene();

        if (currentActiveScene.isDirty)
        {
            string title = currentActiveScene.name + " Has Been Modified";
            string message = "Do you want to save the changes you made to " + currentActiveScene.path + "?\nChanges will be lost if you don't save them.";
            int option = EditorUtility.DisplayDialogComplex(title, message, "Save", "Don't Save", "Cancel");

            if (option == 0)
            {
                EditorSceneManager.SaveScene(currentActiveScene);
            }
            else if (option == 2)
            {
                return;
            }
        }

        CreateScene(presetName);
    }

    protected void CreateScene(string presetName)
    {
        string[] result = AssetDatabase.FindAssets(presetName);

        if (result.Length > 0)
        {
            string newScenePath = "Assets/" + "Scenes/" + "GameScene/" + _newSceneName + ".unity"; //作成するシーンのパス
            AssetDatabase.CopyAsset(AssetDatabase.GUIDToAssetPath(result[0]), newScenePath);
            AssetDatabase.Refresh();
            Scene newScene = EditorSceneManager.OpenScene(newScenePath, OpenSceneMode.Single);
            Close();
        }
        else
        {
            //Debug.LogError("The template scene <b>_TemplateScene</b> couldn't be found ");
            EditorUtility.DisplayDialog("Error",
                "The scene _TemplateScene was not found in Gamekit2D/Scenes folder. This scene is required by the New Scene Creator.",
                "OK");
        }
    }

    protected GameObject InstantiatePrefab(string folderPath, string prefabName)
    {
        GameObject instance = null;
        string[] prefabFolderPath = { folderPath };
        string[] guids = AssetDatabase.FindAssets(prefabName, prefabFolderPath);

        if (guids.Length == 0)
            Debug.LogError("The " + prefabName + " prefab could not be found in " + folderPath + " and could therefore not be instantiated.  Please create one manually.");
        else if (guids.Length > 1)
            Debug.LogError("Multiple " + prefabName + " prefabs were found in " + folderPath + " and one could therefore not be instantiated.  Please create one manually.");
        else
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            instance = Instantiate(prefab);
        }

        return instance;
    }
}