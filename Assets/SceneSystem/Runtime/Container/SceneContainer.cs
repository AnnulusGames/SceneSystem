using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using AnnulusGames.SceneSystem.LoadSceneOperations;

namespace AnnulusGames.SceneSystem
{
    public sealed class SceneContainer
    {
        SceneContainer() { }
        public static SceneContainer Create()
        {
            return new SceneContainer();
        }

        private Dictionary<string, List<(int buildIndex, int order)>> _container = new Dictionary<string, List<(int buildIndex, int order)>>();
        private Stack<string> _stack = new Stack<string>();

        private List<int> _permanentScenes = new List<int>();
        private List<int> _scenes = new List<int>();

        private List<ISceneContainerCallbackReceiver> _callbackReceivers = new List<ISceneContainerCallbackReceiver>();

        private bool _isBuilt = false;
        private bool _isDisposed = false;
        private bool _isLoading = false;

        private static List<LoadSceneOperationBase> operationListCache = new List<LoadSceneOperationBase>();

        private const string PERMANENT_SCENE_ID = "SCENE_CONTAINER_PERMANENT_KEY";
        private const string CURRENT_SCENE_ID = "SCENE_CONTAINER_CURRENT_KEY";

        private List<(int buildIndex, int order)> GetOrAddList(string key)
        {
            if (!_container.TryGetValue(key, out var list))
            {
                list = new List<(int buildIndex, int order)>();
                _container.Add(key, list);
            }
            return list;
        }

        private LoadSceneOperationBase LoadInternal(string key, bool throwIfKeyNotExists)
        {
            bool isPermanent = key == PERMANENT_SCENE_ID;

            if (_container.TryGetValue(key, out var list))
            {
                operationListCache.Clear();

                foreach (var group in list.GroupBy(x => x.order))
                {
                    var indexList = group.Select(x => x.buildIndex);
                    var merge = new List<LoadSceneOperation>();

                    foreach (int i in indexList)
                    {
                        int index = i;

                        if (isPermanent) _permanentScenes.Add(index);
                        else _scenes.Add(index);

                        var sceneName = SceneUtility.GetScenePathByBuildIndex(index);
                        merge.Add(Scenes.Loader.GetLoadSceneOperation(sceneName, LoadSceneMode.Additive));
                    }

                    operationListCache.Add(new MergeLoadSceneOperation(merge.ToArray()));
                }

                return new ConcatLoadSceneOperation(operationListCache.ToArray());
            }

            if (throwIfKeyNotExists) throw new KeyNotFoundException();

            return new CompletedLoadSceneOperation();
        }

        private LoadSceneOperationBase UnloadInternal(string key, bool throwIfKeyNotExists)
        {
            bool isPermanent = key == PERMANENT_SCENE_ID;

            if (_container.TryGetValue(key, out var list))
            {
                operationListCache.Clear();

                foreach (var group in list.GroupBy(x => x.order))
                {
                    var indexList = group.Select(x => x.buildIndex);
                    var merge = new List<LoadSceneOperation>();

                    foreach (int i in indexList)
                    {
                        int index = i;

                        if (isPermanent) _permanentScenes.Remove(index);
                        else _scenes.Remove(index);

                        var sceneName = SceneUtility.GetScenePathByBuildIndex(index);
                        merge.Add(Scenes.Loader.GetUnloadSceneOperation(sceneName));
                    }

                    operationListCache.Add(new MergeLoadSceneOperation(merge.ToArray()));
                }

                return new ConcatLoadSceneOperation(operationListCache.ToArray());
            }

            if (throwIfKeyNotExists) throw new KeyNotFoundException();

            return new CompletedLoadSceneOperation();
        }

        private void CallOnBeforePush(string enter, string exit)
        {
            OnBeforePush?.Invoke(enter, exit);
            foreach (var receiver in _callbackReceivers) receiver.OnBeforePush(enter, exit);
        }

        private void CallOnBeforePop(string enter, string exit)
        {
            OnBeforePop?.Invoke(enter, exit);
            foreach (var receiver in _callbackReceivers) receiver.OnBeforePop(enter, exit);
        }

        private void CallOnAfterPush(string enter, string exit)
        {
            OnAfterPush?.Invoke(enter, exit);
            foreach (var receiver in _callbackReceivers) receiver.OnAfterPush(enter, exit);
        }

        private void CallOnAfterPop(string enter, string exit)
        {
            OnAfterPop?.Invoke(enter, exit);
            foreach (var receiver in _callbackReceivers) receiver.OnAfterPop(enter, exit);
        }

        private void ValidateContainer()
        {
            ValidateDisposed();
            if (_isLoading) ThrowHelper.Throw_Loading_InvalidOperationException();
        }

        private void ValidateDisposed()
        {
            if (_isDisposed) ThrowHelper.Throw_ObjectDisposedException(nameof(SceneContainer));
        }

        private void ValidateIsNotBuilt()
        {
            if (_isBuilt) ThrowHelper.Throw_ContainerAlreadyBuilt_Exception();
        }

        private void ValidateIsBuilt()
        {
            if (!_isBuilt) ThrowHelper.Throw_ContainerNotBuilt_Exception();
        }

