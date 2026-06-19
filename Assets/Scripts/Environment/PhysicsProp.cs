using UnityEngine;

public class PhysicsProp : MonoBehaviour, IDamageable
{
    [SerializeField] private GameObject prop;

    [Header("Breakable ettings")]
    [SerializeField] private GameObject fracturedPartsParent; // Ссылка на префаб разрушенного состояния (осколков)
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float explosionForce = 500f;     // Сила, с которой разлетятся осколки
    [SerializeField] private float explosionRadius = 5f;      // Радиус действия силы
    [SerializeField] private float upwardsModifier = 0.5f;    // Модификатор для силы вверх

    [Header("Condition")]
    [SerializeField] private float minimumImpactForce = 5f;  // Минимальная сила удара для разрушения

    // Можно добавить звуки и эффекты частиц
    [Header("Effect")]
    [SerializeField] private GameObject particleEffectPrefab;

    private float health;
    private Rigidbody _rb;
    private Collider _collider;
    public bool IsDead {  get { if (health < 0)  return true; return false; } }
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }
    private void OnEnable()
    {
        ResetProp();
    }
    private void OnCollisionEnter(Collision collision)
    {
        // Проверяем, достаточно ли сильным был удар, чтобы разрушить объект
        if (collision.relativeVelocity.magnitude > minimumImpactForce)
        {
            DestroyProp(Vector3.zero);
        }
    }

    // Вызывается, когда объект должен быть разрушен (можно вызвать и извне, например, от взрыва)
    public void DestroyProp(Vector3 direction)
    {
        // 1. Создаем экземпляр префаба разрушенного состояния
        //GameObject destroyedInstance = ObjectPoolManager.SpawnObject(fracturedPartsParent, transform.position, Quaternion.identity, ObjectPoolManager.PoolType.GameObjects);
        prop.SetActive(false);
        fracturedPartsParent.SetActive(true);

        // 2. Применяем силу к каждому осколку
        foreach (Transform child in fracturedPartsParent.transform)
        {
            Rigidbody rb = child.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, direction, explosionRadius, upwardsModifier, ForceMode.Impulse);
            }
        }

        if (particleEffectPrefab != null)
        {
            ObjectPoolManager.SpawnObject(particleEffectPrefab, transform.position, Quaternion.identity, ObjectPoolManager.PoolType.ParticleSystems);
        }

        // 4. Уничтожаем оригинальный (цельный) объект
        //ObjectPoolManager.ReturnObjectToPool(gameObject, ObjectPoolManager.PoolType.GameObjects);
    }

    public void TakeDamage(float amount, Vector3 direction, Vector3 hitPoint)
    {
        health -= amount;
        //explosionForce = force;
        if (IsDead)
        {
            Die(direction, hitPoint);
        }
    }

    public void Die(Vector3 direction, Vector3 hitPoint)
    {
        _collider.enabled = false;
        _rb.isKinematic = true;
        DestroyProp(direction);
    }

    public void ResetProp()
    {
        if (!IsDead) return;
        prop.SetActive(true);
        fracturedPartsParent.SetActive(false);
        foreach (Transform part in fracturedPartsParent.transform)
        {
            part.localPosition = Vector3.zero;
            part.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        }

        health = maxHealth;
        _collider.enabled = true;
        _rb.isKinematic = false;
    }
}