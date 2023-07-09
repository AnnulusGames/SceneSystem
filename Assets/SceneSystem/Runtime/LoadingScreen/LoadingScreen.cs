using System;
using UnityEngine;
using UnityEngine.Events;
#if ENABLE_INPUT_SYSTEM && SCENESYSTEM_SUPPORT_INPUTSYSTEM
using UnityEngine.InputSystem;
#endif

namespace AnnulusGames.SceneSystem
{
    [AddComponentMenu("Scene System/Loading Screen")]
    public class LoadingScreen : MonoBehaviour
    {
        [Header("Settings")]
        public LoadingActionSkipMode skipMode;

        [Range(0f, 10f)] 
        [Tooltip("During the Minimum Loading Time, the loading screen will continue to be displayed even after loading is completed.")]
        public float minimumLoadingTime;
        
        [Tooltip("If true, the object will be automatically destroyed upon completion.")]
        public bool destroyOnCompleted;

        [Header("Events")]
        [Space(5)]
        public UnityEvent<float> onLoading = new UnityEvent<float>();
        [Space(5)]
        public UnityEvent onLoadCompleted = new UnityEvent();
        [Space(5)]
        public UnityEvent onCompleted = new UnityEvent();

        private LoadSceneOperationHandle handle;
        private bool callOnCompleted;
        private bool allowCompletion;
        private float startTime;

        private Action onCompletedInternal;

        internal void Show(LoadSceneOperationHandle handle)
        {
            callOnCompleted = false;
            allowCompletion = false;
            startTime = Time.realtimeSinceStartup;

            this.handle = handle;
            this.handle.AllowSceneActivation(false);
        }

        void Update()
        {
            if (!callOnCompleted)
            {
                float time = Time.realtimeSinceStartup - startTime;
                float progress = 0f;
                float requestProgress = Mathf.InverseLerp(0f, 0.9f, handle.Progress);

                if (minimumLoadingTime <= 0f)
                {
                    progress = requestProgress;
                }
                else
                {
                    progress = Math.Min(
                        requestProgress, // Real
                        Mathf.InverseLerp(0f, minimumLoadingTime, time) // Fake
                    );
                }

                onLoading.Invoke(progress);
                OnLoading(progress);

                if (!callOnCompleted && progress >= 1f)
                {
                    callOnCompleted = true;
                    OnLoadCompleted();
                    onLoadCompleted?.Invoke();

                    if (skipMode == LoadingActionSkipMode.InstantComplete)
                    {
                        AllowCompletion();
                    }
                }
            }

            if (!callOnCompleted) return;

            if (skipMode == LoadingActionSkipMode.AnyKey)
            {
#if ENABLE_LEGACY_INPUT_MANAGER
                if (Input.anyKeyDown) AllowCompletion();
#endif
#if ENABLE_INPUT_SYSTEM && SCENESYSTEM_SUPPORT_INPUTSYSTEM
                if (Keyboard.current != null &&
                    Keyboard.current.anyKey.wasPressedThisFrame)
                {
                    AllowCompletion();
                }
                if (Mouse.current != null &&
                    (Mouse.current.leftButton.wasPressedThisFrame || 
                     Mouse.current.rightButton.wasPressedThisFrame ||
                     Mouse.current.middleButton.wasPressedThisFrame))
                {
                    AllowCompletion();
                }
                if (Gamepad.current != null &&
                    (Gamepad.current.buttonNorth.wasPressedThisFrame ||
                    Gamepad.current.buttonSouth.wasPressedThisFrame ||
                    Gamepad.current.buttonWest.wasPressedThisFrame ||
                    Gamepad.current.buttonEast.wasPressedThisFrame))
                {
                    AllowCompletion();
                }
                if (Touchscreen.current != null &&
                    Touchscreen.current.primaryTouch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    AllowCompletion();
                }
#endif
            }

            if (allowCompletion)
            {
                handle.AllowSceneActivation(true);
                handle.onCompleted += () =>
                {
                    CallOnCompletedEvent();
                };
            }
        }

        private void CallOnCompletedEvent()
        {
            OnCompleted();
            onCompleted?.Invoke();
            onCompletedInternal?.Invoke();

            if (destroyOnCompleted) Destroy(gameObject);
        }

        public virtual void OnLoading(float progress) { }
        public virtual void OnLoadCompleted() { }
        public virtual void OnCompleted() { }
        public void AllowCompletion()
        {
            allowCompletion = true;
        }
    }

    public enum LoadingActionSkipMode
    {
        InstantComplete,
        AnyKey,
        Manual
    }
}