﻿using System;
using UnityEngine;

namespace Scripts.Utility
{
    #region Static Instance
    public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance != null) return instance;
                instance = FindObjectOfType<T>();
                return instance != null ? instance : null;
            }
            protected set => instance = value;
        }

        protected virtual void Awake()
        {
            InitializeInstance();
        }

        private void InitializeInstance()
        {
            if (this is T instance)
            {
                Instance = instance;
            }
            else
            {
                throw new InvalidOperationException($"Instance of type {typeof(T)} could not be created.");
            }
        }
    }
    #endregion

    #region Singleton
    public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour
    {
        protected override void Awake()
        {
            base.Awake();
            if (this is T instance)
            {
                if (Instance != null && Instance != this)
                {
                    Debug.LogWarning($"Instance of type {typeof(T)} already exists. Destroying {gameObject.name}.");
                    Destroy(gameObject);
                }
                else
                {
                    Instance = instance;
                }

            }
            else
            {
                Debug.LogError($"Instance of type {typeof(T)} could not be created.");
                throw new InvalidOperationException($"Instance of type {typeof(T)} could not be created.");
            }
        }
    }
    #endregion

    #region DontDestroyOnLoad
    public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
    {
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
    }
    #endregion

    #region Private Singleton
    public abstract class PrivateSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        protected virtual void Awake()
        {
            if (this is T instance)
            {
                if (_instance != null && _instance != this)
                    Destroy(gameObject);
                else
                    _instance = instance;
            }
            else
            {
                Debug.LogError($"Instance of type {typeof(T)} could not be created.");
                throw new InvalidOperationException($"Instance of type {typeof(T)} could not be created.");
            }
        }
    }
    #endregion

    public abstract class PrivatePersistentSingleton<T> : PrivateSingleton<T> where T : MonoBehaviour
    {
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
    }
}
