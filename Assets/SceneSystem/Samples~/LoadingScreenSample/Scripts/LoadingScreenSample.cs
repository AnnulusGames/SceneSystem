using UnityEngine;
using AnnulusGames.SceneSystem;

public sealed class LoadingScreenSample : MonoBehaviour
{
    public LoadingScreen loadingScreenPrefab;
    public SceneReference sceneReference;

    public void Load()
    {
        var loadingScreen = Instantiate(loadingScreenPrefab);
        DontDestroyOnLoad(loadingScreen);

        Scenes.LoadSceneAsync(sceneReference)
            .WithLoadingScreen(loadingScreen);
    }
}
