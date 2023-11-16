using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SettingsMenu : MonoBehaviour
{
    public GameObject SettingsMenuObject; // assign in inspector
    public Slider MouseSensitivitySlider; // assign in inspector
    public Slider DespawnTimerSlider; // assign in inspector
    public TMP_Dropdown DifficultyDropdown; // assign in inspector
    public Button BackButton; // assign in inspector
    public GameObject MenuObject;
    public Toggle unlockAllPerks;

    private const string MouseSensitivityKey = "MouseSensitivity";
    private const string DespawnTimerKey = "DespawnTimer";
    private const string DifficultyKey = "Difficulty";


    public TMP_Text text;

    public float difficultyCoefficient;

    // These could be stored somewhere else, like a GameSettings object
    public float despawnTimer = 15f;
    public float mouseSensitivity = 300f; // Default to 0.5
    public string difficulty = "PLAYABLE"; // Default to Medium

    public List<string> textList;
    private bool isMouseOverDropdown = false;

    void Start()
    {

        LoadSettings();

        unlockAllPerks.onValueChanged.AddListener(GameManager.Instance.SetKatanaAcquired);
        unlockAllPerks.onValueChanged.AddListener(GameManager.Instance.SetDashAcquired);
        unlockAllPerks.onValueChanged.AddListener(GameManager.Instance.SetDoubleJumpAcquired);
        unlockAllPerks.onValueChanged.AddListener(GameManager.Instance.SetKickAcquired);
        unlockAllPerks.onValueChanged.AddListener(GameManager.Instance.SetNukeAcquired);
        unlockAllPerks.onValueChanged.AddListener(GameManager.Instance.SetWallJumpAcquired);
        BackButton.onClick.AddListener(GoBack);

        text.enabled = false;

        // Set up the mouse sensitivity slider
        MouseSensitivitySlider.value = mouseSensitivity;
        MouseSensitivitySlider.onValueChanged.AddListener(SetMouseSensitivity);

        DespawnTimerSlider.value = despawnTimer;
        DespawnTimerSlider.onValueChanged.AddListener(SetDespawnTimer);
        // Set up the difficulty dropdown
        DifficultyDropdown.value = GetDifficultyIndex(difficulty);
        DifficultyDropdown.onValueChanged.AddListener(SetDifficulty);
        difficultyCoefficient = 1f;
        
        SettingsMenuObject.SetActive(false); // Make sure settings menu is not showing at the start
    }
    void SetDespawnTimer(float value)
    {
        despawnTimer = value;
        SaveSettings();

        // Here, you should update the mouse sensitivity in your input system
        // This will depend on how you have set up your input
    }
    void SetMouseSensitivity(float value)
    {
        mouseSensitivity = value;
        SaveSettings();

        // Here, you should update the mouse sensitivity in your input system
        // This will depend on how you have set up your input
    }

    void SetDifficulty(int index)
    {
        difficulty = DifficultyDropdown.options[index].text;
        SaveSettings();

        StartCoroutine(BroadcastAll());
        GameSettings.isASissy = true;
        //BroadcastAll("DifficultyChange", difficultyCoefficient);
        
        // Here, you should update the difficulty in your game
        // This will depend on how you have set up your game
    }
    IEnumerator BroadcastAll()
    {
        

        yield return new WaitForSecondsRealtime(0.05f);
        GameObject[] gos = GameObject.FindGameObjectsWithTag("EnemyHolder");

        foreach (GameObject go in gos)
        {
            
            go.gameObject.GetComponent<EnemyStats>().DifficultyChange(difficultyCoefficient);
            
        }
    }
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

    void GoBack()
    {
        // Implement your go back logic here
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
        //Debug.Log(difficultyCoefficient);
        //Debug.Log(GameSettings.despawnTimer);
        //Debug.Log(difficulty + "+" + difficultyCoefficient );
        

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
        return RectTransformUtility.RectangleContainsScreenPoint(
            DifficultyDropdown.GetComponent<RectTransform>(),
            Input.mousePosition,
            null);
    }
    private string GetRandomText()
    {
        if (textList.Count > 0)
        {
            int randomIndex = Random.Range(0, textList.Count);
            return textList[randomIndex];
        }
        else
        {
            return "";
        }
    }

    // New function to save settings to PlayerPrefs
    private void SaveSettings()
    {
        PlayerPrefs.SetFloat(MouseSensitivityKey, mouseSensitivity);
        PlayerPrefs.SetFloat(DespawnTimerKey, despawnTimer);
        PlayerPrefs.SetString(DifficultyKey, difficulty);
        PlayerPrefs.Save();
    }

    // New function to load settings from PlayerPrefs
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
