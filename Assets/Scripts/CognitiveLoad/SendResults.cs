using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class SendResults : MonoBehaviour
{
    public String difficulty;
    public String codeName;
    public float gameplayTime;
    public float totalAttentionTime;
    public float activeTime;

    public float nrObjects;
    public float timeObjects;

    public float nrInventory;
    public float timeInventory;

    public float nrInstructions;
    public float timeInstructions;

    public float nrNotebook;
    public float timeNotebook;

    public float nrNoti;
    public float timeNoti;

    public float FirstPuzzleStart;
    public float NumberOfLeversInteracted;
    public float FirstPuzzleEnd;

    public float SecondPuzzleStart;
    public float NumberOfSPsInteracted; //SpherePlaceholders interacted
    public float timeOrbSelection;
    public float SecondPuzzleEnd;


    public float MoveInteractInterfaceNoti;
    public float InteractInterfaceNoti;
    public String reactionTimes;
    public String eventsLog;


    private float timesSent = 0;

    private static string mailToSend = "";
    private static string mailToSendHeader = "";


    IEnumerator Post(String codeName, float gameplayTime, float totalAttentionTime, float activeTime,
                     float nrObjects, float timeObjects,
                     float nrInventory, float timeInventory,
                     float nrInstructions, float timeInstructions,
                     float nrNotebook, float timeNotebook,
                     float nrNoti, float timeNoti,
                     float firstPstart, float leversInteracted, float firstPend,
                     float secondPstart, float SPsInteracted, float timeOrbSelection, float secondPend,
                     float moveInteractInterfaceNoti, float interactInterfaceNoti, String reactionTimes, 
                     String eventsLog, System.Action<string> onCompleted)
                     {
            string BASE_URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSc6WR15pdf-HG2DFItp80xhF7x73_xAeSTEhFSc_aplr-A6rw/formResponse";


            WWWForm form = new WWWForm();

            form.AddField("entry.1806259079", difficulty);
            form.AddField("entry.1848118055", codeName);
            form.AddField("entry.726679629", string.Format("{0:N3}", gameplayTime));
            form.AddField("entry.1040305561", string.Format("{0:N3}", totalAttentionTime));
            form.AddField("entry.1824688064", string.Format("{0:N3}", activeTime));

            form.AddField("entry.476264530", string.Format("{0:N3}", nrObjects));
            form.AddField("entry.189834301", string.Format("{0:N3}", timeObjects));

            form.AddField("entry.1100957265", string.Format("{0:N3}", nrInventory));
            form.AddField("entry.1558385086", string.Format("{0:N3}", timeInventory));

            form.AddField("entry.347818023", string.Format("{0:N3}", nrInstructions));
            form.AddField("entry.1169424719", string.Format("{0:N3}", timeInstructions));

            form.AddField("entry.1718453378", string.Format("{0:N3}", nrNotebook));
            form.AddField("entry.1023461316", string.Format("{0:N3}", timeNotebook));

            form.AddField("entry.1522211870", string.Format("{0:N3}", nrNoti));
            form.AddField("entry.1253734070", string.Format("{0:N3}", timeNoti));

            form.AddField("entry.118912436", string.Format("{0:N3}", firstPstart));
            form.AddField("entry.1792388682", string.Format("{0:N3}", leversInteracted));
            form.AddField("entry.1155788460", string.Format("{0:N3}", firstPend));

            form.AddField("entry.758819434", string.Format("{0:N3}", secondPstart));
            form.AddField("entry.674803246", string.Format("{0:N3}", SPsInteracted));
            form.AddField("entry.1536395787", string.Format("{0:N3}", timeOrbSelection));
            form.AddField("entry.1270842295", string.Format("{0:N3}", secondPend));

            form.AddField("entry.1116421184", string.Format("{0:N3}", moveInteractInterfaceNoti));
            form.AddField("entry.571586739", string.Format("{0:N3}", interactInterfaceNoti));
            form.AddField("entry.955766916", reactionTimes);
            form.AddField("entry.691864212", eventsLog);

            timesSent ++;


            byte[] rawData = form.data;
            WWW www = new WWW(BASE_URL, rawData);
            yield return www;

            onCompleted("");

    }

    public void AfterForm(string s)
    {
        gameObject.GetComponent<Menu>().enableQuestionnaire(codeName);

    }



    public void Send(String difficulty, String codeName, float gameplayTime, float totalAttentionTime, float activeTime,
                     float nrObjects, float timeObjects,
                     float nrInventory, float timeInventory,
                     float nrInstructions, float timeInstructions,
                     float nrNotebook, float timeNotebook,
                     float nrNoti, float timeNoti,
                     float firstPstart, float leversInteracted, float firstPend,
                     float secondPstart, float SPsInteracted, float timeOrbSelection, float secondPend,
                     float moveInteractInterfaceNoti, float interactInterfaceNoti, String reactionTimes,
                     String eventsLog)
    {
        

        this.difficulty = difficulty;
        this.codeName = codeName;
        this.gameplayTime = gameplayTime;
        this.totalAttentionTime = totalAttentionTime;
        this.activeTime = activeTime;

        this.nrObjects = nrObjects;
        this.timeObjects = timeObjects;

        this.nrInventory = nrInventory;
        this.timeInventory = timeInventory;

        this.nrInstructions = nrInstructions;
        this.timeInstructions = timeInstructions;

        this.nrNotebook = nrNotebook;
        this.timeNotebook = timeNotebook;

        this.nrNoti = nrNoti;
        this.timeNoti = timeNoti;

        this.FirstPuzzleStart = firstPstart;
        this.NumberOfLeversInteracted = leversInteracted;
        this.FirstPuzzleEnd = firstPend;

        this.SecondPuzzleStart = secondPstart;
        this.NumberOfSPsInteracted = SPsInteracted;
        this.timeOrbSelection = timeOrbSelection;
        this.SecondPuzzleEnd = secondPend;

        this.MoveInteractInterfaceNoti = moveInteractInterfaceNoti;
        this.InteractInterfaceNoti = interactInterfaceNoti;
        this.reactionTimes = reactionTimes;
        this.eventsLog = eventsLog;


        if (timesSent == 0)
        {
            StartCoroutine(Post(codeName, gameplayTime, totalAttentionTime, activeTime,
                      nrObjects, timeObjects,
                      nrInventory, timeInventory,
                      nrInstructions, timeInstructions,
                      nrNotebook, timeNotebook,
                      nrNoti, timeNoti,
                      firstPstart, leversInteracted, firstPend,
                      secondPstart, SPsInteracted, timeOrbSelection, secondPend,
                      moveInteractInterfaceNoti, interactInterfaceNoti, reactionTimes, eventsLog, AfterForm));
        }

    }

  




}