        private void ValidateScenePath(string path)
        {
            if (SceneUtility.GetBuildIndexByScenePath(path) == -1) ThrowHelper.Throw_Scene_NotFound_Exception(path);
        }

        public bool IsLoading => _isLoading;
        public int StackCount => _stack.Count;

        public event Action<string, string> OnBeforePush;
        public event Action<string, string> OnAfterPush;
        public event Action<string, string> OnBeforePop;
        public event Action<string, string> OnAfterPop;

        public void Register(string key, int sceneBuildIndex, int order = 0)
        {
            ValidateContainer();
            ValidateIsNotBuilt();

            GetOrAddList(key).Add((sceneBuildIndex, order));
        }

        public void Register(string key, string sceneName, int order = 0)
        {
            ValidateContainer();
            ValidateIsNotBuilt();
            ValidateScenePath(sceneName);

            var sceneBuildIndex = SceneUtility.GetBuildIndexByScenePath(sceneName);
            GetOrAddList(key).Add((sceneBuildIndex, order));
        }

        public void RegisterPermanent(int sceneBuildIndex, int order = 0)
        {
            Register(PERMANENT_SCENE_ID, sceneBuildIndex);
        }

        public void RegisterPermanent(string sceneName, int order = 0)
        {
            Register(PERMANENT_SCENE_ID, sceneName);
        }

        public LoadSceneOperationHandle Build()
        {
            ValidateContainer();
            ValidateIsNotBuilt();

            if (_container.ContainsKey(CURRENT_SCENE_ID))
            {
                _stack.Push(CURRENT_SCENE_ID);
            }

            _isBuilt = true;
            var operation = LoadInternal(PERMANENT_SCENE_ID, false);
            operation.Execute();
            return new LoadSceneOperationHandle(operation);
        }

        public LoadSceneOperationHandle Release()
        {
            ValidateContainer();

            _isDisposed = true;

            operationListCache.Clear();
            foreach (var index in _scenes)
            {
                var sceneName = SceneUtility.GetScenePathByBuildIndex(index);
                operationListCache.Add(Scenes.Loader.GetUnloadSceneOperation(sceneName));
            }
            foreach (var index in _permanentScenes)
            {
                var sceneName = SceneUtility.GetScenePathByBuildIndex(index);
                operationListCache.Add(Scenes.Loader.GetUnloadSceneOperation(sceneName));
            }

            var operation = new MergeLoadSceneOperation(operationListCache.ToArray());
            operation.Execute();
            return new LoadSceneOperationHandle(operation);
        }

        public LoadSceneOperationHandle Push(string key)
        {
            ValidateContainer();
            ValidateIsBuilt();

            _isLoading = true;

            _stack.TryPeek(out var exit);
            CallOnBeforePush(key, exit);

            LoadSceneOperationHandle handle;
            if (exit != null)
            {
                var operation1 = UnloadInternal(exit, false);
                var operation2 = LoadInternal(key, true);

                var operation = new MergeLoadSceneOperation(operation1, operation2);
                operation.Execute();
                handle = new LoadSceneOperationHandle(operation);
            }
            else
            {
                var operation = LoadInternal(key, true);
                operation.Execute();
                handle = new LoadSceneOperationHandle(operation);
            }

            handle.onCompleted += () =>
            {
                _isLoading = false;
                CallOnAfterPush(key, exit);
            };

            _stack.Push(key);

            return handle;
        }

        public LoadSceneOperationHandle Pop()
        {
            ValidateContainer();
            ValidateIsBuilt();

            if (_stack.TryPop(out var key))
            {
                _isLoading = true;
                _stack.TryPeek(out var enter);
                CallOnBeforePop(enter, key);

                LoadSceneOperationHandle handle;
                if (enter != null)
                {
                    var operation1 = UnloadInternal(key, false);
                    var operation2 = LoadInternal(enter, true);

                    var operation = new MergeLoadSceneOperation(operation1, operation2);
                    operation.Execute();
                    handle = new LoadSceneOperationHandle(operation);
                }
                else
                {
                    var operation = UnloadInternal(key, false);
                    operation.Execute();
                    handle = new LoadSceneOperationHandle(operation);
                }

                handle.onCompleted += () =>
                {
                    _isLoading = false;
                    CallOnAfterPop(enter, key);
                };
                return handle;
            }

            ThrowHelper.Throw_StackEmpty_Exception();
            return default;
        }

        public LoadSceneOperationHandle ClearStack()
        {
            ValidateContainer();
            ValidateIsBuilt();

            _isLoading = true;

            var handle = Scenes.UnloadScenesAsync(_scenes.ToArray());
            handle.onCompleted += () =>
            {
                _isLoading = false;
            };

            _scenes.Clear();
            _stack.Clear();

            return handle;
        }

        public void AddCallbackReceiver(ISceneContainerCallbackReceiver receiver)
        {
            ValidateDisposed();
            _callbackReceivers.Add(receiver);
        }

        public void RemoveCallbackReceiver(ISceneContainerCallbackReceiver receiver)
        {
            ValidateDisposed();
            _callbackReceivers.Remove(receiver);
        }
    }
}