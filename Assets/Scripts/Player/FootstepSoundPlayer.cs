using UnityEngine;

public class FootstepSoundPlayer : MonoBehaviour
{
    public Animator Animator;
    public SurfaceImpact SurfaceImpact;

    public LayerMask Environment;

    private float _lastFootstep;

    private void OnValidate()
    {
        if (!Animator) Animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        var footstep = Animator.GetFloat("Footstep");
        if (Mathf.Abs(footstep) < .00001f) footstep = 0f;

        if (_lastFootstep > 0 && footstep < 0 || _lastFootstep < 0 && footstep > 0)
        {
            var position = transform.position + Vector3.up * .01f;
            if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, .1f, Environment))
            {
                var rotation = Quaternion.LookRotation(Vector3.down, transform.forward);
                SurfaceManager.Instance.SpawnEffect(position, rotation, hit.normal, hit.collider, SurfaceImpact, transform);
            }
        }
        _lastFootstep = footstep;
    }
}
