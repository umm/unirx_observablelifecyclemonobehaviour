using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable VirtualMemberNeverOverridden.Global

namespace UniRx {

    public class ObservableLifecycleMonoBehaviourAttribute : Attribute {

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ObservableAwakeMonoBehaviourAttribute : ObservableLifecycleMonoBehaviourAttribute {

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ObservableStartMonoBehaviourAttribute : ObservableLifecycleMonoBehaviourAttribute {

    }

    public interface IObservableLifecycleMonoBehaviour {

    }

    public interface IObservableAwakeMonoBehaviour : IObservableLifecycleMonoBehaviour {

        IObservable<Unit> OnAwakeAsObservable();

    }

    public interface IObservableStartMonoBehaviour : IObservableLifecycleMonoBehaviour {

        IObservable<Unit> OnStartAsObservable();

    }

    public abstract class ObservableLifecycleMonoBehaviour : MonoBehaviour, IObservableAwakeMonoBehaviour, IObservableStartMonoBehaviour {

        private AsyncSubject<Unit> awaken;

        private AsyncSubject<Unit> Awaken {
            get {
                if (this.awaken == default(AsyncSubject<Unit>)) {
                    this.awaken = new AsyncSubject<Unit>();
                }
                return this.awaken;
            }
            set {
                this.awaken = value;
            }
        }

        private AsyncSubject<Unit> started;

        private AsyncSubject<Unit> Started {
            get {
                if (this.started == default(AsyncSubject<Unit>)) {
                    this.started = new AsyncSubject<Unit>();
                }
                return this.started;
            }
            set {
                this.started = value;
            }
        }

        private readonly List<IObservable<Unit>> onAwakeObservableList = new List<IObservable<Unit>>();

        private List<IObservable<Unit>> OnAwakeObservableList {
            get {
                return this.onAwakeObservableList;
            }
        }

        private readonly List<IObservable<Unit>> onStartObservableList = new List<IObservable<Unit>>();

        private List<IObservable<Unit>> OnStartObservableList {
            get {
                return this.onStartObservableList;
            }
        }

        public IObservable<Unit> OnAwakeAsObservable() {
            return this.Awaken.AsObservable();
        }

        public IObservable<Unit> OnStartAsObservable() {
            return this.Started.AsObservable();
        }

        protected virtual void Awake() {
            // 自身の Interface から先読みする MonoBehaviour を決定する
            this.PrepareObservableList<IObservableAwakeMonoBehaviour, ObservableAwakeMonoBehaviourAttribute>(
                x => this.OnAwakeObservableList.Add(x.OnAwakeAsObservable())
            );
            // 全ての先読み MonoBehaviour の Awake() 呼び出しが完了したら処理を行う
            this.OnAwakeObservableList
                .WhenAll()
                .Subscribe(
                    (_) => {
                        this.OnAwake();
                        Awaken.OnNext(Unit.Default);
                        Awaken.OnCompleted();
                    }
                );
        }

        protected virtual void Start() {
            // 自身の Interface から先読みする MonoBehaviour を決定する
            this.PrepareObservableList<IObservableStartMonoBehaviour, ObservableStartMonoBehaviourAttribute>(
                x => this.OnStartObservableList.Add(x.OnStartAsObservable())
            );
            // 全ての先読み MonoBehaviour の Start() 呼び出しが完了したら処理を行う
            this.OnStartObservableList
                .WhenAll()
                .Subscribe(
                    (_) => {
                        this.OnStart();
                        Started.OnNext(Unit.Default);
                        Started.OnCompleted();
                    }
                );
        }

        protected virtual void OnDestroy() {
            Awaken = default(AsyncSubject<Unit>);
            Started = default(AsyncSubject<Unit>);
        }

        protected virtual void OnAwake() {
            // Do nothing.
        }

        protected virtual void OnStart() {
            // Do nothing.
        }

        private void PrepareObservableList<TInterface, TAttribute>(Action<TInterface> callback)
            where TInterface : IObservableLifecycleMonoBehaviour
            where TAttribute : ObservableLifecycleMonoBehaviourAttribute {
            // やや重ための Reflection が走るが初回だけなので妥協する
            // Interface の縛りを入れても良かったが、それはそれで重いので可読性を優先した
            this.GetType()
                // ViewController が実装している全ての Interface を取得
                .GetInterfaces()
                // 各 Interface の Property のうち、[ObservableLifecycleMonoBehaviour] Attribute が付いたプロパティの一覧に変換
                .SelectMany(
                    // Interface の全 Property を取得
                    x => x.GetProperties()
                        // 絞り込み
                        .Where(
                            // [ObservableLifecycleMonoBehaviour] Attribute が付いているかどうかを判定
                            y => Attribute
                                .GetCustomAttributes(y, typeof(TAttribute))
                                .Any()
                        )
                )
                // .NET 3.5 をベースにするので GetValue() は第二引数を明示する
                .Where(x => x.GetValue(this, new object[] {}) is TInterface)
                .ToList()
                // 条件に合致した Property の値を取得し、 OnAwakeAsObservable() なストリームをリストに追加
                .ForEach(x => callback((TInterface)x.GetValue(this, new object[] {})));
        }

    }

}