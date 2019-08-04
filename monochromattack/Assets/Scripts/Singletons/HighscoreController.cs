using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighscoreController : MonoBehaviour
{
    public static HighscoreController instance;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI finalScoreText;
    private int score;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Update()
    {
        scoreText.text = score.ToString();
    }

    public void StartScoring()
    {
        score = 0;
        scoreText.gameObject.SetActive(true);
        StartCoroutine(TimedScoring());
    }

    private IEnumerator TimedScoring()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.99f);
            score++;
        }
    }

    public void IncreaseScore()
    {
        score += 1;
    }

    public void ToFinalScore()
    {
        finalScoreText.text = score.ToString();
        finalScoreText.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(false);
    }
}
