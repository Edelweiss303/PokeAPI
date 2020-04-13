using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PokedexController : Singleton<PokedexController>
{
    [System.Serializable]
    public class Sprites
    {
        public string back_default;
        public string back_female;
        public string back_shiny;
        public string back_shiny_female;
        public string front_default;
        public string front_female;
        public string front_shiny;
        public string front_shiny_female;
    }

    [System.Serializable]
    public class PokemonData
    {
        public string name;
        public Sprites sprites;
    }

    public Text pokemonNameText;
    public Image pokemonSpriteDisplay;
    public InputField pokemonNumberInputField;
    public Button showPokemonButton;

    public PokemonData pokemon;

    private void Start()
    {
        showPokemonButton.interactable = false;
    }

    private void Update()
    {
        if (string.IsNullOrEmpty(pokemonNumberInputField.text))
        {
            showPokemonButton.interactable = false;
        }
        else
        {
            showPokemonButton.interactable = true;
        }
    }
    public void GetPokemon()
    {
        if (pokemonNumberInputField.text != string.Empty)
        {
            int num = int.Parse(pokemonNumberInputField.text);


            string url = string.Format("https://pokeapi.co/api/v2/pokemon/{0}/", num);
            HttpWebRequest request = WebRequest.CreateHttp(url);
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonResponse = reader.ReadToEnd();

            reader.Close();
            reader.Dispose();

            pokemon = JsonUtility.FromJson<PokemonData>(jsonResponse);
            StartCoroutine(LoadPokemon());
        }
    }

    private IEnumerator LoadPokemon()
    {
        if(pokemon.name != null)
        {
            pokemonNameText.text = pokemon.name;
        }

        if(pokemon.sprites != null)
        {
            UnityWebRequest request = UnityWebRequest.Get(pokemon.sprites.front_default);
            yield return request.SendWebRequest();

            if(request.isNetworkError || request.isHttpError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                byte[] spriteBytes = request.downloadHandler.data;
                Texture2D pokemonTexture = new Texture2D(2,2);
                pokemonTexture.LoadImage(spriteBytes);
                Rect spriteRect = new Rect(0, 0, pokemonTexture.width, pokemonTexture.height);
                pokemonSpriteDisplay.sprite = Sprite.Create(
                    pokemonTexture,
                    spriteRect,
                    new Vector2(0.5f, 0.5f),
                    100.0f);

              
            }
        }
    }
}
