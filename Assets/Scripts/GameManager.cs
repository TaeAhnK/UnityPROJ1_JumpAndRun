using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int totalPoint;          // Total Point of Game
    public int stagePoint;          // Stage Point of Game
    public int stage;               // Current Stange
    public int life;                // Life
    public Player_Move player;      // Player_Move.cs
    public GameObject[] Stages;     // Stages
    public Image[] UILife;          // UI Image of Life
    public Text UIPoint;            // UI Text of Score
    public Text UIStage;            // UI Text of Stage
    public GameObject RetryButton;  // UI Retry Button

///////////////////////////////////////////////////////////////////////////////////////////////////
    
    void Update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }

///////////////////////////////////////////////////////////////////////////////////////////////////
    public void NextStage()
    {
        // Calculate Point
        totalPoint += stagePoint;
        stagePoint = 0;

        // Move to Next Stage
        if (stage < Stages.Length-1)
        {
            Stages[stage].SetActive(false);
            Stages[++stage].SetActive(true);
            PlayerReposition();
            UIStage.text = "STAGE" + (stage + 1);
        }

        // All Clear
        else
        {
            Time.timeScale = 0;

            // Retry UI
            Text btnText = RetryButton.GetComponentInChildren<Text>();
            btnText.text = "Game Clear";
            RetryButton.SetActive(true);
        }
    }

///////////////////////////////////////////////////////////////////////////////////////////////////
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Fallen
        if (collision.gameObject.tag == "Player")
        {
            if (life > 1)
            {
                PlayerReposition();
            }
            LifeMinus();
        }
    }

    public void LifeMinus()
    {
        // Not GameOver
        if (life > 1)
        {
            UILife[--life].color = new Color(1,1,1,0.1f);
        }
        // GameOver
        else
        {
            // Death Effect
            player.OnDeath();
            UILife[0].color = new Color(1,1,1,0.1f);
            // Retry UI
            Text btnText = RetryButton.GetComponentInChildren<Text>();
            btnText.text = "Retry?";
            RetryButton.SetActive(true);
        }
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(0,0,-1);
        player.VelocityZero();
    }

///////////////////////////////////////////////////////////////////////////////////////////////////
    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

}
///////////////////////////////////////////////////////////////////////////////////////////////////
