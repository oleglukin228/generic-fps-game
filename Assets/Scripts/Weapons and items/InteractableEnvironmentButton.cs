using LlamAcademy.Guns.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableEnvironmentButton : InteractableEnvironment
{
    float timeElapsed = 0;
    public override void Pickup(PlayerIK playerIK, WeaponManagerFullBody wpnManager)
    {
        base.Pickup(playerIK, wpnManager);
        whoIsHolding.IsBusy = true;
        StartCoroutine(ActivateButton(playerIK, wpnManager));
    }

    public override void MoveItem(Transform position)
    {
        //base.MoveItem(position);
    }

    IEnumerator ActivateButton(PlayerIK playerIK, WeaponManagerFullBody wpnManager)
    {
        timeElapsed = 0;
        while (timeElapsed < 0.2f)
        {
            float t = timeElapsed / 0.2f;
            transform.position = Vector3.Lerp(transform.position, interactableActions.items[0].tweens[0].transformEndPosition.position, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        interactableActions.items[0].onEndActionTrigger.Invoke();
        while (timeElapsed > 0f)
        {
            float t = timeElapsed / 0.2f;
            transform.position = Vector3.Lerp(transform.position, interactableActions.items[0].tweens[0].transformStartPosition.position, t);
            timeElapsed -= Time.deltaTime;
            yield return null;
        }
        whoIsHolding.IsBusy = false;
        DropItem(playerIK, wpnManager);
    }
}
