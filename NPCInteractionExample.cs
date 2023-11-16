using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;



public class NPCInteraction : MonoBehaviour
{

    // Assign in inspector where appropriate
    [TextArea(3, 10)]
    public List<string> dialogues; // List of dialogue options
    private int currentDialogueIndex = 0; // Index of the currently displayed dialogue
    private Coroutine currentDialogueCoroutine; // Coroutine for the current dialogue
    public List<string> textOfButton; 
    public string characterName; 
    
    public int numberOfDialogues; // Number of dialogue options
    
    // Unity UI objects
    public Button[] dialogueButtons; // Array of buttons to display dialogues
    private TMP_Text[] buttonTexts; // Array of texts for buttons
    public TMP_Text charName;

    // Static list of NPCs within player's range
    public static List<NPCInteraction> npcsInRange = new List<NPCInteraction>();

    // Static Dictionary for resetCoroutines. Decided to not use after all. Can be uncommented.
    public static Dictionary<NPCInteraction, Coroutine> resetCoroutines = new Dictionary<NPCInteraction, Coroutine>();

    

    private Coroutine resetSceneCoroutine; // Coroutine for resetting scene

    public Transform facePosition; // Point of reference for Player's camera to look at

    // Assign in inspector where appropriate
    [TextArea(3, 10)]
    //public string dialogue;
    //public string NPCname;
    public Camera mainCamera;
    public GameObject dialogueTextBox;
    public TMP_Text dialogueText;
    public float distanceToTalk = 3f; // Maximum distance to talk to NPC
    public float dialogueSpeed = 0.01f;
    public AudioClip letterSound; // Sound to be played every time a letter is displayed
    public RectTransform topBar;
    public RectTransform bottomBar;
    public float barSpeed = 2f;

    public GameObject pressE; // UI in-game 3d component that displays a press E option on top of the NPC model
    private Coroutine dialogueCoroutine;
    public float transitionTime = 1.5f; // Cinematic transition time
    private Coroutine showDialogueCoroutine;
    private Coroutine startDialogueCoroutine;

    public PlayerMovementTutorial playerMovement; // Old player movement script, can be uncommented

    public CPMPlayer cPMPlayer; // Player movement script


    public PlayerCam playerCam;
    public MoveCamera moveCamera;

    private EnemyStats enemyStats;

    private bool isPlayerNear = false;
    public bool isTalking = false;
    private AudioSource audioSource;
    private GameObject player;
    private Vector3 topBarInitialPosition;
    private Vector3 bottomBarInitialPosition;

    // Render pipeline stuff
    public Volume volume;
    private DepthOfField depthOfField;

    // Checks and arrays for UI purposes
    private GameObject[] foundObjects;
    public Weapon weapon;
    private bool dialogueCheckForMenu = false;
    private NPCBehaviour nPCBehaviour;
    public Button[] childButtons;
    private AcquisitionTrigger acquisitionTrigger;
    public Transform[] childTransforms;

    public bool acquisitionTriggerCheck = false;

    void Start()
    {

        foundObjects = GameObject.FindGameObjectsWithTag("playerCanvas1");

        

        // Render pipeline object tag search for that sweet cinematic effect on dialogue start
        volume = GameObject.FindGameObjectWithTag("Volume").GetComponent<Volume>();
        if (!volume.profile.TryGet(out depthOfField))
        {
            Debug.LogError("Failed to get the Depth of Field effect.");
        }


        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");


        // Script references, can remove for performance tuning as it is also found in the Awake() method
        playerMovement = player.GetComponent<PlayerMovementTutorial>();
        cPMPlayer = player.GetComponent<CPMPlayer>();
        playerCam = player.GetComponentInChildren<PlayerCam>();
        moveCamera = player.GetComponentInChildren<MoveCamera>();
        mainCamera = player.GetComponentInChildren<Camera>();

        // Cinematic black bars references
        topBar = GameObject.Find("topblackbar").GetComponent<RectTransform>();
        bottomBar = GameObject.Find("bottomblackbar").GetComponent<RectTransform>();

        
        dialogueTextBox.SetActive(false);

        pressE.SetActive(false);

        // Store the initial position of the cinematic bars
        topBarInitialPosition = topBar.position;
        bottomBarInitialPosition = bottomBar.position;
        
        

    }

