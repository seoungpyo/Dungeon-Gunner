using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    #region Header OBJECT REFERENCES
    [Space(10)]
    [Header("OBJECT REPERENCES")]
    #endregion Header OBJECT REFERENCES
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject highScoresButton;
    [SerializeField] private GameObject returnToMainMenuButton;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject instructionsButton;
    private bool isHighScoresSceneLoaded = false;
    private bool isInstructionsSceneLoaded = false;

    private void Start()
    {
        MusicManager.Instance.PlayMusic(GameResources.Instance.mainMenuMusic, 0f, 2f);

        SceneManager.LoadScene("CharacterSelectorScene", LoadSceneMode.Additive);

        returnToMainMenuButton.SetActive(false);
    }

    /// <summary>
    /// Call from the high scores button
    /// </summary>
    public void LoadHighScores()
    {
        playButton.SetActive(false);
        quitButton.SetActive(false);
        highScoresButton.SetActive(false);
        instructionsButton.SetActive(false);
        isHighScoresSceneLoaded = true;

        SceneManager.UnloadSceneAsync("CharacterSelectorScene");

        returnToMainMenuButton.SetActive(true);

        SceneManager.LoadScene("HighScoreScene", LoadSceneMode.Additive);
    }

    public void PlayGame()
    {
        // call from the play game / enter the dungeon button
        SceneManager.LoadScene("MainGameScene");
    }

    /// <summary>
    /// Called from the return to main menu button
    /// </summary>
    public void LoadCharacterSelector()
    {
        returnToMainMenuButton.SetActive(false);

        if (isHighScoresSceneLoaded)
        {
            SceneManager.UnloadSceneAsync("HighScoreScene");
            isHighScoresSceneLoaded = false;
        }
        else if (isInstructionsSceneLoaded)
        {
            SceneManager.UnloadSceneAsync("InstructionScene");
            isInstructionsSceneLoaded = false;
        }


        playButton.SetActive(true);
        highScoresButton.SetActive(true);
        quitButton.SetActive(true);
        instructionsButton.SetActive(true);

        SceneManager.LoadScene("CharacterSelectorScene", LoadSceneMode.Additive);
    }

    /// <summary>
    /// Called from the instructions button
    /// </summary>
    public void LoadInstructions()
    {
        playButton.SetActive(false);
        quitButton.SetActive(false);
        highScoresButton.SetActive(false);
        instructionsButton.SetActive(false);
        isInstructionsSceneLoaded = true;

        SceneManager.UnloadSceneAsync("CharacterSelectorScene");

        returnToMainMenuButton.SetActive(true);

        SceneManager.LoadScene("InstructionScene", LoadSceneMode.Additive);
    }

    /// <summary>
    /// Quit the game - this method is called from the onclick event set in the inspector
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilitie.ValidateCheckNullValue(this, nameof(playButton), playButton);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(highScoresButton), highScoresButton);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(returnToMainMenuButton), returnToMainMenuButton);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(quitButton), quitButton);
        HelperUtilitie.ValidateCheckNullValue(this, nameof(instructionsButton), instructionsButton);
    }
#endif
    #endregion Validation
}
