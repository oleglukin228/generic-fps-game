using UnityEngine;
using UnityEngine.Events;

public enum CullingBeheviour { None, ToggleScripts, FadeInOut, ToggleGameObject }

public class CullingTarget : MonoBehaviour
{
    public UnityEvent onCulled, onVisible;
    public GameObject targetObject;
    public float boundarySphereRadius = 1f;
    //public CullingBeheviour cullingBeheviour = CullingBeheviour.ToggleGameObject;
    public bool isPriorityObject;

    MonoBehaviour[] scripts;

    private void Start()
    {
        //if (isPriorityObject) onVisible?.Invoke();
        //else CullingManager.Instance.Register(this);
    }

    void OnEnable()
    {
        if (isPriorityObject) onVisible?.Invoke();
        else CullingManager.Instance?.Register(this);
    }
    void OnDisable()
    {
        if (!isPriorityObject) CullingManager.Instance.Deregister(this);
    }

    public void ToggleOn() 
    {
        SetState(true);
        if (isPriorityObject)
        {
            onVisible?.Invoke();
            return;
        }
    }
    public void ToggleOff() 
    {
        if (isPriorityObject) return;

        SetState(false);
        onCulled?.Invoke();
    }

    void SetState(bool enabled)
    {
        //targetObject.SetActive(enabled);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, boundarySphereRadius);
    }
}
