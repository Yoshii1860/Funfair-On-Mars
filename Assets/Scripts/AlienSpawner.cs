using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AlienSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _alienPrefab;
    [SerializeField] private Transform _pathParent;
    [SerializeField] private int _alienCount;

    private Dictionary<Transform, bool> _alienSpawnPoints = new Dictionary<Transform, bool>();
    private float _yOffset = 10f;

    private void Awake()
    {
        for (int i = 0; i < _pathParent.childCount; i++)
        {
            _alienSpawnPoints.Add(_pathParent.GetChild(i), false);
        }

        Debug.Log($"AlienSpawner initialized with {_alienSpawnPoints.Count} spawn points.");
    }

    private void Start()
    {
        SpawnAliens(_alienCount);
    }

    private void SpawnAliens(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Transform randomSpawnPoint = GetAvailableSpawnPoint();
            Vector3 spawnPosition = randomSpawnPoint.position + Vector3.up * _yOffset;

            RaycastHit hit;
            if (Physics.Raycast(spawnPosition, Vector3.down, out hit, _yOffset * 2))
            {
                spawnPosition = hit.point;
                GameObject alien = Instantiate(_alienPrefab, spawnPosition, Quaternion.identity, transform);
                alien.GetComponent<AlienBehavior>().InitializeAlien(randomSpawnPoint.GetComponent<Waypoint>());
            }
            else
            {
                Debug.LogWarning("No surface found below the spawn point.");
            }
        }
    }

    private Transform GetAvailableSpawnPoint()
    {
        int randomIndex = Random.Range(0, _alienSpawnPoints.Count);

        while (_alienSpawnPoints.Values.ElementAt(randomIndex))
        {
            randomIndex = Random.Range(0, _alienSpawnPoints.Count);
        }

        _alienSpawnPoints[_alienSpawnPoints.Keys.ElementAt(randomIndex)] = true;
        return _alienSpawnPoints.Keys.ElementAt(randomIndex);
    }
}
