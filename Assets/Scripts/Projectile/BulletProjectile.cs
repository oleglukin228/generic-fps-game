using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : DecalSystem
{
    [Header("Data")]
    protected SurfaceImpact surfaceImpact;
    protected Vector3 previousPosition;
    protected Vector3 shootDir;
    protected float bulletSpeed;
    public float bulletForce = 3.5f;
    protected Transform bulletOwner;
    protected Rigidbody rb;
    public float damage;
    float range;
    public float maxDistance = 100f;
    float bulletDistanceFlewed;
    [SerializeField] TrailRenderer[] trails;

    [SerializeField] private LayerMask ballisticLayersToHit;
    [SerializeField] private LayerMask ballisticStoppingLayers;

    private bool bulletPenetration;
    private bool leaveExitDecal;
    private int numberOfPenetrations;

    private int passes = 0;

    public virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        foreach (var trail in trails)
        {
            trail.Clear();
        }
        previousPosition = transform.position;
        Invoke(nameof(ReturnBulletToPool), 1f);
    }

    public void Setup(Vector3 shootDir, Transform bulletOwner, SurfaceImpact surfaceImpact, float bulletSpeed = 300f, float bulletForce = 3.5f, float damage = 0f, float range = 40f)
    {
        this.shootDir = shootDir;
        this.bulletSpeed = bulletSpeed;
        this.bulletOwner = bulletOwner;
        this.bulletForce = bulletForce;
        this.damage = damage;
        this.range = range;
        this.surfaceImpact = surfaceImpact;
    }

    private void Update()
    {
        transform.position += bulletSpeed * Time.deltaTime * shootDir;
    }

    private void FixedUpdate()
    {
        //transform.position += bulletSpeed * Time.fixedDeltaTime * shootDir;

        Vector3 currentPosition = transform.position;

        RaycastHit hit;

        Vector3 origin = transform.position - transform.forward.normalized * 1f;
        Vector3 direction = transform.forward.normalized;

        if (bulletPenetration)
        {
            if (Physics.Raycast(origin, direction, out hit, 1f, ballisticLayersToHit) && passes == numberOfPenetrations)
            {
                Vector3 spawnPos = hit.point + hit.normal.normalized * 0.001f;
                //LeaveDecal(spawnPos, Quaternion.FromToRotation(Vector3.forward, hit.normal), hit, true);
                //SurfaceManager.Instance.SpawnEffect(spawnPos, Quaternion.FromToRotation(Vector3.forward, hit.normal), hit.collider, surfaceImpact, hit.transform);
                CancelInvoke();
                ObjectPoolManager.ReturnObjectToPool(gameObject);
            }
            else if (Physics.Raycast(origin, direction, out hit, 1f, ballisticStoppingLayers))
            {
                Vector3 spawnPos = hit.point + hit.normal.normalized * 0.001f;
                //LeaveDecal(spawnPos, Quaternion.FromToRotation(Vector3.forward, hit.normal), hit, true);
                //SurfaceManager.Instance.SpawnEffect(spawnPos, Quaternion.FromToRotation(Vector3.forward, hit.normal), hit.collider, surfaceImpact, hit.transform);
                CancelInvoke();
                ObjectPoolManager.ReturnObjectToPool(gameObject);
            }
            else if (Physics.Raycast(origin, direction, out hit, 1f, ballisticLayersToHit))
            {
                Vector3 spawnPos = hit.point + hit.normal.normalized * 0.001f;
                LeaveDecal(spawnPos, Quaternion.FromToRotation(Vector3.forward, hit.normal), hit, true);
                //SurfaceManager.Instance.SpawnEffect(spawnPos, Quaternion.FromToRotation(Vector3.forward, hit.normal), hit.collider, surfaceImpact, hit.transform);
                passes++;
            }
        }
        else if (!bulletPenetration)
        {
            if (Physics.Raycast(previousPosition, currentPosition - previousPosition, out hit, Vector3.Distance(previousPosition, currentPosition), ballisticLayersToHit))
            {
                if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
                    //damageable.TakeDamage(damage, hit.point, bulletForce);
                if (hit.rigidbody)
                    hit.rigidbody.AddForceAtPosition(bulletForce * shootDir, hit.point, ForceMode.Impulse);
                Vector3 spawnPos = hit.point + hit.normal.normalized * 0.001f;
                //LeaveDecal(spawnPos, Quaternion.FromToRotation(Vector3.forward, hit.normal), hit, true);
                SurfaceManager.Instance.SpawnEffect(spawnPos, Quaternion.FromToRotation(Vector3.forward, -hit.normal), hit.normal, hit.collider, surfaceImpact, hit.transform);
                CancelInvoke();
                ObjectPoolManager.ReturnObjectToPool(gameObject);
            }
        }

        previousPosition = currentPosition;
    }

    /*private void CreateBulletHole(Vector3 position, Vector3 normal)
    {
        RaycastHit hit;
        if (Physics.Raycast(position + normal * 0.01f, -normal, out hit, 0.1f))
        {
            GameObject hitObject = hit.collider.gameObject;

            // Получить тег объекта, на который попала пуля
            string surfaceTag = hitObject.tag;

            // Создать эффект попадания для соответствующего тега
            GameObject bulletHolePrefab = bulletHoles.GetBulletHolePrefab(surfaceTag);
            GameObject decalPrefab = bulletHoles.GetBulletHoleDecalMaterial(surfaceTag);
            if (bulletHolePrefab != null)
            {
                // Создать дыру от пули
                Vector3 direction;
                if (surfaceTag == "Flesh")
                {
                    direction = bulletOwner.position - transform.position + -hit.normal;
                }
                else
                {
                    direction = hit.normal;
                }
                GameObject bulletHole = Instantiate(bulletHolePrefab, position, Quaternion.LookRotation(direction));
                bulletHole.transform.SetParent(hitObject.transform);
                if (decalPrefab != null)
                {
                    // Создать и настроить декаль
                    GameObject decalProjector = Instantiate(decalPrefab, position, Quaternion.LookRotation(hit.normal));
                    decalProjector.transform.SetParent(hitObject.transform);
                    //decalProjector.GetComponent<DecalProjector>().size = new Vector3(0.1f, 0.1f, 0.1f);
                    decalProjector.transform.rotation = Quaternion.FromToRotation(Vector3.forward, -hit.normal);
                    
                    // Создать и настроить доп. декаль крови
                    if (surfaceTag == "Flesh")
                    {
                        // Выполнять дополнительный рейкаст для создания декали крови
                        GameObject bloodDecal = bulletHoles.surfaceBloodDecalPrefabs[Random.Range(0, bulletHoles.surfaceBloodDecalPrefabs.Length)];

                        RaycastHit bloodHit;
                        if (Physics.Raycast(hit.point + hit.normal * 0.01f, -direction, out bloodHit, 4, ~LayerMask.GetMask("Hitbox", "Enemy")))
                        {
                            // Создать и настроить декаль крови
                            bloodDecal = Instantiate(bloodDecal, bloodHit.point, Quaternion.LookRotation(bloodHit.normal));
                            bloodDecal.transform.SetParent(bloodHit.collider.gameObject.transform);
                            bloodDecal.transform.rotation = Quaternion.FromToRotation(Vector3.forward, -bloodHit.normal);
                        }
                        RaycastHit downwardHit;
                        if (Physics.Raycast(hit.point + hit.normal * 0.01f, -Vector3.up, out downwardHit, 2, ~LayerMask.GetMask("Hitbox", "Enemy")))
                        {
                            // Создать и настроить декаль крови
                            bloodDecal = Instantiate(bloodDecal, downwardHit.point, Quaternion.LookRotation(downwardHit.normal));
                            bloodDecal.transform.SetParent(downwardHit.collider.gameObject.transform);
                            bloodDecal.transform.rotation = Quaternion.FromToRotation(Vector3.forward, -downwardHit.normal);
                        }

                    }
                }
            }
        }
    }*/

    public bool DrawRayForDuration(Vector3 start, Vector3 direction, float duration)
    {
        Ray ray = new Ray(start, direction);
        RaycastHit hit;

        // Проверяем попадание
        if (Physics.Raycast(ray, out hit, 4f))
        {
            // Получаем точку попадания
            Vector3 targetPoint = hit.point;

            // Отрисовываем луч до точки попадания
            Debug.DrawLine(start, targetPoint, Color.red, duration);

            // Возвращаем значение true, чтобы указать, что луч был успешно отрисован
            return true;
        }
        else
        {
            // Если попадание не произошло, отрисовываем луч на максимальную дальность
            Vector3 targetPoint = ray.GetPoint(4f);
            Debug.DrawLine(start, targetPoint, Color.red, duration);

            // Возвращаем значение true, чтобы указать, что луч был успешно отрисован
            return true;
        }
    }

    void ReturnBulletToPool()
    {
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }
}
