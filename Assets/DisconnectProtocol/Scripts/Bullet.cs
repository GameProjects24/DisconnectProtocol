using UnityEditor.UI;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject _sparksPrefab;
    private void OnCollisionEnter(Collision other) {
        //Vector3 carVelocity = GetComponent<Rigidbody>().;
        //Vector3 sparkDirection = carVelocity.normalized;
        Instantiate(_sparksPrefab, transform.position,  Quaternion.LookRotation(transform.forward));
        Destroy(gameObject);
    }
}
