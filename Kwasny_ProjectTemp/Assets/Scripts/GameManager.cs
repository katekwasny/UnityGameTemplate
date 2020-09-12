using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

/*
 * Author
 * Date Created
 * Date Modified:
 * Description: The Game Manager that manages basic game 
 * resources such as score, health, and outputs to UI 
 * elements in the game scene.
 * 
 */

public class GameManager : MonoBehaviour
{
    /******************** PUBLIC VARIABLES ********************/
    #region GM Singleton
    public static GameManager gm;

    private void Awake()
    {
        if (gm != null)
        {
            Debug.LogWarning("More than one instance of the game manager found!");
        }
        else
        {
            gm = this;
            DontDestroyOnLoad(gm);
        }
    } //end Awake
    #endregion

    public static int playerLives;
    public static int score;

    [Header("GENERAL SETTINGS")]

    [Space(10)]

    public TMP_Text titleDisplay;
    public string gameTitle = "Gamey Game";

    public TMP_Text creditsDisplay;
    public string gameCredits = "Made by Kate Kwasny";

    public TMP_Text copyrightDisplay;
    public string copyrightDate = "Copyright" + thisDay; //date created

    public TMP_Text gameOverDisplay;
    public string gameOver = "YOU LOST";

    public TMP_Text gameMessageDisplay;
    public string loseMessage = "Heh you lost";
    public string winMessage = "I guess you won, congrats.";

    [Header("GAME SETTINGS")]
    public GameObject player;
    [Tooltip("Can the level be beat by a score?")]
    public bool canBeatLevel = false;//is level beat by score?
    public int beatLevelScore; //score value to beat level
    public bool timedLevel = false; // level timed?
    public float startTime = 5.0f; //time for level, if needed

    public AudioSource bgMusicAudio;
    [HideInInspector] public bool isMusicOver;
    public AudioClip gameOverSFX;

    [HideInInspector] public enum gameStates { Playing, Death, GameOver, BeatLevel };
    [HideInInspector] public gameStates gameState = gameStates.Playing;
    [HideInInspector] public bool gameIsOver = false;
     public bool playerIsDead = false;

    [Header("MENU SYSTEMS")]
    public GameObject MenuCanvas;
    public GameObject HUDCanvas;
    public GameObject EndScreenCanvas;
    public GameObject FooterCanvas;

    public string scoreTitle = "Score: ";
    public TMP_Text scoreTitleDisplay;
    public TMP_Text scoreValueDisplay;
    public int defaultScore = 0;

    [Space(10)]

    public string livesTitle = "Lives: ";
    public TMP_Text livesTitleDisplay;
    public TMP_Text livesValueDisplay;
    public int defaultLives = 5;

    [Space(10)]

    public string timerTitle = "Timer: ";
    public TMP_Text timerTitleDisplay;
    public TMP_Text timerValueDisplay;

    [Space(10)]

    [HideInInspector] public string LeveltoLoad;
    [HideInInspector]public string currentLevel;
    public string nextLevel;
    public string firstLevel;

    /******************** PRIVATE VARIABLES ********************/
    private float currentTime;
    private bool gameStarted = false;
    private static bool replay = false;
    private static string thisDay = System.DateTime.Now.ToString("yyyy");


    /******************** METHODS ********************/


    //resets the in-component variables
    public void Reset()
    {
        defaultScore = 0;
        score = defaultScore;
        defaultLives = 5;
        playerLives = defaultLives;
        canBeatLevel = false;
        timedLevel = false;
        startTime = 5.0f;

        scoreTitle = "Score: ";
        livesTitle = "Lives: ";
        timerTitle = "Timer: ";

        gameTitle = "Gamey Game";
        gameCredits = "Kate Kwasny";
        copyrightDate = "Copyright " + thisDay;

        gameOver = "Game Over";
        loseMessage = "Heh you lost";
        winMessage = "I guess you won, congrats.";

    } //end Reset

    //hides all the Canvases
    public void HideMenu()
    {

        if (MenuCanvas)
            MenuCanvas.SetActive(false);
        if (HUDCanvas)
            HUDCanvas.SetActive(false);
        if (EndScreenCanvas)
            EndScreenCanvas.SetActive(false);
        if (FooterCanvas)
            FooterCanvas.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        LeveltoLoad = firstLevel;
        MainMenu();
        

    } //end Start

    //displays and sets the MainMenu
    public void MainMenu()
    {
        HideMenu();
        playerLives = defaultLives;
        score = defaultScore;

        titleDisplay.text = gameTitle;
        creditsDisplay.text = gameCredits;
        copyrightDisplay.text = copyrightDate;

        if (MenuCanvas)
            MenuCanvas.SetActive(true);

        if (FooterCanvas)
            FooterCanvas.SetActive(true);
    }// end MainMenu

    //quits the game
    public void QuitGame()
    {
        Application.Quit();
    } // end QuitGame

