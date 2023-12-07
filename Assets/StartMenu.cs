using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    // This is the button that will be clicked to start the game
    public Button StartButton;

    void Start()
    {
        StartButton.onClick.AddListener(StartGame);
    }

    // Start is called before the first frame update
    public void StartGame()
    {
        // Load the game scene
        SceneManager.LoadScene("SimplePoly City - Low Poly Assets_Demo Scene");
    }
}