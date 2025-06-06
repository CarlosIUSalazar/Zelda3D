using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimatorNewInput : MonoBehaviour
{
    [Header("Vox Models for Each Pose")]
    public GameObject linkStand;       // Idle pose
    public GameObject linkRightStep;   // Walk frame 1
    public GameObject linkLeftStep;    // Walk frame 2
    public GameObject linkThrustSword; // Attack pose

    [HideInInspector] 
    public PlayerControls controls;

    private Vector2 moveInput;
    private bool isAttacking = false;

    private float walkSpeed = 7f;
    private float animationTimer = 0f;
    private float frameDuration = 0.15f;
    private int frameIndex = 0;

    private GameObject[] walkCycle;

    private void Awake()
    {
        // Create & bind the InputActions asset
        controls = new PlayerControls();

        // Move action (vector2)  
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled  += ctx => moveInput = Vector2.zero;

        // Attack action (button)  
        controls.Player.SwordThrust.performed += ctx => OnAttack();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Start()
    {
        // Define the walk cycle (frames 0–3).
        walkCycle = new GameObject[] 
        { 
            linkRightStep, 
            linkStand, 
            linkLeftStep, 
            linkStand 
        };

        // At start, show the idle pose
        SetOnlyActive(linkStand);
    }

    private void Update()
    {
        // If we’re currently attacking, skip movement/rotation
        if (isAttacking) 
            return;

        // Build a Vector3 from the Vector2 (x→right, y→forward)
        Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        if (movement != Vector3.zero)
        {
            AnimateWalk();

            // Move character by translation (or use Rigidbody.MovePosition in FixedUpdate)
            transform.position += movement * Time.deltaTime * walkSpeed;

            // Face movement direction: full 360°
            float angle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
        else
        {
            // No input → return to idle pose
            frameIndex = 0;
            animationTimer = 0f;
            SetOnlyActive(linkStand);
        }
    }

    private void AnimateWalk()
    {
        animationTimer += Time.deltaTime;

        if (animationTimer >= frameDuration)
        {
            // Advance through the 4-step cycle
            frameIndex = (frameIndex + 1) % walkCycle.Length;
            SetOnlyActive(walkCycle[frameIndex]);
            animationTimer = 0f;
        }
    }

    private void OnAttack()
    {
        // Show the thrust‐sword pose immediately
        isAttacking = true;
        SetOnlyActive(linkThrustSword);

        // After a short delay, end attack and revert to idle/walk
        float attackDuration = 0.2f; 
        Invoke(nameof(EndAttack), attackDuration);
    }

    private void EndAttack()
    {
        isAttacking = false;

        // If player is still moving, resume walk animation; otherwise, show idle
        if (moveInput != Vector2.zero)
        {
            AnimateWalk();
        }
        else
        {
            SetOnlyActive(linkStand);
        }
    }

    /// <summary>
    /// Only the “active” GameObject is visible; all others are hidden.
    /// </summary>
    private void SetOnlyActive(GameObject active)
    {
        linkStand.SetActive(active == linkStand);
        linkRightStep.SetActive(active == linkRightStep);
        linkLeftStep.SetActive(active == linkLeftStep);
        linkThrustSword.SetActive(active == linkThrustSword);
    }
}
