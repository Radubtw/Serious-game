using UnityEngine;
using UnityEngine.UI; // Adăugat pentru componentele UI

public class UIManager : MonoBehaviour
{
    [Header("HUD Elements")]
    public Text scoreText;
    public Text levelText;
    public Image progressBar;
    public Text objectInfoText;
    public GameObject recycleMessagePanel;
    public Text recycleMessageText;
    
    [Header("Level UI")]
    public GameObject levelUpPanel;
    public Text levelUpText;
    public Text newTrashTypesText;
    
    [Header("Interaction UI")]
    public GameObject interactionPrompt;
    public Text interactionText;
    
    // Reference to game manager
    private GameManager gameManager;
    
    private void Start()
    {
        gameManager = GameManager.Instance;
        
        // Hide optional panels at start
        if (levelUpPanel != null)
            levelUpPanel.SetActive(false);
            
        if (recycleMessagePanel != null)
            recycleMessagePanel.SetActive(false);
            
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
            
        if (objectInfoText != null)
            objectInfoText.gameObject.SetActive(false);
            
        UpdateUI();
    }
    
    // Update main UI elements
    public void UpdateUI()
    {
        if (gameManager == null)
            gameManager = GameManager.Instance;
            
        if (gameManager != null)
        {
            // Update score
            if (scoreText != null)
                scoreText.text = "Score: " + gameManager.totalScore;
                
            // Update level
            if (levelText != null)
                levelText.text = "Level: " + gameManager.currentLevel;
                
            // Update progress bar
            if (progressBar != null)
            {
                int pointsInCurrentLevel = gameManager.totalScore - ((gameManager.currentLevel - 1) * gameManager.pointsToNextLevel);
                float progress = (float)pointsInCurrentLevel / gameManager.pointsToNextLevel;
                progressBar.fillAmount = progress;
            }
        }
    }
    
    // Show message when recycling
    public void ShowRecycleMessage(string message, Color textColor)
    {
        if (recycleMessagePanel != null && recycleMessageText != null)
        {
            recycleMessageText.text = message;
            recycleMessageText.color = textColor;
            
            // Show panel
            recycleMessagePanel.SetActive(true);
            
            // Hide after delay
            StartCoroutine(HidePanelAfterDelay(recycleMessagePanel, 2f));
        }
    }
    
    // Show level up panel
    public void ShowLevelUp(int level, string newItems)
    {
        if (levelUpPanel != null && levelUpText != null)
        {
            levelUpText.text = "Level " + level + " Atins!";
            
            if (newTrashTypesText != null)
                newTrashTypesText.text = "Deblocat: " + newItems;
                
            // Show panel
            levelUpPanel.SetActive(true);
            
            // Hide after delay
            StartCoroutine(HidePanelAfterDelay(levelUpPanel, 3f));
        }
    }
    
    // Show interaction prompt
    public void ShowInteractionPrompt(string message)
    {
        if (interactionPrompt != null && interactionText != null)
        {
            interactionText.text = message;
            interactionPrompt.SetActive(true);
        }
    }
    
    // Hide interaction prompt
    public void HideInteractionPrompt()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }
    
    // Show info about held object
    public void ShowObjectInfo(string info)
    {
        if (objectInfoText != null)
        {
            objectInfoText.text = info;
            objectInfoText.gameObject.SetActive(true);
        }
    }
    
    // Hide object info
    public void HideObjectInfo()
    {
        if (objectInfoText != null)
            objectInfoText.gameObject.SetActive(false);
    }
    
    // Helper coroutine to hide panels after delay
    private System.Collections.IEnumerator HidePanelAfterDelay(GameObject panel, float delay)
    {
        yield return new WaitForSeconds(delay);
        panel.SetActive(false);
    }
}