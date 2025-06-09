using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    [Header("Inventory Settings")]
    public int plasticSlots = 10;
    public int metalSlots = 10;
    public int glassSlots = 10;

    private int plasticCount = 0;
    private int metalCount = 0;
    private int glassCount = 0;

    [Header("UI Elements")]
    public GameObject inventoryUI; // Referință la UI-ul inventarului
    public UnityEngine.UI.Text plasticText;
    public UnityEngine.UI.Text metalText;
    public UnityEngine.UI.Text glassText;

    private void Awake()
    {
        // Singleton pentru inventar
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Ascunde inventarul la început
        inventoryUI.SetActive(false);
    }

    private void Update()
    {
        // Deschide sau închide inventarul când apăsăm 'i'
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
        }
    }

    public void AddItem(TrashType type)
    {
        switch (type)
        {
            case TrashType.Plastic:
                if (plasticCount < plasticSlots)
                    plasticCount++;
                break;
            case TrashType.Metal:
                if (metalCount < metalSlots)
                    metalCount++;
                break;
            case TrashType.Glass:
                if (glassCount < glassSlots)
                    glassCount++;
                break;
        }

        UpdateUI();
    }

    public void RemoveItem(TrashType type)
    {
        switch (type)
        {
            case TrashType.Plastic:
                if (plasticCount > 0)
                    plasticCount--;
                break;
            case TrashType.Metal:
                if (metalCount > 0)
                    metalCount--;
                break;
            case TrashType.Glass:
                if (glassCount > 0)
                    glassCount--;
                break;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        // Actualizează UI-ul inventarului
        if (plasticText != null)
            plasticText.text = "Plastic: " + plasticCount + "/" + plasticSlots;
        if (metalText != null)
            metalText.text = "Metal: " + metalCount + "/" + metalSlots;
        if (glassText != null)
            glassText.text = "Glass: " + glassCount + "/" + glassSlots;
    }
}
