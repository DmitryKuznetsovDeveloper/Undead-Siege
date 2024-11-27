using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Pool
{
    public sealed class ObjectPool<T> where T : Component
    {
        public Transform Parent => _parent;
        private readonly Queue<T> _objects = new Queue<T>(); // Очередь для хранения объектов
        private readonly T _prefab; // Префаб для создания новых объектов
        private readonly Transform _parent; // Родительский объект для хранения всех объектов пула

        public ObjectPool(T prefab, int initialSize, Transform parent = null)
        {
            _prefab = prefab;
            _parent = parent;

            // Инициализация пула объектами
            for (int i = 0; i < initialSize; i++)
            {
                T newObj = CreateObject();
                _objects.Enqueue(newObj);
            }
        }

        // Метод для получения объекта из пула
        public T GetObject()
        {
            if (_objects.Count > 0)
            {
                T obj = _objects.Dequeue();
                obj.gameObject.SetActive(true);
                return obj;
            }
            else
            {
                // Если в пуле нет объектов, создаем новый
                T newObj = CreateObject();
                return newObj;
            }
        }

        // Метод для возврата объекта обратно в пул
        public void ReturnObject(T obj)
        {
            obj.gameObject.SetActive(false);
            _objects.Enqueue(obj);
        }

        // Метод для создания нового объекта
        private T CreateObject()
        {
            T newObj = GameObject.Instantiate(_prefab, _parent);
            newObj.gameObject.SetActive(false);
            return newObj;
        }
    }
}