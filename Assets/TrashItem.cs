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
    
    // Opțional - efecte vizuale când obiectul este selectat
    public void Highlight(bool isHighlighted)
    {
        // Cod pentru efectul de evidențiere (outline, strălucire)
    }
    
    // Metoda apelată când obiectul este colectat
    public void Collect()
    {
        // Cod pentru colectare
    }
}