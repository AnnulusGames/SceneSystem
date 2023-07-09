# Scene System
 Provides efficient and versatile scene management functionality for Unity.

[![license](https://img.shields.io/badge/LICENSE-MIT-green.svg)](LICENSE)

[日本語版READMEはこちら](README_JP.md)

## Overview
Scene System is a library that provides functions related to scene management in Unity.
This library includes an API for loading scenes and a function that enables scene settings on the editor.

### feature
* API for multi-functional scene management that extends SceneManager
* Add SceneReference that can set scene reference on Inspector
* SceneContainer for efficient multi-scene management
* Supports coroutines and async/await
* Support UniRx/UniTask

## Setup

### Requirement
* Unity 2019.4 or higher

### Install
1. Open the Package Manager from Window > Package Manager
2. "+" button > Add package from git URL
3. Enter the following to install
   * https://github.com/AnnulusGames/SceneSystem.git?path=/Assets/SceneSystem


or open Packages/manifest.json and add the following to the dependencies block.

```json
{
    "dependencies": {
        "com.annulusgames.scene-system": "https://github.com/AnnulusGames/SceneSystem.git?path=/Assets/SceneSystem"
    }
}
```

### Namespace
When using Scene System, add the following line at the beginning of the file.

```cs
using AnnulusGames.SceneSystem;
```

## Scenes
The Scene System provides a Scenes class as an alternative to Unity's SceneManager.
The Scenes class is implemented as a wrapper class for SceneManager and provides richer functionality than a normal SceneManager.

To load/unload a scene, write as follows.

``` cs
using UnityEngine;
using UnityEngine.SceneManagement;
using AnnulusGames.SceneSystem;

void Example()
{
    // Load the scene with BuildSettings Index
    Scenes.LoadSceneAsync(0);
    // load scene by scene name
    Scenes.LoadSceneAsync("SceneName", LoadSceneMode.Additive);
    // Synchronous loading is also possible
    Scenes.LoadScene(0);

    // Unload the scene at Index of BuildSettings
    Scenes.UnloadSceneAsync(0);
    // unload scene by scene name
    Scenes.UnloadSceneAsync("SceneName");
    // Synchronous unloading is also possible
    Scenes.UnloadScene(0);
}
```

It is also possible to load/unload multiple scenes simultaneously.

``` cs
// Load multiple scenes simultaneously (LoadSceneMode is Addictive only)
Scenes.LoadScenesAsync("Scene1", "Scene2", "Scene3");

// unload multiple scenes simultaneously
Scenes.UnloadScenesAsync("Scene1", "Scene2");
```

For LoadScenesAsync only, you can set the behavior of loading multiple scenes by setting LoadMultiSceneMode.

``` cs
// load multiple scenes simultaneously
Scenes.LoadScenesAsync(LoadMultiSceneMode.Parallel, "Scene1", "Scene2", "Scene3");

// load multiple scenes one by one
Scenes.LoadScenesAsync(LoadMultiSceneMode.Sequential, "Scene1", "Scene2");
```

### Events
As with a normal SceneManager, it is possible to acquire timings such as scene loading with events.

``` cs
Scenes.onSceneLoaded += (scene, loadSceneMode) =>
{
    Debug.Log(scene.name + " loaded");
};

Scenes.onSceneUnLoaded += scene =>
{
    Debug.Log(scene.name + " unloaded");
};

Scenes.onActiveSceneChanged += (current, next) =>
{
    Debug.Log($"active scene changed from {current.name} to {next.name}");
};
```

Also, by passing a class that implements ILoadSceneCallbackReceiver, it is possible to process these events collectively.

``` cs
using UnityEngine;
using UnityEngine.SceneManagement;
using AnnulusGames.SceneSystem;

public class Example : MonoBehaviour, ILoadSceneCallbackReceiver
{
    void Start()
    {
        Scenes.AddCallbackReceiver(this);
    }

    void ILoadSceneCallbackReceiver.OnActiveSceneChanged(Scene current, Scene next)
    {
        Debug.Log($"active scene changed from {current.name} to {next.name}");
    }

    void ILoadSceneCallbackReceiver.OnLoad(Scene scene, LoadSceneMode loadSceneMode)
    {
        Debug.Log(scene.name + "loaded");
    }

    void ILoadSceneCallbackReceiver.OnUnload(Scene scene)
    {
        Debug.Log(scene.name + "unloaded");
    }
}
```

## SceneReference
By using SceneReference, it becomes possible to edit Scene assets on the Inspector.

``` cs
using UnityEngine;
using AnnulusGames.SceneSystem;

public class SceneReferenceExample : MonoBehaviour
{
    public SceneReference sceneReference;

    void Load()
    {
        // Can be used as an argument for LoadScene
        Scenes.LoadScene(sceneReference);

        // get scene asset file path from assetPath
        Debug.Log(sceneReference.assetPath);
    }
}
```

<img src="https://github.com/AnnulusGames/SceneSystem/blob/main/Assets/SceneSystem/Documentation~/img1.png" width="350">

## LoadSceneOperationHandle
All asynchronous methods in the Scene System have a structure called LoadSceneOperationHandle as a return value.
By using LoadSceneOperationHandle, it is possible to wait for transitions, enable scenes, etc.

### Wait for the process to complete
Use onCompleted to wait for the completion of processing in a callback.

``` cs
var handle = Scenes.LoadSceneAsync("SceneName");
handle.onCompleted += () =>
{
     Debug.Log("completed");
};
```

To wait in a coroutine, use the ToYieldInteraction method.

``` cs
var handle = Scenes.LoadSceneAsync("SceneName");
handle.onCompleted += () =>
{
     Debug.Log("completed");
};
```

To wait with async/await, use the ToTask method.

``` cs
var handle = Scenes.LoadSceneAsync("SceneName");
await handle.ToTask();
```

### Get Progress
It is also possible to get the progress from the LoadSceneOperationHandle.

``` cs
var handle = Scenes.LoadSceneAsync("SceneName");

// get the progress as a float between 0 and 1
var progress = handle.Progress;

// get if completed
var isDone = handle.IsDone;
```

### Activate Scene
By using the AllowSceneActivation method, it is possible to adjust the timing of scene loading completion.
Here is an example of using AllowSceneActivation inside a coroutine.

``` cs
var handle = Scenes.LoadSceneAsync("SceneName");

// set allowSceneActivation to false
handle.AllowSceneActivation(false);

// wait until progress reaches 0.9 (loading is complete)
yield return new WaitWhile(() => handle.Progress < 0.9f);

// set allowSceneActivation to true
handle.AllowSceneActivation(true);

// wait until the scene is activated
yield return handle.ToYieldInteraction();
```

Regarding the behavior of Progress and IsDone values when AllowSceneActivation is set to false, it conforms to Unity's allowSceneActivation.
https://docs.unity3d.com/2019.4/Documentation/ScriptReference/AsyncOperation-allowSceneActivation.html

## SceneContainer
When adopting a project structure that uses multiple scenes in Unity, it is necessary to implement the transition of multiple scenes in some way. Scene System provides the SceneContainer class as a function for performing such complex scene transitions.

### Create a container
When using SceneContainer, first create a new container with SceneContainer.Create().

``` cs
// create a new container
var container = SceneContainer.Create();
```

Register a scene to be loaded/unloaded at runtime with the Register method.

``` cs
// Pass the Key associated with the scene to the first argument, and the scene name and scene buildIndex to the second argument
container.Register("Page1", "Sample1A");
container.Register("Page1", "Sample1B");

container.Register("Page2", "Sample2");
```

Register a scene that exists permanently at runtime with the RegisterPermanent method.

``` cs
// Pass the scene name and scene buildIndex as arguments
container.RegisterPermanent("Permanent1");
container.RegisterPermanent("Permanent2");
```

Finally call the Build method. Calling this method will enable the container and load the scene registered with RegisterPermanent at the same time.
This process is asynchronous and can be waited for in the same way as a normal scene load.

``` cs
// build the container
var handle = container.Build();

// wait for completion
yield return handle.ToYieldInteraction();
```

### Scene transition using containers
Use the Push method to perform scene transitions with SceneContainer.
The history of scenes is stacked, and it is possible to return to the previous scene by calling the Pop method.

``` cs
// Transition to the scene associated with the registered Key
var handle = container. Push("Page1");
yield return handle.ToYieldInteraction();

// return to previous scene
handle = container.Pop();
yield return handle.ToYieldInteraction();
```

By calling the ClearStack method, you can reset the history and unload any scenes you have loaded with push.

``` cs
var handle = container.ClearStack();
```

You can also call Release to destroy the container and unload all scenes, including persistent scenes.
``` cs
var handle = container.Release();
```

## Scene System + UniRx
By introducing UniRx, it becomes possible to observable events related to scene loading.

To get scene loading/unloading events and active scene switching events as IObservable, write as follows.

``` cs
using AnnulusGames.SceneSystem;
using UniRx;

void Example()
{
    Scenes.OnSceneLoadedAsObservable().Subscribe(x =>
    {
        var scene = x.scene;
        var loadSceneMode = x.loadSceneMode;

        Debug.Log("scene loaded");
    });

    Scenes.OnSceneUnloadedAsObservable().Subscribe(scene =>
    {
        Debug.Log("scene unloaded");
    });

    Scenes.OnActiveSceneChangedAsObservable().Subscribe(x =>
    {
        var current = x.current;
        var next = x.next;

        Debug.Log("active scene changed");
    });
}
```

It is also possible to get SceneContainer events as IObservable.

``` cs
SceneContainer container;

void Example()
{
    container.OnBeforePushAsObservable().Subscribe(x =>
    {
        Debug.Log("Current: " + x.current + " Next: " + x.next);
    });

    container.OnAfterPushAsObservable().Subscribe(x =>
    {
        Debug.Log("Current: " + x.current + " Next: " + x.next);
    });

    container.OnBeforePopAsObservable().Subscribe(x =>
    {
        Debug.Log("Current: " + x.current + " Next: " + x.next);
    });

    container.OnAfterPopAsObservable().Subscribe(x =>
    {
        Debug.Log("Current: " + x.current + " Next: " + x.next);
    });
}
```

## Scene System + UniTask
By introducing UniTask, it becomes possible to wait for LoadSceneOperationHandle with UniTask.

Use ToUniTask to convert the LoadSceneOperationHandle to a UniTask.

``` cs
using AnnulusGames.SceneSystem;
using Cysharp.Threading.Tasks;

async UniTaskVoid ExampleAsync()
{
    await Scenes.LoadAsync("SceneName").ToUniTask();
}
```

## Experimental Features
Under AnnulusGames.SceneSystem.Experimental are the classes currently under development.
Although it is possible to use these features, we recommend that you do not use them until they are officially released as they may be subject to breaking changes without notice.

## License

[MIT License](LICENSE)