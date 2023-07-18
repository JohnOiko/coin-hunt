using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    private float gameSpeed = 27.5f;
    private float rewardRotateSpeed = 400f;
    private PlayerController playerControllerScript;
    private SpawnManager spawnManagerScript;
    private Queue<GameObject> obstacles;

    // Start is called before the first frame update
    void Start()
    {
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        spawnManagerScript = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerControllerScript.gameOver) {
            // Move the game object to the left
            transform.position += Vector3.left * Time.deltaTime * gameSpeed;
            // If the game object is a reward then make it rotate in place (so that the coin rewards rotate)
            if (gameObject.CompareTag("Reward")) {
                transform.Rotate(0, 0, rewardRotateSpeed * Time.deltaTime);
            }
        }

        // If there is at least one existing obstacle and the oldest one is far enough to the left of the player, then destroy it
        obstacles = spawnManagerScript.obstacles;
        if (obstacles.Count != 0 && obstacles.Peek().transform.position.x < -GameObject.Find("Player").transform.position.x) {
            spawnManagerScript.DestroyOldestObstacle();
        }
    }
}
