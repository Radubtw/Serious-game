using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance;
    
    [Header("Game Settings")]
    public int totalScore = 0;
    public int currentLevel = 1;
    public int pointsToNextLevel = 100;
    
    [Header("Audio")]
    public AudioClip pointsSound;
    public AudioClip levelUpSound;
    public AudioClip backgroundMusic;
    
    // References to other managers
    private UIManager uiManager;
    private TrashSpawner trashSpawner;
    private AudioSource audioSource;
    
    // Lists for level unlocks
    private string[] levelUnlocks = new string[] {
        "",
        "Obiecte plastic de nivel 1",
        "Deșeuri organice și obiecte noi",
        "Materiale reciclabile avansate",
        "Obiecte speciale cu valoare mare"
    };
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Find references - corectarea erorilor cu FindFirstObjectByType
        uiManager = FindFirstObjectByType<UIManager>();
        trashSpawner = FindFirstObjectByType<TrashSpawner>();
        
        // Setup audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
            
        // Play background music
        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
        
        // Update UI
        if (uiManager != null)
            uiManager.UpdateUI();
    }
    
    // Add points when recycling correctly
    public void AddPoints(int points)
    {
        totalScore += points;
        
        // Check for level up
        if (totalScore >= currentLevel * pointsToNextLevel && points > 0)
        {
            LevelUp();
        }
        
        // Update UI
        if (uiManager != null)
            uiManager.UpdateUI();
        
        // Play sound effect
        if (points > 0 && pointsSound != null)
            audioSource.PlayOneShot(pointsSound, 0.7f);
    }
    
    // Level up logic
    private void LevelUp()
    {
        currentLevel++;
        
        // Play level up sound
        if (levelUpSound != null)
            audioSource.PlayOneShot(levelUpSound);
            
        // Show level up UI
        if (uiManager != null)
            uiManager.ShowLevelUp(currentLevel, GetUnlockTextForLevel(currentLevel));
            
        // Spawn more trash or new types
        if (trashSpawner != null)
        {
            // Verificăm dacă metoda există înainte de a o apela
            trashSpawner.SpawnTrashForLevel(currentLevel);
        }
    }
    
    // Get text description for new items in this level
    private string GetUnlockTextForLevel(int level)
    {
        if (level > 0 && level < levelUnlocks.Length)
            return levelUnlocks[level];
        else
            return "Nivel maxim atins!";
    }
    
    // Reset game (for restart)
    public void ResetGame()
    {
        totalScore = 0;
        currentLevel = 1;
        
        if (trashSpawner != null)
            trashSpawner.SpawnTrash();
            
        if (uiManager != null)
            uiManager.UpdateUI();
    }
    
    // Pause game
    public void PauseGame()
    {
        Time.timeScale = 0f;
    }
    
    // Resume game
    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}