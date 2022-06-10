using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;





public class CognitiveLoad : MonoBehaviour
{
#if (UNITY_IOS || UNITY_TVOS || UNITY_WEBGL) && !UNITY_EDITOR
	[DllImport ("__Internal")]
#else
	[DllImport ("SecondaryTaskPlugin")]
#endif
        private static extern void initializeSecondaryTaskWithStimulusHandler(IntPtr signalHandler, IntPtr stopSignalHandler, IntPtr debugHandler);

#if (UNITY_IOS || UNITY_TVOS || UNITY_WEBGL) && !UNITY_EDITOR
	[DllImport ("__Internal")]
#else
	[DllImport ("SecondaryTaskPlugin")]
#endif
        private static extern void startMeasurement();

#if (UNITY_IOS || UNITY_TVOS || UNITY_WEBGL) && !UNITY_EDITOR
	[DllImport ("__Internal")]
#else
	[DllImport ("SecondaryTaskPlugin")]
#endif
        private static extern void stopMeasurement();

#if (UNITY_IOS || UNITY_TVOS || UNITY_WEBGL) && !UNITY_EDITOR
	[DllImport ("__Internal")]
#else
	[DllImport ("SecondaryTaskPlugin")]
#endif
        private static extern void respondToStimulus();
        
#if (UNITY_IOS || UNITY_TVOS || UNITY_WEBGL) && !UNITY_EDITOR
	[DllImport ("__Internal")]
#else
	[DllImport ("SecondaryTaskPlugin")]
#endif
    private static extern void addMilestone();
        
#if (UNITY_IOS || UNITY_TVOS || UNITY_WEBGL) && !UNITY_EDITOR
	[DllImport ("__Internal")]
#else
	[DllImport ("SecondaryTaskPlugin")]
#endif
    private static extern void addEventLog(char[] eventName);

#if (UNITY_IOS || UNITY_TVOS || UNITY_WEBGL) && !UNITY_EDITOR
	[DllImport ("__Internal")]
#else
	[DllImport ("SecondaryTaskPlugin")]
#endif
        private static extern string exportReactionData();

#if (UNITY_IOS || UNITY_TVOS || UNITY_WEBGL) && !UNITY_EDITOR
	[DllImport ("__Internal")]
#else
	[DllImport ("SecondaryTaskPlugin")]
#endif
        private static extern string exportEventsData();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] public delegate void RespondToStimulusDelegate();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] public delegate void stopStimulusDelegate();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] public delegate void DebugLogDelegate(string message);

    public ParticleSystem smokeVisualStimulus;
    public GameObject defaultVisualStimulus;
    public AudioSource secondaryTaskSuccessAudio;
    public AudioSource secondaryTaskFailAudio;
    private bool shouldStartSmoke = false;
    private bool shouldStopSmoke = false;
    private bool isSmokePlaying = false;
    private bool hasResponded = false;
    private bool updateLives = false;
    private bool hasSeenFirstInstructions = false;
    private String reactionTimes = "[]";
    private String eventsLog = "[]";
    private bool shouldPenalizeNextWrongReactionTime = false;

    private List<Attention> attentionCatchers = new List<Attention>();

    //TOTAL GAMEPLAY TIME
    private float currentTime = 0;

    //TIME SPENT BY THE PLAYER INTERACTING WITH OBJECTS OR CHECKING INV./NOTEBOOK/INSTRUCTIONS
    private float attentionTime = 0;


    //WHEN PLAYER IS MOVING
    bool playerActive;
    float activeTime;

    //INVENTORY RELATED
    public GameObject inventory;
    int timesInventoryOpened = 0;
    private bool wasInventoryOpened = false;
    float timeSpentCheckingInventory = 0;

    //NOTEBOOK RELATED
    public GameObject notebook;
    bool stopCountingNotebookInteractions = false;
    int timesNotebookOpened = 0;
    private bool wasNotebookOpened = false;
    float timeSpentCheckingNotebook = 0;

    //INSTRUCTIONS/HELP RELATED
    public GameObject instructions;
    bool stopCountingInstructionsInteractions = false;
    int timesInstructionsOpened = 0;
    private bool wasInstructionOpened = false;
    float timeSpentCheckingInstructions = 0;

    //OBJECTS INTERACTED WITH RELATED
    int numberOfObjectInteractions = 0;
    float timeSpentInteractingWithObjects = 0;
    bool interactionActive = false;

    //NOTIFICATIONS RELATED
    int numberOfNotificationsShown = 0;
    private bool wasNotiOpened = false;
    float timeNotificationsWereOnScreen = 0;
    float attentionByNoti = 0;

    public bool notiActive = false;

    //PUZZLE 1 (LEVER PUZZLE) RELATED
    public float FirstPuzzleStart = 0;
    public float numberOfLevers = 0;
    public float FirstPuzzleEnd = 0;

    //PUZZLE 2 (ORB PUZZLE) RELATED
    public float SecondPuzzleStart = 0;
    public float numberOfOrbs = 0;
    public float SecondPuzzleEnd = 0;
    public GameObject orbSelection;
    public float timeOrbSelectionWasOnScreen = 0;
    private bool wasOrbMenuOpened = false;

    private float MoveInteractInterfaceNoti = 0;
    private float InteractInterfaceNoti = 0;



    //SEND RESULTS BUTTON
    public Button sendResultsButton;

    private float timesSent = 0;


    #region Singleton
    public static CognitiveLoad instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of CognitiveLoad found.");
        }
        instance = this;
    }

    #endregion


    void Start()
    {
        currentTime = 0;
        attentionTime = 0;

        playerActive = false;
        activeTime = 0;

        if(smokeVisualStimulus) {
            smokeVisualStimulus.Stop();
        } else {
            defaultVisualStimulus.SetActive(false);
        }
        RespondToStimulusDelegate callback_delegate = new RespondToStimulusDelegate(engageStimulus);
        IntPtr callbackPtr = Marshal.GetFunctionPointerForDelegate(callback_delegate);

        stopStimulusDelegate stop_callback_delegate = new stopStimulusDelegate(stopStimulus);
        IntPtr stopCallbackPtr = Marshal.GetFunctionPointerForDelegate(stop_callback_delegate);

        DebugLogDelegate debug_callback_delegate = new DebugLogDelegate(debugLog);
        IntPtr debugCallbackPtr = Marshal.GetFunctionPointerForDelegate(debug_callback_delegate);

        initializeSecondaryTaskWithStimulusHandler(callbackPtr, stopCallbackPtr, debugCallbackPtr);
        GameObject.Find("GameManager").GetComponent<Menu>().openInstructions();
    }

    public void engageStimulus() {
        instance.startSmoke();
    }
    public void stopStimulus() {
        instance.checkLives();
        instance.stopSmoke();
    }

    public void checkLives() {
        if(!hasResponded) {
            updateLives = true;
        } else {
            secondaryTaskSuccessAudio.Play();
        }
        hasResponded = false;
    }

    public void startSmoke() {
        shouldStartSmoke = true;
    }

    public void stopSmoke() {
        shouldStopSmoke = true;
    }

    public void debugLog(string message) {
       Debug.Log(message);
    }

    void OnApplicationQuit() {
        stopMeasurement();
    }

    // Update is called once per frame
    void Update()
    {
        if(shouldStartSmoke) {
            if(smokeVisualStimulus) {
                smokeVisualStimulus.Play();
            } else {
                defaultVisualStimulus.SetActive(true);
            }
            shouldStartSmoke = false;
            isSmokePlaying = true;
        } else if(shouldStopSmoke) {
            if(smokeVisualStimulus) {
                smokeVisualStimulus.Stop();
            } else {
                defaultVisualStimulus.SetActive(false);
            }
            shouldStopSmoke = false;
            isSmokePlaying = false;
        }

        if (Input.GetKeyDown("space")) {
            if(isSmokePlaying) {
                shouldPenalizeNextWrongReactionTime = false;
                hasResponded = true;
                respondToStimulus();
                if(smokeVisualStimulus) {
                    smokeVisualStimulus.Stop();
                } else {
                    defaultVisualStimulus.SetActive(false);
                }
                isSmokePlaying = false;
            } else {
                if(shouldPenalizeNextWrongReactionTime) {
                    updateLives = true;
                    shouldPenalizeNextWrongReactionTime = false;
                } else {
                    shouldPenalizeNextWrongReactionTime = true;
                }
            }
        }

        if(updateLives) {
            updateLives = false;
            GameObject heatGauge = GameObject.Find("HeatGauge");
            HeatGauge heatGaugeScript = heatGauge.GetComponent<HeatGauge>();
            if(heatGaugeScript.lives == 1) {
                reactionTimes = exportReactionData();
                reactionTimes += "[DNF]";
                eventsLog = exportEventsData();
                eventsLog += "[DNF]";
                stopMeasurement();
                heatGauge.SetActive(false);
                gameObject.GetComponent<Menu>().finishGame(true);
            } else {
                secondaryTaskFailAudio.Play();
                heatGaugeScript.lives -= 1;
            }
        }

        if(!gameObject.GetComponent<Menu>().hasFinished())
            currentTime += Time.deltaTime;

        if(isPlayerWalking() || Input.GetMouseButton(1) || interactionActive || AreInterfacesOpen()  || notiActive  )
        {
            MoveInteractInterfaceNoti += Time.deltaTime;
        }
        if (interactionActive || AreInterfacesOpen() || notiActive )
        {
            InteractInterfaceNoti += Time.deltaTime;
        }

        /*
         * TIME WHERE THE PLAYER WAS MOVING
         */
        if (isPlayerWalking() || Input.GetMouseButton(1)){
            activeTime += Time.deltaTime;
        }

        /*
         * TOTAL ATTETION TIME RELATED WITH INVENTORY/NOTEBOOK/INSTRUCTIONS
         */
        if(AreInterfacesOpen())
        {
            attentionTime += Time.deltaTime;
            if (isInventoryOpen() && !isNotebookOpen())
            {
                timeSpentCheckingInventory += Time.deltaTime;
            }
            if (isNotebookOpen()){
                timeSpentCheckingNotebook += Time.deltaTime;

            }
            if (areInstructionsOpened()){
                timeSpentCheckingInstructions += Time.deltaTime;
            }
            if(IsOrbSelectionOpen())
            {
                timeOrbSelectionWasOnScreen += Time.deltaTime; 
            }
        }

        /*
         * NUMBER OF TIMES INVENTORY WAS OPENED
         */
        if (Input.GetButtonDown("Inventory") && Inventory.instance.isUnlocked() && !inventory.activeSelf){
            InventoryWasOpened();
        }

        /*
         * NUMBER OF TIMES INSTRUCTIONS WERE OPENED
         */
        if (instructions.activeSelf && !stopCountingInstructionsInteractions){
            helpWasClicked();
            stopCountingInstructionsInteractions = true;
        }else if (!instructions.activeSelf){
            if(!hasSeenFirstInstructions) {
                hasSeenFirstInstructions = true;
                startMeasurement();
            }
            stopCountingInstructionsInteractions = false;
        }

        /*
         * NUMBER OF TIMES NOTEBOOK WAS OPENED
         */
        if (notebook.activeSelf && !stopCountingNotebookInteractions){
            notebookWasOpened();
            stopCountingNotebookInteractions = true;
        }
        else if (!notebook.activeSelf){
            stopCountingNotebookInteractions = false;
        }

        /*
         * TIME NOTIFICATIONS SHOWED ON SCREEN
         */
        if(notiActive && !AreInterfacesOpen() && !interactionActive)
        {
            timeNotificationsWereOnScreen += Time.deltaTime;
            attentionByNoti += Time.deltaTime;
        }

        /*
         * TIME INTERACTIONS
         */
        if(interactionActive && !AreInterfacesOpen())
        {
            timeSpentInteractingWithObjects += Time.deltaTime;
        } 

         

    }


    public float getCurrentTime()
    {
        return currentTime;
    }

    /*
     * ADDS TIME TO THE "ATTENTION TIME", WHEN THE PLAYER INTERACTS/CLICKS ON INTERACTIBLE OBJECTS
     */

    public float GetAttentionObject()
    {
        return timeSpentInteractingWithObjects;
    }


    /*
     * TO VERIFY IF THE PLAYER IS ACTIVE OR NOT
     */
    public bool isPlayerWalking(){
        return GameObject.Find("Player").GetComponent<PlayerMovement>().isWalking();
    }

    /*
     * INVENTORY INTERACTIONS
     */
    public bool isInventoryOpen(){
        if (inventory.activeSelf) {
            if(!wasInventoryOpened) {
                wasInventoryOpened = true;
                addEventLog("InventoryOpened".ToCharArray());
            }
            return true;
        } else {
            if(wasInventoryOpened) {
                wasInventoryOpened = false;
                addEventLog("InventoryClosed".ToCharArray());
            }
            return false;
        }
    }
    public void InventoryWasOpened(){
        timesInventoryOpened++;
    }

    /*
     * NOTEBOOK INTERACTIONS
     */
    public bool isNotebookOpen(){
        if (notebook.activeSelf) {
            if(!wasNotebookOpened) {
                wasNotebookOpened = true;
                addEventLog("NotebookOpened".ToCharArray());
            }
            return true;
        }
        else {
            if(wasNotebookOpened) {
                wasNotebookOpened = false;
                addEventLog("NotebookClosed".ToCharArray());
            }
            return false;
        }
    }
    public void notebookWasOpened()
    {
        timesNotebookOpened++;
    }



    /*
     * INSTRUCTIONS INTERACTIONS
     */
    public bool areInstructionsOpened(){
        if (instructions.activeSelf) {
            if(!wasInstructionOpened) {
                wasInstructionOpened = true;
                addEventLog("InstructionsOpened".ToCharArray());
            }
            return true;
        }
        else {
            if(wasInstructionOpened) {
                wasInstructionOpened = false;
                addEventLog("InstructionsClosed".ToCharArray());
            }
            return false;
        }
    }

    public void helpWasClicked(){
        timesInstructionsOpened++;
    }

    /*
     * OBJECTS INTERACTED WITH
     */
     public void addInteraction(){
        numberOfObjectInteractions++;
        addEventLog("ObjectInteraction".ToCharArray());
     }
    public void setInteractionActive(bool state)
    {
        this.interactionActive = state;

    }


    /*
     * NOTIFICATIONS
     */
    public void addNotification()
    {
        numberOfNotificationsShown++;
        wasNotiOpened = true;
        addEventLog("NotificationShown".ToCharArray());
    }

    public void setNotiActive(bool state)
    {
        this.notiActive = state;
        if (!state)
        {
            attentionTime += attentionByNoti;
            attentionByNoti = 0;

            if(wasNotiOpened) {
                wasNotiOpened = false;
                addEventLog("NotificationRemoved".ToCharArray());
            }
        }

    }


    /*
     * PUZZLE 1: LEVER
     */
    public void triggerFirstPuzzle(bool state)
    {
        if (state) {
            FirstPuzzleStart = currentTime;
            addMilestone();
        } else if (!state) {
            FirstPuzzleEnd = currentTime;
        }
        Debug.Log("1st START: " + FirstPuzzleStart + "     END: " + FirstPuzzleEnd);
    }
    public void addLeverInteraction()
    {
        numberOfLevers++;
    }

    /*
     * PUZZLE 2: ORBS
     */
    public void triggerSecondPuzzle(bool state)
    {
        if (state) {
            SecondPuzzleStart = currentTime;
            addMilestone();
        } else if (!state) {
            SecondPuzzleEnd = currentTime;

            reactionTimes = exportReactionData();
            eventsLog = exportEventsData();
            stopMeasurement();
        }
        Debug.Log("2nd START: " + SecondPuzzleStart + "     END: " + SecondPuzzleEnd);
    }
    public void addOrbInteraction()
    {
        numberOfOrbs++;
    }


    public bool IsOrbSelectionOpen()
    {
        if (orbSelection.activeSelf){
            if(!wasOrbMenuOpened) {
                wasOrbMenuOpened = true;
                addEventLog("OrbMenuOpened".ToCharArray());
            }
            return true;
        } else {
            if(wasOrbMenuOpened) {
                wasOrbMenuOpened = false;
                addEventLog("OrbMenuClosed".ToCharArray());
            }
            return false;
        }
    }

    /*
     * GENERATE RANDOM CODE NAME
     */
    public string GenerateName(int len)
    {
        System.Random r = new System.Random();
        string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "y", "z" };
        string[] vowels = {"a", "e", "i", "o", "u"};
        string Name = "";
        Name += consonants[r.Next(consonants.Length)].ToUpper();
        Name += vowels[r.Next(vowels.Length)];
        int b = 2; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
        while (b < len)
        {
            Name += consonants[r.Next(consonants.Length)];
            b++;
            Name += vowels[r.Next(vowels.Length)];
            b++;
        }
        
        Debug.Log(Name);
        return Name;

    }


    public bool AreInterfacesOpen()
    {
        if (isInventoryOpen() || isNotebookOpen() || areInstructionsOpened() || IsOrbSelectionOpen())
            return true;
        else return false;
    }



    /*
     * SEND RESULTS
     */
    public void sendResults()
    {
        if (timesSent == 0)
        {
            String codeName = GenerateName(7);

            String difficulty = SceneManager.GetActiveScene().name;

            attentionTime += timeSpentInteractingWithObjects;

            if (sendResultsButton.GetComponent<CanvasGroup>().alpha == 1)
            {
                Debug.Log("SeNT");
                gameObject.GetComponent<SendResults>().Send(difficulty, codeName, currentTime, attentionTime, activeTime,
                                                            numberOfObjectInteractions, timeSpentInteractingWithObjects,
                                                            timesInventoryOpened, timeSpentCheckingInventory,
                                                            timesInstructionsOpened, timeSpentCheckingInstructions,
                                                            timesNotebookOpened, timeSpentCheckingNotebook,
                                                            numberOfNotificationsShown, timeNotificationsWereOnScreen,
                                                            FirstPuzzleStart, numberOfLevers, FirstPuzzleEnd,
                                                            SecondPuzzleStart, numberOfOrbs, timeOrbSelectionWasOnScreen, SecondPuzzleEnd,
                                                            MoveInteractInterfaceNoti, InteractInterfaceNoti, reactionTimes, eventsLog);

            }

            Debug.Log("Player Code Name: " + codeName + "\n" +
                  "Total GamePlay Time: " + currentTime + "\n" +
                  "Total Attention Time: " + attentionTime + "\n" +
                  "Total Active Time: " + activeTime + "\n" +
                  "\n" +
                  "Nr Objects Interacted with: " + numberOfObjectInteractions + "\n" +
                  "Total Time spent interacting with objects: " + timeSpentInteractingWithObjects + "\n" +
                   "\n" +
                  "Nr of Times Inventory Was opened: " + timesInventoryOpened + "\n" +
                  "Total Time spent with Inventory opened: " + timeSpentCheckingInventory + "\n" +
                  "\n" +
                  "Nr of Times Instructions were opened: " + timesInstructionsOpened + "\n" +
                  "Total Time spent on instructions: " + timeSpentCheckingInstructions + "\n" +
                  "\n" +
                  "Nr of Times Notebook was opened: " + timesNotebookOpened + "\n" +
                  "Total Time spent on Notebook: " + timeSpentCheckingNotebook + "\n" +
                  "\n" +
                  "Nr Notifications shown: " + numberOfNotificationsShown + "\n" +
                  "Total Time notifications were on screen: " + timeNotificationsWereOnScreen + "\n" +
                  "\n"
                  +
                  "First Puzzle Start: " + FirstPuzzleStart + "\n" +
                  "Number of Levers interacted: " + numberOfLevers + "\n" +
                  "First Puzzle End: " + FirstPuzzleEnd + "\n" +

                  "Second Puzzle Start: " + SecondPuzzleStart + "\n" +
                  "Number of SPs interacted: " + numberOfOrbs + "\n" +
                  "Total Time OrbSelection were on screen: " + timeOrbSelectionWasOnScreen + "\n" +
                  "Second Puzzle End: " + SecondPuzzleEnd + "\n" +
                  "Reaction Times: " + reactionTimes + "\n" +
                  "Events Log: " + eventsLog + "\n" +
                  "\n");

            timesSent++;
        }
            
    }
      
        
    






}
