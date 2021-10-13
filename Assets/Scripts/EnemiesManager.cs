using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    public Transform spawner;
    public SuperEnemyController enemyPrefab;
    public Transform container;

    [Range(0.5f, 20)]
    public float delay = 20f;

    private Coroutine _spawnRoutine;

    public void StartSpawning()
    {
        _spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning()
    {
        StopCoroutine(_spawnRoutine);
    }

    IEnumerator SpawnRoutine()
    {
        Spawn();
        yield return new WaitForSeconds(delay);
        StartSpawning();
    }

    private void Spawn()
    {
        SuperEnemyController enemy = Instantiate(enemyPrefab, spawner.position, Quaternion.identity, container);
    }
}
