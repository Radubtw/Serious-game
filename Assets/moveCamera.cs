using UnityEngine;

public class moveCamera : MonoBehaviour
{
    public Transform cameraPosition;
    public float smoothSpeed = 10f;
    public float bobAmplitude = 0.05f;
    public float bobFrequency = 10f;
    
    private Vector3 targetPosition;
    private float defaultY;
    private float timer = 0f;
    private playerMovement movement;
    
    private void Start()
    {
        defaultY = cameraPosition.localPosition.y;
        
        // Corectarea erorii - înlocuirea FindObjectOfType cu FindFirstObjectByType
        movement = FindFirstObjectByType<playerMovement>();
    }
    
    private void Update()
    {
        // Calculate headbob when moving on ground
        Debug.Log("Camera Direction: " + Camera.main.transform.forward); // Verifică direcția camerei
        float bobOffset = 0f;
        if (movement != null && movement.grounded)
        {
            Vector3 velocity = movement.GetComponent<Rigidbody>().linearVelocity;
            float speed = new Vector3(velocity.x, 0, velocity.z).magnitude;
            
            if (speed > 0.1f)
            {
                // Apply head bob
                timer += Time.deltaTime * bobFrequency * (Input.GetKey(KeyCode.LeftShift) ? 1.5f : 1f);
                bobOffset = Mathf.Sin(timer) * bobAmplitude * speed / movement.moveSpeed;
            }
        }
        
        // Apply camera position with headbob
        targetPosition = new Vector3(
            cameraPosition.position.x, 
            cameraPosition.position.y + bobOffset, 
            cameraPosition.position.z
        );
        
        // Smooth camera movement
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
    }
    
    // Use this for camera effects during interactions
    public void CameraRecoil(float amount)
    {
        transform.localPosition -= Vector3.forward * amount;
    }
}