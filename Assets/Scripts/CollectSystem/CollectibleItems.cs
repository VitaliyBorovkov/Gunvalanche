using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class CollectibleItems : MonoBehaviour, ICollectible
{
    [SerializeField] private float rotationSpeed = 43f;
    [SerializeField] private float floatDistance = 0.5f;

    protected Transform spawnPoint;

    private bool floatingUp = true;
    private Vector3 initialPosition;

    protected bool isCollected = false;

    protected virtual void Start()
    {
        initialPosition = transform.position;
    }

    protected virtual void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        var rotation = transform.rotation.eulerAngles;
        rotation.x = 0f;
        transform.rotation = Quaternion.Euler(rotation);


        float floatStep = floatDistance * Time.deltaTime;
        transform.position += (floatingUp ? Vector3.up : Vector3.down) * floatStep;

        if (transform.position.y > initialPosition.y + floatDistance)
        {
            floatingUp = false;
        }
        else if (transform.position.y < initialPosition.y)
        {
            floatingUp = true;
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect(other.gameObject);
        }
    }

    public virtual void Collect() 
    {
        Collect(null); 
    }

    protected virtual void Collect(GameObject player)
    {
        if (isCollected)
        {
            return;
        }
        isCollected = true;

        gameObject.SetActive(false);
    }

    public void SetSpawnPoint(Transform point)
    {
        spawnPoint = point;
    }
}
