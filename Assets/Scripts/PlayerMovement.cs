using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 12f;
    public float wallSlideSpeed = 18f;
    public float gravity = -9.81f;
    public float slideGravity = -4.9f;
    public float jumpHeight = 3f;

    public float wallCheckDistance = 0.7f;
    public float wallJumpForce = 15f;
    public float wallJumpUpForce = 12f;
    public float wallJumpDecay = 6f;
    public bool wallJumpCooldown = false;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public LayerMask wallMask;

    public Transform playCam;
    public float mouseSensitivity = 100f;

    Vector3 velocity;
    Vector3 wallJumpMomentum;

    bool isGrounded;
    bool isWalling;
    Vector3 wallNormal;
    bool wallOnRight;

    float xRotation = 0f;

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded)
            wallJumpCooldown = false;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.right, out hit, wallCheckDistance, wallMask))
        {
            isWalling = true;
            wallNormal = hit.normal;
            wallOnRight = true;
        }
        else if (Physics.Raycast(transform.position, -transform.right, out hit, wallCheckDistance, wallMask))
        {
            isWalling = true;
            wallNormal = hit.normal;
            wallOnRight = false;
        }
        else
        {
            isWalling = false;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        float camTilt = 0f;
        if (isWalling && !isGrounded && velocity.y <= 0)
        {
            camTilt = wallOnRight ? 30f : -30f;
        }

        playCam.localRotation = Quaternion.Euler(xRotation, 0f, camTilt);
        transform.Rotate(Vector3.up * mouseX);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        float activeSpeed = (isWalling && !isGrounded && velocity.y <= 0) ? wallSlideSpeed : speed;
        controller.Move(move * activeSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            else if (isWalling && !wallJumpCooldown)
            {
                velocity.y = wallJumpUpForce;
                wallJumpMomentum = wallNormal * wallJumpForce;
                wallJumpCooldown = true;
            }
        }

        if (isWalling && !isGrounded && velocity.y <= 0)
        {
            velocity.y += slideGravity * Time.deltaTime;
            velocity.y = Mathf.Max(velocity.y, -wallSlideSpeed);
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            wallJumpMomentum = Vector3.zero;
        }

        wallJumpMomentum = Vector3.Lerp(wallJumpMomentum, Vector3.zero, wallJumpDecay * Time.deltaTime);

        Vector3 finalMove = wallJumpMomentum + Vector3.up * velocity.y;
        controller.Move(finalMove * Time.deltaTime);
    }
}