# unirx_observablelifecyclemonobehaviour

* Awake(), Start() の実行完了を待つためのストリームを提供します。

## Requirement

* Unity 2017.1
* [@umm/unirx](https://github.com/umm-projects/unirx)

## Install

```shell
npm install github:umm-projects/unirx_observablelifecyclemonobehaviour
```

## Usage

### `ObservableLifecycleMonoBehaviour` を継承

* 実行順待ちを行うためのクラスを継承します。
* 待つ側も待たれる側も `ObservableLifecycleMonoBehaviour` を継承します。

### 属性をプロパティに付与

* 待ちたいメソッドに対応する属性を**プロパティ**に付与します。
  * `Awake()`: `[ObservableAwakeMonoBehaviour]`
  * `Start()`: `[ObservableStartMonoBehaviour]`

### 必要に応じて Awake(), Start() で行いたい処理を実装

* 対応するコールバックメソッドに、本来 Awake() や Start() で行いたかった処理を実装します。
  * `Awake()`: `void OnAwake()`
  * `Start()`: `void OnStart()`

### サンプルコード

```csharp
public class Sample : ObservableLifecycleMonoBehaviour {

    [SerializeField]
    private FirstAwake firstAwake;

    [ObservableAwakeMonoBehaviour]
    public FirstAwake FirstAwake {
        get {
            return this.firstAwake;
        }
    }

    [SerializeField]
    private FirstStart firstStart;

    [ObservableStartMonoBehaviour]
    public FirstStart FirstStart {
        get {
            return this.firstStart;
        }
    }

    public void OnAwake() {
        // Awake() で行うべき処理
    }

    public void OnStart() {
        // Start() で行うべき処理
    }

}
```

## License

Copyright (c) 2018 Tetsuya Mori

Released under the MIT license, see [LICENSE.txt](LICENSE.txt)

