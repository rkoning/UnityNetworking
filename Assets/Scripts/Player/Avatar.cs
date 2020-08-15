using UnityEngine;
using Mirror;

public class Avatar : NetworkBehaviour
{
    public GamePlayer player;
    private CharacterController controller;
    private Health health;

    public float walkSpeed = 7.5f;
    public float runSpeed = 11.5f;
    public float jumpSpeed = 8f;
    public float gravity = 20f;

    public float lookSpeed = 2.0f;
    public float lookXLimit = 45f;

    public Camera playerCamera;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    
    [HideInInspector]
    public bool canMove = true;

    public Deck deck;

    private void Start()
    {
        health = GetComponent<Health>();
        if (hasAuthority)
        {
            controller = GetComponent<CharacterController>();
            // Cursor.lockState = CursorLockMode.Locked;
            // Cursor.visible = false;
        }
        else
        {
            // Disable the camera if this isn't the local player
            playerCamera.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!hasAuthority)
            return;
        
        if (health.IsDead)
            return;

        
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        Vector3 currSpeed = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        currSpeed = canMove ? (isRunning ? currSpeed * runSpeed : currSpeed * walkSpeed) : Vector3.zero;
        float moveDirectionY = moveDirection.y;

        moveDirection = transform.forward * currSpeed.z + transform.right * currSpeed.x;
     
        if (Input.GetButton("Jump") && controller.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = moveDirectionY;
        }

        if (!controller.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        controller.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        if (Input.GetMouseButtonDown(0))
        {
            deck.Cast(0);
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            deck.StartShuffle();
        }
    }
}