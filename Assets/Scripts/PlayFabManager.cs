using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFabManager : Singleton<PlayFabManager>
{
    public enum LoginState
    {
        Startup,
        Instantiated,
        Success,
        Failed,
    }

    public LoginState state = LoginState.Startup;
    public string playerGUID = "";
    public bool createNewPlayer = false;

    private void Awake()
    {
        playerGUID = PlayerPrefs.GetString("PlayFabPlayerId", "");
        if (string.IsNullOrEmpty(playerGUID) || createNewPlayer == true)
        {
            createNewPlayer = false;
            playerGUID = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString("PlayFabPlayerId", playerGUID);
        }
    }

    private void Start()
    {
        state = LoginState.Instantiated;

        LoginWithCustomIDRequest request = new LoginWithCustomIDRequest { CustomId = playerGUID, CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request, onLoginSuccess, onLoginFailure);
    }

    private void onLoginSuccess(LoginResult result)
    {
        state = LoginState.Success;
        Debug.Log("Congratulations, you have logged into PlayFab!!");
    }

    private void onLoginFailure(PlayFabError error)
    {
        state = LoginState.Failed;
        Debug.LogWarning("Something went wront logging into PlayFab :(");
        Debug.LogError(error.GenerateErrorReport());
    }

}
