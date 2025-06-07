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
    private float turnSpeed = 150f;     // degrees/second
    private float animationTimer = 0f;
    private float frameDuration = 0.15f;
    private int frameIndex = 0;

    private GameObject[] walkCycle;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled  += ctx => moveInput = Vector2.zero;
        controls.Player.SwordThrust.performed += ctx => OnAttack();
    }

    private void OnEnable()  => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Start()
    {
        walkCycle = new GameObject[]
        {
            linkRightStep,
            linkStand,
            linkLeftStep,
            linkStand
        };
        SetOnlyActive(linkStand);
    }

    private void Update()
    {
        // 1) If attacking, do nothing
        if (isAttacking)
            return;

        // 2) Read input and build camera‐relative directions
        Vector2 input = moveInput;
        Transform cam = Camera.main.transform;
        Vector3 camForward = cam.forward; camForward.y = 0f; camForward.Normalize();
        Vector3 camRight   = cam.right;   camRight.y   = 0f; camRight.Normalize();

        // 3) Build worldMove vector
        Vector3 worldMove = camRight * input.x + camForward * input.y;

        // 4) Dead‐zone check
        if (worldMove.sqrMagnitude < 0.01f)
        {
            // Idle pose
            frameIndex = 0;
            animationTimer = 0f;
            SetOnlyActive(linkStand);
            return;
        }

        // 5) Walk animation
        AnimateWalk();

        // 6) Compute target rotation (Y) and smoothly move toward it
        float targetY = Mathf.Atan2(worldMove.x, worldMove.z) * Mathf.Rad2Deg;
        float currentY = transform.eulerAngles.y;
        float newY = Mathf.MoveTowardsAngle(currentY, targetY, turnSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, newY, 0f);

        // 7) Move Link immediately in that direction
        transform.position += worldMove.normalized * walkSpeed * Time.deltaTime;
    }

    private void AnimateWalk()
    {
        animationTimer += Time.deltaTime;
        if (animationTimer >= frameDuration)
        {
            frameIndex = (frameIndex + 1) % walkCycle.Length;
            SetOnlyActive(walkCycle[frameIndex]);
            animationTimer = 0f;
        }
    }

    private void OnAttack()
    {
        isAttacking = true;
        SetOnlyActive(linkThrustSword);
        Invoke(nameof(EndAttack), 0.2f);
    }

    private void EndAttack()
    {
        isAttacking = false;
        if (moveInput != Vector2.zero)
            AnimateWalk();
        else
            SetOnlyActive(linkStand);
    }

    private void SetOnlyActive(GameObject active)
    {
        linkStand.SetActive(active == linkStand);
        linkRightStep.SetActive(active == linkRightStep);
        linkLeftStep.SetActive(active == linkLeftStep);
        linkThrustSword.SetActive(active == linkThrustSword);
    }
}



///
/// //
/// 
/// 
/// 


// using UnityEngine;
// using UnityEngine.InputSystem;

// public class PlayerAnimatorNewInput : MonoBehaviour
// {
//     [Header("Vox Models for Each Pose")]
//     public GameObject linkStand;       // Idle pose
//     public GameObject linkRightStep;   // Walk frame 1
//     public GameObject linkLeftStep;    // Walk frame 2
//     public GameObject linkThrustSword; // Attack pose

//     [HideInInspector]
//     public PlayerControls controls;

//     private Vector2 moveInput;
//     private bool isAttacking = false;

//     private float walkSpeed = 7f;

//     [Tooltip("How long it takes to rotate toward the target direction (lower = snappier, higher = smoother).")]
//     public float turnSmoothTime = 0.1f;
//     private float turnSmoothVelocity;

//     private float animationTimer = 0f;
//     private float frameDuration = 0.15f;
//     private int frameIndex = 0;

//     private GameObject[] walkCycle;

//     private void Awake()
//     {
//         controls = new PlayerControls();
//         controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
//         controls.Player.Move.canceled  += ctx => moveInput = Vector2.zero;
//         controls.Player.SwordThrust.performed += ctx => OnAttack();
//     }

//     private void OnEnable()  => controls.Enable();
//     private void OnDisable() => controls.Disable();

//     private void Start()
//     {
//         walkCycle = new GameObject[]
//         {
//             linkRightStep,
//             linkStand,
//             linkLeftStep,
//             linkStand
//         };
//         SetOnlyActive(linkStand);
//     }

//     private void Update()
//     {
//         if (isAttacking)
//             return;

//         // 1) Gather camera axes (flattened)
//         Transform cam = Camera.main.transform;
//         Vector3 camForward = cam.forward;
//         Vector3 camRight   = cam.right;
//         camForward.y = 0f;
//         camRight.y   = 0f;
//         camForward.Normalize();
//         camRight.Normalize();

//         // 2) Build the input vector in camera space
//         Vector3 inputDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

//         if (inputDirection.sqrMagnitude < 0.01f)
//         {
//             // No input → Idle
//             frameIndex = 0;
//             animationTimer = 0f;
//             SetOnlyActive(linkStand);
//             return;
//         }

//         // 3) Determine the desired move direction in world space:
//         Vector3 worldInputDir = camRight * moveInput.x + camForward * moveInput.y;
//         worldInputDir.Normalize();

//         // 4) Compute the desired Y‐angle (in degrees) the player should face:
//         //    Atan2(worldInputDir.x, worldInputDir.z) gives the angle relative to world-Z,
//         //    which is already camera relative. No need to add camera.eulerAngles.y because
//         //    worldInputDir is built from camForward/camRight.
//         float targetAngle = Mathf.Atan2(worldInputDir.x, worldInputDir.z) * Mathf.Rad2Deg;

