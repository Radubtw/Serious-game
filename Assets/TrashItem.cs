using UnityEngine;

public enum TrashType
{
    Plastic,
    Glass,
    Paper,
    Metal,
    Organic
}

public class TrashItem : MonoBehaviour
{
    public TrashType trashType;
    public int pointValue = 10;
    public string trashName;
    private Renderer rend;
    private Color originalColor;
    
    void Start()
    {
        // Salvăm culoarea originală a obiectului pentru a putea reveni la ea
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            originalColor = rend.material.color;
        }
    }

    // Opțional - efecte vizuale când obiectul este selectat (hovered)
    public void Highlight(bool isHighlighted)
    {
        if (rend != null)
        {
            if (isHighlighted)
            {
                // Schimbăm culoarea pentru a evidenția obiectul (de exemplu, o culoare galbenă)
                rend.material.color = Color.yellow;
            }
            else
            {
                // Revenim la culoarea originală
                rend.material.color = originalColor;
            }
        }
    }

    // Metoda apelată când obiectul este colectat
    public void Collect()
    {
        // Cod pentru colectare (aceasta va fi folosită în `playerMovement.cs`)
        Destroy(gameObject); // Obiectul dispare atunci când este colectat
    }
}
