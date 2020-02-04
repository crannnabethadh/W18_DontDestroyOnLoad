using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    // Cached references
    public InputField emailInputField;
    public InputField passwordInputField;
    public Button loginButton;
    public Button logoutButton;
    public Text messageBoardText;
    public GameManager gameManager;

    private string httpServerAddress;

    private void Start()
    {
        httpServerAddress = gameManager.GetHttpServer();
    }

    public void OnLoginButtonClicked()
    {
        TryLogin();
    }

    private void GetToken()
    {
        UnityWebRequest httpClient = new UnityWebRequest(httpServerAddress + "/Token", "POST");

        // application/x-www-form-urlencoded
        WWWForm dataToSend = new WWWForm();
        dataToSend.AddField("grant_type", "password");
        dataToSend.AddField("username", emailInputField.text);
        dataToSend.AddField("password", passwordInputField.text);

        httpClient.uploadHandler = new UploadHandlerRaw(dataToSend.data);
        httpClient.downloadHandler = new DownloadHandlerBuffer();

        httpClient.SetRequestHeader("Accept", "application/json");

        httpClient.SendWebRequest();

        while (!httpClient.isDone)
        {
            Task.Delay(1);
        }

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            Debug.Log(httpClient.error);
        }
        else
        {
            string jsonResponse = httpClient.downloadHandler.text;
            AuthorizationToken authToken = JsonUtility.FromJson<AuthorizationToken>(jsonResponse);
            gameManager.Token = authToken.access_token;
        }
        httpClient.Dispose();
    }

    private void TryLogin()
    {
        // The Content-Type header will be set to application/x-www-form-urlencoded by default.
        if (string.IsNullOrEmpty(gameManager.Token))
        {
            GetToken();  // Blocking
            // TODO: Show waiting spinner
        }

        UnityWebRequest httpClient = new UnityWebRequest(httpServerAddress + "/api/Account/UserId", "GET");
        
        httpClient.SetRequestHeader("Authorization", "bearer " + gameManager.Token);
        httpClient.SetRequestHeader("Accept", "application/json");

        httpClient.downloadHandler = new DownloadHandlerBuffer();

        httpClient.SendWebRequest();

        while (!httpClient.isDone)
        {
            Task.Delay(1);
        }

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            Debug.Log(httpClient.error);
        }
        else
        {
            gameManager.PlayerId = httpClient.downloadHandler.text;
            messageBoardText.text += "\nWelcome " + gameManager.PlayerId + ". You are logged in!";
            loginButton.interactable = false;
            logoutButton.interactable = true;
        }

        httpClient.Dispose();
    }

    public void OnLogoutButtonClicked()
    {
        TryLogout();
    }

    private void TryLogout()
    {
        // The Content-Type header will be set to application/x-www-form-urlencoded by default.
        UnityWebRequest httpClient = new UnityWebRequest(httpServerAddress + "/api/Account/Logout", "POST");

        httpClient.SetRequestHeader("Authorization", "bearer " + gameManager.Token);

        httpClient.SendWebRequest();  // Return control to LoginButtonClickedAPI since web request returns

        while (!httpClient.isDone)
        {
            Task.Delay(1);
        }

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            Debug.Log(httpClient.error);
        }
        else
        {
            messageBoardText.text += $"\n{httpClient.responseCode} Bye bye {gameManager.PlayerId}.";
            gameManager.Token = string.Empty;
            gameManager.PlayerId = string.Empty;
            loginButton.interactable = true;
            logoutButton.interactable = false;
        }
    }
}
