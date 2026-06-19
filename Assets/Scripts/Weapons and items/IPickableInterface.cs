using LlamAcademy.Guns.Demo;
using System.Collections;
using UnityEngine;

public interface IPickableInterface
{
    Transform ItemParent { get; }
    void Pickup(PlayerIK playerIK, WeaponManagerFullBody wpnManager);
    void DropItem(PlayerIK playerIK, WeaponManagerFullBody wpnManager);
    void MoveItem(Transform position);
    void InteractItem(Vector3 position, Vector3 direction);
    void PickupItemFromSlot(InventorySlot slot, WeaponManagerFullBody wpnManager, PlayerIK playerIK);
}

public interface IPickableWeaponInterface
{
    IEnumerator BindWeapon(Transform slot, WeaponManagerFullBody wpnManager);
    IEnumerator Equip(InventorySlot invSlot, WeaponManagerFullBody wpnOwner);
    void Unequip(Transform transformParent);
    void Reload(bool i);
    void UpdateWeapon();
    void OnHandSwitchTrigger(bool isControllingHand);
    IEnumerator AssignWeaponToSlot(Transform slotTransform, WeaponManagerFullBody wpnManager);
    void UpdateCursorPositionOverride(Transform cursor, Camera playerCamera, Vector3 mousePosition);
    void OnSprintingState(bool i);
    void DropWeapon();
    WeaponStatsSO CurrentStats { get; }
    WeaponType GetWeaponType { get; }
}