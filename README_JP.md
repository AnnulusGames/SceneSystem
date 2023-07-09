# Scene System
 Provides efficient and versatile scene management functionality for Unity.

<img src="https://github.com/AnnulusGames/SceneSystem/blob/main/Assets/SceneSystem/Documentation~/Header.png" width="800">

[![license](https://img.shields.io/badge/LICENSE-MIT-green.svg)](LICENSE)

[English README](README.md)

## 概要
Scene SystemはUnityにおけるシーンの管理に関する機能を提供するライブラリです。
このライブラリには、シーンに関するAPIやエディタ上でシーンの設定を可能にする機能が含まれます。

### 特徴
* SceneManagerを拡張した多機能なシーン管理用のAPI
* Inspector上でシーンの参照を設定可能なSceneReferenceを追加
* ロード画面を簡単に実装可能なLoadingScreenコンポーネント
* マルチシーンの管理を効率的に行うSceneContainer
* コルーチン・async/awaitに対応
* UniRx/UniTaskに対応

## セットアップ

### 要件
* Unity 2019.4 以上

### インストール
1. Window > Package ManagerからPackage Managerを開く
2. 「+」ボタン > Add package from git URL
3. 以下を入力する
   * https://github.com/AnnulusGames/SceneSystem.git?path=/Assets/SceneSystem


あるいはPackages/manifest.jsonを開き、dependenciesブロックに以下を追記

```json
{
    "dependencies": {
        "com.annulusgames.scene-system": "https://github.com/AnnulusGames/SceneSystem.git?path=/Assets/SceneSystem"
    }
}
```

### Namespace
Scene Systemを利用する場合は、ファイルの冒頭に以下の一行を追加します。

```cs
using AnnulusGames.SceneSystem;
```

## Scenes
Scene Systemでは、UnityのSceneManagerに代わるクラスとしてScenesクラスが提供されています。
ScenesクラスはSceneManagerのラッパークラスとして実装されており、通常のSceneManagerよりも豊富な機能を提供します。

### ロード/アンロード
シーンのロード/アンロードを行いたい場合には以下のように記述します。

```cs
using UnityEngine;
using UnityEngine.SceneManagement;
using AnnulusGames.SceneSystem;

void Example()
{
    // BuildSettingsのIndexでシーンをロード
    Scenes.LoadSceneAsync(0);
    // シーン名でシーンをロード
    Scenes.LoadSceneAsync("SceneName", LoadSceneMode.Additive);
    // 同期ロードも可能
    Scenes.LoadScene(0);

    // BuildSettingsのIndexでシーンをアンロード
    Scenes.UnloadSceneAsync(0);
    // シーン名でシーンをアンロード
    Scenes.UnloadSceneAsync("SceneName");
    // 同期アンロードも可能
    Scenes.UnloadScene(0);
}    
```

また、複数のシーンを同時にロード/アンロードすることも可能です。

```cs
// 複数のシーンを同時にロード (LoadSceneModeはAddictiveのみ)
Scenes.LoadScenesAsync("Scene1", "Scene2", "Scene3");

// 複数のシーンを同時にアンロード
Scenes.UnloadScenesAsync("Scene1", "Scene2");
```

LoadScenesAsyncのみ、LoadMultiSceneModeを設定することで複数シーンの読み込みに関する挙動を設定できます。

```cs
// 複数のシーンを同時にロードする
Scenes.LoadScenesAsync(LoadMultiSceneMode.Parallel, "Scene1", "Scene2", "Scene3");

// 複数のシーンを一つずつロードする
Scenes.LoadScenesAsync(LoadMultiSceneMode.Sequential, "Scene1", "Scene2");
```

### イベント
通常のSceneManagerと同様に、シーンのロードなどのタイミングをイベントで取得することが可能です。

```cs
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

また、ILoadSceneCallbackReceiverを実装したクラスを渡すことで、これらのイベントをまとめて処理することが可能です。

```cs
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
        Debug.Log(scene.name + " loaded");
    }

    void ILoadSceneCallbackReceiver.OnUnload(Scene scene)
    {
        Debug.Log(scene.name + " unloaded");
    }
}
```

## SceneReference
SceneReferenceを利用することで、SceneのアセットをInspector上で編集することが可能になります。

```cs
using UnityEngine;
using AnnulusGames.SceneSystem;

public class SceneReferenceExample : MonoBehaviour
{
    public SceneReference sceneReference;