    private void Awake()
    {

        acquisitionTrigger = GetComponent<AcquisitionTrigger>();

        dialogueTextBox = GameObject.Find("OnSceneLoad").GetComponent<OnSceneLoad>().dialogueTextBox; // Image object
        

        
        childButtons = GameObject.Find("OnSceneLoad").GetComponent<OnSceneLoad>().childButtons;

        // Assign the found buttons to the buttons array
        dialogueButtons = childButtons;

        // Initialize the buttonTexts array with the same length as buttons
        buttonTexts = new TMP_Text[dialogueButtons.Length];

        // Loop through each button and find its TMP_Text child
        for (int i = 0; i < dialogueButtons.Length; i++)
        {
            TMP_Text tmpText = dialogueButtons[i].GetComponentInChildren<TMP_Text>(true);

            // Assign the TMP_Text component to the corresponding index in buttonTexts
            buttonTexts[i] = tmpText;

            // Optionally can log the TMP_Text components found for each button, used for debugging
            if (tmpText != null)
            {
                //Debug.Log("Found TMP_Text for Button " + i + ": " + tmpText.text);
            }
            else
            {
                Debug.LogWarning("TMP_Text not found for Button " + i);
            }
        }


        // More references, done here in conjunction with Start() as sometimes Unity does not process Start() method early enough and fails to reference these objects. Can remove the part of the Start() method thats is identical to here for performance tuning, if desired
        player = GameObject.FindGameObjectWithTag("Player");
        nPCBehaviour = GetComponent<NPCBehaviour>();
        enemyStats = GetComponent<EnemyStats>();
        playerMovement = player.GetComponent<PlayerMovementTutorial>();
        cPMPlayer = player.GetComponent<CPMPlayer>();
        playerCam = player.GetComponentInChildren<PlayerCam>();
        moveCamera = player.GetComponentInChildren<MoveCamera>();
        mainCamera = player.GetComponentInChildren<Camera>();

        
        topBar = GameObject.Find("topblackbar").GetComponent<RectTransform>();
        bottomBar = GameObject.Find("bottomblackbar").GetComponent<RectTransform>();

        
        
        childTransforms = GameObject.Find("OnSceneLoad").GetComponent<OnSceneLoad>().childTransforms; // Include inactive GameObjects

        // Find all TMP_Text components with the DialogueText tag in all childTransforms. Unity doesn't have so many good ways of searching for child transforms' components
        foreach (Transform childTransform in childTransforms)
        {
            if (childTransform.CompareTag("DialogueText"))
            {
                dialogueText = childTransform.GetComponent<TMP_Text>();
                if (dialogueText != null)
                {
                    // Found the TMP_Text component with the "DialogueText" tag
                    break;
                }
            }
        }
        
        
        // Ditto, this time for CharName components
        foreach (Transform childTransform in childTransforms)
        {
            if (childTransform.CompareTag("CharName"))
            {
                charName = childTransform.GetComponent<TMP_Text>();
                if (charName != null)
                {
                    // Found the TMP_Text component with the "CharName" tag
                    break;
                }
            }
        }
        
    }


    void Update()
    {
        weapon = player.GetComponentInChildren<Weapon>();

        if (player == null)
        {
            Debug.LogError("Player is null");
            return;
        }
        if (player.transform == null)
        {
            Debug.LogError("Player transform is null");
            return;
        }
        if (transform == null)
        {
            Debug.LogError("NPC transform is null");
            return;
        }

        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance <= distanceToTalk)
        {
            if (!isPlayerNear && !enemyStats.dead)
            {
                pressE.SetActive(true);
                // When the Player enters range, add this NPC to the list
                npcsInRange.Add(this);
                isPlayerNear = true;
            }

            if (Input.GetKeyDown(KeyCode.T) && !isTalking && !nPCBehaviour.isAlerted) // The T key is used for initiating dialogues
            {
                // Only respond to the E key if this NPC is the closest one to the Player
                if (GetClosestNPC() == this)
                {
                    acquisitionTriggerCheck = true;
                    Time.timeScale = 0;
                    startDialogueCoroutine = StartCoroutine(StartDialogue());
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    // Disable all dialogue buttons initially
                    for (int i = 0; i < dialogueButtons.Length; i++)
                    {
                        dialogueButtons[i].gameObject.SetActive(false);
                    }

                    charName.text = characterName;
                    // Ensure that the number of dialogues matches the number of buttons
                    numberOfDialogues = dialogues.Count;

                    // Assign dialogues to buttons starting from index 1
                    for (int i = 1; i < numberOfDialogues; i++)
                    {
                        dialogueButtons[i - 1].gameObject.SetActive(true);
                        int index = i; // Capture the current index for the button's onClick event
                        dialogueButtons[i - 1].onClick.AddListener(() => StartNewDialogue(dialogues[index]));

                        // Set button text if available
                        if (i - 1 < buttonTexts.Length)
                        {
                            dialogueButtons[i - 1].GetComponentInChildren<TMP_Text>().text = textOfButton[i - 1];
                        }
                    }
                }
            }
            else if ((Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape)) && isTalking)
            {
                acquisitionTriggerCheck = false;
                Debug.Log("Attempting to reset scene for NPC: " + this.gameObject.name);
                Time.timeScale = 1;
                resetSceneCoroutine = StartCoroutine(ResetScene());
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                if (acquisitionTrigger != null)
                {
                    acquisitionTrigger.oneTimeTriggerCheck = false;
                }

                // Remove dialogues to buttons
                for (int i = 0; i < dialogueButtons.Length; i++)
                {
                    dialogueButtons[i].gameObject.SetActive(false);
                    int index = i; // Capture the current index for the button's onClick event
                    dialogueButtons[i].onClick.RemoveAllListeners();
                }
            }

