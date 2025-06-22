using UnityEngine;
using UnityEngine.UI;

public class UICounterManager : MonoBehaviour
{
    [Header("Collection UI Text Elements")]
    public Text plasticText;
    public Text aluminiumText;
    public Text glassText;
    
    [Header("Recycling Objectives UI Text Elements")]
    public Text plasticObjectiveText; // Pentru "Plastic: x/15"
    public Text aluminiumObjectiveText; // Pentru "Aluminiu: x/30"
    public Text glassObjectiveText; // Pentru "Hârtie: x/20"
    
    [Header("Game Over UI")]
    public Text gameOverText;
    public GameObject gameOverPanel;
    
    [Header("UI Panels to Hide on Game Over")]
    public GameObject objectivesPanel; // Panelul cu obiectivele
    public GameObject inventoryPanel; // Panelul cu inventarul
    
    [Header("Respawn Settings")]
    public Transform spawnPoint; // Punctul principal unde să se respawneze playerul
    public GameObject player; // Referința la player (opțional, se poate găsi automat)
    
    // Poziție inițială a playerului (backup dacă nu e setat spawn point)
    private Vector3 initialPlayerPosition;
    private Quaternion initialPlayerRotation;
    private bool hasInitialPosition = false;

    [Header("Collection Target Values")]
    public int plasticTarget = 10;
    public int aluminiumTarget = 10;
    public int glassTarget = 10;

    [Header("Recycling Objective Values")]
    public int plasticRecyclingObjective = 15; // Câte plastic trebuie reciclate
    public int aluminiumRecyclingObjective = 30; // Câte aluminiu trebuie reciclate (game over)
    public int glassRecyclingObjective = 20; // Câte sticlă trebuie reciclate

    // Current collection counts (în inventar)
    private int plasticCount = 0;
    private int aluminiumCount = 0;
    private int glassCount = 0;
    
    // Current recycling counts (reciclate în containere)
    private int plasticRecycledCount = 0;
    private int aluminiumRecycledCount = 0;
    private int glassRecycledCount = 0;
    
    private bool gameOver = false;

    // Singleton instance
    public static UICounterManager Instance;

