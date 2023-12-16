/*
    Controller: forward (W), backward (S), turn right (A), turn left (D)
    Acceleration : 
    Deceleration :
    Variables:
        Speed/Velocity
        Character controller
        caught
        has_won
        num_lives
    ------------------------------------------------------------------------
    RULES:
        Stays within bounds of roads
        Will remain stopped at the end of a road unless user presses turn
        Has to stop if caught by police
        Stop when pedestrians are crossing the road
        Stop when traffic light is red
        Do not speed
        Do not crash into any other objects
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using TMPro;

public class CarController : MonoBehaviour
{
    // private Animator animation_controller;
    private CharacterController car; // The car controller
    public Button PlayAgainButton; // This is the button that will be clicked to play again the same level
    public Button TryAgainButton; // This is the button that will be clicked to play again the same level
    public float acceleration = 5.0f;
    private const float SpeedLimit = 40.0f / 2.237f; // 40 mph converted to Unity units, if necessary
    public float turnSpeed = 60.0f;
    public float driftTurnSpeedMultiplier = 1.5f; // Multiplier for turn speed during drift
    public float topSpeed = 60.0f; // Maximum speed of the car in mph
    public float currentVelocity = 0.0f; // Current speed of the car
    private float currentTurn = 0.0f; // Current turn speed
    private float originalYPosition; // The original y position of the car
    public static int? levelSeed = null; // Seed for random number generation
    public int num_lives; // Number of lives the player has
    public bool has_won; // True if the player has won the game
    public TMP_Text text;  // Text to display the number of lives
    public TMP_Text speedDisplayText; // Text to display the speed of the car
    private bool currentHasLostLifeForSpeeding = false;
    private AudioSource source;
    public AudioClip brake;
    public AudioClip horn;
    public AudioClip police;
    public AudioClip driving;
    public AudioClip gameOverSound;
    public AudioClip winSound;
    public bool pl_played = false;
    public bool pl_chasing = false;
    public GameObject finishZonePrefab;
    private GameObject currentFinishZone;
    public GameObject policeCar;
    public TMP_Text lawBroken; // Text to display law is broken

    void Start()
    {
        if (!levelSeed.HasValue)
        {
            levelSeed = Random.Range(int.MinValue, int.MaxValue);
        }
        // Use the seed for random number generation
        Random.InitState(levelSeed.Value);

        // animation_controller = GetComponent<Animator>();
        car = GetComponent<CharacterController>();
        originalYPosition = transform.position.y;
        num_lives = 5;
        has_won = false;
        PlayAgainButton.gameObject.SetActive(false);
        TryAgainButton.gameObject.SetActive(false);
        lawBroken.gameObject.SetActive(false);
        source = GetComponent<AudioSource>();

        // Randomly select and place a finish zone
        Vector3[] possibleFinishZones = new Vector3[]
        {
            new Vector3(120, 0, 160),
            new Vector3(240, 0, 60),
            new Vector3(0, 0, 0)
        };

        int randomIndex = Random.Range(0, possibleFinishZones.Length);
        Vector3 finishZonePosition = possibleFinishZones[randomIndex];
        CreateFinishZone(finishZonePosition);
        Debug.Log("Finish zone position: " + finishZonePosition);
    }

    void Update()
    {
        if (currentVelocity >= SpeedLimit/2.0f && !pl_chasing)
        {
            source.PlayOneShot(driving);           
            Debug.Log("Driving"); 
        }
        GameLogic();
        HandleMovement();
        HandleTurning();
        ApplyMovement();
        MaintainCarHeight();
        UpdateSpeedDisplay();

        if (currentVelocity > SpeedLimit && !currentHasLostLifeForSpeeding)
        {
            LoseLife();
            currentHasLostLifeForSpeeding = true; // Set the flag to true
            lawBroken.text = "You are speeding, you have lost a life!";
            lawBroken.color = Color.blue;
            lawBroken.gameObject.SetActive(true);
            // Trigger the cop car chase
            TriggerChase();
        }
        else if (currentVelocity <= SpeedLimit)
        {
            currentHasLostLifeForSpeeding = false;
        }
        // Check if car is within a certain radius of the finish zone
        float finishZoneRadius = 5f; // Radius to check
        if (Vector3.Distance(transform.position, currentFinishZone.transform.position) <= finishZoneRadius)
        {
            FinishZoneReached();
            has_won = true;
            GameLogic();
        }
    }

    private void FinishZoneReached()
    {
        // Change finish zone color to green
        Renderer finishZoneRenderer = currentFinishZone.GetComponent<Renderer>();
        if (finishZoneRenderer != null)
        {
            finishZoneRenderer.material.color = Color.green;
        }

        // Handle logic when the finish zone is reached (e.g., winning the game)
        Debug.Log("Finish zone reached!");
    }

    private void CreateFinishZone(Vector3 position)
    {
        currentFinishZone = Instantiate(finishZonePrefab, position, Quaternion.identity);

        // Add a red light above the finish zone
        GameObject lightGameObject = new GameObject("FinishZoneLight");
        Light lightComp = lightGameObject.AddComponent<Light>();
        lightComp.color = Color.red;
        lightComp.type = LightType.Point;
        lightComp.range = 10f;
        lightComp.intensity = 15.0f;

        // Position the light above the finish zone
        lightGameObject.transform.position = position + new Vector3(0, 5, 0);
        lightGameObject.transform.parent = currentFinishZone.transform; // Optional: Make the light a child of the finish zone
    }

    private void GenerateNewEndpoint()
    {
        // Example of setting a random seed
        Random.InitState((int)System.DateTime.Now.Ticks);

        // Now generate the new endpoint for the car
    }

    void OnCollisionEnter(Collision collision)
    {
        // Call LoseLife when the car collides with another object
        // Debug.Log("Collided with "+collision.gameObject.name);
        if (collision.gameObject.name.Contains("Female") || collision.gameObject.name.Contains("Male") || collision.gameObject.name.Contains("boy"))
        {
            lawBroken.text = "You have hit a pedestrian, POLICE IS CHASING!";
            lawBroken.color = Color.red;
            lawBroken.gameObject.SetActive(true);
            policeCar.GetComponent<copCar>().lawsBroken = true;
            Debug.Log("Hit a pedestrian, police is chasing you!");
            source.PlayOneShot(horn);
        }
        LoseLife();
    }

    void LoseLife()
    {
        // Reduce the number of lives
        num_lives--;
        // Check if the number of lives is less than or equal to 0
        if (num_lives <= 0)
        {
            // Game over scenario
            GameLogic();
        }
    }


    private void GameLogic()
    {
        text.text = "Lives left: " + num_lives;

        ////////////////////////////////////////////////

        if (has_won) {
            text.text = "You won!";
            text.color = Color.green;
            currentVelocity = 0.0f;
            PlayAgainButton.gameObject.SetActive(true);
            PlayAgainButton.onClick.RemoveAllListeners();
            PlayAgainButton.onClick.AddListener(PlayGame);
            source.PlayOneShot(winSound);
            return;
        }

        if (num_lives <= 0) {
            text.text = "You have lost the game!";
            text.color = Color.red;
            currentVelocity = 0.0f;
            TryAgainButton.gameObject.SetActive(true);
            TryAgainButton.onClick.RemoveAllListeners();
            TryAgainButton.onClick.AddListener(TryLevelAgain);
            source.PlayOneShot(gameOverSound);
            return;
        }
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

    void HandleMovement()
    {
        // Define the lower speed limit
        float lowerTopSpeed = 40.0f / 2.237f; // Convert 40 mph to Unity units (if necessary)
        float effectiveTopSpeed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? topSpeed / 2.237f : lowerTopSpeed;

        // Basic forward and backward movement
        if (Input.GetKey(KeyCode.W))
        {
            currentVelocity += Time.deltaTime * acceleration;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            // Apply the same logic for reverse speed if needed
            currentVelocity -= Time.deltaTime * acceleration;
            //Changed 
            // currentVelocity = -(Mathf.Abs(currentVelocity) + Time.deltaTime * acceleration);
        }

        // Braking with space bar
        if (Input.GetKey(KeyCode.Space))
        {
            source.PlayOneShot(brake);
            // Apply a stronger negative acceleration (braking force)
            if (currentVelocity > 0 && currentVelocity < effectiveTopSpeed)
            {
                //Car moving in forward direction
                currentVelocity -= Time.deltaTime * acceleration * 5;
                currentVelocity = Mathf.Max(currentVelocity, 0); // Prevent the car from reversing  
            }
            if (currentVelocity < 0 && currentVelocity > -effectiveTopSpeed)
            {
                //Car is reversing
                currentVelocity = 0; //Just stop - brake;
            }
        }

        // Gradual deceleration when not accelerating or braking
        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift) && !Input.GetKey(KeyCode.Space))
        {
            currentVelocity = Mathf.Lerp(currentVelocity, 0, Time.deltaTime);
        }

        // Limiting the speed based on whether Shift is pressed
        currentVelocity = Mathf.Clamp(currentVelocity, -effectiveTopSpeed, effectiveTopSpeed);
    }

    void HandleTurning()
    {
        float speedMph = currentVelocity * 2.237f;
        float turnMultiplier = (speedMph >= 45) ? driftTurnSpeedMultiplier : 1.0f;

        if (Input.GetKey(KeyCode.D))
        {
            currentTurn = turnSpeed * turnMultiplier;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            currentTurn = -turnSpeed * turnMultiplier;
        }
        else
        {
            currentTurn = 0;
        }
    }

    void ApplyMovement()
    {
        transform.Rotate(0, currentTurn * Time.deltaTime, 0);
        car.Move(transform.forward * currentVelocity * Time.deltaTime);
    }

    void MaintainCarHeight()
    {
        if (transform.position.y != originalYPosition)
        {
            transform.position = new Vector3(transform.position.x, originalYPosition, transform.position.z);
        }
    }

    void UpdateSpeedDisplay()
    {
        // Convert the speed from Unity units to mph
        int speedMph = Mathf.RoundToInt(currentVelocity * 2.237f);
        // Update the text element with the current speed
        speedDisplayText.text = "Speed:\n" + speedMph + " mph";
    }


    public List<RoadNode> BuildRoadGraph(float connectionThreshold)
    {
        List<RoadNode> roadNodes = new List<RoadNode>();
        GameObject[] roadObjects = GameObject.FindGameObjectsWithTag("Road");
        // Create nodes for each road object
        foreach (var roadObject in roadObjects)
        {
            if (roadObject.name.Contains("Lane") || roadObject.name.Contains("Intersection"))
            {
                RoadNode node = new RoadNode(roadObject.transform.position);
                roadNodes.Add(node);
            }
        }
        // Connect nodes based on proximity
        foreach (var node in roadNodes)
        {
            foreach (var otherNode in roadNodes)
            {
                if (node != otherNode && Vector3.Distance(node.position, otherNode.position) <= connectionThreshold)
                {
                    node.neighbors.Add(otherNode);
                }
            }
        }
        return roadNodes;
    }


    private RoadNode FindClosestNode(Vector3 position, List<RoadNode> roadNodes)
    {
        RoadNode closestNode = null;
        float minDistance = float.MaxValue;
        foreach (var node in roadNodes)
        {
            float distance = Vector3.Distance(position, node.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestNode = node;
            }
        }
        return closestNode;
    }

    void TriggerChase()
    {
        GameObject copCar = GameObject.FindGameObjectWithTag("CopCar"); // Ensure the cop car has a tag "CopCar"
        pl_chasing = true;
        if (copCar != null)
        {
            Vector3 playerCarPosition = transform.position;
            TriggerCopCarChase(copCar, playerCarPosition);
            if (!pl_played)
            {
                Debug.Log("Playing Police Sounds");
                source.PlayOneShot(police);
            }
        }
    }


    // Method to trigger the cop car chase
    public void TriggerCopCarChase(GameObject copCar, Vector3 playerCarPosition)
    {
        float connectionThreshold = 10f; // Set a suitable threshold for connecting nodes
        List<RoadNode> roadGraph = BuildRoadGraph(connectionThreshold);

        RoadNode startNode = FindClosestNode(copCar.transform.position, roadGraph);
        RoadNode targetNode = FindClosestNode(playerCarPosition, roadGraph);

        List<RoadNode> path = PathfindingAlgorithm.FindPath(startNode, targetNode, roadGraph);
        // Now, path contains the nodes from the cop car to the player's car
        // Set the path for the cop car
        copCar.GetComponent<CopCarController>().SetPath(path);
    }

    public class PathfindingAlgorithm
    {
        public static List<RoadNode> FindPath(RoadNode start, RoadNode end, List<RoadNode> roadGraph)
        {
            var openSet = new List<RoadNode>();
            var closedSet = new HashSet<RoadNode>();
            start.gScore = 0;
            start.hScore = Heuristic(start, end);
            openSet.Add(start);

            while (openSet.Count > 0)
            {
                RoadNode current = openSet[0];
                foreach (var node in openSet)
                {
                    if (node.fScore < current.fScore)
                        current = node;
                }

                if (current == end)
                    return ReconstructPath(end);

                openSet.Remove(current);
                closedSet.Add(current);

                foreach (var neighbor in current.neighbors)
                {
                    if (closedSet.Contains(neighbor))
                        continue;

                    float tentativeGScore = current.gScore + Vector3.Distance(current.position, neighbor.position);
                    if (tentativeGScore < neighbor.gScore)
                    {
                        neighbor.parent = current;
                        neighbor.gScore = tentativeGScore;
                        neighbor.hScore = Heuristic(neighbor, end);
                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }

            return new List<RoadNode>(); // Path not found
        }

        private static float Heuristic(RoadNode node, RoadNode end)
        {
            return Vector3.Distance(node.position, end.position);
        }

        private static List<RoadNode> ReconstructPath(RoadNode end)
        {
            List<RoadNode> path = new List<RoadNode>();
            while (end != null)
            {
                path.Add(end);
                end = end.parent;
            }
            path.Reverse();
            return path;
        }
    }

}