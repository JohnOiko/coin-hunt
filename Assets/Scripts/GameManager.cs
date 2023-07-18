using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI congratulationsText;
    public int score;
    public int maxScore = 300;
    public int rewardValue = 10;
    public Image gaveOverPanel;
    public AudioSource cameraAudio;

    // Start is called before the first frame update
    void Start()
    {
        // Make the cursor invisible
        Cursor.visible = false;

        // Set the score to -rewardValue and then add rewardValue to it so that the score is 0 and show it on the UI
        score = -rewardValue;
        AddRewardToScore();
    }

    public void AddRewardToScore() {
        // Add the value of the reward to the score
        score += rewardValue;

        // If the max score has been reached, then end the game with a win for the player
        if (score >= maxScore) {
            GameObject.Find("Player").GetComponent<PlayerController>().GameWon();
            GameOver();
        }

        // Update the score on the UI
        scoreText.text = "Score: " + score + "/" + maxScore;
    }

    public void GameOver() {
        // Stop the music coming from the camera
        cameraAudio.Stop();

        // If the player won (the max scored was reached), then activate the congratulations text
        if (score >= maxScore) {
            congratulationsText.gameObject.SetActive(true);
        }

        // Else if the player lost (the max scored wasn't reached), then activate the game over text
        else {
            gameOverText.gameObject.SetActive(true);
        }

        // In either case, activate the game over panel which holds both the congratulations and game over text,
        // as well as the restart and exit game buttons
        gaveOverPanel.gameObject.SetActive(true);

        // Also make the cursor visible so that the user can click one of the two active buttons
        Cursor.visible = true;
    }

    public void RestartGame() {
        // Reload the scene just like in the slides
        Physics.gravity /= GameObject.Find("Player").GetComponent<PlayerController>().gravityModifier;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame() {
        // Quit the application if the exit game button is pressed
        Application.Quit();
    }

    // Returns true if the object is the final one before the max score is reached,
    // meaning that if its respective reward is collected, the max score will have been reached
    public bool IsFinalObject() {
        return maxScore - score <= rewardValue;
    }
}