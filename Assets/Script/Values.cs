using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Values : MonoBehaviour
{
    public Button valuesButton;
    public Button valuesNButton;
    public InputField valuesNInputField;
    public Text messageBoardText;
    public GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnValuesButtonClicked()
    {
        StartCoroutine(GetValues());
    }

    private IEnumerator GetValues()
    {
        if (string.IsNullOrEmpty(gameManager.Token))
        {
            throw new Exception("Error: You must log in to send requests.");
        }

        UnityWebRequest httpClient = new UnityWebRequest(gameManager.GetHttpServer() + "/api/Values", "GET");

        httpClient.SetRequestHeader("Authorization", "bearer " + gameManager.Token);
        httpClient.SetRequestHeader("Accept", "application/json");

        httpClient.downloadHandler = new DownloadHandlerBuffer();

        yield return httpClient.SendWebRequest();

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            Debug.Log(httpClient.error);
        }
        else
        {
            string jsonResponse = httpClient.downloadHandler.text;
            messageBoardText.text += "\nResponse: " + jsonResponse;
        }

        httpClient.Dispose();
    }

    public void OnValuesNButtonClicked()
    {
        StartCoroutine(GetValue());
    }

    private IEnumerator GetValue()
    {
        if (string.IsNullOrEmpty(gameManager.Token))
        {
            throw new Exception("Error: You must log in to send requests.");
        }

        int value;
        if (!int.TryParse(valuesNInputField.text, out value))
        {
            throw new Exception("Error: Input must be an integer number.");
        }

        UnityWebRequest httpClient = new UnityWebRequest(gameManager.GetHttpServer() + "/api/Values/" + value, "GET");

        httpClient.SetRequestHeader("Authorization", "bearer " + gameManager.Token);
        httpClient.SetRequestHeader("Accept", "application/json");

        httpClient.downloadHandler = new DownloadHandlerBuffer();

        yield return httpClient.SendWebRequest();

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            Debug.Log(httpClient.error);
        }
        else
        {
            string jsonResponse = httpClient.downloadHandler.text;
            messageBoardText.text += "\nResponse: " + jsonResponse;
        }

        httpClient.Dispose();
    }
}
