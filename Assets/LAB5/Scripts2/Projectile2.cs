using System;
using Unity.Netcode;
using UnityEngine;

public class Projectile2 : NetworkBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;

    private float timer;
    private int damage;
    private Player owner;

    public void Initialize(Player shooter, int atk)
    {
        owner = shooter;
        damage = atk;
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        if (IsServer)
        {
            timer += Time.deltaTime;
            if (timer >= lifetime)
            {
                NetworkObject.Despawn();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        Player target = other.GetComponent<Player>();
        if (target != null && target != owner)
        {
            target.TakeDamagePlayerRpc(damage);
            NetworkObject.Despawn();
        }
        else if (other.CompareTag("Pared"))
        {
            NetworkObject.Despawn();
        }
    }
}