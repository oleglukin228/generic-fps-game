using AudioSystem;
using LlamAcademy.Guns.Demo;
using System;
using System.Collections;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;


public class InventorySlot : MonoBehaviour
{
    public string[] compaibleItems;
    Transform occupiedSlot;
    public Transform slotPath;
    public GameObject poungeCap;
    protected BoxCollider slotCollider;
    public Transform OccupiedSlot { get { return occupiedSlot; } set { occupiedSlot = value; } }

    private void Start()
    {
        if (slotCollider == null) slotCollider = GetComponent<BoxCollider>();
        if (slotPath == null) slotPath = GetComponent<Transform>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.TryGetComponent<PickableStuff>(out var item)) return;
        if (item.WhoIsHolding == null) return;
        if (item.WhoIsHolding.HoldingItem == null) return;
        if (item.WhoIsHolding.IsBusy) return;
        foreach (var tag in compaibleItems)
        {
            if (item.itemType == tag)
            {
                //DisableCollider();
                item.transform.parent = slotPath.transform;
                item.StartLerpCoroutine((t) => item.transform.localRotation = Quaternion.Lerp(item.transform.localRotation, item.invRot, t), 0.5f);
                break;
            }
        }
    }

    public virtual IEnumerator ApplyItemRotation(Transform item, Transform parent, Quaternion rotation)
    {
        float timeElapsed = 0;
        //item.parent = parent.transform;
        while (timeElapsed < 0.5f)
        {
            //if (!wpnManager.HoldingItem) yield break;
            float t = timeElapsed / 0.5f;
            item.localRotation = Quaternion.Lerp(item.localRotation, rotation, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    public virtual void EnableCollider() { slotCollider.enabled = true; }
    public virtual void DisableCollider() { slotCollider.enabled = false; }


    public virtual void PutDownToSlot(Transform holdingItem, WeaponManagerFullBody wpnManager)
    {
        if (occupiedSlot != null) return;
        var item = holdingItem.GetComponent<PickableStuff>();
        if (CheckItemСompatibility(item))
        {
            if (poungeCap) poungeCap.SetActive(false);
            holdingItem.parent = slotPath;
            holdingItem.SetLocalPositionAndRotation(item.invPos, item.invRot);
            occupiedSlot = holdingItem;
            foreach (var col in item.colliders)
            {
                col.enabled = false;
            }
            wpnManager.HoldingItem = null;
            wpnManager.TargetItem = null;
            item.WhoIsHolding = null;
            wpnManager.playerIK.SetupRig(wpnManager.cursor, 0);
            SoundManager.Instance.CreateSoundBuilder().Play(wpnManager.ctx.PlayerSFX.putdownSound);
        }
        /*else
        {
            item.DropItem(wpnManager.playerIK, wpnManager);
        }*/
    }

    public virtual bool CheckItemСompatibility(PickableStuff item)
    {
        if (item == null) return false;
        foreach (var tag in compaibleItems)
        {
            if (item.itemType == tag) return true;
        }
        return false;
    }

    public virtual void PickupItemFromSlot(WeaponManagerFullBody wpnManager)
    {
        if (occupiedSlot != null)
        {
            var item = occupiedSlot.GetComponent<PickableStuff>();
            //if (poungeCap) poungeCap.SetActive(true);
            item.PickupItemFromSlot(this, wpnManager, wpnManager.playerIK);
            /*wpnManager.HoldingItem = occupiedSlot;
            foreach (var col in item.colliders)
            {
                col.enabled = true;
            }
            item.whoIsHolding = wpnManager;
            wpnManager.playerIK.SetupRig(item.PickupPoseParent, 0);
            occupiedSlot = null;
            wpnManager.TargetItem = item;*/
        }
    }
}
