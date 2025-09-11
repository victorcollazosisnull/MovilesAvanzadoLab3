using Unity.Netcode;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class SimplePlayerController : NetworkBehaviour
{
    [Header("Network Values")]
    public NetworkVariable<ulong> PlayerID;
    public NetworkVariable<int> Life;
    [Header("Settings")]
    public float JumpForce = 5;
    public float Speed = 10;
    public LayerMask groundLayer;
    [Header("References")]
    private Animator animator;
    private Rigidbody rb;
    private LineRenderer lineRenderer;
    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
    }
    void Update()
    {
        if (!IsOwner) return;

        float VelX = Input.GetAxisRaw("Horizontal") * Speed * Time.deltaTime;
        float VelY = Input.GetAxisRaw("Vertical") * Speed * Time.deltaTime;
        transform.position += new Vector3(VelX, 0, VelY);

        AimMouse();

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            JumpTriggerRpc("Jump"); 
        }

        if (Input.GetMouseButtonDown(0))
        {
            ShootServerRpc();
        }

        CheckGroundRpc(); 
    }
    [Rpc(SendTo.Server)]
    public void JumpTriggerRpc(string animationName)
    {
        animator.SetTrigger(animationName);
    }
    [Rpc(SendTo.Server)]
    public void CheckGroundRpc()
    {
        bool grounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);
        animator.SetBool("Grounded", grounded);
        animator.SetBool("FreeFall", !grounded);
    }
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);
    }
    [ServerRpc]
    private void ShootServerRpc(ServerRpcParams rpcParams = default)
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject projectileInstance = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        projectileInstance.GetComponent<NetworkObject>().Spawn();
    }
    private void AimMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 point = ray.GetPoint(distance);
            Vector3 dir = point - transform.position;
            dir.y = 0;

            if (dir.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(dir);
                lineRenderer.enabled = true;
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, firePoint.position);  
                lineRenderer.SetPosition(1, point);          
            }
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }
}
