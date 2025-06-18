using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace MyNamespace
{
    public class UnityMainThreadDispatcher : MonoBehaviour
    {
        private static readonly Queue<Action> executeOnMainThread = new Queue<Action>();
        private static UnityMainThreadDispatcher instance;

        void Awake()
        {
            instance = this;
        }

        public static void Enqueue(Action action)
        {
            lock (executeOnMainThread)
            {
                executeOnMainThread.Enqueue(action);
            }
        }

        public static void EnqueueCoroutine(IEnumerator coroutine)
        {
            Enqueue(() => instance.StartCoroutine(coroutine));
        }

        void Update()
        {
            while (executeOnMainThread.Count > 0)
            {
                executeOnMainThread.Dequeue().Invoke();
            }
        }
    }
}
