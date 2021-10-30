using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private int _score;

    [SerializeField] private Text _scoreText;

    public void ChangeScore(int score)
    {

        _score += score * (score / 3);
        _scoreText.text = _score.ToString();
    }

    public void RestartGame() 
    {
        SceneManager.LoadScene("SampleScene");
    }
}
