using System;

namespace AnnulusGames.SceneSystem.Experimental
{
    public interface ILoadingScreen
    {
        void Show(LoadSceneOperationHandle handle, Action onCompleted);
    }
}