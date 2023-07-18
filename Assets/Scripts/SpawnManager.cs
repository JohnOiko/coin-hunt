using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpawnManager : MonoBehaviour
{
    // These are references to the 9 obstacles (prefabs) so that a random object can be placed each time
    public GameObject obstacleBarrel01;
    public GameObject obstacleBarrel02;
    public GameObject obstacleCrate01;
    public GameObject obstacleBarrier01;
    public GameObject obstacleBarrier02;
    public GameObject obstacleBarrier03;
    public GameObject obstacleSpool01;
    public GameObject obstacleWall01;
    public GameObject obstacleLog02;
    
    // These are references to the BoxCollider of the 9 obstacles (prefabs) so that the random object can be placed at the right y level
    public BoxCollider obstacleBarrel01Bc;
    public BoxCollider obstacleBarrel02Bc;
    public BoxCollider obstacleCrate01Bc;
    public BoxCollider obstacleBarrier01Bc;
    public BoxCollider obstacleBarrier02Bc;
    public BoxCollider obstacleBarrier03Bc;
    public BoxCollider obstacleSpool01Bc;
    public BoxCollider obstacleWall01Bc;
    public BoxCollider obstacleLog02Bc;

    public GameObject obstaclePrefab;
    public BoxCollider obstaclePrefabBc;
    public GameObject rewardPrefab;
    private Vector3 obstacleSpawnPos = new Vector3(25, 0, 0);
    private Vector3 rewardSpawnPosGround = new Vector3(25, 3.5f, 0);
    private Vector3 rewardSpawnPos = new Vector3(25, 1.2625f, 0);
    private PlayerController playerControllerScript;
    private GameManager gameManagerScript;
    public Queue<GameObject> obstacles;
    public Queue<GameObject> rewards;
    private System.Random rnd;
    
    public GameObject obstaclesGameObject;
    public GameObject rewardsGameObject;

    // Start is called before the first frame update
    void Start()
    {
        // This just gets the BoxCollider of the respective obstacle
        obstacleBarrel01Bc = obstacleBarrel01.GetComponent<BoxCollider>();
        obstacleBarrel02Bc = obstacleBarrel02.GetComponent<BoxCollider>();
        obstacleCrate01Bc = obstacleCrate01.GetComponent<BoxCollider>();
        obstacleBarrier01Bc = obstacleBarrier01.GetComponent<BoxCollider>();
        obstacleBarrier02Bc = obstacleBarrier02.GetComponent<BoxCollider>();
        obstacleBarrier03Bc = obstacleBarrier03.GetComponent<BoxCollider>();
        obstacleSpool01Bc = obstacleSpool01.GetComponent<BoxCollider>();
        obstacleWall01Bc = obstacleWall01.GetComponent<BoxCollider>();
        obstacleLog02Bc = obstacleLog02.GetComponent<BoxCollider>();

        // Queues for the placed obstacles and rewards
        obstacles = new Queue<GameObject>();
        rewards = new Queue<GameObject>();
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager>();
        rnd = new System.Random();

        SpawnMultipleObsRew(Math.Ceiling((double)gameManagerScript.maxScore / (double)gameManagerScript.rewardValue));
    }

    // Spawn the specified number of random obstacles and their rewards
    void SpawnMultipleObsRew(double obsCount) {
        for (int i = 2; i <= obsCount + 1; i++) {
            SpawnSingleObsRew(i * 25);
        }
    }

    // Spawn a random obstacle and its reward at the given x position, y is chosen randomly between floating and on the ground and z is static
    void SpawnSingleObsRew(float posX) {
        // Set the obstacle to be on the ground by default
        float obstacleY = 0;
        float rewardY = 3.5f;

        // 30% chance to make the obstacle float in the air
        if (rnd.Next(0, 9) < 3) {
            obstacleY = 2.525f;
            rewardY = 1.25f;
        }

        switch (rnd.Next(0, 8)) {
            case 0:
                obstaclePrefab = obstacleBarrel01;
                obstaclePrefabBc = obstacleBarrel01Bc;
                break;
            case 1:
                obstaclePrefab = obstacleBarrel02;
                obstaclePrefabBc = obstacleBarrel02Bc;
                break;
            case 2:
                obstaclePrefab = obstacleCrate01;
                obstaclePrefabBc = obstacleCrate01Bc;
                break;
            case 3:
                obstaclePrefab = obstacleBarrier01;
                obstaclePrefabBc = obstacleBarrier01Bc;
                break;
            case 4:
                obstaclePrefab = obstacleBarrier02;
                obstaclePrefabBc = obstacleBarrier02Bc;
                break;
            case 5:
                obstaclePrefab = obstacleBarrier03;
                obstaclePrefabBc = obstacleBarrier03Bc;
                break;
            case 6:
                obstaclePrefab = obstacleSpool01;
                obstaclePrefabBc = obstacleSpool01Bc;
                break;
            case 7:
                obstaclePrefab = obstacleWall01;
                obstaclePrefabBc = obstacleWall01Bc;
                break;
            case 8:
                obstaclePrefab = obstacleLog02;
                obstaclePrefabBc = obstacleLog02Bc;
                break;
        }

        // Set the new obstacle's y coordinate so that there is exactly obstacleY (0 for ground objects or 2.525 for air objects) distance between the object's box collider's bottom side
        // and the ground.
        //2.525 is the lowest y coordinate at which the crouched player character doesn't collide with the object
        obstacleY += obstaclePrefabBc.size.y / 2f - obstaclePrefabBc.center.y;

        // Place the new object and its respective reward
        obstacles.Enqueue(Instantiate(obstaclePrefab, new Vector3(posX, obstacleY, obstacleSpawnPos.z), obstaclePrefab.transform.rotation, obstaclesGameObject.transform));
        rewards.Enqueue(Instantiate(rewardPrefab, new Vector3(posX, rewardY, rewardSpawnPos.z), rewardPrefab.transform.rotation, rewardsGameObject.transform));
    }

    public void DestroyOldestObstacle() {
        // If there are obstacles in the obstacle queue, get the oldest one (top of the queue) and destroy it if it isn't null
        if (obstacles.Count > 0) {
            GameObject oldestObstacle = obstacles.Dequeue();
            if (oldestObstacle != null) Destroy(oldestObstacle);
        }
    }

    public void DestroyOldestReward() {
        // If there are rewards in the reward queue, get the oldest one (top of the queue) and destroy it if it isn't null
        if (rewards.Count > 0) {
            GameObject oldestReward = rewards.Dequeue();
            if (oldestReward != null) Destroy(oldestReward);
        }
    }
}