    void Load()
    {
        // LoadSceneの引数として使用可能
        Scenes.LoadScene(sceneReference);

        // assetPathからシーンアセットのファイルパスを取得可能
        Debug.Log(sceneReference.assetPath);
    }
}
```

<img src="https://github.com/AnnulusGames/SceneSystem/blob/main/Assets/SceneSystem/Documentation~/img1.png" width="420">

## LoadSceneOperationHandle
Scene Systemにおける非同期メソッドは全てLoadSceneOperationHandleという構造体を戻り値に持ちます。
LoadSceneOperationHandleを利用することで、遷移の待機やシーンの有効化などを行うことが可能です。

### 処理の完了を待機する
処理の完了をコールバックで待ちたい場合には、onCompletedを利用します。

```cs
var handle = Scenes.LoadSceneAsync("SceneName");
handle.onCompleted += () =>
{
    Debug.Log("completed");
};
```

コルーチンで待機したい場合には、ToYieldInteractionメソッドを利用します。

```cs
var handle = Scenes.LoadSceneAsync("SceneName");
yield return handle.ToYieldInteraction();
```

async/awaitで待機したい場合には、ToTaskメソッドを利用します。

```cs
var handle = Scenes.LoadSceneAsync("SceneName");
await handle.ToTask();
```

### 進行状況の取得
LoadSceneOperationHandleから進行状況を取得することも可能です。

```cs
var handle = Scenes.LoadSceneAsync("SceneName");

// 進行状況を0〜1のfloatで取得
var progress = handle.Progress;

// 完了しているかどうかを取得
var isDone = handle.IsDone;
```

### シーンの有効化
AllowSceneActivationメソッドを利用することで、シーンの読み込み完了のタイミングを調整することが可能です。
以下は、コルーチン内でAllowSceneActivationを使った例です。

```cs
var handle = Scenes.LoadSceneAsync("SceneName");

// allowSceneActivationをfalseに設定
handle.AllowSceneActivation(false);

// progressが0.9になる(ロードが完了する)まで待機
yield return new WaitWhile(() => handle.Progress < 0.9f);

// allowSceneActivationをtrueに設定
handle.AllowSceneActivation(true);

// シーンが有効化されるまで待機
yield return handle.ToYieldInteraction();
```

AllowSceneActivationをfalseに設定した際のProgressやIsDone値などの挙動に関しては、UnityのallowSceneActivationに準拠します。
https://docs.unity3d.com/2019.4/Documentation/ScriptReference/AsyncOperation-allowSceneActivation.html

## LoadingScreen
Scene Systemでは、ロード画面を表示する機能としてLoadingScreenコンポーネントが用意されています。

<img src="https://github.com/AnnulusGames/SceneSystem/blob/main/Assets/SceneSystem/Documentation~/img2.png" width="500">

LoadingScreenコンポーネントをカスタマイズすることで、独自のロード画面を作成できます。

### 設定項目
#### Skip Mode
ロード完了時の挙動を設定します。

|  SkipMode         |  設定時の挙動                                                  |
| ----------------- | ------------------------------------------------------------ |
|  Instant Complete |  ロード完了後すぐに次のシーンを有効化します。                        |
|  Any Key          |  ロード完了後、任意のキーが押されたタイミングで次のシーンを有効化します。 |
|  Manual           |  ロード完了後、Scriptから手動で次のシーンを有効化します。             |

Manualに設定した場合、AllowCompletion()を呼び出すことで次のシーンを有効化できます。

```cs
LoadingScreen loadingScreen;
loadingScreen.AllowCompletion();
```

#### Minimum Loading Time
ロードにかかる最小限の時間を設定します。
シーンのロードが完了しても、設定された時間の間はロードが行われているように見せかけることが可能です。

#### Destroy On Completed
trueに設定した場合、シーン遷移が完了した後に自動でオブジェクトを削除します。

#### On Loading
シーンのロード中に毎フレーム呼び出されます。

#### On Load Completed
シーンのロードが完了した際に呼び出されます。この時点でシーンは有効化されていません。

#### On Completed
シーンが有効化された際に呼び出されます。

### ロード画面の実装
LoadingScreenで作成したロード画面を使用するには、WithLoadingScreenメソッドを利用します。このメソッドはLoadSceneOperationHandleの拡張メソッドとして定義されており、Scene Systemの任意の非同期メソッドに対して利用することができます。

```cs
using UnityEngine;
using AnnulusGames.SceneSystem;

public sealed class LoadingScreenSample : MonoBehaviour
{
    public LoadingScreen loadingScreenPrefab;

