using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace AnnulusGames.SceneSystem.Editor
{
    [CustomPropertyDrawer(typeof(SceneReference))]
    public sealed class SceneReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty sceneAssetProperty = property.FindPropertyRelative("sceneAsset");
            SceneAsset sceneAsset = sceneAssetProperty.objectReferenceValue as SceneAsset;

            EditorGUI.BeginProperty(position, label, property);

            if (sceneAsset != null)
            {
                if (!IsValidSceneAsset(sceneAsset))
                {
                    position.height = EditorGUIUtility.singleLineHeight * 2f;
                    EditorGUI.HelpBox(position, "The scene asset is not registered in the build settings.", MessageType.Error);
                    position.y += position.height;
                }
            }
            position.height = EditorGUIUtility.singleLineHeight;
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                EditorGUI.PropertyField(position, sceneAssetProperty, label);
                if (scope.changed)
                {
                    property.FindPropertyRelative("_assetPath").stringValue = AssetDatabase.GetAssetPath(sceneAssetProperty.objectReferenceValue);
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SceneAsset sceneAsset = property.FindPropertyRelative("sceneAsset").objectReferenceValue as SceneAsset;
            if (sceneAsset != null)
            {
                if (!IsValidSceneAsset(sceneAsset))
                {
                    return EditorGUIUtility.singleLineHeight * 3f;
                }
            }
            return EditorGUIUtility.singleLineHeight;
        }

        private bool IsValidSceneAsset(SceneAsset asset)
        {
            if (asset == null) return false;
            return SceneUtility.GetBuildIndexByScenePath(AssetDatabase.GetAssetPath(asset)) != -1;
        }
    }
}
