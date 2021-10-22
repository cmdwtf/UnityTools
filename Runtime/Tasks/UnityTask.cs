using System;
using System.Collections;
using UnityEngine;

namespace cmdwtf.UnityTools.Tasks
{
    // inspired by: https://github.com/EuleMitKeule/mono-routine
    public class UnityTask
    {
        protected Func<IEnumerator> Enumerator { get; }
        protected MonoBehaviour Behaviour { get; }
        protected Coroutine Coroutine { get; set; }

        private WaitForCompletion _waitForCompletion;

        public WaitForCompletion WaitForCompletion
        {
            get
            {
                if (_waitForCompletion != null)
                {
                    return _waitForCompletion;
                }

                _waitForCompletion = new WaitForCompletion(this);
                Start();

                return _waitForCompletion;
            }
        }

        private bool _isRunning;

        public bool IsRunning
        {
            get => _isRunning;
            private set
            {
                if (value && !_isRunning)
                {
                    OnStarted(Behaviour);
                }

                if (!value)
                {
                    _isPaused = false;
                }

                _isRunning = value;
            }
        }

        private bool _isPaused;

        public bool IsPaused
        {
            get => _isPaused;
            set
            {
                if (value)
                {
                    OnPaused(Behaviour);
                }
                else
                {
                    OnUnpaused(Behaviour);
                }

                _isPaused = value;
            }
        }

        public event EventHandler<UnityTaskEventArgs> Stopped;
        public event EventHandler Paused;
        public event EventHandler Unpaused;
        public event EventHandler Started;

        public UnityTask(Func<IEnumerator> enumerator, MonoBehaviour behaviour = null)
        {
            Enumerator = enumerator;
            Behaviour = behaviour ? behaviour : UnityTaskComponent.Instance;
        }

        public virtual void Start()
        {
            if (IsRunning)
            {
                return;
            }

            IsRunning = true;
            Coroutine = Behaviour.StartCoroutine(Wrapper());
        }

        public virtual void Stop()
        {
            if (!IsRunning)
            {
                return;
            }

            IsRunning = false;
            if (Coroutine != null)
            {
                Behaviour.StopCoroutine(Coroutine);
            }

            OnStopped(Behaviour, isForced: true);
        }

        public virtual void Restart()
        {
            Stop();
            Start();
        }

        public virtual void Pause()
        {
            if (!IsRunning)
            {
                return;
            }

            IsPaused = true;
        }

        public virtual void Unpause()
        {
            if (!IsRunning & !IsPaused)
            {
                return;
            }

            IsPaused = false;
        }

        public virtual void TogglePause()
        {
            if (!IsRunning)
            {
                return;
            }

            IsPaused = !IsPaused;
        }

        private IEnumerator Wrapper()
        {
            IEnumerator enumerator = Enumerator?.Invoke();

            while (IsRunning)
            {
                if (IsPaused)
                {
                    yield return null;
                }
                else
                {
                    if (enumerator != null && enumerator.MoveNext())
                    {
                        object current = enumerator.Current;
                        yield return current;
                        
                        OnEnumeratorStep(current);
                    }
                    else
                    {
                        IsRunning = false;
                        OnStopped(Behaviour, isForced: false);
                        yield break;
                    }
                }
            }
        }

        protected virtual void OnStarted(MonoBehaviour behaviour) => Started?.Invoke(behaviour, EventArgs.Empty);
        protected virtual void OnStopped(MonoBehaviour behaviour, bool isForced) => Stopped?.Invoke(behaviour, new UnityTaskEventArgs(isForced));
        protected virtual void OnPaused(MonoBehaviour behaviour) => Paused?.Invoke(behaviour, EventArgs.Empty);
        protected virtual void OnUnpaused(MonoBehaviour behaviour) => Unpaused?.Invoke(behaviour, EventArgs.Empty);
        protected virtual void OnEnumeratorStep(object value) { }
    }

    public class UnityTask<TReturnType> : UnityTask
    {
        public virtual TReturnType ReturnValue { get; private set; }

        public event EventHandler<UnityTaskEventArgs<TReturnType>> Completed;

        public UnityTask(Func<IEnumerator> enumerator, MonoBehaviour behaviour = null)
            : base(enumerator, behaviour)
        { }

        protected virtual void OnCompleted(MonoBehaviour behaviour, TReturnType value)
        {
            var e = new UnityTaskEventArgs<TReturnType>(isForced: false, value);
            Completed?.Invoke(Behaviour, e);
        }
        
        protected override void OnStopped(MonoBehaviour behaviour, bool isForced)
        {
            base.OnStopped(behaviour, isForced);

            if (isForced)
            {
                return;
            }

            OnCompleted(behaviour, ReturnValue);
        }

        protected override void OnEnumeratorStep(object value)
        {
            if (value is TReturnType currentValue)
            {
                ReturnValue = currentValue;
            }
        }
    }
}
