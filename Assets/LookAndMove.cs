using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAndMove : MonoBehaviour
{
    public float sensitivity;
    public float walkSpeed;
    public float runSpeed;
    private float speed;
    private float xRot;
    public float walkAcceleration;
    public float runAcceleration;
    private float acceleration;
    private Vector2 mouseInput;
    private Vector2 keyInput;
    public Transform head;
    public float jumpForce;
    private CharacterController controller;
    private List<GameObject> groundObjects;
    private bool isGrounded;
    public float frictionCoefficient;
    private Vector3 velocity;
    private float yVel;
    public float gravity = -9.81f;
    public Transform orientation;
    bool isJumping = false;
    private Weapon weapon;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        groundObjects = new List<GameObject>();
        isGrounded = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        speed = walkSpeed;
        weapon = GetComponentInChildren<Weapon>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Ground")
        {
            groundObjects.Add(other.gameObject);
            isGrounded = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Ground")
        {
            groundObjects.Remove(other.gameObject);
            if(groundObjects.Count == 0)
            {
                isGrounded = false;
                isJumping = false;
            }
        }
    }

    void Update()
    {
        mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        keyInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if(!weapon.isReloading && !weapon.adjustingADS)
        {
            xRot -= mouseInput.y * sensitivity;
            xRot = Mathf.Clamp(xRot, -90, 90);
            head.transform.localRotation = Quaternion.Euler(xRot, 0, 0);
            transform.rotation *= Quaternion.Euler(0, mouseInput.x * sensitivity, 0);
        }
        if(isGrounded)
        {
            if(isJumping) return;
            yVel = gravity;
            if(Input.GetButtonDown("Jump"))
            {
                yVel = jumpForce;
                isJumping = true;
            }
        }
        else
        {
            yVel += gravity * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.LeftShift))
        {
            speed = runSpeed;
            acceleration = runAcceleration;
        }
        else
        {
            speed = walkSpeed;
            acceleration = walkAcceleration;
        }
        velocity += orientation.forward * keyInput.y * acceleration * Time.deltaTime + (orientation.right * keyInput.x * acceleration * Time.deltaTime);
        velocity.x = Mathf.Clamp(velocity.x, -speed, speed);
        velocity.y = Mathf.Clamp(velocity.y , -speed, speed);
    }

    void FixedUpdate()
    {
        velocity += -velocity * frictionCoefficient;
        velocity.y = yVel;
        controller.Move(velocity);
    }
}
