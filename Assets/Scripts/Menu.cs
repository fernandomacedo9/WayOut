using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;



public class Menu : MonoBehaviour
{

    private List<CanvasGroup> displayMessages;
    private List<CanvasGroup> notifications;

    public Button startButton, aboutUsButton, helpButton;
    public GameObject sendResults;

    public Camera initialCam, mainCam, finalCam;
    public Canvas menu;
    public Canvas notiCanvas;
    public Image fadeImgMenu, imageFinal;
    public GameObject aboutUsText, instructions, gameContextMenu, secondaryTaskTutorial;
    public TextMeshProUGUI secondaryTaskTutorialText;
    public InputField inputCodeName;
    public GameObject fullScreen;

    public GameObject quitButton;

    public string difficulty = "";

    public Canvas pauseCanvas;
    public GameObject pauseButton;


    private bool started = false, finish = false, isGameOver = false;
    private string debugMap = "";

    void Start()
    { 
        if (SceneManager.GetActiveScene().name.Equals("Tese_Scene_Intro"))
        {
            Loader load = GetComponent<Loader>();
            load.Load();
            Screen.fullScreen = true;
        }
       
        displayMessages = new List<CanvasGroup>();
        foreach (CanvasGroup cg in menu.GetComponentsInChildren<CanvasGroup>())
        {
            displayMessages.Add(cg);
        }
            notifications = new List<CanvasGroup>();
            foreach (CanvasGroup cg in notiCanvas.GetComponentsInChildren<CanvasGroup>())
            {
                notifications.Add(cg);
                cg.blocksRaycasts = false;
            }

       /*
        * Starts "Tese_Scene_Intro".
        */
        if (menu.isActiveAndEnabled)
        {
            fadeImgMenu.canvasRenderer.SetAlpha(1.0f);
            imageFinal.canvasRenderer.SetAlpha(0.0f);
            sendResults.GetComponent<CanvasGroup>().blocksRaycasts = false;
            if (started == false)
            {
                //BLOCKING QUESTIONNAIRE
                displayMessages[5].gameObject.SetActive(false);
                Fade(fadeImgMenu, 1.5f, 0f);
                initialCam.enabled = true;
                mainCam.enabled = false;
                finalCam.enabled = false;
            }
        }

       /*
        * Starts on "Tese_Scene" & "Tese_Scene_Hard".
        */
        else if (!menu.isActiveAndEnabled && !SceneManager.GetActiveScene().name.Equals("Tese_Scene_Intro"))
        {
          StartGameWithoutMenu();
        }

  }
    public void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Screen.fullScreen = !Screen.fullScreen;
            closeInstructions();
        }

        if (Input.GetKey(KeyCode.F5)) {
            debugMap = "A1";
        } else if (Input.GetKey(KeyCode.F6)) {
            debugMap = "A2";
        } else if (Input.GetKey(KeyCode.F7)) {
            debugMap = "B1";
        } else if (Input.GetKey(KeyCode.F8)) {
            debugMap = "B2";
        }
    }


    /*
    * ------------------- TESE_SCENE_INTRO -------------------
    */

    /*
     * Invoked when the scene Tese_Scene_Intro.
     */
    public void StartGameFromIntro()
        {
        if (started == false && startButton.GetComponent<CanvasGroup>().alpha == 1)
        {
            foreach (CanvasGroup cg in displayMessages)
            {
                StartCoroutine(FadeCanvasGroup(cg, cg.alpha, 0f));
            }
            gameContextMenu.GetComponent<CanvasGroup>().interactable = true;
            gameContextMenu.GetComponent<CanvasGroup>().blocksRaycasts = true;
            StartCoroutine(FadeCanvasGroup(gameContextMenu.GetComponent<CanvasGroup>(), gameContextMenu.GetComponent<CanvasGroup>().alpha, 1f));
        }
    }

    public void openSecondaryTaskTutorial() {
            foreach (CanvasGroup cg in displayMessages)
            {
                StartCoroutine(FadeCanvasGroup(cg, cg.alpha, 0f));
            }
            secondaryTaskTutorial.GetComponent<CanvasGroup>().interactable = true;
            secondaryTaskTutorial.GetComponent<CanvasGroup>().blocksRaycasts = true;

            string nextScene = NextScene();
            if(nextScene == "A1" || nextScene == "A2") {
                secondaryTaskTutorial.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                secondaryTaskTutorial.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                secondaryTaskTutorial.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
                secondaryTaskTutorialText.text = "Due to the energy required to make your body move, occasionally you might overheat, letting out some smoke, at which point you should press [space bar] as fast as you can. Be careful to press [space bar] only when you see smoke or it could have a detrimental effect to you.\nMiss it 3 times and you will be destroyed, ending the game";
            } else {
                secondaryTaskTutorial.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                secondaryTaskTutorial.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
                secondaryTaskTutorial.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
                secondaryTaskTutorialText.text = "You will be asked to respond to a stimulus(Red Marker) that will appear on the screen. When the Red Marker appears you should press [space bar] as fast as you can. Be careful to press [space bar] only when you see the Marker or it could have a detrimental effect to you.\nIf you miss it 3 times it's game over";
            }
            StartCoroutine(FadeCanvasGroup(secondaryTaskTutorial.GetComponent<CanvasGroup>(), secondaryTaskTutorial.GetComponent<CanvasGroup>().alpha, 1f));
    }

    public void StartAfterTutorial() {
            startButton.interactable = false;
            aboutUsButton.interactable = false;
            foreach (CanvasGroup cg in displayMessages)
            {
                StartCoroutine(FadeCanvasGroup(cg, cg.alpha, 0f));
            }
            changeRaycastOfNoti(true);
            Fade(fadeImgMenu, 6f, 1f);
            initialCam.GetComponent<Animator>().Play("ChangePos");
            StartCoroutine(CoroutineBeginChangeScene(6f));
    }


    public bool hasStarted()
    {
        return started;
    }
    IEnumerator CoroutineBegin(float secs)
    {
        yield return new WaitForSeconds(secs);
        mainCam.enabled = true;
        initialCam.enabled = false;
        finalCam.enabled = false;
        Fade(fadeImgMenu, 3f, 0f);
        started = true;
        helpButton.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(true);

        GameObject.Find("GameManager").GetComponent<UIFader>().FadeIn(12);
        
        menu.gameObject.SetActive(false);
        fadeImgMenu.gameObject.SetActive(false);
    }

    IEnumerator CoroutineBeginChangeScene(float secs)
    {
        yield return new WaitForSeconds(secs);

        startButton.gameObject.GetComponent<CanvasGroup>().interactable = false;
        startButton.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;

        string nextScene = NextScene();

        GameObject.Find("ProgressSceneLoader").GetComponent<ProgressSceneLoader>().LoadScene(nextScene);
    }

    public string NextScene() {
        if(!debugMap.Equals("")) {
            return debugMap;
        }

        if (difficulty.Equals("A1"))
            return "A2";
        else if (difficulty.Equals("A2"))
            return "B1";
        else if (difficulty.Equals("B1"))
            return "B2";
        else
            return "A1";
    }


    /*
     * ABOUT US
     */
    public void AboutUs()
    {
        if (started == false)
        {
            foreach (CanvasGroup cg in displayMessages)
            {
                StartCoroutine(FadeCanvasGroup(cg, cg.alpha, 0f));
            }
            secondaryTaskTutorial.GetComponent<CanvasGroup>().interactable = false;
            secondaryTaskTutorial.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
            StartCoroutine(FadeCanvasGroup(aboutUsText.GetComponent<CanvasGroup>(), aboutUsText.GetComponent<CanvasGroup>().alpha, 1f));
        }
    }
    public void BackToMenu()
    {
        if (started == false)
        {
            StartCoroutine(FadeCanvasGroup(aboutUsText.GetComponent<CanvasGroup>(), aboutUsText.GetComponent<CanvasGroup>().alpha, 0f));
            StartCoroutine(FadeCanvasGroup(startButton.GetComponent<CanvasGroup>(), startButton.GetComponent<CanvasGroup>().alpha, 1f));
            StartCoroutine(FadeCanvasGroup(aboutUsButton.GetComponent<CanvasGroup>(), aboutUsButton.GetComponent<CanvasGroup>().alpha, 1f));
            StartCoroutine(FadeCanvasGroup(fullScreen.GetComponent<CanvasGroup>(), aboutUsButton.GetComponent<CanvasGroup>().alpha, 1f));

        }
    }


