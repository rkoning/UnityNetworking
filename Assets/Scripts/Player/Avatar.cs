using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Avatar : BaseAvatar
{
    [SyncVar]
    public uint playerNetId;
    
    public GamePlayer gamePlayer;
    private CharacterController controller;

    public CharacterAttribute jumpSpeed;

    [Header("Casting Attributes")]
    public CharacterAttribute manaRegen;
    public CharacterAttribute maxMana;
    public CharacterAttribute drawSpeed;
    public CharacterAttribute shuffleSpeed;


    public float runModifier = 2f;

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

    private ButtonState cast = new ButtonState("Cast");
    public bool CastDown { get { return cast.Down; } }
    public bool CastHeld { get { return cast.Held; } }
    public bool CastReleased { get { return cast.Released; } }

    private ButtonState cancel = new ButtonState("CancelCast");
    public bool CancelDown { get { return cancel.Down; } }
    public bool CancelHeld { get { return cancel.Held; } }
    public bool CancelReleased { get { return cancel.Released; } }


    public Vector3 lookPoint;
    
    public bool initialized { get; private set; }

    public void Init(GamePlayer gamePlayer)
    {
        base.Init();
        aimTransform = playerCamera.transform;
        this.gamePlayer = gamePlayer;
        health = GetComponent<PlayerHealth>();
        health.OnDeath += (float damage) => {
            animator.SetTrigger("Dying");
            this.gamePlayer.PlayerDead();
        };

        ((PlayerHealth) health).OnRespawn += (float damage) => {
            animator.SetTrigger("Alive");
        };
        deck = GetComponent<Deck>();
        if (gamePlayer.hasAuthority)
        {
            controller = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            // Disable the camera if this isn't the local player
            playerCamera.enabled = false;
            playerCamera.GetComponent<AudioListener>().enabled = false;
        }
        initialized = true;
    }

    private void Update()
    {
        if (!initialized || !gamePlayer.hasAuthority)
            return;
        
        if (health.IsDead) {
            playerCamera.transform.position = transform.position + observerOffset;
            playerCamera.transform.rotation = Quaternion.LookRotation(transform.position - playerCamera.transform.position);
            return;
        }
        
        SetAttributes();
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit)) {
            this.lookPoint = hit.point;
        } else {
            this.lookPoint = playerCamera.transform.position + playerCamera.transform.forward * 1000f;
        }

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        Vector3 currSpeed = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        currSpeed = canMove ? 
            (isRunning ?
                currSpeed * moveSpeed.CurrentValue * runModifier :
                currSpeed * moveSpeed.CurrentValue) 
            : Vector3.zero;
        float moveDirectionY = moveDirection.y;

        moveDirection = transform.forward * currSpeed.z + transform.right * currSpeed.x;
     
        if (Input.GetButton("Jump") && controller.isGrounded)
        {
            animator.SetTrigger("isJumping");
            moveDirection.y = jumpSpeed.CurrentValue;
        }
        else
        {
            moveDirection.y = moveDirectionY;
        }

        if (!controller.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        
        if (deck && !deck.IsAnchored) {
            currSpeed = currSpeed.normalized;
            if (!isRunning) {
                currSpeed /= 2;
            }

            animator.SetFloat("velx", currSpeed.x);
            animator.SetFloat("vely", currSpeed.z);

            controller.Move(moveDirection * Time.deltaTime);
        }

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            playerCamera.transform.localPosition = cameraLocation.transform.localPosition;
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        didCast = false;
        cast.Evaluate();

        if (cast.Down) {
            Cast();
        }

        if (cast.Held) {
            deck.Hold();
        }

        if (cast.Released) {
            deck.Release();
        }
        
        cancel.Evaluate();
        if (cancel.Down) {
            CancelSpell();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            SelectSpell(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            SelectSpell(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            SelectSpell(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            SelectSpell(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5)) {
            SelectSpell(4);
        }

        if (didCast) {
            // animator.SetTrigger("isCasting");
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            deck.StartShuffle();
        }
    }

    private void SetAttributes() {
        health.maxHealth = maxHealth.CurrentValue;
        health.maxArmor = maxArmor.CurrentValue;

        deck.drawDelay = 1 / drawSpeed.CurrentValue;
        deck.manaRegen = manaRegen.CurrentValue;
        deck.maxMana = maxMana.CurrentValue;
        deck.shuffleSpeed = shuffleSpeed.CurrentValue;
    }

    private void SelectSpell(int index) {
        deck.SelectSpell(index);
    }

    private void Cast() {
        deck.Cast();
        didCast = true;
    }

    private void CancelSpell() {
        deck.Cancel();
    }

    private class ButtonState {
        private string buttonId;
        private bool down;
        private bool held;
        private bool up;
        private bool released;

        public bool Down { get { return down; } }
        public bool Held { get { return held; } }
        public bool Up { get { return up; } }
        public bool Released { get { return released; } }

        public ButtonState(string buttonId) {
            this.buttonId = buttonId;
        }

        public void Evaluate() {
           down = Input.GetButtonDown(buttonId);
           held = Input.GetButton(buttonId);
           up = !held;
           released = Input.GetButtonUp(buttonId);
        }
    }

    public override void GetFromPool(string name, Vector3 position, Quaternion rotation) {
        gamePlayer.GetFromPool(name, position, rotation);
    }

    public override void ApplyStatus(Health health, StatusFactory factory) {
        gamePlayer.ApplyStatus(health, factory);
    }

    public override void DealDamage(Health other, float amount) {
        gamePlayer.DealDamage(other, amount);
    }
}