            // This handles changing the dialogue options
            if (isTalking && dialogues.Count > 1)
            {
                for (int i = 0; i < numberOfDialogues; i++)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1 + i)) // Change to the corresponding dialogue option
                    {
                        currentDialogueIndex = i;
                        StartNewDialogue(dialogues[currentDialogueIndex]);
                    }
                }
            }
        }
        else
        {
            if (isPlayerNear || enemyStats.dead) // Checks for either Player leaving NPC range or the NPC is dead
            {
                if (GetComponent<AcquisitionTrigger>() != null && GetComponent<AcquisitionTrigger>().theText != null)
                {
                    GetComponent<AcquisitionTrigger>().theText.enabled = false; // This part is related to an AcquisitionTrigger Player script that handles skills, perks and special weapons the Player acquires when selecting the last dialogue option of certain specific NPCs
                }

                pressE.SetActive(false);

                // When the player leaves range, remove this NPC from the list
                npcsInRange.Remove(this);
                isPlayerNear = false;
            }
        }
    }





    // Method that returns the class object that is attached to the closest NPC, from the list of npcsInRange. Used for a check in Update() method. 
    private NPCInteraction GetClosestNPC()
    {
        return npcsInRange.OrderBy(npc => Vector3.Distance(player.transform.position, npc.transform.position)).FirstOrDefault();
    }

    void StartNewDialogue(string dialogue)
    {
        // Stop the current dialogue coroutine if it's running
        if (currentDialogueCoroutine != null)
        {
            StopCoroutine(currentDialogueCoroutine);
        }

        // Start a new coroutine for the chosen dialogue
        currentDialogueCoroutine = StartCoroutine(ShowDialogue(dialogue));
    }


    IEnumerator StartDialogue()
    {
        GameObject.Find("GamePauseMenu").GetComponent<MenuManager>().isInDialogue = true;


        Vector3 directionToPlayer = player.transform.position - transform.position;
        directionToPlayer.y = 0;
        Quaternion targetRotation2 = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = targetRotation2;
        pressE.SetActive(false);

        if (weapon != null)
            weapon.showCrosshair = false;

        foreach (GameObject go in foundObjects)
        {
            go.SetActive(false);
        }
        depthOfField.active = true;

        // Prepare initial and target values for the DepthOfField parameters
        float initialFocalLength = depthOfField.focalLength.value;
        float targetFocalLength = 1000f;  // Set target focal length
        float initialFocusDistance = depthOfField.focusDistance.value;
        float targetFocusDistance = 1f;  // Set target focus distance


        Time.timeScale = 0; // Freeze time

        // Stop resetting the scene when a new dialogue starts
        if (resetSceneCoroutine != null)
        {
            StopCoroutine(resetSceneCoroutine);
            resetSceneCoroutine = null;
        }

        // Cancel any ongoing dialogue
        if (startDialogueCoroutine != null)
        {
            StopCoroutine(startDialogueCoroutine);
            startDialogueCoroutine = null;
        }

        // When a conversation starts, set isPlayerNear to false for all other NPCs
        foreach (var npc in npcsInRange.Where(n => n != this))
        {
            npc.isPlayerNear = false;
            npc.pressE.SetActive(false);
        }

        isTalking = true;
        CPMPlayer cpmplayer = player.GetComponent<CPMPlayer>();
        cpmplayer.isTalking = true;

        Vector3 originalCameraRotation = mainCamera.transform.eulerAngles;
        Quaternion targetRotation = Quaternion.LookRotation(facePosition.position - mainCamera.transform.position);

        float startTime = Time.unscaledTime;
        float endTime = startTime + transitionTime;


        while (Time.unscaledTime < endTime) // Adjust for time freeze
        {
            float fractionCompleted = (Time.unscaledTime - startTime) / transitionTime;

            // Transition the DepthOfField parameters
            depthOfField.focalLength.value = Mathf.Lerp(initialFocalLength, targetFocalLength, fractionCompleted);
            depthOfField.focusDistance.value = Mathf.Lerp(initialFocusDistance, targetFocusDistance, fractionCompleted);


            topBar.position = Vector3.Lerp(topBarInitialPosition, topBarInitialPosition + new Vector3(0, -topBar.rect.height, 0), fractionCompleted);
            bottomBar.position = Vector3.Lerp(bottomBarInitialPosition, bottomBarInitialPosition + new Vector3(0, bottomBar.rect.height, 0), fractionCompleted);

            mainCamera.transform.rotation = Quaternion.Lerp(Quaternion.Euler(originalCameraRotation), targetRotation, fractionCompleted);


            yield return null;
        }

        depthOfField.focalLength.value = targetFocalLength;
        depthOfField.focusDistance.value = targetFocusDistance;

        topBar.position = topBarInitialPosition + new Vector3(0, -topBar.rect.height, 0);
        bottomBar.position = bottomBarInitialPosition + new Vector3(0, bottomBar.rect.height, 0);
        mainCamera.transform.rotation = targetRotation;

        dialogueTextBox.SetActive(true);

        if (showDialogueCoroutine != null)
        {
            StopCoroutine(showDialogueCoroutine);
        }

        StartNewDialogue(dialogues[currentDialogueIndex]);



        yield return showDialogueCoroutine;

        yield return new WaitForSecondsRealtime(30000); // Adjust for time freeze

        // Reset the isPlayerNear flag for all NPCs that are still in range
        foreach (var npc in npcsInRange)
        {
            npc.isPlayerNear = Vector3.Distance(player.transform.position, npc.transform.position) <= distanceToTalk;
            npc.pressE.SetActive(npc.isPlayerNear);
        }

        dialogueTextBox.SetActive(false);
        isTalking = false;
        
        dialogueText.text = "";
        
        Time.timeScale = 1; // Unfreeze time
        

    }






    IEnumerator ShowDialogue(string dialogue)
    {
        dialogueText.text = ""; // Naking sure dialogue text is reset at the start
        StringBuilder dialogueBuilder = new StringBuilder();

        foreach (char letter in dialogue.ToCharArray())
        {
            dialogueBuilder.Append(letter);
            dialogueText.text = dialogueBuilder.ToString();
            audioSource.PlayOneShot(letterSound);

            if (letter == '.' || letter == '?' || letter == '!')
            {
                yield return new WaitForSecondsRealtime(0.5f); // add delay after a period
            }
            if (letter == ',')
            {
                yield return new WaitForSecondsRealtime(0.25f); // add delay after a comma
            }

            yield return new WaitForSecondsRealtime(dialogueSpeed);
        }
    }

    IEnumerator ResetScene()
    {
        
        player.GetComponent<CPMPlayer>().isTalking = false;
        foreach (GameObject go in foundObjects)
        {
            go.SetActive(true);
        }
        if (weapon != null)
            weapon.showCrosshair = true;

        pressE.SetActive(true);

        depthOfField.active = false;
        if (startDialogueCoroutine != null)
        {
            StopCoroutine(startDialogueCoroutine);
            startDialogueCoroutine = null;
        }

        if (showDialogueCoroutine != null)
        {
            StopCoroutine(showDialogueCoroutine);
            showDialogueCoroutine = null;
        }
        
        if (currentDialogueCoroutine != null)
        {
            StopCoroutine(currentDialogueCoroutine);
            currentDialogueCoroutine = null;

        }

        if (isTalking)
        {
            audioSource.Stop();
            isTalking = false;
            dialogueText.text = "";
            dialogueTextBox.SetActive(false);
        }

        //Unfreeze time
        

        while (Vector2.Distance(topBar.position, topBarInitialPosition) > 0.01f ||
                Vector2.Distance(bottomBar.position, bottomBarInitialPosition) > 0.01f)
        {
            topBar.position = Vector3.Lerp(topBar.position, topBarInitialPosition, barSpeed * Time.deltaTime);
            bottomBar.position = Vector3.Lerp(bottomBar.position, bottomBarInitialPosition, barSpeed * Time.deltaTime);

            yield return null;
        }
        Time.timeScale = 1f;
        GameObject.Find("GamePauseMenu").GetComponent<MenuManager>().isInDialogue = false;

    }

}

