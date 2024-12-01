using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DI.Objects;
using Game.Components;
using Game.Pool;
using UnityEngine;
using UnityEngine.AI;
using VContainer;
namespace Game
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Spawner Settings")]
        [SerializeField] private EnemyLifetimeScope enemyPrefab; // Префаб врага
        [SerializeField] private int maxActiveEnemies = 10; // Максимальное количество активных врагов
        [SerializeField] private float spawnInterval = 5f; // Интервал спавна
        [SerializeField] private float despawnDelay = 3f; // Задержка перед возвратом в пул
        [SerializeField] private float spawnRadius = 20f; // Радиус спавна
        [SerializeField] private Transform poolParent; // Родитель объектов пула

        private ObjectPool<EnemyLifetimeScope> _enemyPool; // Пул врагов
        private List<EnemyLifetimeScope> _activeEnemies = new List<EnemyLifetimeScope>(); // Активные враги
        private IObjectResolver _resolver;

        [Inject]
        public void Construct(IObjectResolver resolver) => _resolver = resolver; // Инъекция контейнера

        private void Start()
        {
            InitializePool();
            StartSpawnLoop().Forget(); // Запускаем асинхронный цикл спавна
        }

        private void InitializePool() => _enemyPool = new ObjectPool<EnemyLifetimeScope>(enemyPrefab, maxActiveEnemies, poolParent);

        private async UniTaskVoid StartSpawnLoop()
        {
            while (true)
            {
                if (_activeEnemies.Count < maxActiveEnemies)
                {
                    TrySpawnEnemy();
                }

                await UniTask.Delay((int)(spawnInterval * 1000));
            }
        }

        private void TrySpawnEnemy()
        {
            if (!_enemyPool.TryGetObject(out var enemy)) return;

            if (TryGetRandomNavMeshPoint(out Vector3 spawnPoint))
            {
                SpawnEnemy(enemy, spawnPoint);
            }
            else
            {
                _enemyPool.ReturnObject(enemy);
            }
        }

        private void SpawnEnemy(EnemyLifetimeScope enemy, Vector3 spawnPoint)
        {
            enemy.transform.position = spawnPoint;
            enemy.transform.rotation = Quaternion.identity;
            enemy.gameObject.SetActive(true);

            var health = enemy.GetComponent<HealthComponent>();
            health.ResetDefaultHealth(); // Сброс здоровья врага
            health.OnDeath += () => HandleEnemyDeath(enemy);

            _activeEnemies.Add(enemy);
        }

        private void HandleEnemyDeath(EnemyLifetimeScope enemy)
        {
            Debug.Log($"{enemy.name} умер. Будет возвращён в пул через {despawnDelay} секунд.");

            UniTask.Delay((int)(despawnDelay * 1000)).ContinueWith(() =>
            {
                if (_activeEnemies.Contains(enemy))
                {
                    enemy.GetComponent<HealthComponent>().OnDeath -= () => HandleEnemyDeath(enemy); // Отписка от события
                    enemy.gameObject.SetActive(false);
                    _activeEnemies.Remove(enemy);
                    _enemyPool.ReturnObject(enemy);

                    Debug.Log($"{enemy.name} возвращён в пул.");
                }
            }).Forget();
        }


        private bool TryGetRandomNavMeshPoint(out Vector3 result)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector3 randomPoint = transform.position + Random.insideUnitSphere * spawnRadius;
                randomPoint.y = transform.position.y;

                if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, spawnRadius, NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                }
            }

            result = Vector3.zero;
            return false;
        }
    }
}
