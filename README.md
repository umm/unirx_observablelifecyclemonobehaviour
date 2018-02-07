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

### Inspector から読み込み待ち対象の GameObject, Component を設定

* 下記4つのリストを `[SerializeField]` として公開しています。
  * `Pre Awake GameObject List`
  * `Pre Awake Component List`
  * `Pre Start GameObject List`
  * `Pre Start Component List`
* GameObject を設定した場合、当該 GameObject にアタッチされている全ての `ObservableLifecycleMonoBehaviour` を待ちます。

### 必要に応じて Awake(), Start() で行いたい処理を実装

* 対応するコールバックメソッドに、本来 Awake() や Start() で行いたかった処理を実装します。
  * `Awake()`: `void OnAwake()`
  * `Start()`: `void OnStart()`

## License

Copyright (c) 2018 Tetsuya Mori

Released under the MIT license, see [LICENSE.txt](LICENSE.txt)

