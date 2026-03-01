using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 12f;
    public float wallSlideSpeed = 18f;
    public float wallSlideEngageVelocity = 0.5f;
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
    public LayerMask deathMask;
    public Transform playCam;
    public float mouseSensitivity = 100f;
    public AudioClip jumpSound;
    public AudioClip deathSound;
    public AudioMixerGroup MixerGroup;
    public DeathMenu deathMenu;

    private Vector3 velocity;
    private Vector3 wallJumpMomentum;
    private bool isGrounded;
    private bool isWalling;
    private Vector3 wallNormal;
    private bool wallOnRight;
    private float xRotation = 0f;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = MixerGroup;
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        // --- Ground Check (slope aware) ---
        isGrounded = controller.isGrounded;

        // Death Check
        if (Physics.CheckSphere(groundCheck.position, 1f, deathMask))
        {
            audioSource.PlayOneShot(deathSound);
            deathMenu.Show();
        }

        // --- Wall Check ---
        RaycastHit hit;
        Vector3[] directions = { transform.right, -transform.right, transform.forward, -transform.forward };
        isWalling = false;
        foreach (Vector3 dir in directions)
        {
            if (Physics.Raycast(transform.position, dir, out hit, wallCheckDistance, wallMask))
            {
                isWalling = true;
                wallNormal = hit.normal;
                wallOnRight = Vector3.Dot(transform.right, hit.normal) < 0;
                break;
            }
        }

        // --- Camera Rotation ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playCam.localRotation = Quaternion.Euler(xRotation, 0f,
            isWalling && !isGrounded && velocity.y <= wallSlideEngageVelocity ? (wallOnRight ? 30f : -30f) : 0f);
        transform.Rotate(Vector3.up * mouseX);

        // --- Player Movement Input ---
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        // Project movement along slope
        if (isGrounded)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out hit, groundDistance + 0.1f, groundMask))
            {
                move = Vector3.ProjectOnPlane(move, hit.normal);
            }
        }

        // Determine speed
        float activeSpeed = (isWalling && !isGrounded && velocity.y <= wallSlideEngageVelocity) ? wallSlideSpeed : speed;
        Vector3 horizontalMove = move * activeSpeed;

        // --- Jump Input ---
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                audioSource.PlayOneShot(jumpSound);
            }
            else if (isWalling && !wallJumpCooldown)
            {
                velocity.y = wallJumpUpForce;
                wallJumpMomentum = wallNormal * wallJumpForce;
                wallJumpCooldown = true;
                audioSource.PlayOneShot(jumpSound);
            }
        }

        // --- Wall Slide / Gravity ---
        if (isWalling && !isGrounded && velocity.y <= wallSlideEngageVelocity)
        {
            velocity.y += slideGravity * Time.deltaTime;
            velocity.y = Mathf.Max(velocity.y, -wallSlideSpeed);
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        // --- Reset on Ground ---
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // small downward force to keep grounded
            wallJumpMomentum = Vector3.zero;
            wallJumpCooldown = false;
        }

        // --- Decay Wall Jump Momentum ---
        wallJumpMomentum = Vector3.Lerp(wallJumpMomentum, Vector3.zero, wallJumpDecay * Time.deltaTime);

        // --- Final Move ---
        Vector3 finalMove = horizontalMove + wallJumpMomentum + Vector3.up * velocity.y;
        controller.Move(finalMove * Time.deltaTime);
    }
}