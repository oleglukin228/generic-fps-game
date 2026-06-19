using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    Rigidbody[] rigidBodies;
    public GameObject[] gibs;
    public GameObject gibsParticle;
    private bool exploded;
    Animator animator;
    BoxCollider collision;

    // Start is called before the first frame update
    void Start()
    {
        rigidBodies = transform.GetComponentsInChildren<Rigidbody>();
        animator = GetComponent<Animator>();
        collision = GetComponent<BoxCollider>();

        DeactivateRagdoll();
        //ActivateRagdoll();
    }

    public void DeactivateRagdoll()
    {
        collision.enabled = true;
        foreach (var rigidBody in rigidBodies)
        {
            rigidBody.isKinematic = true;
        }
        animator.enabled = true;
    }

    public void ActivateRagdoll()
    {
        collision.enabled = false;
        foreach (var rigidBody in rigidBodies)
        {
            rigidBody.isKinematic = false;
        }
        animator.enabled = false;
    }

    public void ApplyForce(Vector3 force, Vector3 hitPoint)
    {
        var rigidBody = animator.GetBoneTransform(HumanBodyBones.Hips).GetComponent<Rigidbody>();
        //Debug.Log(force  + " " + hitPoint);
        rigidBody.AddForceAtPosition(transform.position - force, hitPoint, ForceMode.Impulse);
    }

    public void TearFagToPieces(Vector3 force, Vector3 hitPoint)
    {
        if (!exploded)
        {
            exploded = true;
            animator.enabled = false;
            collision.enabled = false;
            foreach (var gib in gibs)
            {
                GameObject currentGib = Instantiate(gib, animator.GetBoneTransform(HumanBodyBones.Hips).position, Quaternion.identity);
                ApplyForce(force, hitPoint);
                currentGib.GetComponent<Rigidbody>().AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
            }
            animator.GetBoneTransform(HumanBodyBones.Hips).rotation = Quaternion.identity;
            GameObject currentParticle = Instantiate(gibsParticle, animator.GetBoneTransform(HumanBodyBones.Hips).position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public void EnableBoxCollider(bool b)
    {
        if (b)
        {
            collision.enabled = true;
        }
        else
        {
            collision.enabled = false;
        }
    }
}
