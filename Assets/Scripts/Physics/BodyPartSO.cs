using UnityEngine;

[CreateAssetMenu(menuName = "Generic fps/Body part info")]
public class BodyPartSO : ScriptableObject
{
    public string bodyPartName = "Body";
    public float damageMultiplier = 1f;
}
