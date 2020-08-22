using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Avatar : NetworkBehaviour
{
    public GamePlayer player;
    private CharacterController controller;
    private PlayerHealth health;

    public Animator animator;

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

    private bool didCast = false;
    public AnimationClip currentCastAnimation;
    private AnimatorOverrideController overrideController;

    public Vector3 observerOffset = new Vector3(8f, 4f, 12f);
    public Transform cameraLocation;

    private void Start()
    {

        overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = overrideController;
        health = GetComponent<PlayerHealth>();
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
        
        if (health.IsDead) {
            playerCamera.transform.position = transform.position + observerOffset;
            health.OnDeath += () => {};
            playerCamera.transform.rotation = Quaternion.LookRotation(transform.position - playerCamera.transform.position);
            return;
        }

        
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        Vector3 currSpeed = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        currSpeed = canMove ? (isRunning ? currSpeed * runSpeed : currSpeed * walkSpeed) : Vector3.zero;
        float moveDirectionY = moveDirection.y;

        moveDirection = transform.forward * currSpeed.z + transform.right * currSpeed.x;
     
        if (Input.GetButton("Jump") && controller.isGrounded)
        {
            animator.SetTrigger("isJumping");
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
        
        currSpeed = currSpeed.normalized;
        if (!isRunning) {
            currSpeed /= 2;
        }

        animator.SetFloat("velx", currSpeed.x);
        animator.SetFloat("vely", currSpeed.z);

        controller.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            playerCamera.transform.localPosition = cameraLocation.transform.localPosition;
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        didCast = false;
        if (Input.GetMouseButtonDown(0))
        {
            CastSpell(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            CastSpell(0);

        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            CastSpell(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            CastSpell(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            CastSpell(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5)) {
            CastSpell(4);
        }

        if (didCast) {
            // animator.SetTrigger("isCasting");
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            deck.StartShuffle();
        }
    }

    private void CastSpell(int index) {
        deck.Cast(index);
        didCast = true;
    }

    public void SetCastAnimation(AnimationClip clip) {
        overrideController[currentCastAnimation.name] = clip;
        // currentCastAnimation = clip;
    }
}