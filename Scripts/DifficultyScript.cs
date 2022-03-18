using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyScript : MonoBehaviour
{
    int difficulty; // 1 for pathetic, 2 for weak, 3 for sturdy, 4 for warrior, 5 for inhuman, 6 for no mercy
    Text difficultyText;

    // Start is called before the first frame update
    void Start()
    {
        difficultyText = GetComponent<Text>();
        difficulty = 3;   
    }

    public void ChangeDifficulty(bool isRight)
    {
        if (isRight)
            difficulty++;
        else
            difficulty--;

        if (difficulty > 6) // loops difficulty
            difficulty = 1;
        else if (difficulty < 1)
            difficulty = 6;

        if (difficulty == 1)
            difficultyText.text = "pathetic";
        else if (difficulty == 2)
            difficultyText.text = "weak";
        else if (difficulty == 3)
            difficultyText.text = "sturdy";
        else if (difficulty == 4)
            difficultyText.text = "warrior";
        else if (difficulty == 5)
            difficultyText.text = "inhuman";
        else if (difficulty == 6)
            difficultyText.text = "NO MERCY";
    }

    public int GetDifficulty()
    {
        return difficulty;
    }

}
