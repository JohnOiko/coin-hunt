using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    private BoxCollider playerBc;
    public float jumpForce = 25;
    public float gravityModifier = 15;
    public bool isOnGround = true;
    public bool isCrouching = false;
    public bool gameOver;
    private Animator playerAnim;
    public ParticleSystem explosionParticle;
    public ParticleSystem dirtParticle;
    public AudioClip jumpSound;
    public AudioClip crouchSound;
    public AudioClip crashSound;
    private AudioSource playerAudio;
    private GameManager gameManagerScript;
    private SpawnManager spawnManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerBc = GetComponent<BoxCollider>();
        playerAnim = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
        gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager>();
        spawnManagerScript = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        Physics.gravity *= gravityModifier;
    }

    // Update is called once per frame
    void Update()
    {
        // If up arrow or W is pressed and the player is on the ground, isn't crouching and the game isn't over, then make the player jump
        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && isOnGround && !isCrouching && !gameOver) {
            // Jump just like how it is done in the slides
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isOnGround = false;
            dirtParticle.Stop();

            // Play the jump animation and sound
            playerAnim.SetTrigger("Jump_trig");
            playerAudio.PlayOneShot(jumpSound, 0.4f);
        }

        // If down arrow or S is pressed and the player is on the ground, isn't crouching and the game isn't over, then make the player jump
        else if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && isOnGround && !isCrouching && !gameOver) {
            // Change the player's BoxCollider so that it matches the crouching animation for better collision detection
            playerBc.center = new Vector3(playerBc.center.x, 1.225f, 0.68f);
            playerBc.size = new Vector3(playerBc.size.x, 2.5f, playerBc.size.z);

            // Stop the dirt particles and signify the player is crouching
            dirtParticle.Stop();
            isCrouching = true;

            // Play the crouch animation and sound
            playerAnim.SetBool("Crouch_b", true);
            playerAudio.PlayOneShot(crouchSound, 0.4f);

            // Stop the crouch animation and go back to running after a set amount of time
            Invoke("StopCrouching", 0.4f);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (!gameOver) {
            // If the player collided with the ground start the dirt particles
            if (collision.gameObject.CompareTag("Ground")) {
                dirtParticle.Play();
                isOnGround = true;
            }

            // Else if the player collided with an obstacle play the death animation and stop the game
            else if (collision.gameObject.CompareTag("Obstacle")) {
                // Place the player on the ground, update the game over flag and stop the dirt particles
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
                gameOver = true;
                dirtParticle.Stop();

                // Play the death animation and sound
                playerAnim.SetBool("Death_b", true);
                playerAnim.SetInteger("DeathType_int", 1);
                playerAudio.PlayOneShot(crashSound, 1.0f);

                // Play the explosion particles and stop the game
                explosionParticle.Play();
                gameManagerScript.GameOver();
            }

            // Else if the player collided with a reward destroy the oldest reward immediately (the one the player collided with) and update the score
            else if (collision.gameObject.CompareTag("Reward")) {
                spawnManagerScript.DestroyOldestReward();
                // The score is updated after some time to ensure the player doesn't collide with an obstacle right after colliding with the reward
                // and still have the reward cound towards their score
                Invoke("UpdateScore", 0.35f);
            }
        }
    }

    private void StopCrouching() {
        // Stop the crouching animation
        playerAnim.SetBool("Crouch_b", false);

        // Reset the player's BoxCollider to its non-crouching values
        playerBc.center = new Vector3(playerBc.center.x, 1.425f, 0f);
        playerBc.size = new Vector3(playerBc.size.x, 2.85f, playerBc.size.z);

        // Update the crouching flag
        isCrouching = false;
    }

    public void GameWon() {
        gameOver = true;
        dirtParticle.Stop();

        // Set the player's animation state to idle and play the sitting on ground animation
        playerAnim.SetFloat("Speed_f", 0);
        playerAnim.SetInteger("Animation_int", 9);
    }

    private void UpdateScore() {
        if (!gameOver) {
            gameManagerScript.AddRewardToScore();
        }
    }
}
