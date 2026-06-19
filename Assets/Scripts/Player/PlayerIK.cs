using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace LlamAcademy.Guns.Demo
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class PlayerIK : MonoBehaviour
    {
        public bool debugWeaponRigging;

        public Transform LeftShoulderIKTarget;
        public Transform LeftElbowIKTarget;
        public Transform LeftHandIKTarget;
        public Transform RightShoulderIKTarget;
        public Transform RightElbowIKTarget;
        public Transform RightHandIKTarget;

        public Transform LeftFootIKTarget;
        public Transform RightFootIKTarget;

        [Header("Left fingers IK target")]
        public bool AcivateLeftFingersIK;
        public Transform LeftHandRigsParent;
        public Transform LeftWrist;

        public Transform LeftThumbProximal;
        public Transform LeftThumbIntermediate;
        public Transform LeftThumbDistal;

        public Transform LeftIndexProximal;
        public Transform LeftIndexIntermediate;
        public Transform LeftIndexDistal;

        public Transform LeftMiddleProximal;
        public Transform LeftMiddleIntermediate;
        public Transform LeftMiddleDistal;

        public Transform LeftRingProximal;
        public Transform LeftRingIntermediate;
        public Transform LeftRingDistal;

        public Transform LeftPinkyProximal;
        public Transform LeftPinkyIntermediate;
        public Transform LeftPinkyDistal;

        [Header("Right fingers IK target")]
        public bool AcivateRightFingersIK;
        public Transform RightHandRigsParent;
        public Transform RightWrist;

        public Transform RightThumbProximal;
        public Transform RightThumbIntermediate;
        public Transform RightThumbDistal;

        public Transform RightIndexProximal;
        public Transform RightIndexIntermediate;
        public Transform RightIndexDistal;

        public Transform RightMiddleProximal;
        public Transform RightMiddleIntermediate;
        public Transform RightMiddleDistal;

        public Transform RightRingProximal;
        public Transform RightRingIntermediate;
        public Transform RightRingDistal;

        public Transform RightPinkyProximal;
        public Transform RightPinkyIntermediate;
        public Transform RightPinkyDistal;

        [Range(0, 1f)]
        public float LeftArmIKAmount = 1f;
        [Range(0, 1f)]
        public float RightArmIKAmount = 1f;
        [Range(0, 1f)]
        public float LeftFootIKAmount = 1f;
        [Range(0, 1f)]
        public float RightFootIKAmount = 1f;

        Coroutine lerpCoroutine;
        Coroutine lerpCoroutine1;
        Coroutine leftHandRigCoroutine;
        Coroutine rightHandRigCoroutine;
        Coroutine footLerpCoroutine;
        Coroutine footLerpCoroutine1;
        Quaternion chestRotation;
        bool isChangingIK;
        [SerializeField] bool oneHandedWeapon;
        [SerializeField] bool controlRightHand;
        Transform leftHandHelper;
        Transform rightHandHelper;
        float timeElapsed;
        float elbowHintPosition;
        public bool ControlRightHand { get { return controlRightHand; } set { controlRightHand = value; } }
        public float ElbowHintPosition { get { return elbowHintPosition; } set { elbowHintPosition = value; } }
        public static PlayerIK instance;

        [SerializeField] private Animator Animator;
        [SerializeField] private Rig headRig;
        [SerializeField] private RigBuilder rigBuilder;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            //Animator = GetComponent<Animator>();
        }

        private void OnAnimatorIK(int layerIndex)
        {
            //Animator.SetBoneLocalRotation(HumanBodyBones.Head, Quaternion.LookRotation(ctx.cameraRoot.position))
            Animator.SetBoneLocalRotation(HumanBodyBones.Neck, Quaternion.AngleAxis(-chestRotation.eulerAngles.y, Vector3.up));

            if (LeftHandIKTarget != null)
            {
                Animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, LeftArmIKAmount);
                Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, LeftArmIKAmount);
                Animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandIKTarget.position);
                Animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandIKTarget.rotation);
            }
            if (RightHandIKTarget != null)
            {
                Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, RightArmIKAmount);
                Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, RightArmIKAmount);
                Animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandIKTarget.rotation);
                Animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandIKTarget.position);
            }
            if (LeftElbowIKTarget != null)
            {
                //Vector3 newElbowHintPos = new Vector3(LeftElbowIKTarget.localPosition.x, LeftElbowIKTarget.localPosition.y, elbowHintPosition);
                //LeftElbowIKTarget.localPosition = newElbowHintPos;
                Animator.SetIKHintPosition(AvatarIKHint.LeftElbow, LeftElbowIKTarget.position);
                Animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, LeftArmIKAmount);
            }
            if (RightElbowIKTarget != null)
            {
                Animator.SetIKHintPosition(AvatarIKHint.RightElbow, RightElbowIKTarget.position);
                Animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, RightArmIKAmount);
            }

            if (LeftShoulderIKTarget != null)
            {
                Animator.SetBoneLocalRotation(HumanBodyBones.LeftShoulder, Animator.GetBoneTransform(HumanBodyBones.LeftShoulder).localRotation * 
                    LeftShoulderIKTarget.localRotation);
            }

            if (LeftFootIKTarget != null)
            {
                Animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, LeftFootIKAmount);
                Animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, LeftFootIKAmount);
                Animator.SetIKPosition(AvatarIKGoal.LeftFoot, LeftFootIKTarget.position);
                Animator.SetIKRotation(AvatarIKGoal.LeftFoot, LeftFootIKTarget.rotation);
            }

            if (RightFootIKTarget != null)
            {
                Animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, RightFootIKAmount);
                Animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, RightFootIKAmount);
                Animator.SetIKRotation(AvatarIKGoal.RightFoot, RightFootIKTarget.rotation);
                Animator.SetIKPosition(AvatarIKGoal.RightFoot, RightFootIKTarget.position);
            }
            
            //Animator.SetBoneLocalRotation(HumanBodyBones.Chest, Quaternion.Euler(0.0f, 45f, 0.0f));
            if (RightShoulderIKTarget != null && RightArmIKAmount == 1)
            {
                Animator.SetBoneLocalRotation(HumanBodyBones.RightShoulder, RightShoulderIKTarget.localRotation);
            }

            if (Animator.GetBool("isControllingHand") || AcivateLeftFingersIK && !oneHandedWeapon || debugWeaponRigging)
            {
                Animator.SetBoneLocalRotation(HumanBodyBones.LeftThumbProximal, LeftThumbProximal.localRotation);
                Animator.SetBoneLocalRotation(HumanBodyBones.LeftThumbIntermediate, LeftThumbIntermediate.localRotation);
                Animator.SetBoneLocalRotation(HumanBodyBones.LeftThumbDistal, LeftThumbDistal.localRotation);

                Animator.SetBoneLocalRotation(HumanBodyBones.LeftIndexProximal, LeftIndexProximal.localRotation);
                Animator.SetBoneLocalRotation(HumanBodyBones.LeftIndexIntermediate, LeftIndexIntermediate.localRotation);
                Animator.SetBoneLocalRotation(HumanBodyBones.LeftIndexDistal, LeftIndexDistal.localRotation);

                Animator.SetBoneLocalRotation(HumanBodyBones.LeftMiddleProximal, LeftMiddleProximal.localRotation);
                Animator.SetBoneLocalRotation(HumanBodyBones.LeftMiddleIntermediate, LeftMiddleIntermediate.localRotation);
                Animator.SetBoneLocalRotation(HumanBodyBones.LeftMiddleDistal, LeftMiddleDistal.localRotation);

                Animator.SetBoneLocalRotation(HumanBodyBones.LeftRingProximal, LeftRingProximal.localRotation);
                Animator.SetBoneLocalRotation(HumanBodyBones.LeftRingIntermediate, LeftRingIntermediate.localRotation);
                Animator.SetBoneLocalRotation(HumanBodyBones.LeftRingDistal, LeftRingDistal.localRotation);

                Animator.SetBoneLocalRotation(HumanBodyBones.LeftLittleProximal, LeftPinkyProximal.localRotation);
                Animator.SetBoneLocalRotation(HumanBodyBones.LeftLittleIntermediate, LeftPinkyIntermediate.localRotation);
                Animator.SetBoneLocalRotation(HumanBodyBones.LeftLittleDistal, LeftPinkyDistal.localRotation);
            }

            if (AcivateRightFingersIK)
            {
                if (LeftShoulderIKTarget != null) Animator.SetBoneLocalRotation(HumanBodyBones.LeftShoulder, LeftShoulderIKTarget.localRotation);

                Animator.SetBoneLocalRotation(HumanBodyBones.RightThumbProximal, RightThumbProximal.localRotation);
                Animator.SetBoneLocalRotation(HumanBodyBones.RightThumbIntermediate, RightThumbIntermediate.localRotation);
                Animator.SetBoneLocalRotation(HumanBodyBones.RightThumbDistal, RightThumbDistal.localRotation);

                Animator.SetBoneLocalRotation(HumanBodyBones.RightIndexProximal, RightIndexProximal.localRotation);
                Animator.SetBoneLocalRotation(HumanBodyBones.RightIndexIntermediate, RightIndexIntermediate.localRotation);
                Animator.SetBoneLocalRotation(HumanBodyBones.RightIndexDistal, RightIndexDistal.localRotation);

                Animator.SetBoneLocalRotation(HumanBodyBones.RightMiddleProximal, RightMiddleProximal.localRotation);
                Animator.SetBoneLocalRotation(HumanBodyBones.RightMiddleIntermediate, RightMiddleIntermediate.localRotation);
                Animator.SetBoneLocalRotation(HumanBodyBones.RightMiddleDistal, RightMiddleDistal.localRotation);

                Animator.SetBoneLocalRotation(HumanBodyBones.RightRingProximal, RightRingProximal.localRotation );
                Animator.SetBoneLocalRotation(HumanBodyBones.RightRingIntermediate, RightRingIntermediate.localRotation);
                Animator.SetBoneLocalRotation(HumanBodyBones.RightRingDistal, RightRingDistal.localRotation);

                Animator.SetBoneLocalRotation(HumanBodyBones.RightLittleProximal, RightPinkyProximal.localRotation);
                Animator.SetBoneLocalRotation(HumanBodyBones.RightLittleIntermediate, RightPinkyIntermediate.localRotation);
                Animator.SetBoneLocalRotation(HumanBodyBones.RightLittleDistal, RightPinkyDistal.localRotation);
            }
        }

        public void SetGunStyle(bool OneHanded)
        {
            oneHandedWeapon = OneHanded;
        }

        public void Setup(Transform GunParent)
        {
            Transform[] allChildren = GunParent.GetComponentsInChildren<Transform>();
            LeftElbowIKTarget = allChildren.FirstOrDefault(child => child.name == "LeftElbow");
            RightElbowIKTarget = allChildren.FirstOrDefault(child => child.name == "RightElbow");
            LeftHandIKTarget = allChildren.FirstOrDefault(child => child.name == "LeftHand");
            RightHandIKTarget = allChildren.FirstOrDefault(child => child.name == "RightHand");
        }

        public void ChangeLeftHandIKValue(float targetValue, float time)
        {
            if (lerpCoroutine != null)
            {
                StopCoroutine(lerpCoroutine);
                lerpCoroutine = null;
            }
            lerpCoroutine = StartCoroutine(LerpEnumerator.OnUpdate((t) => LeftArmIKAmount = Mathf.SmoothStep(LeftArmIKAmount, targetValue, t), time));
        }

        public void ChangeRightHandIKValue(float targetValue, float time)
        {
            if (lerpCoroutine1 != null)
            {
                StopCoroutine(lerpCoroutine1);
                lerpCoroutine1 = null;
            }
            lerpCoroutine1 = StartCoroutine(LerpEnumerator.OnUpdate((t) => RightArmIKAmount = Mathf.SmoothStep(RightArmIKAmount, targetValue, t), time));
        }

        public void ChangeLeftFootIKValue(float targetValue, float time)
        {
            if (footLerpCoroutine != null)
            {
                StopCoroutine(footLerpCoroutine);
                footLerpCoroutine = null;
            }
            footLerpCoroutine = StartCoroutine(LerpEnumerator.OnUpdate((t) => LeftFootIKAmount = Mathf.SmoothStep(LeftFootIKAmount, targetValue, t), time));
        }

        public void ChangeRightFootIKValue(float targetValue, float time)
        {
            if (footLerpCoroutine1 != null)
            {
                StopCoroutine(footLerpCoroutine1);
                footLerpCoroutine1 = null;
            }
            footLerpCoroutine1 = StartCoroutine(LerpEnumerator.OnUpdate((t) => RightFootIKAmount = Mathf.SmoothStep(RightFootIKAmount, targetValue, t), time));
        }

        public void SetupRig(Transform RigParent, int variants)
        {
            List<Transform> allChildren = new (RigParent.GetComponentsInChildren<Transform>());
            //StopCoroutine(SetupLeftHandRig(allChildren, RigParent));

            if (variants == 0)
            {
                if (leftHandRigCoroutine != null)
                {
                    StopCoroutine(leftHandRigCoroutine);
                    leftHandRigCoroutine = null;
                }
                SetupLeftHandRig(allChildren);
            }
            else if (variants == 1) 
            {
                if (rightHandRigCoroutine != null)
                {
                    StopCoroutine(rightHandRigCoroutine);
                    rightHandRigCoroutine = null;
                }
                SetupRightHandRig(allChildren);
            }
            else if (variants == 2)
            {
                SetupLeftHandRig(allChildren);
                SetupRightHandRig(allChildren);
            }
        }

        void SetupLeftHandRig(List<Transform> allChildren)
        {
            if (allChildren == null || allChildren.Count == 0) return;
            Dictionary<string, Transform> childDict = new (allChildren.Count);

            foreach (var child in allChildren)
            {
                if (!childDict.ContainsKey(child.name)) childDict.Add(child.name, child);
            }

            //LeftHandIKTarget = allChildren.FirstOrDefault(child => child.name == "LeftWristTransform");
            childDict.TryGetValue("LeftThumbProximal", out LeftThumbProximal);
            childDict.TryGetValue("LeftThumbIntermediate", out LeftThumbIntermediate);
            childDict.TryGetValue("LeftThumbDistal", out LeftThumbDistal);

            childDict.TryGetValue("LeftHandIndexProximal", out LeftIndexProximal);
            childDict.TryGetValue("LeftHandIndexIntermediate", out LeftIndexIntermediate);
            childDict.TryGetValue("LeftHandIndexDistal", out LeftIndexDistal);

            childDict.TryGetValue("LeftHandMiddleProximal", out LeftMiddleProximal);
            childDict.TryGetValue("LeftHandMiddleIntermediate", out LeftMiddleIntermediate);
            childDict.TryGetValue("LeftHandMiddleDistal", out LeftMiddleDistal);

            childDict.TryGetValue("LeftHandRingProximal", out LeftRingProximal);
            childDict.TryGetValue("LeftHandRingIntermediate", out LeftRingIntermediate);
            childDict.TryGetValue("LeftHandRingDistal", out LeftRingDistal);

            childDict.TryGetValue("LeftHandPinkyProximal", out LeftPinkyProximal);
            childDict.TryGetValue("LeftHandPinkyIntermediate", out LeftPinkyIntermediate);
            childDict.TryGetValue("LeftHandPinkyDistal", out LeftPinkyDistal);

            childDict.TryGetValue("LeftShoulderRotation", out LeftShoulderIKTarget);
            //LeftElbowIKTarget = allChildren.FirstOrDefault(child => child.name == "LeftElbowHint");

            LeftHandRigsParent.SetPositionAndRotation(LeftHandIKTarget.position, LeftHandIKTarget.rotation);
            LeftHandIKTarget = LeftHandRigsParent;
            childDict.TryGetValue("LeftHand", out Transform targetIK);

            leftHandRigCoroutine = StartCoroutine(
                LerpEnumerator.OnUpdate(
                    (t) => LeftHandIKTarget.SetPositionAndRotation(Vector3.Lerp(LeftHandIKTarget.position, targetIK.position, t),
                        LeftHandIKTarget.rotation = Quaternion.Lerp(LeftHandIKTarget.rotation, targetIK.rotation, t)),
                    () => LeftHandIKTarget = targetIK,
                    0.5f
                )
            );
        }

        void SetupRightHandRig(List<Transform> allChildren)
        {
            if (allChildren == null || allChildren.Count == 0) return;
            Dictionary<string, Transform> childDict = new (allChildren.Count);

            foreach (var child in allChildren)
            {
                if (!childDict.ContainsKey(child.name)) childDict.Add(child.name, child);
            }

            //LeftHandIKTarget = allChildren.FirstOrDefault(child => child.name == "LeftWristTransform");
            childDict.TryGetValue("RightThumbProximal", out RightThumbProximal);
            childDict.TryGetValue("RightThumbIntermediate", out RightThumbIntermediate);
            childDict.TryGetValue("RightThumbDistal", out RightThumbDistal);

            childDict.TryGetValue("RightHandIndexProximal", out RightIndexProximal);
            childDict.TryGetValue("RightHandIndexIntermediate", out RightIndexIntermediate);
            childDict.TryGetValue("RightHandIndexDistal", out RightIndexDistal);

            childDict.TryGetValue("RightHandMiddleProximal", out RightMiddleProximal);
            childDict.TryGetValue("RightHandMiddleIntermediate", out RightMiddleIntermediate);
            childDict.TryGetValue("RightHandMiddleDistal", out RightMiddleDistal);

            childDict.TryGetValue("RightHandRingProximal", out RightRingProximal);
            childDict.TryGetValue("RightHandRingIntermediate", out RightRingIntermediate);
            childDict.TryGetValue("RightHandRingDistal", out RightRingDistal);

            childDict.TryGetValue("RightHandPinkyProximal", out RightPinkyProximal);
            childDict.TryGetValue("RightHandPinkyIntermediate", out RightPinkyIntermediate);
            childDict.TryGetValue("RightHandPinkyDistal", out RightPinkyDistal);

            childDict.TryGetValue("RightShoulderRotation", out RightShoulderIKTarget);
            childDict.TryGetValue("RightElbowHint", out RightElbowIKTarget);

            RightHandRigsParent.SetPositionAndRotation(RightHandIKTarget.position, RightHandIKTarget.rotation);
            RightHandIKTarget = RightHandRigsParent;
            childDict.TryGetValue("RightHand", out Transform targetIK);

            rightHandRigCoroutine = StartCoroutine(
                LerpEnumerator.OnUpdate(
                    (t) => RightHandIKTarget.SetPositionAndRotation(Vector3.Lerp(RightHandIKTarget.position, targetIK.position, t),
                        RightHandIKTarget.rotation = Quaternion.Lerp(RightHandIKTarget.rotation, targetIK.rotation, t)),
                    () => RightHandIKTarget = targetIK,
                    0.5f
                )
            );
        }

        void SetupRightHandRigOld(Transform[] allChildren)
        {
            RightHandIKTarget = allChildren.FirstOrDefault(child => child.name == "RightHand");

            RightElbowIKTarget = allChildren.FirstOrDefault(child => child.name == "RightElbowHint");

            RightThumbProximal = allChildren.FirstOrDefault(child => child.name == "RightThumbProximal");
            RightThumbIntermediate = allChildren.FirstOrDefault(child => child.name == "RightThumbIntermediate");
            RightThumbDistal = allChildren.FirstOrDefault(child => child.name == "RightThumbDistal");

            RightIndexProximal = allChildren.FirstOrDefault(child => child.name == "RightHandIndexProximal");
            RightIndexIntermediate = allChildren.FirstOrDefault(child => child.name == "RightHandIndexIntermediate");
            RightIndexDistal = allChildren.FirstOrDefault(child => child.name == "RightHandIndexDistal");

            RightMiddleProximal = allChildren.FirstOrDefault(child => child.name == "RightHandMiddleProximal");
            RightMiddleIntermediate = allChildren.FirstOrDefault(child => child.name == "RightHandMiddleIntermediate");
            RightMiddleDistal = allChildren.FirstOrDefault(child => child.name == "RightHandMiddleDistal");

            RightRingProximal = allChildren.FirstOrDefault(child => child.name == "RightHandRingProximal");
            RightRingIntermediate = allChildren.FirstOrDefault(child => child.name == "RightHandRingIntermediate");
            RightRingDistal = allChildren.FirstOrDefault(child => child.name == "RightHandRingDistal");

            RightPinkyProximal = allChildren.FirstOrDefault(child => child.name == "RightHandPinkyProximal");
            RightPinkyIntermediate = allChildren.FirstOrDefault(child => child.name == "RightHandPinkyIntermediate");
            RightPinkyDistal = allChildren.FirstOrDefault(child => child.name == "RightHandPinkyDistal");

            RightShoulderIKTarget = allChildren.FirstOrDefault(child => child.name == "RightShoulderRotation");

            //UpperChestIKTarget = allChildren.FirstOrDefault(child => child.name == "UpperChestAim");
        }

        public void SetupLeftLeg(Transform leftLegIKTarget)
        {
            LeftFootIKTarget = leftLegIKTarget;
        }

        public void SetupRightLeg(Transform rightLegIKTarget)
        {
            RightFootIKTarget = rightLegIKTarget;
        }
    }
}
