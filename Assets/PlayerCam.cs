using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [Header("Sensitivity")]
    public float sensX = 400f;
    public float sensY = 400f;

    [Header("References")]
    public Transform orientation;
    public Transform player; // Referința la jucător
    public Vector3 offset = new Vector3(0, 1.6f, -3f); // Offset pentru camera (ajustează după preferință)

    [Header("Camera Settings")]
    public float fovDefault = 80f;
    public float fovSprint = 85f;
    public float fovChangeSpeed = 10f;

    // Internal variables
    private float xRotation;
    private float yRotation;
    private Camera cam;
    private playerMovement movement;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cam = GetComponent<Camera>();
        movement = FindFirstObjectByType<playerMovement>();

        // Setează offset-ul inițial pentru camera
        offset = new Vector3(0, 1.6f, -3f); // Exemplu de offset
    }

    private void Update()
    {
        // Mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotește camera și orientarea jucătorului
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);

        // FOV schimbat la sprint
        if (movement != null)
        {
            float targetFOV = Input.GetKey(KeyCode.LeftShift) ? fovSprint : fovDefault;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * fovChangeSpeed);
        }

        // Actualizează poziția camerei pentru a urma jucătorul cu offset-ul
        transform.position = player.position + offset;
    }

    // Camera shake pentru evenimente (salt, etc.)
    public void ShakeCamera(float intensity, float duration)
    {
        StartCoroutine(CameraShakeCoroutine(intensity, duration));
    }

    private System.Collections.IEnumerator CameraShakeCoroutine(float intensity, float duration)
    {
        Vector3 originalPosition = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;

            transform.localPosition = new Vector3(x, y, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}
