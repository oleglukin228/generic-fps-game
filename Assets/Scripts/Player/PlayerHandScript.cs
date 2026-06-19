using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class PlayerHandScript : MonoBehaviour
{
    public Camera playerCamera;
    public WeaponManagerFullBody wpnManager;
    BoxCollider boxCollider;
    Transform targetInteracteble;
    Transform previousInteracteble;
    Vector3 mousePosition = new Vector3(0, 0, 0.3f);
    Vector3 targetPosition;
    public Vector3 TargetPosition {  get { return targetPosition; } }

    public Transform TargetInteracteble { get { return targetInteracteble; } }

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void FixedUpdate()
    {
        if (wpnManager.HoldingItem == null) return;
        //mousePosition = playerCamera.ScreenPointToRay(Input.mousePosition).direction;
        transform.rotation = Quaternion.LookRotation(wpnManager.HoldingItem.position - playerCamera.transform.position, playerCamera.transform.up);
        targetPosition = transform.position + Vector3.forward * boxCollider.size.z;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        //if (targetInteracteble != null) return;
        if (other.gameObject.layer == LayerMask.NameToLayer("UI"))
        {
            targetInteracteble = other.transform;
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Pickable"))
        {
            targetInteracteble = other.transform;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        //previousInteracteble = other.transform;
        //targetInteracteble = null;
    }

    public bool SwitchHandState(bool state)
    {
        /*if (state == true)
        {
            boxCollider.center = new Vector3(boxCollider.center.x, boxCollider.center.y, 0.15f);
            boxCollider.size = new Vector3(boxCollider.size.x, boxCollider.size.y, 0.3f);
        }
        else
        {
            boxCollider.center = new Vector3(boxCollider.center.x, boxCollider.center.y, 0.5f);
            boxCollider.size = new Vector3(boxCollider.size.x, boxCollider.size.y, 1f);
        }*/
        return !state;
    }
}
