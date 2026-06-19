using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GibsCollisionScript : MonoBehaviour
{
    public GameObject[] decalPrefab;
    public AudioClip[] gutsSounds;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rg = GetComponent<Rigidbody>();

        if (rg.linearVelocity.magnitude >= 1f && !collision.gameObject.CompareTag("Particle"))
        {
            audioSource.pitch = Random.Range(0.8f, 1f);
            int random = Random.Range(0, gutsSounds.Length);
            audioSource.PlayOneShot(gutsSounds[random]);
            ContactPoint contact = collision.contacts[0];
            Vector3 position = contact.point;
            Vector3 normal = contact.normal;

            CreateDecal(position, normal, collision);
        }
    }

    private void CreateDecal(Vector3 position, Vector3 normal, Collision collision)
    {
        if (decalPrefab.Length > 0)
        {
            int randomIndex = Random.Range(0, decalPrefab.Length);
            GameObject randomObject = decalPrefab[randomIndex];

            GameObject hitObject = collision.gameObject;
            // Создаем декаль крови на поверхности
            GameObject decal = Instantiate(randomObject, position, Quaternion.LookRotation(normal));
            decal.transform.SetParent(hitObject.transform);
            // Дополнительные настройки декали, если необходимо

            // Дальнейшая обработка выбранного случайного объекта
            // ...
        }
    }
}