//         // 5) Smoothly rotate Link toward targetAngle using SmoothDampAngle:
//         float smoothedAngle = Mathf.SmoothDampAngle(
//             transform.eulerAngles.y,   // current yaw
//             targetAngle,               // target yaw
//             ref turnSmoothVelocity,    // velocity reference
//             turnSmoothTime             // how long to reach target (approx)
//         );
//         transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);

//         // 6) Animate walking frames
//         AnimateWalk();

//         // 7) Finally, move Link forward in the direction he's looking (not directly worldInputDir):
//         //    This ensures forward always goes “out of his chest,” even if input is slightly angled.
//         Vector3 moveDir = transform.forward;
//         transform.position += moveDir * walkSpeed * Time.deltaTime;
//     }

//     private void AnimateWalk()
//     {
//         animationTimer += Time.deltaTime;
//         if (animationTimer >= frameDuration)
//         {
//             frameIndex = (frameIndex + 1) % walkCycle.Length;
//             SetOnlyActive(walkCycle[frameIndex]);
//             animationTimer = 0f;
//         }
//     }

//     private void OnAttack()
//     {
//         isAttacking = true;
//         SetOnlyActive(linkThrustSword);
//         Invoke(nameof(EndAttack), 0.2f);
//     }

//     private void EndAttack()
//     {
//         isAttacking = false;
//         if (moveInput != Vector2.zero)
//             AnimateWalk();
//         else
//             SetOnlyActive(linkStand);
//     }

//     private void SetOnlyActive(GameObject active)
//     {
//         linkStand.SetActive(active == linkStand);
//         linkRightStep.SetActive(active == linkRightStep);
//         linkLeftStep.SetActive(active == linkLeftStep);
//         linkThrustSword.SetActive(active == linkThrustSword);
//     }
// }

// /
// / ///
// / //
// / 
// / //
// / 
// / 

// using UnityEngine;
// using UnityEngine.InputSystem;

// public class PlayerAnimatorNewInput : MonoBehaviour
// {
//     [Header("Vox Models for Each Pose")]
//     public GameObject linkStand;       // Idle pose
//     public GameObject linkRightStep;   // Walk frame 1
//     public GameObject linkLeftStep;    // Walk frame 2
//     public GameObject linkThrustSword; // Attack pose

//     [HideInInspector] 
//     public PlayerControls controls;

//     private Vector2 moveInput;
//     private bool isAttacking = false;

//     private float walkSpeed = 7f;
//     private float animationTimer = 0f;
//     private float frameDuration = 0.15f;
//     private int frameIndex = 0;

//     private GameObject[] walkCycle;

//     private void Awake()
//     {
//         // Create & bind the InputActions asset
//         controls = new PlayerControls();

//         // Move action (vector2)  
//         controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
//         controls.Player.Move.canceled  += ctx => moveInput = Vector2.zero;

//         // Attack action (button)  
//         controls.Player.SwordThrust.performed += ctx => OnAttack();
//     }

//     private void OnEnable()
//     {
//         controls.Enable();
//     }

//     private void OnDisable()
//     {
//         controls.Disable();
//     }

//     private void Start()
//     {
//         // Define the walk cycle (frames 0–3).
//         walkCycle = new GameObject[] 
//         { 
//             linkRightStep, 
//             linkStand, 
//             linkLeftStep, 
//             linkStand 
//         };

//         // At start, show the idle pose
//         SetOnlyActive(linkStand);
//     }

//     private void Update()
//     {
//         // If we’re currently attacking, skip movement/rotation
//         if (isAttacking) 
//             return;

//         // Build a Vector3 from the Vector2 (x→right, y→forward)
//         Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

//         if (movement != Vector3.zero)
//         {
//             AnimateWalk();

//             // Move character by translation (or use Rigidbody.MovePosition in FixedUpdate)
//             transform.position += movement * Time.deltaTime * walkSpeed;

//             // Face movement direction: full 360°
//             float angle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg;
//             transform.rotation = Quaternion.Euler(0f, angle, 0f);
//         }
//         else
//         {
//             // No input → return to idle pose
//             frameIndex = 0;
//             animationTimer = 0f;
//             SetOnlyActive(linkStand);
//         }
//     }

//     private void AnimateWalk()
//     {
//         animationTimer += Time.deltaTime;

//         if (animationTimer >= frameDuration)
//         {
//             // Advance through the 4-step cycle
//             frameIndex = (frameIndex + 1) % walkCycle.Length;
//             SetOnlyActive(walkCycle[frameIndex]);
//             animationTimer = 0f;
//         }
//     }

//     private void OnAttack()
//     {
//         // Show the thrust‐sword pose immediately
//         isAttacking = true;
//         SetOnlyActive(linkThrustSword);

//         // After a short delay, end attack and revert to idle/walk
//         float attackDuration = 0.2f; 
//         Invoke(nameof(EndAttack), attackDuration);
//     }

//     private void EndAttack()
//     {
//         isAttacking = false;

//         // If player is still moving, resume walk animation; otherwise, show idle
//         if (moveInput != Vector2.zero)
//         {
//             AnimateWalk();
//         }
//         else
//         {
//             SetOnlyActive(linkStand);
//         }
//     }

//     /// <summary>
//     /// Only the “active” GameObject is visible; all others are hidden.
//     /// </summary>
//     private void SetOnlyActive(GameObject active)
//     {
//         linkStand.SetActive(active == linkStand);
//         linkRightStep.SetActive(active == linkRightStep);
//         linkLeftStep.SetActive(active == linkLeftStep);
//         linkThrustSword.SetActive(active == linkThrustSword);
//     }
// }
