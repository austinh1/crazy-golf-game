using UnityEngine;

public class StickOnCollide : MonoBehaviour
{
    private Rigidbody rigid;
    private Collider collide;

    void Start()
    {
        rigid = transform.GetComponent<Rigidbody>();
        collide = transform.GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (rigid != null && !rigid.isKinematic)
        {
            rigid.velocity = Vector3.zero;
            rigid.useGravity = false;
            rigid.isKinematic = true;
            rigid.collisionDetectionMode = CollisionDetectionMode.Discrete;
            transform.SetParent(other.transform, true);

            if (other.gameObject.CompareTag("Player"))
            {
                collide.enabled = false;
                // Tacks attached to the player last FOREVER :)
                var despawner = GetComponent<DespawnTimer>();
                if (despawner != null)
                {
                    despawner.Cancel();
                }
            }
        }
    }
}
