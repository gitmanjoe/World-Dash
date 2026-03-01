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
	float gravity = -9.81f;
	public float jumpHeight = 3f;
	public int health = 50;
	public Transform groundCheck;
	public Transform WallCheckleft;
	public Transform WallCheckright;
	public float groundDistance = 0.4f;
	public float WallDistance = 0.5f;
	public LayerMask groundMask;
	public LayerMask wallMask;
	public Transform playCam;

	Vector3 velocity;
	bool isGrounded;
	public bool isWalling;

    // Update is called once per frame
    void Update()
    {
		isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
		isWalling = Physics.CheckSphere(WallCheckleft.position, WallDistance, wallMask) || Physics.CheckSphere(WallCheckright.position, WallDistance, wallMask);
		if (isWalling && !isGrounded && !Input.GetButtonDown("Jump") && Input.GetAxis("Horizontal") == 0)
		{
			playCam.transform.localRotation = Quaternion.Euler(0, 0, playCam.transform.localRotation.z - 30f);
            Vector3 move = transform.forward * 1.1f;
			controller.Move(move * speed * Time.deltaTime);
        }
		else
		{
			if (isGrounded && velocity.y < 0)
			{
				velocity.y = -2f;
			}

			float x = Input.GetAxis("Horizontal");
			float z = Input.GetAxis("Vertical");

			Vector3 move = transform.right * x + transform.forward * z;

			controller.Move(move * speed * Time.deltaTime);

			if (Input.GetButtonDown("Jump") && isGrounded)
			{
				velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
			}

			velocity.y += gravity * Time.deltaTime;

			controller.Move(velocity * Time.deltaTime);
		}
    }
}
