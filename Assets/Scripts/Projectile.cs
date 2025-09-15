using Unity.Netcode;
using UnityEngine;
using System.Collections;
public class Projectile : NetworkBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;

    private float timer = 0f;

    public Player player;
    //private void Awake()
    //{
    //    player = GetComponent<Player>();
    //}
    void Update()
    {
        if (!IsServer) return; 

        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            NetworkObject.Despawn(); 
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;
        if (collision.gameObject.CompareTag("Pared"))
        {
            NetworkObject.Despawn();
            Debug.Log("Bala destruida por pared");
        }
        else if (collision.gameObject.CompareTag("Player2"))
        {
            Debug.Log("le di a mi player");
            player.TakeDamagePlayerRpc(10);
            NetworkObject.Despawn();
        }
    }
}
