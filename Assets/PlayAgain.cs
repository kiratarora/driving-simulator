using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayAgain : MonoBehaviour
{
    // This is the button that will be clicked to play again, either restart same level if num lives is > 0 and state is 'loss'
    // else play next level if num lives is > 0 and state is 'win'
    public Button PlayAgainButton;
    public static int? levelSeed = null;
    private int? numLives = null;

    // Start is called before the first frame update
    void Start()
    {
        if (!levelSeed.HasValue)
        {
            levelSeed = Random.Range(int.MinValue, int.MaxValue);
        }

        // Use the seed for random number generation
        Random.InitState(levelSeed.Value);

        PlayAgainButton.gameObject.SetActive(false);
        PlayAgainButton.onClick.AddListener(PlayGame);

        // Generate the new endpoint for the car
        GenerateNewEndpoint();

        // Set the number of lives
        numLives = 3;

        // Set the state of the player
        // PlayerController.state = "playing";
    }

    private void GenerateNewEndpoint()
    {
        // Example of setting a random seed
        Random.InitState((int)System.DateTime.Now.Ticks);

        // Now generate the new endpoint for the car
    }


    // Update is called once per frame
    void Update()
    {   
        // if (PlayerController.state == "win" || numLives == 0)
        // {
        //     // If the player has won show the play again button - trying new level - new seed and new endpoint
        //     PlayAgainButton.gameObject.SetActive(true);
        //     PlayAgainButton.onClick.AddListener(PlayGame);
        // }
        // else if (PlayerController.state == "loss" && numLives > 0)
        // {
        //     // If the player has lost and num lives is > 0, show the play again button - trying same level again
        //     PlayAgainButton.gameObject.SetActive(true);
        //     PlayAgainButton.onClick.AddListener(TryLevelAgain);
        // }
    }

    void PlayGame()
    {
        levelSeed = null;
        Debug.Log("Loading game with new endpoint and seed");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void TryLevelAgain()
    {
        Debug.Log("Loading game with same endpoint and seed");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
