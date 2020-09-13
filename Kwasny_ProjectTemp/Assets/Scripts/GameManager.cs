using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

/*
 * Author: Kate Kwasny
 * Date Created: 9/11/2020
 * Date Modified: 9/12/2020
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

        //titles 
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
        //sets all menus to false so as to not display them
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
        //loads the firs level
        LeveltoLoad = firstLevel;
        MainMenu();
        

    } //end Start

    //displays and sets the MainMenu
    public void MainMenu()
    {
        //hides menus
        HideMenu();
        //sets the default values for the lives and scores
        playerLives = defaultLives;
        score = defaultScore;

        //title displays for the main menu
        titleDisplay.text = gameTitle;
        creditsDisplay.text = gameCredits;
        copyrightDisplay.text = copyrightDate;

        //turn on menu canvas
        if (MenuCanvas)
            MenuCanvas.SetActive(true);
        //turn on footer canvas
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

        //display and set time
        if(timedLevel)
        {
            currentTime = startTime;
            timerTitleDisplay.text = timerTitle;
            timerValueDisplay.text = startTime.ToString("0.00");
        }

        //display and set score
        if(scoreValueDisplay)
        {
            scoreValueDisplay.text = score.ToString();
            scoreTitleDisplay.text = scoreTitle;
        }
        //display and set lives
        if(livesValueDisplay)
        {
            livesValueDisplay.text = playerLives.ToString();
            livesTitleDisplay.text = livesTitle;
        }

        //says the game is started
        gameStarted = true;

        playerIsDead = false;
        gameState = gameStates.Playing;

        //loads the current scene additively 
        SceneManager.LoadScene(LeveltoLoad, LoadSceneMode.Additive);
        currentLevel = LeveltoLoad;

    }

    // Update is called once per frame
    void Update()
    {        

        //key inputs
        if (Input.GetKey("escape"))
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

        //displays score
        if(scoreValueDisplay)
        {
            scoreValueDisplay.text = score.ToString();
        }

        //displays lives
        if(livesValueDisplay)
        {
            livesValueDisplay.text = playerLives.ToString();
        }
        //displays or doesn't display timer depending on timedLevel being true/false
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
                //checks if player died, if so, descreases playerLives value
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

                //check if background music is playing
                if(bgMusicAudio)
                {
                    bgMusicAudio.volume -= 0.01f;
                    if (bgMusicAudio.volume <= 0.0f)
                       isMusicOver = true;                                       
                }
                if (isMusicOver || bgMusicAudio == null)
                {
                    //sound effect playing if it exists
                    if (gameOverSFX)
                    {
                        AudioSource.PlayClipAtPoint(gameOverSFX,
                       gameObject.transform.position);
                    }

                    //displays losing game over message
                    gameMessageDisplay.text = loseMessage;
                    gameState = gameStates.GameOver;
                }

                break;
            case gameStates.BeatLevel:

                //check if background music is playing
                if (bgMusicAudio)
                {
                    bgMusicAudio.volume -= 0.01f;
                    if (bgMusicAudio.volume <= 0.0f)
                        isMusicOver = true;
                }
                if (isMusicOver || bgMusicAudio == null)
                {
                    //Scene nextScene = SceneManager.GetSceneAt(SceneManager.GetActiveScene().buildIndex + 1);
                    //nextLevel = nextScene.name;

                    

                    if (nextLevel != null)
                    {
                        //sound effect playing if it exists
                        if (gameOverSFX)
                        {
                            AudioSource.PlayClipAtPoint(gameOverSFX,
                           gameObject.transform.position);
                        }
                        StartNextLevel();                        
                    }
                    else
                    {
                        //sound effect playing if it exists
                        if (gameOverSFX)
                        {
                            AudioSource.PlayClipAtPoint(gameOverSFX,
                           gameObject.transform.position);
                        }
                       
                        //display win game over message
                        gameMessageDisplay.text = winMessage;
                        gameState = gameStates.GameOver;

                    }
                    
                } // end if

                break;
            
            case gameStates.GameOver:
                //makes it so player is not able to do anything
                if (player)
                    player.SetActive(false);

                //hides menus
                HideMenu();

                //starts up end screen canvas
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
        //sets level to load as the next level
        LeveltoLoad = nextLevel;

        PlayGame();
        //if the current level is equal to the last level, next level is equal to null
        //this logic doesn't work
        if (currentLevel == nextLevel)
            nextLevel = null;

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
