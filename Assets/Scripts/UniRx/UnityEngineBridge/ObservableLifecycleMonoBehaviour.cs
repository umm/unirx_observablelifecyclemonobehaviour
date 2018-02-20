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

        IObservable<IObservableAwakeMonoBehaviour> OnAwakeAsObservable();

    }

    public interface IObservableStartMonoBehaviour : IObservableLifecycleMonoBehaviour {

        IObservable<IObservableStartMonoBehaviour> OnStartAsObservable();

    }

    public abstract class ObservableLifecycleMonoBehaviour : MonoBehaviour, IObservableAwakeMonoBehaviour, IObservableStartMonoBehaviour {

        private AsyncSubject<IObservableAwakeMonoBehaviour> awaken;

        private AsyncSubject<IObservableAwakeMonoBehaviour> Awaken {
            get {
                if (this.awaken == default(AsyncSubject<IObservableAwakeMonoBehaviour>)) {
                    this.awaken = new AsyncSubject<IObservableAwakeMonoBehaviour>();
                }
                return this.awaken;
            }
            set {
                this.awaken = value;
            }
        }

        private AsyncSubject<IObservableStartMonoBehaviour> started;

        private AsyncSubject<IObservableStartMonoBehaviour> Started {
            get {
                if (this.started == default(AsyncSubject<IObservableStartMonoBehaviour>)) {
                    this.started = new AsyncSubject<IObservableStartMonoBehaviour>();
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

        private readonly List<IObservable<IObservableAwakeMonoBehaviour>> onAwakeObservableList = new List<IObservable<IObservableAwakeMonoBehaviour>>();

        private List<IObservable<IObservableAwakeMonoBehaviour>> OnAwakeObservableList {
            get {
                return this.onAwakeObservableList;
            }
        }

        private readonly List<IObservable<IObservableStartMonoBehaviour>> onStartObservableList = new List<IObservable<IObservableStartMonoBehaviour>>();

        private List<IObservable<IObservableStartMonoBehaviour>> OnStartObservableList {
            get {
                return this.onStartObservableList;
            }
        }

        public IObservable<IObservableAwakeMonoBehaviour> OnAwakeAsObservable() {
            return this.Awaken.AsObservable();
        }

        public IObservable<IObservableStartMonoBehaviour> OnStartAsObservable() {
            return this.Started.AsObservable();
        }

        protected virtual void Awake() {
            // 登録済みの GameObject にアタッチされている全ての IObservableAwakeMonoBehaviour Component から登録
            this.PreAwakeGameObjectList.SelectMany(x => x.GetComponents<IObservableAwakeMonoBehaviour>()).ToList().ForEach(x => this.OnAwakeObservableList.Add(x.OnAwakeAsObservable()));
            // 登録済みの ObservableLifecycleMonoBehaviour Component から登録
            this.PreAwakeComponentList.ForEach(x => this.OnAwakeObservableList.Add(x.OnAwakeAsObservable()));
            // 全ての先読み MonoBehaviour の Awake() 呼び出しが完了したら処理を行う
            this.OnAwakeObservableList
                .WhenAll()
                .Subscribe(
                    (_) => {
                        this.OnAwake();
                        Awaken.OnNext(this);
                        Awaken.OnCompleted();
                    }
                );
        }

        protected virtual void Start() {
           // 登録済みの GameObject にアタッチされている全ての IObservableStartMonoBehaviour Component から登録
            this.PreStartGameObjectList.SelectMany(x => x.GetComponents<IObservableStartMonoBehaviour>()).ToList().ForEach(x => this.OnStartObservableList.Add(x.OnStartAsObservable()));
            // 登録済みの ObservableLifecycleMonoBehaviour Component から登録
            this.PreStartComponentList.ForEach(x => this.OnStartObservableList.Add(x.OnStartAsObservable()));
            // 全ての先読み MonoBehaviour の Start() 呼び出しが完了したら処理を行う
            this.OnStartObservableList
                .WhenAll()
                .Subscribe(
                    (_) => {
                        this.OnStart();
                        Started.OnNext(this);
                        Started.OnCompleted();
                    }
                );
        }

        protected virtual void OnDestroy() {
            Awaken = default(AsyncSubject<IObservableAwakeMonoBehaviour>);
            Started = default(AsyncSubject<IObservableStartMonoBehaviour>);
        }

        protected virtual void OnAwake() {
            // Do nothing.
        }

        protected virtual void OnStart() {
            // Do nothing.
        }

    }

}