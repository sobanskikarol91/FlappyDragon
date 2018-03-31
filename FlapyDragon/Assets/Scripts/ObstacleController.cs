using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    [HideInInspector]
    public FlappyObstaclesSpawner spawner;
    
    private Transform playerTrans;
    // distance between player end left side screen
    float offsetX = 15f;
    // prevents to get many times point by passing this same column
    bool playerPassedColumn= false;

    private void Start()
    {
        playerTrans = FlappyObstaclesSpawner.instance.playerTransform;
    }

    private void Update()
    {
        if (transform.position.x > playerTrans.position.x - offsetX) return;

        spawner.AddObstacleToQueue(transform);
        // Disable GO
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !playerPassedColumn)
        {
            playerPassedColumn = true;
            GameManager.instance.Score++;
            GetComponent<AudioSource>().Play();
        }
    }

    private void OnEnable()
    {
        playerPassedColumn = false;
    }
}
