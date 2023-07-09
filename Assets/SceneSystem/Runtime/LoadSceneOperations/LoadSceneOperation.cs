using System;
using UnityEngine;

namespace AnnulusGames.SceneSystem.LoadSceneOperations
{
    public sealed class LoadSceneOperation : LoadSceneOperationBase
    {
        public LoadSceneOperation(Func<AsyncOperation> asyncOperationFunc)
        {
            this.asyncOperationFunc = asyncOperationFunc;
        }

        private Func<AsyncOperation> asyncOperationFunc;
        private AsyncOperation asyncOperation;

        private bool allowSceneActivation = true;
        private bool hasExecuted;

        public override LoadSceneOperationHandle Execute()
        {
            if (hasExecuted) throw new InvalidOperationException();
            hasExecuted = true;

            asyncOperation = asyncOperationFunc?.Invoke();
            asyncOperation.allowSceneActivation = allowSceneActivation;
            asyncOperation.completed += x => onCompleted?.Invoke();
            return new LoadSceneOperationHandle(this);
        }

        public override void AllowSceneActivation(bool allowSceneActivation)
        {
            this.allowSceneActivation = allowSceneActivation;
            if (asyncOperation != null) asyncOperation.allowSceneActivation = allowSceneActivation;
        }

        public override bool IsDone => asyncOperation != null && asyncOperation.isDone;
        public override float Progress => asyncOperation == null ? 0f : asyncOperation.progress;
        public override bool HasExecuted => hasExecuted;
        public override event Action onCompleted;
    }

}