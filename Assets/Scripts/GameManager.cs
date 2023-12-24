using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    private int score;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        NewGame();
    }

    // Update is called once per frame
    private void LoadLevel(){
        SceneManager.LoadScene("Level_1");
    }

    private void NewGame(){
        score = 0;
        LoadLevel();
    }

    public void LevelComplete(){
        score += 1000;
        LoadLevel();
    }

    public void LevelFailed(){
        NewGame();
    }
}
