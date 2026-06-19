using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugMenu : MonoBehaviour
{
    [SerializeField] private GameObject spawnMenu;
    [SerializeField] private PlayerController player;
    [SerializeField] private TMP_Text fpsCountText;
    private Transform cameraPosition;
    bool QKeyDown => Input.GetKeyDown(KeyCode.Q);
    bool QKeyUp => Input.GetKeyUp(KeyCode.Q);
    float deltaTime;

    private void Start()
    {
        if (player == null)
        {
            player = Object.FindAnyObjectByType<PlayerController>();
            cameraPosition = player.cameraPosition;
        }
    }

    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsCountText.text = $"FPS: " + Mathf.Ceil(fps);

        if (QKeyDown)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            spawnMenu.SetActive(true);
        }
        else if (QKeyUp)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            spawnMenu.SetActive(false);
        } 
    }

    public void SpawnObject(GameObject obj)
    {
        if (Physics.Raycast(cameraPosition.position, cameraPosition.forward, out var hit, 100f))
            ObjectPoolManager.SpawnObject(obj, hit.point + Vector3.up, Quaternion.identity, ObjectPoolManager.PoolType.GameObjects);
    }

    public void ReloadScene()
    {
        ObjectPoolManager.ReturnAllActiveObjectsToPool();
        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);
    }

    public void SpawnBush(GameObject obj)
    {
        if (Physics.Raycast(cameraPosition.position, cameraPosition.forward, out var hit, 100f))
        {
            var bush = ObjectPoolManager.SpawnObject(obj, hit.point, Quaternion.identity, ObjectPoolManager.PoolType.GameObjects);
            GameEnvironment.AddBush(bush);
        }
    }

    public void SpawnProps(GameObject obj)
    {
        if (Physics.Raycast(cameraPosition.position, cameraPosition.forward, out var hit, 100f))
        {
            ObjectPoolManager.SpawnObject(obj, hit.point, Quaternion.identity, ObjectPoolManager.PoolType.GameObjects);
        }
    }

    public void DeleteAllObjects() => ObjectPoolManager.ReturnAllActiveObjectsToPool();

    public void LoadNewScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
