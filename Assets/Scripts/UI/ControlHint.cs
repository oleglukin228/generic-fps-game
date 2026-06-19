using Kineractive;
using TMPro;
using UnityEngine;

public class ControlHint : Singleton<ControlHint>
{
    public WeaponManagerFullBody wpnManager;
    static TMP_Text text;
    string lmbKeyDown = "в текущем состоянии ничего";
    string rmbKeyDown = "в текущем состоянии ничего";
    string eKey = "активировать левую руку";
    string rKey = "в текущем состоянии ничего";
    string cKey = "присесть";
    string gKey = "в текущем состоянии ничего";
    string hKey = "спрятать подсказки";
    string oneKey = "в текущем состоянии ничего";
    string twoKey = "в текущем состоянии ничего";
    string threeKey = "в текущем состоянии ничего";
    bool hideHints = false;
    //public static ControlHint Instance {  get; private set; }

    void Start()
    {
        if (wpnManager == null)
        {
            wpnManager = Object.FindAnyObjectByType<WeaponManagerFullBody>();
        }

        text = GetComponent<TMP_Text>();
        text.text =
            "Управление: \n" +
            "Q - МЕНЮ РАЗРАБОТЧИКА \n" +
            $"H - {hKey} \n" +
            "Shift - бежать \n" +
            $"C - {cKey} \n" +
            $"Е - {eKey} \n" +
            $"ЛКМ - {lmbKeyDown} \n" +
            $"ПКМ - {rmbKeyDown} \n" +
            $"R - {rKey} \n" +
            $"G - {gKey} \n" +
            $"1 - {oneKey} \n" +
            $"2 - {twoKey} \n" +
            $"3 - {threeKey} \n";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            hideHints = !hideHints;
            hKey = hideHints ? "показать подсказки" : "спрятать подсказки";
            UpdateHints();
        }
    }

    void UpdateHints()
    {
        if (hideHints) 
        {
            text.text =
                "Управление: \n" +
                $"H - {hKey} \n";
        }
        else
        {
            text.text =
                "Управление: \n" +
                $"H - {hKey} \n" +
                "Shift - бежать \n" +
                $"C - {cKey} \n" +
                $"Е - {eKey} \n" +
                $"ЛКМ - {lmbKeyDown} \n" +
                $"ПКМ - {rmbKeyDown} \n" +
                $"R - {rKey} \n" +
                $"G - {gKey} \n" +
                $"1 - {oneKey} \n" +
                $"2 - {twoKey} \n" +
                $"3 - {threeKey} \n";
        }
    }

    public void UpdateEKeyHint()
    {
        eKey = wpnManager.ctx.IsControllingHand ? "деактивировать левую руку" : "активировать левую руку";
        lmbKeyDown = wpnManager.ctx.IsControllingHand ? "(удержать) взаимодействовать с предметом" : "стрелять";
        rmbKeyDown = wpnManager.ctx.IsControllingHand ? "в текущем состоянии ничего" : wpnManager.HeldWeapon != null ? "прицелиться" : "в текущем состоянии ничего";
        if (wpnManager.HeldWeapon == null)
        {
            rKey = "в текущем состоянии ничего";
            if (!wpnManager.ctx.IsControllingHand)
            {
                lmbKeyDown = "в текущем состоянии ничего";
            }
        }
        UpdateHints();
    }

    public void UpdateRKeyHint()
    {
        if (wpnManager.HeldWeapon != null && wpnManager.Reloading)
        {
            rKey = "выйти из режима перезарядки";
        }
        else if (wpnManager.HeldWeapon != null && !wpnManager.Reloading)
        {
            rKey = "войти в режим перезарядки";
        }
        UpdateHints();
    }

    public void UpdateLMBHint()
    {
        lmbKeyDown = wpnManager.ctx.IsControllingHand ? "(удержать) взаимодействовать с предметом" : "стрелять";
        UpdateHints();
    }

    public void UpdateCKey(bool isCrouching)
    {
        cKey = isCrouching ? "встать" : "присесть";
        UpdateHints();
    }

    public void UpdateWeaponHint(bool reloadHint = true)
    {
        lmbKeyDown = wpnManager.ctx.IsControllingHand ? "(удержать) взаимодействовать с предметом" : wpnManager.HeldWeapon != null ? "стрелять" : "в текущем состоянии ничего";
        rmbKeyDown = wpnManager.HeldWeapon != null && !wpnManager.ctx.IsControllingHand ? "прицелиться" : "в текущем состоянии ничего";
        if (wpnManager.HeldWeapon != null && reloadHint)
        {
            if (wpnManager.Reloading)
                rKey = "выйти из режима перезарядки";
            else
                rKey = "войти в режим перезарядки";
        }
        else
        {
            rKey = "в текущем состоянии ничего";
        }
        gKey = wpnManager.HeldWeapon != null ? "выбросить оружие" : "в текущем состоянии ничего";
        UpdateHints();
    }

    public void UpdateNumbersHint(int slotIndex)
    {
        if (slotIndex == 0)
        {
            if (wpnManager.weaponSlots[0].OccupiedSlot != null)
                oneKey = "достать основное оружие";
            else if (wpnManager.HeldWeapon != null || wpnManager.TargetItem != null)
                oneKey = "привязать основное оружие к слоту";
            else
                oneKey = "в текущем состоянии ничего";
        }
        else if (slotIndex == 1)
        {
            if (wpnManager.weaponSlots[1].OccupiedSlot != null)
                twoKey = "достать вторичное оружие";
            else if (wpnManager.HeldWeapon != null || wpnManager.TargetItem != null)
                twoKey = "привязать вторичное оружие к слоту";
            else
                twoKey = "в текущем состоянии ничего";
        }
        else if (slotIndex == 2)
        {
            if (wpnManager.weaponSlots[2].OccupiedSlot != null)
                threeKey = "достать холодное оружие";
            else if (wpnManager.HeldWeapon != null || wpnManager.TargetItem != null)
                threeKey = "привязать холодное оружие к слоту";
            else
                threeKey = "в текущем состоянии ничего";
        }
        UpdateHints();
    }
}
