﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScoreController : MonoBehaviour
{
    [Header("UI")]
    public Text score;
    public Text highScore;

    [Header("Score")]
    public ScoreController ScoreController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        score.text = ScoreController.getCurrentScore().ToString();
        highScore.text = ScoreData.highSchore.ToString();
    }
}
