using UnityEngine;

public class moveCamera : MonoBehaviour
{
    public Transform cameraPosition;
    public Camera playerCamera; // Camera principală a jucătorului
    public float interactionRange = 5f; // Distanța maximă la care poți interacționa

    void Update()
    {
        // Mută holder-ul, dar menține camera corect aliniată
        transform.position = cameraPosition.position;

        // Dacă vrei să folosești raycasting, folosește camera principală
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionRange))
        {
            Debug.Log("Obiect hit: " + hit.collider.name);
            
            // Verifică dacă playerul apasă tasta E
            if (Input.GetKeyDown(KeyCode.E))
            {
                // Verifică tipul obiectului și actualizează contoarele
                string objectName = hit.collider.name;
                
                // CONTAINERE PENTRU RECICLARE
                // Container pentru plastic (Gunoi_1 sau alt nume)
                if (objectName == "Gunoi_1" || objectName == "Gunoi_1(Clone)" || 
                    objectName.Contains("PlasticContainer"))
                {
                    Debug.Log("Interacțiune cu containerul de plastic: " + objectName);
                    if (UICounterManager.Instance != null)
                    {
                        int plasticCount = UICounterManager.Instance.GetPlasticCount();
                        if (plasticCount > 0)
                        {
                            UICounterManager.Instance.EmptyPlasticToRecycling();
                            Debug.Log($"Am golit {plasticCount} obiecte de plastic în container!");
                        }
                        else
                        {
                            Debug.Log("Nu ai plastic de golit!");
                        }
                    }
                }
                // Container pentru aluminiu (Gunoi_2)
                else if (objectName == "Gunoi_2" || objectName == "Gunoi_2(Clone)" || 
                         objectName.Contains("AluminiumContainer"))
                {
                    Debug.Log("Interacțiune cu containerul de aluminiu: " + objectName);
                    if (UICounterManager.Instance != null)
                    {
                        int aluminiumCount = UICounterManager.Instance.GetAluminiumCount();
                        if (aluminiumCount > 0)
                        {
                            UICounterManager.Instance.EmptyAluminiumToRecycling();
                            Debug.Log($"Am golit {aluminiumCount} doze de aluminiu în container!");
                        }
                        else
                        {
                            Debug.Log("Nu ai doze de aluminiu de golit!");
                        }
                    }
                }
                // Container pentru sticlă/hârtie (Gunoi_3 sau alt nume)
                else if (objectName == "Gunoi_3" || objectName == "Gunoi_3(Clone)" || 
                         objectName.Contains("GlassContainer") || objectName.Contains("PaperContainer"))
                {
                    Debug.Log("Interacțiune cu containerul de sticlă/hârtie: " + objectName);
                    if (UICounterManager.Instance != null)
                    {
                        int glassCount = UICounterManager.Instance.GetGlassCount();
                        if (glassCount > 0)
                        {
                            UICounterManager.Instance.EmptyGlassToRecycling();
                            Debug.Log($"Am golit {glassCount} obiecte de sticlă/hârtie în container!");
                        }
                        else
                        {
                            Debug.Log("Nu ai sticlă/hârtie de golit!");
                        }
                    }
                }
                
                // OBIECTE PENTRU COLECTARE
                // Aluminiu (colectare)
                else if (objectName == "SM_Aluminum_Can_01_Opened" || 
                         objectName == "SM_Aluminum_Can_01_Opened(Clone)")
                {

                    if (UICounterManager.Instance != null)
                    {
                        if (UICounterManager.Instance.GetAluminiumCount() < UICounterManager.Instance.aluminiumTarget)
                        {
                            UICounterManager.Instance.AddAluminium();

                            Destroy(hit.collider.gameObject);
                        }
                    }
                }
                // Plastic (colectare)
                else if (objectName == "Sticla_Plastic" ||
                            objectName == "Sticla_Plastic(Clone)")
                {
                    Debug.Log("Distrug obiectul de plastic: " + objectName);
                    if (UICounterManager.Instance != null)
                    {
                        if (UICounterManager.Instance.GetPlasticCount() < UICounterManager.Instance.plasticTarget)
                        {
                            UICounterManager.Instance.AddPlastic();
                            Destroy(hit.collider.gameObject);
                        }
                    }
                }
                // Sticlă/Hârtie (colectare)
                else if (objectName == "Pahar_Hartie" ||
                            objectName == "Pahar_Hartie(Clone)")
                {
                    Debug.Log("Distrug obiectul de sticlă/hârtie: " + objectName);
                    if (UICounterManager.Instance != null)
                    {
                        if (UICounterManager.Instance.GetGlassCount() < UICounterManager.Instance.glassTarget)
                        {
                            UICounterManager.Instance.AddGlass();

                            Destroy(hit.collider.gameObject);
                        }
                    }
                }
            }
        }
    }
}