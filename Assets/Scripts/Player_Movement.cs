using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player_Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float gravity = -9.81f;

    [Header("Rolling Settings")]
    public float rollSpeed = 8f;
    public float rollDuration = 0.5f;

    [Header("Camera Reference")]
    public Transform cameraTransform;

    [Header("Animation")]
    public Animator animator;  // Assign this in the Inspector

    private CharacterController controller;
    private Vector3 velocity;
    private bool isRolling = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (animator == null)
            Debug.LogWarning("Animator not assigned! Please assign it in the Inspector.");
    }

    void Update()
    {
        if (!isRolling)
        {
            HandleMovement();
            HandleRollInput();
        }

        ApplyGravity();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 inputDir = new Vector3(moveX, 0f, moveZ).normalized;
        float inputMagnitude = inputDir.magnitude;

        if (animator != null)
            animator.SetFloat("Speed", inputMagnitude);

        if (inputMagnitude >= 0.1f)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;
            camForward.y = 0f;
            camRight.y = 0f;

            Vector3 moveDir = (camForward * moveZ + camRight * moveX).normalized;

            controller.Move(moveDir * moveSpeed * Time.deltaTime);

            Quaternion toRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void HandleRollInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");
            Vector3 inputDir = new Vector3(moveX, 0f, moveZ).normalized;

            if (inputDir.magnitude >= 0.1f)
            {
                StartCoroutine(Roll(inputDir));
            }
        }
    }

    IEnumerator Roll(Vector3 rollDirection)
    {
        isRolling = true;

        if (animator != null)
            animator.SetTrigger("Roll");

        float timer = 0f;

        while (timer < rollDuration)
        {
            controller.Move(rollDirection * rollSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        isRolling = false;
    }

    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
