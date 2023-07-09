using System;

namespace AnnulusGames.SceneSystem.LoadSceneOperations
{
    public abstract class LoadSceneOperationBase
    {
        public abstract LoadSceneOperationHandle Execute();
        public abstract void AllowSceneActivation(bool allowSceneActivation);
        public abstract float Progress { get; }
        public abstract bool IsDone { get; }
        public abstract bool HasExecuted { get; }
        public abstract event Action onCompleted;
    }
}