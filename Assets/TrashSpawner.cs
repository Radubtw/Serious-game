using UnityEngine;
using System.Collections.Generic;

public class TrashSpawner : MonoBehaviour
{
    [Header("Trash Prefabs")]
    public List<GameObject> plasticTrash;
    public List<GameObject> glassTrash;
    public List<GameObject> paperTrash;
    public List<GameObject> metalTrash;
    public List<GameObject> organicTrash;
    
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
    [Range(0, 1)] public float organicPercentage = 0f;
    
    [Header("Level Progression")]
    public int additionalTrashPerLevel = 10;
    public List<GameObject> level2Unlocks; // Obiecte deblocate la nivelul 2
    public List<GameObject> level3Unlocks; // Obiecte deblocate la nivelul 3
    public List<GameObject> level4Unlocks; // Obiecte deblocate la nivelul 4
    
    private List<Vector3> spawnedPositions = new List<Vector3>();
    private List<GameObject> spawnedTrash = new List<GameObject>();
    
    void Start()
    {
        SpawnTrash();
    }
    
    // Am adăugat 'public' pentru a permite accesul din GameManager
    public void SpawnTrash()
    {
        ClearExistingTrash();
        spawnedPositions.Clear();
        
        int plasticCount = Mathf.RoundToInt(totalTrashCount * plasticPercentage);
        int glassCount = Mathf.RoundToInt(totalTrashCount * glassPercentage);
        int paperCount = Mathf.RoundToInt(totalTrashCount * paperPercentage);
        int metalCount = Mathf.RoundToInt(totalTrashCount * metalPercentage);
        int organicCount = Mathf.RoundToInt(totalTrashCount * organicPercentage);
        
        SpawnTrashOfType(plasticTrash, plasticCount, TrashType.Plastic);
        SpawnTrashOfType(glassTrash, glassCount, TrashType.Glass);
        SpawnTrashOfType(paperTrash, paperCount, TrashType.Paper);
        SpawnTrashOfType(metalTrash, metalCount, TrashType.Metal);
        SpawnTrashOfType(organicTrash, organicCount, TrashType.Organic);
    }
    
    // Am adăugat 'public' pentru a permite accesul din GameManager
    public void SpawnTrashForLevel(int level)
    {
        // Adaugă obiecte noi în funcție de nivel
        if (level == 2 && level2Unlocks.Count > 0)
        {
            foreach (GameObject obj in level2Unlocks)
            {
                TrashItem item = obj.GetComponent<TrashItem>();
                if (item != null)
                {
                    switch (item.trashType)
                    {
                        case TrashType.Plastic: plasticTrash.Add(obj); break;
                        case TrashType.Glass: glassTrash.Add(obj); break;
                        case TrashType.Paper: paperTrash.Add(obj); break;
                        case TrashType.Metal: metalTrash.Add(obj); break;
                        case TrashType.Organic: organicTrash.Add(obj); break;
                    }
                }
            }
            
            // Activează categoria Organic la nivelul 2
            if (organicPercentage == 0)
                organicPercentage = 0.1f;
        }
        else if (level == 3 && level3Unlocks.Count > 0)
        {
            foreach (GameObject obj in level3Unlocks)
            {
                TrashItem item = obj.GetComponent<TrashItem>();
                if (item != null)
                {
                    switch (item.trashType)
                    {
                        case TrashType.Plastic: plasticTrash.Add(obj); break;
                        case TrashType.Glass: glassTrash.Add(obj); break;
                        case TrashType.Paper: paperTrash.Add(obj); break;
                        case TrashType.Metal: metalTrash.Add(obj); break;
                        case TrashType.Organic: organicTrash.Add(obj); break;
                    }
                }
            }
            organicPercentage = 0.15f;
        }
        else if (level >= 4 && level4Unlocks.Count > 0)
        {
            foreach (GameObject obj in level4Unlocks)
            {
                TrashItem item = obj.GetComponent<TrashItem>();
                if (item != null)
                {
                    switch (item.trashType)
                    {
                        case TrashType.Plastic: plasticTrash.Add(obj); break;
                        case TrashType.Glass: glassTrash.Add(obj); break;
                        case TrashType.Paper: paperTrash.Add(obj); break;
                        case TrashType.Metal: metalTrash.Add(obj); break;
                        case TrashType.Organic: organicTrash.Add(obj); break;
                    }
                }
            }
            organicPercentage = 0.2f;
        }
        
        // Crește numărul de obiecte la fiecare nivel
        totalTrashCount += additionalTrashPerLevel;
        
        // Spawnează gunoaiele pentru noul nivel
        SpawnTrash();
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
                spawnedTrash.Add(trash);
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
    
    private void ClearExistingTrash()
    {
        foreach (GameObject trash in spawnedTrash)
        {
            if (trash != null)
                Destroy(trash);
        }
        
        spawnedTrash.Clear();
    }
    
    // Metodă de testare pentru dezvoltare
    public void RegenerateTrash()
    {
        SpawnTrash();
    }
}