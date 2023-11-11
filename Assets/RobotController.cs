using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    private PlayerInputManager input;
    private CharacterController controller;
    private Animator animator;
    [SerializeField] GameObject mainCam;
    [SerializeField] float moveSpeed = 8;
    [SerializeField] float sprintSpeed = 11;
    [SerializeField] float rollSpeed = 13;
    [SerializeField] float gravity = -10;
    [SerializeField] float jumpHeight = 2;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
    bool isGrounded;
    Vector3 velocity;
    [SerializeField] Transform cameraFollowTarget;
    private bool ballMode = false;
    float xRotation;
    float yRotation;
    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<PlayerInputManager>();
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        SwitchMode();
        if (ballMode)
        {
            MoveBall();
        }
        else
        {
            MoveNormal();
        }
        MoveNormal();

        jumpAndGravity();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    void CameraRotation()
    {
        xRotation += input.look.x;
        yRotation += input.look.y;
        xRotation = Mathf.Clamp(xRotation, -30, 70);
        Quaternion roation = Quaternion.Euler(xRotation, yRotation, 0);
        cameraFollowTarget.rotation = roation;
    }

    void MoveNormal()
    {
        float speed = 0;
        Vector3 inputDir = new Vector3(input.move.x, 0, input.move.y);
        float targetRotaion = 0;
        if (input.move != Vector2.zero)
        {
            speed = input.sprint ? sprintSpeed : moveSpeed;
            targetRotaion = Quaternion.LookRotation(inputDir).eulerAngles.y + mainCam.transform.eulerAngles.y;
            Quaternion rotate = Quaternion.Euler(0, targetRotaion, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotate, 20 * Time.deltaTime);
            animator.SetFloat("speed", input.sprint ? 2 : input.move.magnitude);
        }
        Vector3 targetDirection = Quaternion.Euler(0, targetRotaion, 0) * Vector3.forward;
        controller.Move(targetDirection * speed * Time.deltaTime);
    }
    void MoveBall()
    {
        float speed = 0;
        Vector3 inputDir = new Vector3(input.move.x, 0, input.move.y);
        float targetRotaion = 0;
        if (input.move != Vector2.zero)
        {
            speed = rollSpeed;
            targetRotaion = Quaternion.LookRotation(inputDir).eulerAngles.y + mainCam.transform.eulerAngles.y;
            Quaternion rotate = Quaternion.Euler(0, targetRotaion, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotate, 20 * Time.deltaTime);
        }
        animator.SetFloat("rollSpeed", input.move.magnitude);
        Vector3 targetDirection = Quaternion.Euler(0, targetRotaion, 0) * Vector3.forward;
        controller.Move(targetDirection * speed * Time.deltaTime);
    }

    void SwitchMode()
    {
        if (input.switchMode)
        {
            ballMode = !ballMode;
            input.switchMode = false;
            animator.SetBool("ballMode",ballMode);
        }
    }

    void jumpAndGravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, .2f, groundLayer);
        
        if (isGrounded)
        {
            if (input.jump && ballMode)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * 2 * -gravity);
                input.jump = false;
            }
        }else
        {
            velocity.y += gravity * Time.deltaTime;
        }
        controller.Move(velocity * Time.deltaTime);
    }

}
