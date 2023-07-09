using System;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AnnulusGames.SceneSystem
{
    [Serializable]
    public sealed class SceneReference
    {
#if UNITY_EDITOR
        public SceneAsset sceneAsset;
#endif
        public string assetPath
        {
            get
            {
#if UNITY_EDITOR
                _assetPath = AssetDatabase.GetAssetPath(sceneAsset);
#endif
                return _assetPath;
            }
        }
        [SerializeField] private string _assetPath;

#if UNITY_EDITOR
        public bool isValidSceneAsset
        {
            get
            {
                return SceneUtility.GetBuildIndexByScenePath(AssetDatabase.GetAssetPath(sceneAsset)) != -1;
            }
        }
#endif
    }

}