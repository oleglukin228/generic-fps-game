using LlamAcademy.Guns.Demo;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class DoorHandleItem : InteractableEnvironment
{
    public float openSpeed = 2f;
    public float minAngle = -90f;
    public float maxAngle = 90f;
    public Transform doorDirectionPivot;
    public Rigidbody door;

    public override void MoveItem(Transform position)
    {
        //Vector3 direction = WhoIsHolding.itemRoot.position - doorDirectionPivot.position;
        door.transform.Rotate(Vector3.up, -Input.GetAxisRaw("Mouse X") * openSpeed);
        //door.AddForceAtPosition(direction.normalized * 100f, door.position, ForceMode.Acceleration);
    }
}