/*
 * ------------------- A1 & A2 -------------------
 */

   /*
    * START:
    * Invoked when the scene A1 or A2 is initiated.
    */
    public void StartGameWithoutMenu()
    {
        if (started == false)
        {
            startButton.interactable = false;
            aboutUsButton.interactable = false;
            foreach (CanvasGroup cg in displayMessages)
            {
                StartCoroutine(FadeCanvasGroup(cg, cg.alpha, 0f));
            }
            changeRaycastOfNoti(true);
            StartCoroutine(CoroutineBegin(0f));
        }
    }


   /*
    * INVENTORY:
    * Closes the inventory/backpack.
    */
    public void closeInventory()
    {
        adjustMainCameraForInventoryClose();
        GameObject.Find("Inventory").SetActive(false);
    }
    public void adjustMainCameraForInventoryOpen()
    {
        mainCam.GetComponent<CameraScript>().cameraOffsetX = 4.0f;
    }
    public void adjustMainCameraForInventoryClose()
    {
        mainCam.GetComponent<CameraScript>().cameraOffsetX = 0.0f;
    }


   /*
    * FINISH:
    * Functions related with the end game.
    */
    public void finishGame()
    {
        finishGame(false);
    }
    public void finishGame(bool gameOver)
    {
        finish = true;
        isGameOver = gameOver;
        gameObject.GetComponent<CanInteract>().setCanInteract(false);
        menu.gameObject.SetActive(true);
        sendResults.GetComponent<CanvasGroup>().blocksRaycasts = true;
        imageFinal.canvasRenderer.SetAlpha(0.0f);
        Fade(imageFinal, 4f, 1f);
        StartCoroutine(CoroutineEnd(4f));
    }

    public bool hasFinished()
    {
        return finish;
    }

    IEnumerator CoroutineEnd(float secs)
    {
        yield return new WaitForSeconds(secs);

        if(isGameOver) {
            //i=8 GameOverText
            StartCoroutine(FadeCanvasGroup(displayMessages[8], displayMessages[8].alpha, 1f));
        }
        
            //i=4 So only the "Quit" button and "Thanks" message show up
        for(int i=4;i<6;i++) {
            if (!displayMessages[i].gameObject.name.Equals("FullScreen"))
            {
                // i=4 Send Results, i=5 Thanks
                StartCoroutine(FadeCanvasGroup(displayMessages[i], displayMessages[i].alpha, 1f));
            }
        }
        sendResults.GetComponent<CanvasGroup>().interactable = true;

        Fade(imageFinal, 4f, 0f);
        mainCam.enabled = false;
        initialCam.enabled = false;
        finalCam.enabled = true;
        finish = true;

        helpButton.gameObject.SetActive(false);
        helpButton.gameObject.GetComponent<Button>().interactable = false;
        pauseButton.gameObject.SetActive(false);
        pauseButton.gameObject.GetComponent<Button>().interactable = false;

        Cursor.visible = true;
    }

               /*
                * QUESTIONNAIRE:
                * Related to the end game, deals with the final panel, where
                * the randomly generated code name is displayed.
                */

                public void enableQuestionnaire(string codeName)
                {
                    StartCoroutine(sendCoroutineQuestionnaire(0.1f, codeName));
                }

                IEnumerator sendCoroutineQuestionnaire(float secs, string codeName)
                {
                    yield return new WaitForSeconds(secs);
                    sendResults.GetComponent<CanvasGroup>().blocksRaycasts = false;
                    sendResults.GetComponent<CanvasGroup>().interactable = true;


                    for (int i = 4; i < 6; i++)
                    {
                          StartCoroutine(FadeCanvasGroup(displayMessages[i], displayMessages[i].alpha, 0f));
                        
                    }
                    for (int i = 6; i < displayMessages.Count; i++)
                    {
                        if (!displayMessages[i].gameObject.name.Equals("OverHeatEnd")) {
                            StartCoroutine(FadeCanvasGroup(displayMessages[i], displayMessages[i].alpha, 1f));
                            displayMessages[i].blocksRaycasts = true;
                            displayMessages[i].interactable = true;
                        } else {
                            StartCoroutine(FadeCanvasGroup(displayMessages[i], displayMessages[i].alpha, 0f));
                        }
                    }
            inputCodeName.text = codeName;
    }


    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    /*
     * FADES:
     * Fades in/out menu components.
     */
    void Fade(Image img, float seconds, float alpha)
    {
        img.CrossFadeAlpha(alpha, seconds, false);
    }


    public IEnumerator FadeCanvasGroupRemove(CanvasGroup cg, float start, float end, float lerpTime = 1f)
    {
        float timeStartedLerping = Time.time;
        float timeSinceStarted = Time.time - timeStartedLerping;
        float percentageComplete = timeSinceStarted / lerpTime;

        while (true)
        {
            timeSinceStarted = Time.time - timeStartedLerping;
            percentageComplete = timeSinceStarted / lerpTime;

            float currentValue = Mathf.Lerp(start, end, percentageComplete);

            cg.alpha = currentValue;

            if (percentageComplete >= 1) break;

            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float lerpTime = 0.6f)
    {
        float timeStartedLerping = Time.time;
        float timeSinceStarted = Time.time - timeStartedLerping;
        float percentageComplete = timeSinceStarted / lerpTime;

        while (true)
        {
            timeSinceStarted = Time.time - timeStartedLerping;
            percentageComplete = timeSinceStarted / lerpTime;

            float currentValue = Mathf.Lerp(start, end, percentageComplete);

            cg.alpha = currentValue;

            if (percentageComplete >= 1) break;

            yield return new WaitForEndOfFrame();
        }
    }


   /*
    * INSTRUCTIONS:
    * Opens/closes intructions.
    */
    public void help()
    {
        if (instructions.active == true)
            closeInstructions();
        else if (instructions.active == false)
            openInstructions();
    }

    public void openInstructions()
    {
        if( Time.timeScale == 1)
            instructions.SetActive(true);

        
    }

    public void closeInstructions()
    {
        if (Time.timeScale == 1)
            instructions.SetActive(false);
    }


   /*
    * RAYCASTS:
    * Block/unblocks raycast notifications.
    */
    public void changeRaycastOfNoti(bool state)
    {
        foreach (CanvasGroup cg in notiCanvas.GetComponentsInChildren<CanvasGroup>())
        {
            cg.blocksRaycasts = state;
        }
    }

   /*
    * DIFFICULTY:
    * Invoked by the class "Loader" to set the string "difficulty" based on the
    * data colected from Google Sheets.
    */
    public void setDifficulty(string new_difficulty)
    {
        difficulty = new_difficulty;
    }
    public int randomizeDifficulty()
    {
        return Random.Range(0, 4);
    }


    public void PauseGame()
    {
        pauseCanvas.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        pauseCanvas.gameObject.SetActive(false);
        Time.timeScale = 1;
    }


}
