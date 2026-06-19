using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttachmentSystem : MonoBehaviour
{
    public List<WeaponAttachment> attachments = new List<WeaponAttachment>();
    private GunItem weapon;
    private void Start()
    {
        weapon = GetComponent<GunItem>();
        if (attachments.Count > 0)
        {
            foreach (var attachment in attachments)
            {
                attachment.ApplyTo(weapon.CurrentStats);
            }
        }
    }
    public void DisableAttachments()
    {
        foreach (var attachment in attachments)
        {
            attachment.enabled = false;
        }
    }
    public void ActivateAttachments()
    {
        foreach (var attachment in attachments)
        {
            attachment.enabled = true;
        }
    }
}
