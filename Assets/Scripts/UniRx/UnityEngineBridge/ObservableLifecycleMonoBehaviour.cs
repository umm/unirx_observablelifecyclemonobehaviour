using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// ReSharper disable ArrangeAccessorOwnerBody

// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable VirtualMemberNeverOverridden.Global

namespace UniRx {

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

        [SerializeField]
        private List<GameObject> preAwakeGameObjectList = new List<GameObject>();

        private List<GameObject> PreAwakeGameObjectList {
            get {
                return this.preAwakeGameObjectList;
            }
        }

        [SerializeField]
        private List<ObservableLifecycleMonoBehaviour> preAwakeComponentList = new List<ObservableLifecycleMonoBehaviour>();

        private List<ObservableLifecycleMonoBehaviour> PreAwakeComponentList {
            get {
                return this.preAwakeComponentList;
            }
        }

        [SerializeField]
        private List<GameObject> preStartGameObjectList = new List<GameObject>();

        private List<GameObject> PreStartGameObjectList {
            get {
                return this.preStartGameObjectList;
            }
        }

        [SerializeField]
        private List<ObservableLifecycleMonoBehaviour> preStartComponentList = new List<ObservableLifecycleMonoBehaviour>();

        private List<ObservableLifecycleMonoBehaviour> PreStartComponentList {
            get {
                return this.preStartComponentList;
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
            // 登録済みの GameObject にアタッチされている全ての ObservableLifecycleMonoBehaviour Component から登録
            this.PreAwakeGameObjectList.SelectMany(x => x.GetComponents<ObservableLifecycleMonoBehaviour>()).ToList().ForEach(x => this.OnAwakeObservableList.Add(x.OnAwakeAsObservable()));
            // 登録済みの ObservableLifecycleMonoBehaviour Component から登録
            this.PreAwakeComponentList.ForEach(x => this.OnAwakeObservableList.Add(x.OnAwakeAsObservable()));
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
           // 登録済みの GameObject にアタッチされている全ての ObservableLifecycleMonoBehaviour Component から登録
            this.PreStartGameObjectList.SelectMany(x => x.GetComponents<ObservableLifecycleMonoBehaviour>()).ToList().ForEach(x => this.OnStartObservableList.Add(x.OnStartAsObservable()));
            // 登録済みの ObservableLifecycleMonoBehaviour Component から登録
            this.PreStartComponentList.ForEach(x => this.OnStartObservableList.Add(x.OnStartAsObservable()));
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

    }

}