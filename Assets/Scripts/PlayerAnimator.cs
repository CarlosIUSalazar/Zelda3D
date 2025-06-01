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

    private GameObject[] walkCycle;

    void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start()
    {
        walkCycle = new GameObject[] { linkRightStep, linkStand, linkLeftStep, linkStand };
        SetOnlyActive(linkStand);
    }

    void Update()
    {
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        if (movement != Vector3.zero)
        {
            AnimateWalk();

            transform.position += movement * Time.deltaTime * 3f;

            // Set rotation to face direction
            Quaternion targetRotation = Quaternion.identity;
            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            {
                targetRotation = Quaternion.Euler(0, moveInput.x > 0 ? 90 : -90, 0); // Right or Left
            }
            else
            {
                targetRotation = Quaternion.Euler(0, moveInput.y > 0 ? 0 : 180, 0); // Up or Down
            }
            transform.rotation = targetRotation;
        }
        else
        {
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
