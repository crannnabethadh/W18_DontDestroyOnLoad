using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const string httpServer = "https://clickycrateswebapi.azurewebsites.net";
    //private const string httpServer = "https://localhost:44363";
    public string GetHttpServer()
    {
        return httpServer;
    }

    [SerializeField] private string _token;
    public string Token
    {
        get { return _token; }
        set { _token = value; }
    }

    [SerializeField] private string _playerId;
    public string PlayerId
    {
        get { return _playerId; }
        set { _playerId = value; }
    }

    private void Awake()
    {
        var gameManagers = FindObjectsOfType<GameManager>();
        if (gameManagers.Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
