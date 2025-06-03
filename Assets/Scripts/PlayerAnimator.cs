using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimatorNewInput : MonoBehaviour
{
    public GameObject linkStand;
    public GameObject linkRightStep;
    public GameObject linkLeftStep;

    public PlayerControls controls;

    private Vector2 moveInput;
    private float animationTimer = 0f;
    private float frameDuration = 0.15f;
    private int frameIndex = 0;
    private float walkSpeed = 7f;

    private GameObject[] walkCycle;

    void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled  += ctx => moveInput = Vector2.zero;
    }

    void OnEnable()  => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start()
    {
        walkCycle = new GameObject[] { linkRightStep, linkStand, linkLeftStep, linkStand };
        SetOnlyActive(linkStand);
    }

    void Update()
    {
        // Build a Vector3 from the Vector2 (x→right, y→forward):
        Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        if (movement != Vector3.zero)
        {
            // 1) Animate the walk frames:
            AnimateWalk();

            // 2) Move the player diagonally/straight at walkSpeed:
            transform.position += movement * Time.deltaTime * walkSpeed;

            // 3) Compute full-360° facing angle from moveInput:
            float angle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
        else
        {
            // Idle state:
            frameIndex = 0;
            animationTimer = 0f;
            SetOnlyActive(linkStand);
        }
    }

    void AnimateWalk()
    {
        animationTimer += Time.deltaTime;
        if (animationTimer >= frameDuration)
        {
            frameIndex = (frameIndex + 1) % walkCycle.Length;
            SetOnlyActive(walkCycle[frameIndex]);
            animationTimer = 0f;
        }
    }

    void SetOnlyActive(GameObject active)
    {
        linkStand.SetActive(active == linkStand);
        linkRightStep.SetActive(active == linkRightStep);
        linkLeftStep.SetActive(active == linkLeftStep);
    }
}
