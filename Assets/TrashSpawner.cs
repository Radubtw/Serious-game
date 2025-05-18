using UnityEngine;
using System.Collections.Generic;

public class TrashSpawner : MonoBehaviour
{
    [Header("Trash Prefabs")]
    public List<GameObject> plasticTrash;
    public List<GameObject> glassTrash;
    public List<GameObject> paperTrash;
    public List<GameObject> metalTrash;
    
    [Header("Spawn Settings")]
    public int totalTrashCount = 50;
    public float spawnAreaWidth = 50f;
    public float spawnAreaLength = 50f;
    public float minDistanceBetweenItems = 1.5f;
    public Transform spawnAreaCenter;
    public LayerMask groundLayer;
    
    [Header("Distribution")]
    [Range(0, 1)] public float plasticPercentage = 0.3f;
    [Range(0, 1)] public float glassPercentage = 0.25f;
    [Range(0, 1)] public float paperPercentage = 0.25f;
    [Range(0, 1)] public float metalPercentage = 0.2f;
    
    private List<Vector3> spawnedPositions = new List<Vector3>();
    
    void Start()
    {
        SpawnTrash();
    }
    
    void SpawnTrash()
    {
        int plasticCount = Mathf.RoundToInt(totalTrashCount * plasticPercentage);
        int glassCount = Mathf.RoundToInt(totalTrashCount * glassPercentage);
        int paperCount = Mathf.RoundToInt(totalTrashCount * paperPercentage);
        int metalCount = Mathf.RoundToInt(totalTrashCount * metalPercentage);
        
        SpawnTrashOfType(plasticTrash, plasticCount, TrashType.Plastic);
        SpawnTrashOfType(glassTrash, glassCount, TrashType.Glass);
        SpawnTrashOfType(paperTrash, paperCount, TrashType.Paper);
        SpawnTrashOfType(metalTrash, metalCount, TrashType.Metal);
    }
    
    void SpawnTrashOfType(List<GameObject> trashPrefabs, int count, TrashType type)
    {
        for (int i = 0; i < count; i++)
        {
            if (trashPrefabs.Count == 0) continue;
            
            GameObject prefab = trashPrefabs[Random.Range(0, trashPrefabs.Count)];
            Vector3 spawnPosition = GetValidSpawnPosition();
            
            if (spawnPosition != Vector3.zero)
            {
                GameObject trash = Instantiate(prefab, spawnPosition, Quaternion.Euler(0, Random.Range(0, 360), 0));
                
                // Asigurăm-ne că are scriptul TrashItem și setăm tipul
                TrashItem trashItem = trash.GetComponent<TrashItem>();
                if (trashItem == null)
                {
                    trashItem = trash.AddComponent<TrashItem>();
                }
                trashItem.trashType = type;
                
                spawnedPositions.Add(spawnPosition);
            }
        }
    }
    
    Vector3 GetValidSpawnPosition()
    {
        for (int attempts = 0; attempts < 30; attempts++)
        {
            // Generăm o poziție aleatorie în zona de spawn
            float x = Random.Range(-spawnAreaWidth/2, spawnAreaWidth/2);
            float z = Random.Range(-spawnAreaLength/2, spawnAreaLength/2);
            
            Vector3 position = spawnAreaCenter.position + new Vector3(x, 0, z);
            
            // Verificăm dacă poziția este pe pământ folosind raycast
            RaycastHit hit;
            if (Physics.Raycast(position + Vector3.up * 10, Vector3.down, out hit, 20f, groundLayer))
            {
                position.y = hit.point.y + 0.05f; // Puțin deasupra solului
                
                // Verificăm distanța față de alte obiecte
                bool isTooClose = false;
                foreach (Vector3 existingPos in spawnedPositions)
                {
                    if (Vector3.Distance(position, existingPos) < minDistanceBetweenItems)
                    {
                        isTooClose = true;
                        break;
                    }
                }
                
                if (!isTooClose)
                {
                    return position;
                }
            }
        }
        
        return Vector3.zero; // Dacă nu găsim o poziție validă după mai multe încercări
    }
}