using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    static class PlayerAnimations
    {
        public static int Idle = Animator.StringToHash("Player_Idle");
        public static int Walk = Animator.StringToHash("Player_Walk");
        public static int Fall = Animator.StringToHash("Player_Fall");
        public static int Jump = Animator.StringToHash("Player_Jump");
    }
    //Components
    private Rigidbody2D rigidBody;
    private SpriteRenderer spriteRendererComponent;
    private Animator animator;
    private BoxCollider2D boxCollider;

    //Movement speed and jump stuff
    [SerializeField] private float horizontalSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float timeSinceLeftGround = 0.0f;
    [SerializeField] private float coyoteTimeDuration = 0.2f;
    [SerializeField] private bool canUseCoyoteTime = false;
    [SerializeField] private bool isJumping = false;
    [SerializeField] private bool canJump = false;

    //Layers
    [SerializeField] private LayerMask GroundLayer;

    //Other
    private float horizontalDirection;

    [SerializeField] private float originalGravity;
    private float fallGravity;
    [SerializeField] float maxFallGravity;
    [SerializeField] private float fallGravityIncrement;
    private int currentAnimation;

    [SerializeField] private PlayerInputManager inputManager;
    private SFXEmitter soundEmitter;

    // Start is called before the first frame update
    private void Start()
    {
        inputManager = GetComponent<PlayerInputManager>();
        soundEmitter = GetComponent<SFXEmitter>();
        inputManager = GetComponent<PlayerInputManager>();
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        spriteRendererComponent = GetComponent<SpriteRenderer>();
        originalGravity = rigidBody.gravityScale;
        fallGravity = rigidBody.gravityScale * 2f;
        PlayAnimation(PlayerAnimations.Idle);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if(!GameManager.Instance.finished)
        {
            if (IsGrounded())
            {
                isJumping = false;
            }
            horizontalDirection = inputManager.GetHorizontal();

            if (IsGrounded())
            {
                rigidBody.velocity = new Vector2(horizontalDirection * horizontalSpeed, rigidBody.velocity.y);
            }
            else if (!IsGrounded())
            {
                rigidBody.velocity = new Vector2(horizontalDirection * horizontalSpeed / 1.5f, rigidBody.velocity.y);
            }
            canJump = IsGrounded() || canUseCoyoteTime;

            UpdateCoyoteTime();
            UpdateGravity();
            UpdateAnimations();
            //UpdateSounds();
        }
        if (inputManager.escape)
        {
            GameManager.Instance.GoToMainMenu();
        }

        if (inputManager.reload)
        {
            GameManager.Instance.ReloadLevel();
        }

    }

    public void OnJump(InputAction.CallbackContext callbackContext)
    {
        if (canJump && !isJumping)
        {
            if (callbackContext.performed)
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpForce);
                GetComponent<PlayerDustEffect>().CreateJumpDust();
            }
            else if (callbackContext.canceled && rigidBody.velocity.y > 0)
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, rigidBody.velocity.y / 2);
                isJumping=true;
            }

        }
    }

    private void UpdateCoyoteTime()
    {
        if (!IsGrounded())
        {
            timeSinceLeftGround += Time.deltaTime;

            if (timeSinceLeftGround <= coyoteTimeDuration)
            {
                canUseCoyoteTime = true;
            }
            else
            {
                canUseCoyoteTime = false;
            }
        }
        else
        {
            timeSinceLeftGround = 0.0f;
            canUseCoyoteTime = false;
        }
    }
    private bool IsGrounded()
    {
        return Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, .4f, GroundLayer);
    }

    private void UpdateGravity()
    {
        if (IsGrounded())
        {
            rigidBody.gravityScale = originalGravity;
            fallGravity = originalGravity * 2f;
        }
        if (rigidBody.velocity.y > 0.1f)
        {
            rigidBody.gravityScale = originalGravity;

        }
        else if (rigidBody.velocity.y < -0.05f)
        {
            rigidBody.gravityScale = fallGravity;

            if (fallGravity < maxFallGravity)
            {
                fallGravity += fallGravityIncrement;
            }
        }
    }

    private void UpdateSounds()
    {
        if (rigidBody.velocity.x == 0f || !IsGrounded())
        {
            soundEmitter.Stop(SoundEffectType.PlayerWalk);
        }
        if ((rigidBody.velocity.x > 0.1f || rigidBody.velocity.x < -0.1f) && IsGrounded())
        {
            soundEmitter.Play(SoundEffectType.PlayerWalk);
        }
        soundEmitter.Play(SoundEffectType.PlayerJump);

    }
    private void UpdateAnimations()
    {
        if (horizontalDirection < 0f)
        {
            spriteRendererComponent.flipX = false;
        }
        else if (horizontalDirection > 0f)
        {
            spriteRendererComponent.flipX = true;
        }

        if (rigidBody.velocity.y > 0.1f)
        {
            PlayAnimation(PlayerAnimations.Jump);
        }
        else if (rigidBody.velocity.y < -0.1f)
        {
            PlayAnimation(PlayerAnimations.Fall);
        }
        else if (rigidBody.velocity.x > 0.1f || rigidBody.velocity.x < -0.1f)
        {
            PlayAnimation(PlayerAnimations.Walk);
        }
        else
        {
            PlayAnimation(PlayerAnimations.Idle);
        }
    }

    private void PlayAnimation(int animation)
    {
        if (currentAnimation == animation) { return; }
        currentAnimation = animation;
        animator.Play(animation);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            GameManager.Instance.finished = true;
            spriteRendererComponent.enabled = false;
            StartCoroutine(GameManager.Instance.LoadNextLevel(2.0f));

        }
    }

}