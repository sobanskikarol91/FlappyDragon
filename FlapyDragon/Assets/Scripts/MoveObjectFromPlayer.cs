using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectFromPlayer : MonoBehaviour
{
    float offset = 11f;
    Transform player;

    void Start()
    {
        player = GameManager.instance.playerTransform;
    }

    void Update()
    {
        transform.position = new Vector3(player.position.x + offset, transform.position.y,transform.position.z); 
    }
}
