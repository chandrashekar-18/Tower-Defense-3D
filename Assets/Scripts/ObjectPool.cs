using UnityEngine;
using System.Collections.Generic;

namespace TowerDefense.Utils
{
    /// <summary>
    /// Generic object pool for reusing game objects
    /// </summary>
    public class ObjectPool<T> where T : Component
    {
        private T _prefab;
        private Transform _container;
        private List<T> _pool;
        private int _initialSize;
        
        public ObjectPool(T prefab, int initialSize, Transform container = null)
        {
            _prefab = prefab;
            _initialSize = initialSize;
            _container = container;
            
            if (_container == null)
            {
                GameObject containerObj = new GameObject($"{typeof(T).Name}Pool");
                _container = containerObj.transform;
            }
            
            _pool = new List<T>();
            
            // Initialize pool
            for (int i = 0; i < _initialSize; i++)
            {
                CreateNewInstance();
            }
        }
        
        public T Get()
        {
            // Find inactive object in pool
            foreach (T obj in _pool)
            {
                if (obj.gameObject.activeInHierarchy == false)
                {
                    obj.gameObject.SetActive(true);
                    return obj;
                }
            }
            
            // No inactive objects found, create a new one
            return CreateNewInstance();
        }
        
        public void Return(T obj)
        {
            obj.gameObject.SetActive(false);
        }
        
        private T CreateNewInstance()
        {
            T instance = Object.Instantiate(_prefab, _container);
            instance.gameObject.SetActive(false);
            _pool.Add(instance);
            
            return instance;
        }
    }
}