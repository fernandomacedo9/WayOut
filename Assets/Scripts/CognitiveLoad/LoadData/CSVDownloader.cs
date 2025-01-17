﻿using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;
using UnityEngine.Networking;

public static class CSVDownloader
{
    private const string k_googleSheetDocID = "11x2D8Iy7ake6SU02CYwMt33wyh8ZqQvxgca87aSGvxs";
    private const string url = "https://docs.google.com/spreadsheets/d/" + k_googleSheetDocID + "/export?format=csv";

    internal static IEnumerator DownloadData(System.Action<string> onCompleted)
    {
        yield return new WaitForEndOfFrame();

        string downloadData = null;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(">>>> Download Error: " + webRequest.error);
            }
            else
            {
                Debug.Log("Download success");
//                Debug.Log("Data: " + webRequest.downloadHandler.text);
                downloadData = webRequest.downloadHandler.text;
            }
        }

        onCompleted(downloadData);
    }


}