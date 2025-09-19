using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
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
    [Header("Respawn Area")]
    public Vector3 respawnCenter = Vector3.zero;  
    public Vector3 respawnSize = new Vector3(20, 0, 20); 
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
        LifePlayerChange(0, currentLife.Value); 
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

        projectileInstance.GetComponent<Projectile2>().Initialize(this, attack.Value);
    }
    public void SetData(PlayerData playerData)
    {
        accoundID.Value = playerData.accoundID;
        currentLife.Value = playerData.health;
        //currentLife.Value = maxLife;
        attack.Value = playerData.attack;
        transform.position = playerData.position;
    }
    public override void OnNetworkDespawn()
    {
        if (IsServer && GameManager2.Instance != null)
        {
            GameManager2.Instance.playerStateByAccountID[accoundID.Value.ToString()] =
                new PlayerData(accoundID.Value.ToString(), transform.position, currentLife.Value, attack.Value);

            Debug.Log($"Jugador {accoundID.Value} desconectado, datos guardados.");
        }
    }
    public void AplyBuff(int amount)
    {
        if (!IsServer) return;

        attack.Value += amount;
        Debug.Log($"{accoundID.Value} tiene un buff de +{amount}, ataque : {attack.Value}");
    }

    [Rpc(SendTo.Server)]
    public void TakeDamagePlayerRpc(int amount)
    {
        if (!IsServer) return;

        currentLife.Value -= amount;
        if (currentLife.Value <= 0)
        {
            Vector3 pos = GetRandomRespawnPosition();
            transform.position = pos;
            currentLife.Value = maxLife;

            RespawnClientRpc(pos);
        }
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void RespawnClientRpc(Vector3 newPosition)
    {
        transform.position = newPosition;
    }
    private Vector3 GetRandomRespawnPosition()
    {
        float x = Random.Range(-respawnSize.x / 2, respawnSize.x / 2);
        float z = Random.Range(-respawnSize.z / 2, respawnSize.z / 2);
        return respawnCenter + new Vector3(x, 0, z);
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
