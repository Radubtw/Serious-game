using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class playerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public Transform orientation;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        MyInput();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        // Preluăm input-ul pentru mișcare pe axele orizontale și verticale
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MovePlayer()
    {
        // Calculăm direcția mișcării pe baza input-ului utilizatorului
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Normalizăm direcția mișcării pentru a asigura că viteza rămâne constantă, indiferent de direcție
        if (moveDirection.magnitude > 0)
        {
            moveDirection.Normalize();
        }

        // Aplicăm forța pentru mișcare constantă
        rb.linearVelocity = moveDirection * moveSpeed;
    }
}