    public void Load()
    {
        // ロード画面のPrefabを生成し、DontDestroyOnLoadに設定
        var loadingScreen = Instantiate(loadingScreenPrefab);
        DontDestroyOnLoad(loadingScreen);

        // WithLoadingScreenで生成したloadingScreenを渡す
        Scenes.LoadSceneAsync("SceneName")
            .WithLoadingScreen(loadingScreen);
    }
}
```

注意として、LoadingScreenを設定したLoadSceneOperationHandleに対してはAllowSceneActivationを呼び出さないでください。LoadingScreen側でallowSceneActivationを操作するため、予期しない動作を引き起こす可能性があります。

### LoadingScreenを拡張する
LoadingScreenを継承して独自のクラスを作成することも可能です。

```cs
using UnityEngine;
using AnnulusGames.SceneSystem;

public class CustomLoadingScreen : LoadingScreen
{
    public override void OnCompleted()
    {
        Debug.Log("completed");
    }

    public override void OnLoadCompleted()
    {
        Debug.Log("load completed");
    }

    public override void OnLoading(float progress)
    {
        Debug.Log("loading...");
    }
}
```

### サンプル
LoadingScreenを用いたロード画面の実装のサンプルが用意されており、Package ManagerのSamplesから導入することが可能です。
実際にロード画面を作成する際に参考にしてみてください。

<img src="https://github.com/AnnulusGames/SceneSystem/blob/main/Assets/SceneSystem/Documentation~/img3.png" width="500">

## SceneContainer
Unityでマルチシーンを利用したプロジェクト構成を採用する場合には何らかの方法で複数シーンの遷移を実装する必要があります。Scene Systemでは、そのような複雑なシーン遷移を行うための機能としてSceneContainerクラスが提供されています。

### コンテナの作成
SceneContainerを利用する際には、まずSceneContainer.Createで新しいコンテナを作成します。

```cs
// 新たなコンテナを作成
var container = SceneContainer.Create();
```

Registerメソッドで実行時にロード/アンロードされるシーンを登録します。

```cs
// 第一引数にシーンに紐づけられたKey、第二引数にシーン名やシーンのbuildIndexを渡す
container.Register("Page1", "Sample1A");
container.Register("Page1", "Sample1B");

container.Register("Page2", "Sample2");
```

RegisterPermanentメソッドで実行時に永続的に存在するシーンを登録します。

```cs
// 引数にシーン名やシーンのbuildIndexを渡す
container.RegisterPermanent("Permanent1");
container.RegisterPermanent("Permanent2");
```

最後にBuildメソッドを呼び出します。このメソッドを呼ぶことでコンテナが使用可能になり、同時にRegisterPermanentで登録されたシーンがロードされます。
この処理は非同期で行われ、通常のシーンのロードと同様の方法で待機が可能です。

```cs
// コンテナを構築する
var handle = container.Build();

// 完了を待機
yield return handle.ToYieldInteraction();
```

### コンテナを利用したシーン遷移
SceneContainerでシーンの遷移を行うにはPushメソッドを利用します。
シーンの履歴はスタックされ、Popメソッドを呼ぶことで前のシーンに戻ることが可能です。

```cs
// 登録されたKeyに紐づけられたシーンに遷移する
var handle = container.Push("Page1");
yield return handle.ToYieldInteraction();

// 前のシーンに戻る
handle = container.Pop();
yield return handle.ToYieldInteraction();
```

ClearStackメソッドを呼ぶことで履歴をリセットし、Pushでロードしたシーンを全てアンロードできます。

```cs
var handle = container.ClearStack();
```

また、Releaseを呼ぶことでコンテナを破棄し、永続シーンを含む全てのシーンをアンロードできます。
```cs
var handle = container.Release();
```

## Scene System + UniRx
UniRxを導入することで、シーンの読み込みに関するイベントをObservable化することが可能になります。

シーンのロード/アンロードやアクティブなシーンの切り替えのイベントをIObservableとして取得したい場合には以下のように記述します。

```cs
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

SceneContainerのイベントをIObservableとして取得することも可能です。

```cs
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
UniTaskを導入することで、LoadSceneOperationHandleの待機をUniTaskで行うことが可能になります。

LoadSceneOperationHandleをUniTaskに変換するには、ToUniTaskを利用します。

```cs
using AnnulusGames.SceneSystem;
using Cysharp.Threading.Tasks;

async UniTaskVoid ExampleAsync()
{
    await Scenes.LoadAsync("SceneName").ToUniTask();
}
```

## ライセンス

[MIT License](LICENSE)