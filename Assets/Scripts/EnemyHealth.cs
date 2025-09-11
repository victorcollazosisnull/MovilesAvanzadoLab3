using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : NetworkBehaviour
{
    private EnemyAI enemy;
    [Header("Enemy Life Values")]
    public int maxLife = 5;
    public NetworkVariable<int> currentLife = new NetworkVariable<int>();
    [Header("Bar Life")]
    public Image barLife;
    private void Awake()
    {
        enemy = GetComponent<EnemyAI>();
    }
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentLife.Value = maxLife;
        }
        currentLife.OnValueChanged += LifeChange;
        UpdateBarLife();
    }
    private void LifeChange(int oldValue, int newValue)
    {
        UpdateBarLife();
    }

    private void UpdateBarLife()
    {
        if (barLife != null)
        {
            barLife.fillAmount = (float)currentLife.Value / maxLife;
        }
    }
    [Rpc(SendTo.Server)]
    public void TakeDamageServerRpc(int amount)
    {
        if (!IsServer) return;

        currentLife.Value -= amount;
        if (currentLife.Value <= 0)
        {
            enemy.Dead();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;

        if (collision.gameObject.CompareTag("Bala"))
        {
            TakeDamageServerRpc(1);
            collision.gameObject.GetComponent<NetworkObject>().Despawn();
        }
    }
}