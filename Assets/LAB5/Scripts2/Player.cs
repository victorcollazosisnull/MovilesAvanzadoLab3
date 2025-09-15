using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using UnityEngine.UI;
public class Player : NetworkBehaviour
{
    [Header("Settings")]
    public float Speed = 10;
    private Rigidbody rb;
    private LineRenderer lineRenderer;
    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    [Header("Network")]
    public NetworkVariable<FixedString32Bytes> accoundID = new();
    public NetworkVariable<int> currentLife = new();
    public NetworkVariable<int> attack = new();
    [Header("Player Life Values")]
    public int maxLife = 100;
    public Image barLife;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
    }
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentLife.Value = maxLife;
        }
        currentLife.OnValueChanged += LifePlayerChange;
        UpdateBarLifePlayer();
    }
    private void Update()
    {
        if (!IsOwner) return;

        float VelX = Input.GetAxisRaw("Horizontal") * Speed * Time.deltaTime;
        float VelY = Input.GetAxisRaw("Vertical") * Speed * Time.deltaTime;
        transform.position += new Vector3(VelX, 0, VelY);

        AimMouse();

        if (Input.GetMouseButtonDown(0))
        {
            ShootServerRpc();
        }
    }
    [ServerRpc]
    private void ShootServerRpc(ServerRpcParams rpcParams = default)
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject projectileInstance = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        projectileInstance.GetComponent<NetworkObject>().Spawn();
    }
    public void SetData(PlayerData playerData)
    {
        accoundID.Value = playerData.accoundID;
        currentLife.Value = playerData.health;
        attack.Value = playerData.attack;
        transform.position = playerData.position;
    }
    public override void OnNetworkDespawn()
    {
        GameManager2.Instance.playerStateByAccountID[accoundID.Value.ToString()] = new PlayerData(accoundID.Value.ToString(), 
            transform.position, 
            currentLife.Value, 
            attack.Value);
        print("Me e desconectado " + NetworkManager.Singleton.LocalClientId + " y se a guardado la data de" + accoundID.Value);
    }
    public void AplyBuff(int amount)
    {
        if (!IsServer) return;

    }    
    [Rpc(SendTo.Server)]
    public void TakeDamagePlayerRpc(int amount)
    {
        if (!IsServer) return;

        currentLife.Value -= amount;
        if (currentLife.Value <= 0)
        {
            //DESTRUIR Y SPAWNEAR EN SITIO RANDOM
        }
    }
    private void UpdateBarLifePlayer()
    {
        if (barLife != null)
        {
            barLife.fillAmount = (float)currentLife.Value / maxLife;
        }
    }
    private void LifePlayerChange(int oldValue, int newValue)
    {
        UpdateBarLifePlayer();
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
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bala"))
        {
            TakeDamagePlayerRpc(1);
            collision.gameObject.GetComponent<NetworkObject>().Despawn();
        }
    }
}
public class PlayerData 
{
    public string accoundID;
    public Vector3 position;
    public int health;
    public int attack;
    public PlayerData(string accounID, Vector3 pos, int hp, int atk)
    {
        accoundID = accounID;
        position = pos;
        health = hp;
        attack = atk;
    }
}
