using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    public float turnSpeed = 10f;

    private Vector2 input;
    private float angle;

    private Quaternion targetRotation;
    private Transform cam;

    public FixedJoystick moveJoystick;
    public float power;
    public Rigidbody rb;


    private Text text;
    public int score = 0;
    public int finalScore = 0;
    public Text scoreText;
    public Text finalScoreText;
    public GameObject finishPanel;


    public GameObject coinPrefab;

    void Start()
    {
        Time.timeScale = 1;
        finishPanel.SetActive(false);
        
        if (PlayerPrefs.HasKey("FinalScore"))
        {
            finalScore = PlayerPrefs.GetInt("FinalScore");
        }
        cam = Camera.main.transform;
    }


    private void Update()
    {
        GetInput();
       
    }

    private void FixedUpdate()
    {
        if ((input.x) == 0 && (input.y) == 0) return;

        CalculateDirection();
        Rotate();
        Move();
    }

    void GetInput()
    {
        //input.x = Input.GetAxisRaw("Horizontal");
        //input.y = Input.GetAxisRaw("Vertical");

        input.y = moveJoystick.Vertical;
        input.x = moveJoystick.Horizontal;
    }
    
    void CalculateDirection()
    {
        angle = Mathf.Atan2(input.x, input.y);
        angle = Mathf.Rad2Deg * angle;
        angle += cam.eulerAngles.y;
    }

    void Rotate()
    {
        targetRotation = Quaternion.Euler(0, angle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }

    void Move()
    {
        Vector3 direction = Vector3.forward * input.y + Vector3.right * input.x;
        rb.AddForce(direction * power * Time.fixedDeltaTime, ForceMode.Force);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.tag == "Coin")
        {
            score += 10;
            
            scoreText.text = "Score  " + score.ToString();
            Destroy(other.gameObject);
            Vector3 randomSpawnPosition = new Vector3(Random.Range(-2, 2), 0, Random.Range(-3, 3));
            Instantiate(coinPrefab, randomSpawnPosition, Quaternion.identity);

            if (score == 100)
            {
                finishPanel.SetActive(true);
                finalScore = score;
                PlayerPrefs.SetInt("FinalScore", finalScore);
                finalScoreText.text = "Final Score " + score.ToString();
                Time.timeScale = 0;
                
            }
        }
    }
}