using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

namespace UnityCoroutineManager
{
    public class ObjectPool<T> where T : new ()
    {
        private readonly Stack<T> m_Stack = new Stack<T>();
 
        #if UNITY_EDITOR
        public int countAll { get; private set; }
        public int countActive { get { return countAll - countInactive; } }
        public int countInactive { get { return m_Stack.Count; } }
        #endif
 
        public T Get()
        {
            T element;
            if (m_Stack.Count == 0)
            {
                element = new T();
                #if UNITY_EDITOR
                countAll++;
            //    Debug.LogFormat( "Pop New {0}, Total {1}", typeof(T).Name, countAll);
                #endif
            }
            else
            {
                element = m_Stack.Pop();
            }
            return element;
        }

        public void Release(T element)
        {
            #if UNITY_EDITOR
            if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
                Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
            #endif
            m_Stack.Push(element);
        }
    }
}