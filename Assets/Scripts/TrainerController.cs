using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainerController : Singleton<TrainerController>
{
    public Text pokemonNameText;
    public InputField usernameInputField;
    public Button catchPokemonButton;

    private int score = -1;
    public class LeaderBoardData
    {
        public int pokemon_amount;
    };

    void Start()
    {
        catchPokemonButton.interactable = false;
    }

    private void Update()
    {
        if(string.IsNullOrEmpty(usernameInputField.text)
            || string.IsNullOrEmpty(pokemonNameText.text))
        {
            catchPokemonButton.interactable = false;
        }
        else
        {
            catchPokemonButton.interactable = true;
        }
    }
    public void CatchPokemon()
    {
        UpdateUsername();

        GetLeaderboardScore();

        score++;

        PostLeaderboardScore();
    }

    public void UpdateUsername()
    {
        PlayFabClientAPI.UpdateUserTitleDisplayName(
            new UpdateUserTitleDisplayNameRequest() { DisplayName = usernameInputField.text },
            result =>
            {
                Debug.Log("Success");
            },
            error =>
            {
                Debug.Log("Failed to update username");
            }
        );
    }

    public void PostLeaderboardScore()
    {

        PlayFabClientAPI.ExecuteCloudScript(
            new ExecuteCloudScriptRequest()
            {
                FunctionName = "updatePlayerStat",
                FunctionParameter = new { StatisticName = "pokemon_amount", Value = score }
            },
            result =>
            {
                if (result != null && result.FunctionResult != null)
                {
                    // Invoke a callback
                    // Do something
                    Debug.Log("Leaderboard update Success!");
                }
            },
            error =>
            {
                Debug.Log("Error updating leaderboard");
            }
        );
    }

    

    public void GetLeaderboardScore()
    {
        PlayFabClientAPI.ExecuteCloudScript(
            new ExecuteCloudScriptRequest()
            {
                FunctionName = "getPlayerStat",
            },
            result =>
            {
                if (result != null && result.FunctionResult != null)
                {
                    LeaderBoardData data = JsonUtility.FromJson<LeaderBoardData>(result.FunctionResult.ToString());
                    score = data.pokemon_amount;
                }
            },
            error =>
            {
                Debug.Log("Error getting leaderboard");
            }
        );
    }
}
