using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlappyObstaclesSpawner : MonoBehaviour
{
    public static FlappyObstaclesSpawner instance;

    public GameObject obstaclePrefab;
    public Transform playerTransform;

    private bool isSpawning = false;

    [SerializeField]
    [Range(2, 10)]
    public float gapHeight;

    [SerializeField]
    [Range(-2, 3)]
    public float minObstacleHight;

    [SerializeField]
    [Range(4, 5)]
    public float maxObstacleHight;

    [SerializeField]
    [Range(2, 3)]
    public float spawnTime = 3f;

    [SerializeField]
    public Vector2 spawnPoint;
    Transform spawnHolder;      // to keep all spawned objects in one place

  
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(this);
    }

    // max  obstacles instances ready to spawn
    const int spawnObjectsAmount = 3;
    // objects're waiting to spawn.
    private Queue<Transform> toSpawnObjects = new Queue<Transform>();

    private void Start()
    {
        spawnHolder = new GameObject("SpawnHolder").transform;
        CreateObjectsToSpawn();
    }

    public IEnumerator StartSpawningObstacles()
    {
        isSpawning = true;

        while (isSpawning)
        {
            float playerX = playerTransform.position.x;
            float y = Random.Range(minObstacleHight, maxObstacleHight);
            Vector2 currentSpawnPoint = new Vector2(playerX, y) + spawnPoint;
            Spawn(currentSpawnPoint, gapHeight);
            yield return new WaitForSeconds(spawnTime);
        }
    }

    public void StopSpawningObstacles()
    {
        isSpawning = false;
    }

    void Spawn(Vector2 spawnPoint, float gapHeight)
    {
        // if there is no ready object to spawn return
        if (toSpawnObjects.Count == 0) return;

        Transform spawnedTrans = toSpawnObjects.Dequeue();
        spawnedTrans.gameObject.SetActive(true);
        spawnedTrans.transform.position = spawnPoint;
        Transform bottomTransform = spawnedTrans.Find("Bottom");
        Transform topTransform = spawnedTrans.Find("Top");

        float bottomY = -gapHeight / 2;
        float topY = +gapHeight / 2;

        bottomTransform.localPosition = Vector3.up * bottomY;
        topTransform.localPosition = Vector3.up * topY;
    }

    void CreateObjectsToSpawn()
    {
        for (int i = 0; i < spawnObjectsAmount; i++)
        {
            Transform spawnedTrans = Instantiate(obstaclePrefab).transform;
            // Hide gameObject
            spawnedTrans.gameObject.SetActive(false);
            // Parent new object to spawnHolder
            spawnedTrans.SetParent(spawnHolder);
            // set reference to FlappyObstaclesSpawner
            spawnedTrans.GetComponent<ObstacleController>().spawner = this;
            // add to queue new obstacle
            toSpawnObjects.Enqueue(spawnedTrans);
        }
    }

    // ObstacleController'll invoke this method when obstacle is beyond the map
    public void AddObstacleToQueue(Transform obstacle)
    {
        toSpawnObjects.Enqueue(obstacle);
    }
}
