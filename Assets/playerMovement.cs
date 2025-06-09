using UnityEngine;
using TMPro;

public class playerMovement : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI plasticCounterText; // Text pentru plastic
    public TextMeshProUGUI metalCounterText;   // Text pentru metal
    public TextMeshProUGUI glassCounterText;   // Text pentru sticlă
    public GameObject crosshair;               // Referință la crosshair (UI Image)

    [Header("Movement Settings")]
    public float moveSpeed = 7f;
    public float sprintSpeed = 10f;
    public float walkSpeed = 7f;
    public float groundDrag = 5f;
    public float jumpForce = 12f;
    public float jumpCooldown = 0.25f;
    public float airMultiplier = 0.4f;
    
    [Header("Ground Check")]
    public float playerHeight = 2f;
    public LayerMask whatIsGround;
    public bool grounded;
    
    [Header("Interaction Settings")]
    public float pickupRange = 300f;  // Range-ul la care poate colecta obiectele
    public LayerMask trashLayer;    // Layer-ul obiectelor de deșeu (plastic, metal, sticlă)

    private int plasticCount = 0;
    private int metalCount = 0;
    private int glassCount = 0;

    private int plasticGoal = 10; // Obiectivul pentru plastic
    private int metalGoal = 10;   // Obiectivul pentru metal
    private int glassGoal = 10;   // Obiectivul pentru sticlă

    [Header("References")]
    public Transform orientation;

    private float horizontalInput;
    private float verticalInput;
    private bool jumpPressed;
    private bool sprinting;
    private bool readyToJump = true;
    
    private Vector3 moveDirection;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        UpdateUI(); // Asigură-te că UI-ul este actualizat la început
    }
    
    private void Update()
    {
        // Ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        
        MyInput();
        SpeedControl();
        
        // Apply drag when on ground
        if (grounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;

        // Colectează obiectul atunci când tasta `E` este apăsată
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Tasta E a fost apăsată!");
            PickupObject(); // Colectăm obiectul
        }

        // Debug Raycast pentru vizualizarea interacțiunii cu obiectele
        ShowRaycastDebug();
    }
    
    private void FixedUpdate()
    {
        MovePlayer();
        
        // Handle jump
        if (jumpPressed && readyToJump && grounded)
        {
            jumpPressed = false;
            Jump();
        }
    }
    
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        
        // Verificăm pentru salt
        if (Input.GetKeyDown(KeyCode.Space) && readyToJump && grounded)
        {
            jumpPressed = true;
        }
        
        // Verificăm pentru sprint
        sprinting = Input.GetKey(KeyCode.LeftShift);
        moveSpeed = sprinting ? sprintSpeed : walkSpeed;
    }
    
    private void MovePlayer()
    {
        // Calculăm direcția mișcării
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        
        // Aplicăm forța corespunzătoare pe sol și în aer
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }
    
    private void SpeedControl()
    {
        // Limităm viteza pe sol
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        
        // Limitează viteza dacă este nevoie
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }
    
    private void Jump()
    {
        // Resetăm viteza pe axa Y
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        
        // Începem cooldown pentru salt
        readyToJump = false;
        Invoke(nameof(ResetJump), jumpCooldown);
    }
    
    private void ResetJump()
    {
        readyToJump = true;
    }
    
    private void PickupObject()
    {
        RaycastHit hit;
        // Verificăm dacă raycast-ul lovește un obiect valid de deșeu
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, pickupRange, trashLayer))
        {
            Debug.Log("Raycast a lovit obiectul: " + hit.transform.name); // Debug: Ce obiect a lovit raycast-ul?

            // Dacă obiectul are scriptul TrashItem, înseamnă că este un obiect de deșeu
            TrashItem trashItem = hit.transform.GetComponent<TrashItem>();
            if (trashItem != null)
            {
                // Evidențiem obiectul
                trashItem.Highlight(true);

                // Dacă obiectul este de tip Metal, creștem counter-ul de metal
                if (trashItem.trashType == TrashType.Metal)
                {
                    metalCount++;
                    Debug.Log("Counter de metal: " + metalCount); // Debug: Verificăm valoarea counter-ului
                }

                // Actualizăm UI-ul
                UpdateUI();

                // Distrugem obiectul colectat de pe hartă
                trashItem.Collect(); // Folosim funcția Collect() din TrashItem
                Debug.Log("Obiectul a fost colectat și distrus."); // Debug: Confirmăm distrugerea obiectului
            }
        }
        else
        {
            Debug.Log("Raycast nu a lovit niciun obiect valid."); // Debug: Dacă raycast-ul nu lovește niciun obiect
        }
    }

    private void DropObject()
    {
        // Nu mai este necesar deoarece obiectul este distrus la colectare
    }

    private void UpdateUI()
    {
        // Actualizăm textul pentru fiecare tip de deșeu
        plasticCounterText.text = "Plastic: " + plasticCount + "/" + plasticGoal;
        metalCounterText.text = "Metal: " + metalCount + "/" + metalGoal;
        glassCounterText.text = "Glass: " + glassCount + "/" + glassGoal;
    }

    // Functie pentru a vizualiza Raycast-ul
    private void ShowRaycastDebug()
    {
        RaycastHit hit;
        // Dacă raycast-ul lovește un obiect valid de deșeu, desenăm o linie roșie pentru a-l vizualiza
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, pickupRange, trashLayer))
        {
            Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red); // Desenăm o linie roșie care arată unde lovește Raycast-ul
            Debug.Log("Raycast a lovit obiectul: " + hit.transform.name); // Afișăm în consola numele obiectului lovit
        }
        else
        {
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * pickupRange, Color.green); // Dacă nu lovește nimic, desenăm o linie verde
        }
    }
}