    void Awake()
    {
        // Singleton pattern pentru acces ușor din alte scripturi
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateUI();
        
        // Ascunde mesajele de game over la început
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
            
        // Găsește playerul automat dacă nu e setat
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                // Încearcă să găsească un obiect cu nume care conține "Player"
                GameObject[] allObjects = FindObjectsOfType<GameObject>();
                foreach (GameObject obj in allObjects)
                {
                    if (obj.name.ToLower().Contains("player"))
                    {
                        player = obj;
                        break;
                    }
                }
            }
        }
    }

    void Update()
    {
        // Verifică dacă se apasă tasta R pentru restart
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    // Funcții pentru colectare (adăugare în inventar)
    public void AddPlastic()
    {
        if (!gameOver)
        {
            plasticCount++;
            UpdateUI();
            Debug.Log("Plastic colectat: " + plasticCount + "/" + plasticTarget);
        }
    }

    public void AddAluminium()
    {
        if (!gameOver)
        {
            aluminiumCount++;
            UpdateUI();
            Debug.Log("Aluminiu colectat: " + aluminiumCount + "/" + aluminiumTarget);
        }
    }

    public void AddGlass()
    {
        if (!gameOver)
        {
            glassCount++;
            UpdateUI();
            Debug.Log("Hârtie colectată: " + glassCount + "/" + glassTarget);
        }
    }

    // Funcții pentru reciclare (golire în containere)
    public void EmptyPlasticToRecycling()
    {
        if (!gameOver && plasticCount > 0)
        {
            plasticRecycledCount += plasticCount;
            Debug.Log($"Adăugat {plasticCount} plastic la reciclare. Total reciclat: {plasticRecycledCount}");
            plasticCount = 0;
            CheckObjectives();
            UpdateUI();
        }
    }

    public void EmptyAluminiumToRecycling()
    {
        if (!gameOver && aluminiumCount > 0)
        {
            aluminiumRecycledCount += aluminiumCount;
            Debug.Log($"Adăugat {aluminiumCount} aluminiu la reciclare. Total reciclat: {aluminiumRecycledCount}");
            aluminiumCount = 0;
            CheckObjectives(); // Folosește aceeași verificare ca toate celelalte
            UpdateUI();
        }
    }

    public void EmptyGlassToRecycling()
    {
        if (!gameOver && glassCount > 0)
        {
            glassRecycledCount += glassCount;
            Debug.Log($"Adăugat {glassCount} sticlă la reciclare. Total reciclat: {glassRecycledCount}");
            glassCount = 0;
            CheckObjectives();
            UpdateUI();
        }
    }

    // Verifică dacă obiectivele de reciclare sunt îndeplinite (TOATE, inclusiv aluminiu)
    void CheckObjectives()
    {
        if (IsAllRecyclingObjectivesComplete())
        {
            Debug.Log("Toate obiectivele îndeplinite! Game Over!");
            ShowGameOverScreen();
        }
    }
    
    // Afișează ecranul de game over
    void ShowGameOverScreen()
    {
        gameOver = true;
        Debug.Log("GAME OVER! Toate obiectivele au fost îndeplinite!");
        
        // Afișează game over screen-ul
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(true);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        
        // Ascunde panelurile pentru obiective și inventar
        if (objectivesPanel != null)
            objectivesPanel.SetActive(false);
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
        
        // Ascunde textele individuale (ca backup)
        if (plasticText != null)
            plasticText.gameObject.SetActive(false);
        if (aluminiumText != null)
            aluminiumText.gameObject.SetActive(false);
        if (glassText != null)
            glassText.gameObject.SetActive(false);
        if (plasticObjectiveText != null)
            plasticObjectiveText.gameObject.SetActive(false);
        if (aluminiumObjectiveText != null)
            aluminiumObjectiveText.gameObject.SetActive(false);
        if (glassObjectiveText != null)
            glassObjectiveText.gameObject.SetActive(false);
        
        // Oprește timpul (freeze ecranul)
        Time.timeScale = 0f;
    }

    void UpdateUI()
    {
        // Actualizează UI-ul pentru colectare (inventar)
        if (plasticText != null)
            plasticText.text = "Plastic: " + plasticCount + "/" + plasticTarget;
        
        if (aluminiumText != null)
            aluminiumText.text = "Aluminiu: " + aluminiumCount + "/" + aluminiumTarget;
        
        if (glassText != null)
            glassText.text = "Hârtie: " + glassCount + "/" + glassTarget;

        // Actualizează UI-ul pentru obiectivele de reciclare (NU afișa aluminiu)
        if (plasticObjectiveText != null)
            plasticObjectiveText.text = "Plastic: " + plasticRecycledCount + "/" + plasticRecyclingObjective;
        
        if (aluminiumObjectiveText != null)
            aluminiumObjectiveText.text = "Aluminiu: " + aluminiumRecycledCount + "/" + aluminiumRecyclingObjective; // Folosește obiectivul de reciclare, nu cel de colectare
        
        if (glassObjectiveText != null)
            glassObjectiveText.text = "Hârtie: " + glassRecycledCount + "/" + glassRecyclingObjective;
    }

    // Funcții pentru a obține valorile curente
    public int GetPlasticCount() { return plasticCount; }
    public int GetAluminiumCount() { return aluminiumCount; }
    public int GetGlassCount() { return glassCount; }
    
    public int GetPlasticRecycledCount() { return plasticRecycledCount; }
    public int GetAluminiumRecycledCount() { return aluminiumRecycledCount; }
    public int GetGlassRecycledCount() { return glassRecycledCount; }

    // Funcții pentru a verifica dacă obiectivele sunt atinse
    public bool IsPlasticComplete() { return plasticCount >= plasticTarget; }
    public bool IsAluminiumComplete() { return aluminiumCount >= aluminiumTarget; }
    public bool IsGlassComplete() { return glassCount >= glassTarget; }
    public bool AreAllObjectivesComplete() { return IsPlasticComplete() && IsAluminiumComplete() && IsGlassComplete(); }
    
    // Funcții pentru obiectivele de reciclare (nu include aluminiu în UI)
    public bool IsPlasticRecyclingComplete() { return plasticRecycledCount >= plasticRecyclingObjective; }
    public bool IsAluminiumRecyclingComplete() { return aluminiumRecycledCount >= aluminiumRecyclingObjective; }
    public bool IsGlassRecyclingComplete() { return glassRecycledCount >= glassRecyclingObjective; }
    public bool IsAllRecyclingObjectivesComplete() { return IsPlasticRecyclingComplete() && IsGlassRecyclingComplete() && IsAluminiumRecyclingComplete(); } // Include aluminiu pentru game over
    
    public bool IsGameOver() { return gameOver; }

    // Funcție pentru restart
    public void RestartGame()
    {
        Debug.Log("Restart cu tasta R apăsat!");
        
        Time.timeScale = 1f;
        plasticCount = 0;
        aluminiumCount = 0;
        glassCount = 0;
        plasticRecycledCount = 0;
        aluminiumRecycledCount = 0;
        glassRecycledCount = 0;
        gameOver = false;
        
        // Ascunde game over screen
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        
        // Reafișează panelurile pentru obiective și inventar
        if (objectivesPanel != null)
            objectivesPanel.SetActive(true);
        if (inventoryPanel != null)
            inventoryPanel.SetActive(true);
        
        // Reafișează textele individuale
        if (plasticText != null)
            plasticText.gameObject.SetActive(true);
        if (aluminiumText != null)
            aluminiumText.gameObject.SetActive(true);
        if (glassText != null)
            glassText.gameObject.SetActive(true);
        if (plasticObjectiveText != null)
            plasticObjectiveText.gameObject.SetActive(true);
        if (aluminiumObjectiveText != null)
            aluminiumObjectiveText.gameObject.SetActive(true);
        if (glassObjectiveText != null)
            glassObjectiveText.gameObject.SetActive(true);
        
        // Respawnează playerul la punctul principal
        RespawnPlayer();
        
        UpdateUI();
    }
    
    // Funcție pentru respawn
    void RespawnPlayer()
    {
        // Încearcă să găsească playerul din nou dacă nu e setat
        if (player == null)
        {
            Debug.Log("Player nu e setat, încerc să îl găsesc...");
            
            // Caută după tag
            player = GameObject.FindGameObjectWithTag("Player");
            
            // Caută după Camera principală
            if (player == null)
            {
                Camera mainCam = Camera.main;
                if (mainCam != null)
                {
                    player = mainCam.gameObject;
                    Debug.Log("Player găsit prin Camera.main: " + player.name);
                }
            }
            
            // Caută după moveCamera script (din script-ul tău)
            if (player == null)
            {
                moveCamera moveCam = FindObjectOfType<moveCamera>();
                if (moveCam != null)
                {
                    player = moveCam.gameObject;
                    Debug.Log("Player găsit prin moveCamera script: " + player.name);
                }
            }
            
            // Caută prin nume
            if (player == null)
            {
                string[] possibleNames = {"Player", "FPSController", "Character", "FirstPersonController", "PlayerController"};
                foreach (string name in possibleNames)
                {
                    GameObject found = GameObject.Find(name);
                    if (found != null)
                    {
                        player = found;
                        Debug.Log("Player găsit prin nume: " + found.name);
                        break;
                    }
                }
            }
        }
        
        if (spawnPoint != null && player != null)
        {
            Debug.Log($"Respawnez playerul '{player.name}' la spawn point: {spawnPoint.position}");
            
            // Dezactivează CharacterController dacă există (previne probleme de teleportare)
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                player.transform.position = spawnPoint.position;
                player.transform.rotation = spawnPoint.rotation;
                cc.enabled = true;
            }
            else
            {
                // Dacă nu are CharacterController, setează direct
                player.transform.position = spawnPoint.position;
                player.transform.rotation = spawnPoint.rotation;
            }
            
            Debug.Log("Player respawnat cu succes la spawn point!");
        }
        else if (player != null && hasInitialPosition)
        {
            Debug.Log($"Spawn point nu e setat, folosesc poziția inițială: {initialPlayerPosition}");
            
            // Folosește poziția inițială ca backup
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                player.transform.position = initialPlayerPosition;
                player.transform.rotation = initialPlayerRotation;
                cc.enabled = true;
            }
            else
            {
                player.transform.position = initialPlayerPosition;
                player.transform.rotation = initialPlayerRotation;
            }
            
            Debug.Log("Player respawnat cu succes la poziția inițială!");
        }
        else if (spawnPoint == null)
        {
            Debug.LogWarning("Spawn point nu este setat în UICounterManager!");
        }
        else if (player == null)
        {
            Debug.LogWarning("Player nu a fost găsit! Verifică că obiectul player există în scenă.");
            
            // Afișează toate GameObjects-urile active pentru debugging
            Debug.Log("GameObjects active în scenă:");
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.activeInHierarchy)
                    Debug.Log("- " + obj.name);
            }
        }
    }
}