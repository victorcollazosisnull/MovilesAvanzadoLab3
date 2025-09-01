using Unity.Android.Gradle;
using Unity.Netcode;
using UnityEngine;
public class SimplePlayerController : NetworkBehaviour
{
    public NetworkVariable<ulong> PlayerID;

    private Rigidbody rb;
    private Animator animator;
    public float Speed;
    public LayerMask LayerMask;
    public float jumpForce = 5f;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            float velX = Input.GetAxisRaw("Horizontal") * Speed * Time.deltaTime;
            float velY = Input.GetAxisRaw("Vertical") * Speed * Time.deltaTime;

            UpdatePositionRpc(velX, velY);
        }
        CheckGroundRpc();

        if (Input.GetButtonDown("Jump"))
        {
            JumpTriggerRpc("Jump");
        }
    }

    #region CheckGround
    [Rpc(SendTo.Server)]
    private void CheckGroundRpc()
    {
        if (Physics.Raycast(transform.position, Vector3.down, 1.1f, LayerMask))
        {
            animator.SetBool("Grounded", true);
            animator.SetBool("FreeFall", false);
        }
        else
        {
            animator.SetBool("Grounded", false);
            animator.SetBool("FreeFall", true);
        }
    }
    #endregion
    #region Movement And Jump
    [Rpc(SendTo.Server)]
    public void UpdatePositionRpc(float x, float y)
    {
        transform.position += new Vector3(x, 0, y);
    }

    [Rpc(SendTo.Server)]
    public void JumpTriggerRpc(string animationName)
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        animator.SetTrigger(animationName);
    }
    #endregion
}
