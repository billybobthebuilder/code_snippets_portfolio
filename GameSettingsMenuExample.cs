using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SettingsMenu : MonoBehaviour
{

    // UI elements created in Unity Editor

    public GameObject SettingsMenuObject; // Assign in inspector
    public Slider MouseSensitivitySlider; // Assign in inspector
    public Slider DespawnTimerSlider; // Assign in inspector
    public TMP_Dropdown DifficultyDropdown; // Assign in inspector
    public Button BackButton; // Assign in inspector
    public GameObject MenuObject; // Assign in inspector
    public Toggle unlockAllPerks; // Assign in inspector
    

    // String constants to be used for saving/loading methods

    private const string MouseSensitivityKey = "MouseSensitivity";
    private const string DespawnTimerKey = "DespawnTimer";
    private const string DifficultyKey = "Difficulty";


    
    // Float that will be used for altering Enemy/Player/Other 
    private float difficultyCoefficient = 1f;

    // Could be stored in the GameSettings script and object, kept here for readability purposes
    public float despawnTimer = 15f;
    public float mouseSensitivity = 300f; // Default to 300
    public string difficulty = "PLAYABLE"; // Default to PLAYABLE

    // Just a little funny bit of just showing different "taunting/teasing" text when hovering over difficulty button
    public List<string> textList; // Text list to be assigned in inspector
    public TMP_Text text; // Assign in inspector
    private bool isMouseOverDropdown = false;

    void Start()
    {
        // Load game settings
        LoadSettings();


        // These are to be used in coordination with a Player related script. It basically unlocks/locks all Player perks. Used predominantly as a debugging, testing or cheat tool.
        unlockAllPerks.onValueChanged.AddListener(GameManager.Instance.SetKatanaAcquired);
        unlockAllPerks.onValueChanged.AddListener(GameManager.Instance.SetDashAcquired);
        unlockAllPerks.onValueChanged.AddListener(GameManager.Instance.SetDoubleJumpAcquired);
        unlockAllPerks.onValueChanged.AddListener(GameManager.Instance.SetKickAcquired);
        unlockAllPerks.onValueChanged.AddListener(GameManager.Instance.SetNukeAcquired);
        unlockAllPerks.onValueChanged.AddListener(GameManager.Instance.SetWallJumpAcquired);


        BackButton.onClick.AddListener(GoBack);

        text.enabled = false;

        // Slider to control mouse sensitivity
        MouseSensitivitySlider.value = mouseSensitivity;
        MouseSensitivitySlider.onValueChanged.AddListener(SetMouseSensitivity);



        // Despawn timer of most entities, to reduce lag and such
        DespawnTimerSlider.value = despawnTimer;

        
        DespawnTimerSlider.onValueChanged.AddListener(SetDespawnTimer);

        // Dropdown for different difficulty settings
        DifficultyDropdown.value = GetDifficultyIndex(difficulty);
        DifficultyDropdown.onValueChanged.AddListener(SetDifficulty);
        //difficultyCoefficient = 1f;
        
        SettingsMenuObject.SetActive(false); // Ensuring the settings menu is not showing at the start
    }

    void SetDespawnTimer(float value)
    {
        despawnTimer = value;
        SaveSettings();

        
    }
    void SetMouseSensitivity(float value)
    {
        mouseSensitivity = value;
        SaveSettings();

        
    }

    void SetDifficulty(int index)
    {
        difficulty = DifficultyDropdown.options[index].text;
        SaveSettings();

        StartCoroutine(BroadcastAll());

        
         
        
        
    }

    
    // Coroutine available to be called to change Enemy settings, based on difficulty coefficient (i.e. HP, damage, etc.)
    IEnumerator BroadcastAll()
    {
        

        yield return new WaitForSecondsRealtime(0.05f);
        GameObject[] gos = GameObject.FindGameObjectsWithTag("EnemyHolder");

        foreach (GameObject go in gos)
        {
            
            go.gameObject.GetComponent<EnemyStats>().DifficultyChange(difficultyCoefficient);
            
        }
    }

    // Integer method used to return the difficulty string's list index
    int GetDifficultyIndex(string difficulty)
    {
        for (int i = 0; i < DifficultyDropdown.options.Count; i++)
        {
            if (DifficultyDropdown.options[i].text == difficulty)
            {
                return i;
            }
        }

        return 0; // Default to the first option if the difficulty wasn't found
    }


    // Just switching back to main menu
    void GoBack()       
    {
        
        SettingsMenuObject.SetActive(false);
        MenuObject.SetActive(true);
    }

    private void Update()
    {

        if (SettingsMenuObject.activeInHierarchy)
        {

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SettingsMenuObject.SetActive(false);
                MenuObject.SetActive(true);


            }

        }
        

        //Changes difficulty coefficient based on difficulty

        if (difficulty == "MOBILE GAMER")
            difficultyCoefficient = 0.1f;

        if (difficulty == "NPC")
            difficultyCoefficient = 0.5f;

        if (difficulty == "PLAYABLE")
            difficultyCoefficient = 1f;

        if (difficulty == "UBER")
            difficultyCoefficient = 2f;

        if (difficulty == "1:12")
            difficultyCoefficient = 5f;


        GameSettings.despawnTimer = despawnTimer;

        bool mouseOverDropdown = IsMouseOverDropdown();

        if (mouseOverDropdown && !isMouseOverDropdown)
        {
            // Mouse has just entered the dropdown
            text.enabled = true;
            text.text = GetRandomText();
        }
        else if (!mouseOverDropdown && isMouseOverDropdown)
        {
            // Mouse has just left the dropdown
            text.enabled = false;
        }

        // Update tracking variable
        isMouseOverDropdown = mouseOverDropdown;

    }

    private bool IsMouseOverDropdown()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(DifficultyDropdown.GetComponent<RectTransform>(), Input.mousePosition, null);
    }

    //Cheeky random "taunting/teasing" text bit method
    private string GetRandomText()
    {
        if (textList.Count > 0)
        {
            int randomIndex = Random.Range(0, textList.Count);
            return textList[randomIndex];
        }
        else
        {
            return "";  // Return nothing if null list count
        }
    }

    // Method to save settings to PlayerPrefs
    private void SaveSettings()
    {
        PlayerPrefs.SetFloat(MouseSensitivityKey, mouseSensitivity);
        PlayerPrefs.SetFloat(DespawnTimerKey, despawnTimer);
        PlayerPrefs.SetString(DifficultyKey, difficulty);
        PlayerPrefs.Save();
    }

    // Method to load settings from PlayerPrefs
    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey(MouseSensitivityKey))
        {
            mouseSensitivity = PlayerPrefs.GetFloat(MouseSensitivityKey);
            MouseSensitivitySlider.value = mouseSensitivity;
        }

        if (PlayerPrefs.HasKey(DespawnTimerKey))
        {
            despawnTimer = PlayerPrefs.GetFloat(DespawnTimerKey);
            DespawnTimerSlider.value = despawnTimer;
        }

        if (PlayerPrefs.HasKey(DifficultyKey))
        {
            difficulty = PlayerPrefs.GetString(DifficultyKey);
            DifficultyDropdown.value = GetDifficultyIndex(difficulty);
        }
    }

}
