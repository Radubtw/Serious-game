using UnityEngine;
using UnityEngine.UI;

public class RecycleBin : MonoBehaviour
{
    [Header("Bin Properties")]
    public TrashType binType;
    public string binName;
    public Color binColor = Color.white;
    
    [Header("UI")]
    public GameObject binInfoUI;
    public Text binTypeText;
    
    [Header("Effects")]
    public GameObject correctEffect;
    public GameObject wrongEffect;
    public AudioClip correctSound;
    public AudioClip wrongSound;
    public float shakeIntensity = 0.1f;
    
    [Header("Visuals")]
    public Material defaultMaterial;
    public Material highlightMaterial;
    
    private AudioSource audioSource;
    private Renderer rend;
    // Variabilă declarată dar neutilizată - am adăugat comentariu pentru a clarifica utilizarea viitoare
    private bool playerInRange = false; // Folosit pentru a verifica dacă jucătorul este în raza de acțiune
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
            
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            defaultMaterial = rend.material;
            
            // Set bin color based on type
            if (defaultMaterial != null)
            {
                switch (binType)
                {
                    case TrashType.Plastic: binColor = new Color(1f, 0.7f, 0f); break; // Galben
                    case TrashType.Glass: binColor = new Color(0f, 0.7f, 0f); break; // Verde
                    case TrashType.Paper: binColor = new Color(0f, 0f, 1f); break; // Albastru
                    case TrashType.Metal: binColor = new Color(0.7f, 0.7f, 0.7f); break; // Gri
                    case TrashType.Organic: binColor = new Color(0.5f, 0.3f, 0.1f); break; // Maro
                }
                
                defaultMaterial.color = binColor;
            }
        }
        
        // Set bin type text
        if (binTypeText != null)
            binTypeText.text = binType.ToString();
            
        // Hide UI initially
        if (binInfoUI != null)
            binInfoUI.SetActive(false);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            HighlightBin(true);
            
            // Show UI
            if (binInfoUI != null)
                binInfoUI.SetActive(true);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            HighlightBin(false);
            
            // Hide UI
            if (binInfoUI != null)
                binInfoUI.SetActive(false);
        }
    }
    
    public void HandleRecycling(TrashItem item, bool isCorrect)
    {
        // Play effects
        if (isCorrect)
        {
            if (correctEffect != null)
                Instantiate(correctEffect, transform.position + Vector3.up, Quaternion.identity);
                
            if (correctSound != null && audioSource != null)
                audioSource.PlayOneShot(correctSound);
                
            // Notify player with popup message
            UIManager uiManager = FindFirstObjectByType<UIManager>();
            if (uiManager != null)
                uiManager.ShowRecycleMessage("Corect! +" + item.pointValue + " puncte", Color.green);
        }
        else
        {
            if (wrongEffect != null)
                Instantiate(wrongEffect, transform.position + Vector3.up, Quaternion.identity);
                
            if (wrongSound != null && audioSource != null)
                audioSource.PlayOneShot(wrongSound);
                
            // Shake camera
            PlayerCam playerCam = FindFirstObjectByType<PlayerCam>();
            if (playerCam != null)
                playerCam.ShakeCamera(shakeIntensity, 0.3f);
                
            // Show message
            UIManager uiManager = FindFirstObjectByType<UIManager>();
            if (uiManager != null)
                uiManager.ShowRecycleMessage("Greșit! " + item.trashName + " merge la " + 
                                             item.trashType.ToString(), Color.red);
        }
    }
    
    private void HighlightBin(bool highlight)
    {
        if (rend != null && highlightMaterial != null)
        {
            rend.material = highlight ? highlightMaterial : defaultMaterial;
            
            if (highlight && highlightMaterial != null)
                highlightMaterial.color = binColor * 1.2f; // Aceeași culoare dar mai strălucitoare
        }
    }
    
    public string GetBinInfo()
    {
        return "Coș pentru " + binType.ToString();
    }
}