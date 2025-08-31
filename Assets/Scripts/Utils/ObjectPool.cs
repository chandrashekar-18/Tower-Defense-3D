using UnityEngine;
using System.Collections.Generic;

namespace TowerDefense.Utils
{
    /// <summary>
    /// Generic object pool for reusing game objects.
    /// </summary>
    public class ObjectPool<T> where T : Component
    {
        #region Variables
        private T prefab;
        private Transform container;
        private List<T> pool;
        private int initialSize;
        #endregion

        #region Constructor
        public ObjectPool(T prefab, int initialSize, Transform container = null)
        {
            this.prefab = prefab;
            this.initialSize = initialSize;
            this.container = container;

            if (this.container == null)
            {
                GameObject containerObj = new GameObject($"{typeof(T).Name}Pool");
                this.container = containerObj.transform;
            }

            pool = new List<T>();

            // Initialize pool
            for (int i = 0; i < this.initialSize; i++)
            {
                CreateNewInstance();
            }
        }
        #endregion

        #region Public Methods
        public T Get()
        {
            // Find inactive object in pool
            foreach (T obj in pool)
            {
                if (!obj.gameObject.activeInHierarchy)
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
        #endregion

        #region Private Methods
        private T CreateNewInstance()
        {
            T instance = Object.Instantiate(prefab, container);
            instance.gameObject.SetActive(false);
            pool.Add(instance);

            return instance;
        }
        #endregion
    }
}