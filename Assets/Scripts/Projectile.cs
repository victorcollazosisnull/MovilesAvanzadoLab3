using Unity.Netcode;
using UnityEngine;
using System.Collections;
public class Projectile : NetworkBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;

    private float timer = 0f;
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
    }
}
