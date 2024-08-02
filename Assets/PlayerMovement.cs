using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("State")]
    [HideInInspector] public bool isRolling;
    private bool isGrounded;
    private bool isMove;
    private bool isMouseHidden;
    private bool isFall;
    private bool isChangeHeight;
    private bool isSliding;

    [Header("Universal")]
    private CharacterController charController;
    private float normalHeight;
    private Vector3 velocity;
    private Vector3 moveDirection;

    [Header("Movement")]
    [SerializeField] private float PlayerSpeed;
    [SerializeField] private float rollingSpeed;
    [SerializeField] private float mouseCamRotationSpeed;
    [SerializeField] private float rotationSmoothTime;
    [SerializeField] private float jumpForce;
    [SerializeField] private float smoothJump;
    [SerializeField] private float rollHeight;
    [SerializeField] private float newHeight;
    [SerializeField] private float slideSpeed;
    [SerializeField] private GameObject[] slider;
    [SerializeField] private Transform camTransform;
    private float currentVelocity;
    private float lastSlideYVelocity;
    private float gravity;

    [Header("Gravity")]
    [SerializeField] private float gravityForce;
    [SerializeField] private float checkRadius;
    [SerializeField] private float slideCheckRadius;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask slideLayer;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    private string moveFloat = "move";
    private string animVelocityY = "velocityY";
    private string animJumpRun = "Run Jump";
    private string animJump = "Jumping";
    private string animGrounded = "isGrounded";
    private string animRolling = "Rolling";
    private string animIsRolling = "isRolling";
    private string animIdle = "Idle";

    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<CharacterController>();
        normalHeight = charController.height;
        slider = GameObject.FindGameObjectsWithTag("Slider");
        gravity = gravityForce;
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        if (isChangeHeight) ChangeHeight(newHeight);
    }

    // Script for Movement Character and Call Gravity Effect 
    private void Move()
    {
        // Input Movement
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float moveMagnitude = new Vector2(x, y).magnitude;


        if (CanSlide())
        {
            if (!isSliding) isSliding = true;
            Sliding();
        }
        else
        {
            if (isSliding) isSliding = false;
            gravity = gravityForce;

            if (IsGrounded())
            {
                //velocity.y = 0;

                velocity = new Vector3(x, velocity.y, y);

                // Set isMove Base Character Velocity
                isMove = moveMagnitude > 0;

                if (Input.GetButtonDown("Jump"))
                {
                    if (isMove)
                    {
                        Jump();
                        animator.CrossFade(animJumpRun, 0.1f);
                    }
                    else
                    {
                        moveDirection = Vector3.zero;
                        animator.CrossFade(animJump, 0.1f);
                    }
                }

                if (Input.GetKeyDown(KeyCode.LeftControl))
                {
                    Rolling();
                }


                if (!isGrounded) isGrounded = true;
            }
            else
            {
                Movement(new Vector3(moveDirection.x, velocity.y, moveDirection.z));

                if (isGrounded) isGrounded = false;

                Gravity();
            }

            if (isRolling)
            {
                velocity.z = rollingSpeed;
                Movement(CharacterDirectionMove(velocity));
            }
            else if (isGrounded)
            {
                if (isMove) Movement(FreeMovement(velocity));
                else Movement(GravityMovement(velocity));
            }

            animator.SetFloat(moveFloat, moveMagnitude);
        }

        animator.SetFloat(animVelocityY, velocity.y);
        animator.SetBool(animGrounded, isGrounded);
        animator.SetBool(animIsRolling, isRolling);

        // Hide and Unhide Cursor
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (isMouseHidden)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                isMouseHidden = false;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                isMouseHidden = true;
            }
        }

    }

    public void Jump()
    {
        velocity.y = jumpForce;
    }

    private void Rolling()
    {
        isRolling = true;

        animator.CrossFade(animRolling, .1f);
        newHeight = rollHeight;
        isChangeHeight = true;
    }

    public void DefaultHeight()
    {
        newHeight = normalHeight;
        isChangeHeight = true;
    }

    private void ChangeHeight(float height)
    {
        charController.height = SmoothTransitionFloat(charController.height, height);

        if (charController.height >= height - 0.1f) isChangeHeight = false;
    }

    private void Gravity()
    {
        //if (velocity.y < -gravity) velocity.y = -gravity; // Set Max Velocity y
        //else 
        //velocity.y -= Time.deltaTime * gravityForce * 0.1f * smoothJump; // Membuat Effect Percepatan Smooth saat Lompat atau Terjatuh
        //Mathf.Lerp(velocity.y, velocity.y - gravity, Time.deltaTime);
        velocity.y = SmoothTransitionFloat(velocity.y, velocity.y - gravity);
        if (!isSliding)
        {
            velocity.x = SmoothTransitionFloat(velocity.x, 0);
            velocity.z = SmoothTransitionFloat(velocity.z, 0);
        }

        //velocity = Mathf.Lerp(velocity, Vector3.zero, Time.deltaTime);
        Movement(GravityMovement(velocity));
    }

    private void Sliding()
    {
        RaycastHit hit;
        Ray ray = new Ray(gameObject.transform.position, -gameObject.transform.up);

        if (Physics.Raycast(ray, out hit, 1f, slideLayer))
        {
            GameObject obj = hit.transform.gameObject;
            foreach (var item in slider)
            {
                print(item);
                print(obj);
                print(obj == item);
                if (obj == item)
                {
                    float angel = Mathf.SmoothDampAngle(transform.eulerAngles.y, obj.transform.rotation.y + 90, ref currentVelocity, rotationSmoothTime);
                    transform.rotation = Quaternion.Euler(0, angel, 0);

                    velocity.z = SmoothTransitionFloat(velocity.z, slideSpeed);
                    Movement(velocity);

                    animator.SetBool(animGrounded, true);

                    animator.CrossFade(animIdle, 0.1f);

                    gravity = velocity.z * 5f;

                    if (Input.GetButtonDown("Jump"))
                    {
                        velocity.y = 0;
                        animator.CrossFade(animJumpRun, 0.1f);
                    }
                    else Gravity();

                    moveDirection = velocity;

                    //lastSlideYVelocity = velocity.y;

                    animator.SetFloat(moveFloat, 0);
                }
            }
        }
    }

    // Menghitung Arah Gerak Berdasarkan Posisi Arah Kamera dan Merotasi Character
    float PlayerRotation(Vector3 move)
    {
        float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + camTransform.eulerAngles.y;
        float angel = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref currentVelocity, rotationSmoothTime);

        transform.rotation = Quaternion.Euler(0, angel, 0);

        return targetAngle;
    }

    // Movement Ketika Character tidak Bergerak agar Hanya Terkena efek Gravitasi
    Vector3 GravityMovement(Vector3 move)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.y = move.y;
        return moveDirection;
    }

    // Movemenet Sesuai Arah Depan Player
    Vector3 CharacterDirectionMove(Vector3 move)
    {
        move = transform.TransformDirection(move);
        return move;
    }

    // Mendapatkan Arah Gerak Character
    Vector3 FreeMovement(Vector3 move)
    {
        Vector3 moveDirection = Quaternion.Euler(0, PlayerRotation(move), 0) * Vector3.forward;
        moveDirection.y = velocity.y;

        this.moveDirection = new Vector3(moveDirection.x, velocity.y, moveDirection.z);

        return moveDirection;
    }

    private void Movement(Vector3 velocity)
    {
        charController.Move(new Vector3(velocity.x * PlayerSpeed, velocity.y, velocity.z * PlayerSpeed) * Time.deltaTime);
    }

    private bool CanSlide()
    {
        return Physics.Raycast(groundCheck.position, Vector3.down, slideCheckRadius, slideLayer);
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, checkRadius, groundLayer);
    }

    private float SmoothTransitionFloat(float startFloat, float endFloat)
    {
        return Mathf.Lerp(startFloat, endFloat, Time.deltaTime);
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (!collision.gameObject.CompareTag("Slider")) return;

    //    foreach (var item in slider)
    //    {
    //        if (collision.gameObject == item)
    //        {
    //            isSliding = true;
    //        }
    //    }
    //}
    //private void OnCollisionStay(Collision collision)
    //{
    //    print("Tes");
    //    if (!collision.gameObject.CompareTag("Slider")) return;

    //    foreach (var item in slider)
    //    {
    //        if (collision.gameObject == item)
    //        {
    //            PlayerRotation(collision.gameObject.transform.rotation * Vector3.forward);
    //            print(collision.gameObject.transform.rotation * Vector3.forward);
    //        }
    //    }
    //}
    //private void OnCollisionExit(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Slider")) isSliding = false;
    //}
}
