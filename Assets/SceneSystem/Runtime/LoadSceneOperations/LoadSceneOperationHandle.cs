using System;
using AnnulusGames.SceneSystem.LoadSceneOperations;

namespace AnnulusGames.SceneSystem
{
    public struct LoadSceneOperationHandle
    {
        public LoadSceneOperationHandle(LoadSceneOperationBase operation)
        {
            this.operation = operation;
        }

        internal LoadSceneOperationBase operation { get; private set; }

        public event Action onCompleted
        {
            add
            {
                if (IsValid) operation.onCompleted += value;
            }
            remove
            {
                if (IsValid) operation.onCompleted -= value;
            }
        }
        public bool IsDone => IsValid && operation.IsDone;
        public float Progress => IsValid ? operation.Progress : 0f;
        public bool IsValid => operation != null;

        public void AllowSceneActivation(bool allowSceneActivation)
        {
            operation.AllowSceneActivation(allowSceneActivation);
        }
    }

}