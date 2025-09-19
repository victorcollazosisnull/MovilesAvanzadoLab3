using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager2 : NetworkBehaviour
{
    [Header("Player Settings")]
    public GameObject playerPrefab;
    public static GameManager2 Instance;
    public Dictionary<string, PlayerData> playerStateByAccountID = new();

    public Action OnConnection;

    [Header("Buff Settings")]
    public GameObject buffPrefab;              
    public float buffSpawnInterval = 10f;      
    public Vector3 buffSpawnArea = new Vector3(20, 0, 20); 
    private float buffTimer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public override void OnNetworkSpawn()
    {
        if (IsServer)
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleDisconnect;

        OnConnection?.Invoke();
    }
    public override void OnNetworkDespawn()
    {
        if (IsServer)
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleDisconnect;
    }
    private void HandleDisconnect(ulong clientID)
    {
        print("El jugador" + clientID + "Se a desconectado");
    }
    [Rpc(SendTo.Server)]
    public void RegisterPlayerServerRpc(string accountID, ulong clientID) 
    {
        PlayerData data;

        if (!playerStateByAccountID.TryGetValue(accountID, out data))
        {
            data = new PlayerData(accountID, Vector3.zero, 100, 5);
            playerStateByAccountID[accountID] = data;
            Debug.Log($"Nuevo jugador registrado, Bienvenido al vicio: {accountID}");
        }
        else
        {
            Debug.Log($"Jugador {accountID} se reconecto, restaurando sus estadisticas.");
        }

        SpawnPlayerServer(clientID, data);
    }
    public void SpawnPlayerServer(ulong ID, PlayerData data)
    {
        if (!IsServer) return;
        GameObject player = Instantiate(playerPrefab);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(ID, true);
        player.GetComponent<Player>().SetData(data);
    }
    private void Update()
    {
        if (!IsServer) return;

        buffTimer += Time.deltaTime;
        if (buffTimer >= buffSpawnInterval)
        {
            SpawnBuff();
            buffTimer = 0f;
        }
    }

    private void SpawnBuff()
    {
        Vector3 pos = new Vector3(UnityEngine.Random.Range(-buffSpawnArea.x / 2, buffSpawnArea.x / 2),0.5f,
                                  UnityEngine.Random.Range(-buffSpawnArea.z / 2, buffSpawnArea.z / 2));

        GameObject buff = Instantiate(buffPrefab, pos, Quaternion.identity);
        buff.GetComponent<NetworkObject>().Spawn(true);
    }
}