    public void PlayGame()
    {

        Debug.Log("PLAYGAME" + LeveltoLoad);

        HideMenu();

        if (HUDCanvas)
            HUDCanvas.SetActive(true);

        if(timedLevel)
        {
            currentTime = startTime;
            timerTitleDisplay.text = timerTitle;
            timerValueDisplay.text = startTime.ToString("0.00");
        }
        if(scoreValueDisplay)
        {
            scoreValueDisplay.text = score.ToString();
            scoreTitleDisplay.text = scoreTitle;
        }
        if(livesValueDisplay)
        {
            livesValueDisplay.text = playerLives.ToString();
            livesTitleDisplay.text = livesTitle;
        }
        gameStarted = true;

        playerIsDead = false;
        gameState = gameStates.Playing;

        SceneManager.LoadScene(LeveltoLoad, LoadSceneMode.Additive);
        currentLevel = LeveltoLoad;
    }


    // Update is called once per frame
    void Update()
    {
        Debug.Log(gameState);

        if(Input.GetKey("escape"))
            QuitGame();
        if (Input.GetKey("g"))
            gameState = gameStates.GameOver;
        if (Input.GetKey("b"))
            gameState = gameStates.BeatLevel;
        if (Input.GetKey("p"))
            gameState = gameStates.Playing;
        if (Input.GetKey("d"))
            gameState = gameStates.Death;
        if (Input.GetKey("r"))
            RestartGame();

        if(scoreValueDisplay)
        {
            scoreValueDisplay.text = score.ToString();
        }

        if(livesValueDisplay)
        {
            livesValueDisplay.text = playerLives.ToString();
        }
        if(timedLevel)
        {
            timerValueDisplay.text = currentTime.ToString("0.00");
            currentTime -= Time.deltaTime;
            if (gameState == gameStates.Playing && currentTime < 0)
            {
                gameMessageDisplay.text = loseMessage;
                gameState = gameStates.GameOver;
            }
        }
        else
        {
            timerTitleDisplay.alpha = 0;
            timerValueDisplay.alpha = 0;
        }
        switch (gameState)
        {
            case gameStates.Playing:
                if(playerIsDead)
                {
                    if(playerLives > 0)
                    {
                        playerLives -= 1;
                        ResetLevel();
                    }
                    else
                    {
                        gameState = gameStates.Death;
                    }
                    if (timedLevel)
                    {
                        if (currentTime < 0)
                        {
                            gameState = gameStates.GameOver;
                        }
                        
                    }
                    else if (canBeatLevel && score >= beatLevelScore)
                    {
                        gameState = gameStates.BeatLevel;
                    }
                   
                }
                break;

            case gameStates.Death:
                if(bgMusicAudio)
                {
                    bgMusicAudio.volume -= 0.01f;
                    if (bgMusicAudio.volume <= 0.0f)
                       isMusicOver = true;                                       
                }
                if (isMusicOver || bgMusicAudio == null)
                {
                    if (gameOverSFX)
                    {
                        AudioSource.PlayClipAtPoint(gameOverSFX,
                       gameObject.transform.position);
                    }
                    gameMessageDisplay.text = loseMessage;
                    gameState = gameStates.GameOver;
                }

                break;
            case gameStates.BeatLevel:
                if (bgMusicAudio)
                {
                    bgMusicAudio.volume -= 0.01f;
                    if (bgMusicAudio.volume <= 0.0f)
                        isMusicOver = true;
                }
                if (isMusicOver || bgMusicAudio == null)
                {
                    if (nextLevel == null)
                    {
                        if (gameOverSFX)
                        {
                            AudioSource.PlayClipAtPoint(gameOverSFX,
                           gameObject.transform.position);
                        }

                        gameMessageDisplay.text = winMessage;
                        gameState = gameStates.GameOver;
                        
                    }
                    else
                    {
                        if (gameOverSFX)
                        {
                            AudioSource.PlayClipAtPoint(gameOverSFX,
                           gameObject.transform.position);
                        }

                        StartNextLevel();

                    }

                    

                } // end if

                break;
            
            case gameStates.GameOver:
                if (player)
                    player.SetActive(false);

                HideMenu();

                if (EndScreenCanvas)
                {
                    EndScreenCanvas.SetActive(true);
                    gameOverDisplay.text = gameOver;
                } // end if

                break;         
        } // end switch

        
    }// end Update

    // resets the current level
    public void ResetLevel()
    {
        playerIsDead = false;
        SceneManager.UnloadSceneAsync(currentLevel);
        PlayGame();
    } // end ResetLevel

    public void StartNextLevel()
    {
        isMusicOver = false;
        playerLives = defaultLives;
        SceneManager.UnloadSceneAsync(currentLevel);

        LeveltoLoad = nextLevel;
        PlayGame();
    }// end StartNextLevel

    public void RestartGame()
    {
        score = defaultScore;
        playerLives = defaultLives;
        SceneManager.UnloadSceneAsync(currentLevel);
        LeveltoLoad = firstLevel;
        PlayGame();
    }// end RestartGame
}
