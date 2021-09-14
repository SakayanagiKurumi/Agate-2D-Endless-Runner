using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    [Header("Score Highlight")]
    public int scoreHighlightRange;
    public CharacterSoundController sound;

    private int currentScore = 0;
    private int lastScoreHighlight = 0;

    private void Start()
    {
        // Reset
        currentScore = 0;
        lastScoreHighlight = 0;
    }

    public float getCurrentScore()
    {
        return currentScore;
    }

    public void increaseCurrentScore(int increment)
    {
        currentScore += increment;

        if (currentScore - lastScoreHighlight > scoreHighlightRange)
        {
            sound.PlayScoreHighlight();
            lastScoreHighlight += scoreHighlightRange;
        }
    }

    public void finishScoring()
    {
        // Set high score
        if (currentScore > ScoreData.highSchore)
        {
            ScoreData.highSchore = currentScore;
        }
    }
}
