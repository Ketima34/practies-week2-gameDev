using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class charactorSript : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4f;
    public float runSpeed = 8f;
    public float rotateSpeed = 12f;

    [Header("Jump")]
    public float jumpHeight = 1.5f;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundMask = ~0;

    private Rigidbody rb;
    private Collider col;
    private Vector2 moveInput;
    private bool runHeld;
    private bool jumpRequested;
    private bool grounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        ReadInput();
    }

    void ReadInput()
    {
        moveInput = Vector2.zero;
        runHeld = false;

        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed) moveInput.y += 1;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed) moveInput.y -= 1;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) moveInput.x += 1;
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) moveInput.x -= 1;

            if (kb.leftShiftKey.isPressed) runHeld = true;
            if (kb.spaceKey.wasPressedThisFrame) jumpRequested = true;
        }

        var pad = Gamepad.current;
        if (pad != null)
        {
            Vector2 stick = pad.leftStick.ReadValue();
            if (stick.sqrMagnitude > moveInput.sqrMagnitude) moveInput = stick;

            if (pad.buttonEast.isPressed || pad.rightTrigger.ReadValue() > 0.5f) runHeld = true;
            if (pad.buttonSouth.wasPressedThisFrame) jumpRequested = true;
        }

        moveInput = Vector2.ClampMagnitude(moveInput, 1f);
    }

    void FixedUpdate()
    {
        CheckGround();

        Transform cam = Camera.main != null ? Camera.main.transform : null;
        Vector3 forward = cam ? Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized : Vector3.forward;
        Vector3 right = cam ? Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized : Vector3.right;

        Vector3 dir = forward * moveInput.y + right * moveInput.x;
        float speed = runHeld ? runSpeed : walkSpeed;

        Vector3 vel = rb.linearVelocity;
        vel.x = dir.x * speed;
        vel.z = dir.z * speed;

        if (jumpRequested)
        {
            if (grounded)
            {
                vel.y = Mathf.Sqrt(2f * Mathf.Abs(Physics.gravity.y) * jumpHeight);
            }
            jumpRequested = false;
        }

        rb.linearVelocity = vel;

        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, rotateSpeed * Time.fixedDeltaTime));
        }
    }

    void CheckGround()
    {
        Bounds b = col.bounds;
        Vector3 origin = new Vector3(b.center.x, b.min.y + 0.1f, b.center.z);
        grounded = Physics.Raycast(origin, Vector3.down, 0.1f + groundCheckDistance, groundMask, QueryTriggerInteraction.Ignore);
    }
}