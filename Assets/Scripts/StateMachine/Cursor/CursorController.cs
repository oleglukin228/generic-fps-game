using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    static Vector3 mousePosition;
    public PlayerController playerStateMachine;
    public Camera playerCamera;
    public static float aimSensitivity = 3f;
    public static float cursorSensitivity = 30f;
    public static float swaySensitivity = 100f;
    static InteractableActions interactableActions;
    float animatorXMouseAxis;
    private static Transform _cursor; 
    private static StateMachine<ECursorState> stateMachine;
    public static ECursorState? PreviousState => stateMachine.PreviousStateId;
    public static float CurrentSensitivity { get; private set; }
    public static bool IsCursorEnabled { get; private set; }
    public static Vector3 MouseAxis { get { return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); } }
    public static Vector3 MousePosition { get { return mousePosition; } }
    public static float SwayBoundsX { get; private set; }
    public static bool RMBdown { get { return Input.GetKeyDown(KeyCode.Mouse1); } }
    public static bool RMBhold { get { return Input.GetKey(KeyCode.Mouse1); } }
    public static bool RMBup { get { return Input.GetKeyUp(KeyCode.Mouse1); } }
    public static bool LMBdown { get { return Input.GetKeyDown(KeyCode.Mouse0); } }
    public static bool LMBhold { get { return Input.GetKey(KeyCode.Mouse0); } }
    public static bool LMBup { get { return Input.GetKeyUp(KeyCode.Mouse0); } }
    public static bool IsInteracting => interactableActions != null;
    private static bool _lockCursor;

    private void Awake()
    {
        stateMachine = gameObject.AddComponent<CursorStateMachine>();
        _cursor = playerStateMachine.weaponManager.cursor;
    }

    private void Start()
    {
        //wpnManager = GetComponent<WeaponManagerFullBody>();
        CurrentSensitivity = aimSensitivity;
        mousePosition.x = playerCamera.pixelWidth / 2f;
        mousePosition.y = playerCamera.pixelHeight / 2f;
        SwayBoundsX = (playerCamera.pixelWidth / 1.8f) - (playerCamera.pixelWidth / 2.2f);
        swaySensitivity = SwayBoundsX;
    }

    public void OnLiveCursorUpdate()
    {
        SwayBoundsX = (playerCamera.pixelWidth / 1.8f) - (playerCamera.pixelWidth / 2.2f);
        
        if (Input.GetKey(KeyCode.Q)) return;
        animatorXMouseAxis += MouseAxis.x * 5f * Time.deltaTime;
        playerStateMachine.animator.SetFloat("X_MouseAxis", animatorXMouseAxis);
        animatorXMouseAxis = Mathf.Lerp(animatorXMouseAxis, 0f, 3f * Time.deltaTime);

        if (IsCursorEnabled)
        {
            if (_lockCursor) return;
            mousePosition += MouseAxis * cursorSensitivity;

            if (mousePosition.x > playerCamera.pixelWidth)
                playerStateMachine.transform.Rotate(Vector3.up, MouseAxis.x * CurrentSensitivity, Space.Self);
            else if (mousePosition.x < 0)
                playerStateMachine.transform.Rotate(Vector3.up, MouseAxis.x * CurrentSensitivity, Space.Self);
            if (mousePosition.y > playerCamera.pixelHeight)
                playerStateMachine.XRotaion -= MouseAxis.y * CurrentSensitivity;
            else if (mousePosition.y < 0)
                playerStateMachine.XRotaion -= MouseAxis.y * CurrentSensitivity;

            mousePosition.x = Mathf.Clamp(mousePosition.x, 0f, playerCamera.pixelWidth);
            mousePosition.y = Mathf.Clamp(mousePosition.y, 0f, playerCamera.pixelHeight);
        }
        else
        {
            mousePosition.x = playerCamera.pixelWidth / 2f;
            mousePosition.y = playerCamera.pixelHeight / 2f;
            //mousePosition.x += MouseAxis.x * cursorSensitivity;
            //mousePosition.y += MouseAxis.y * cursorSensitivity;
            playerStateMachine.XRotaion -= MouseAxis.y * CurrentSensitivity;
            playerStateMachine.transform.Rotate(Vector3.up, MouseAxis.x * CurrentSensitivity, Space.Self);
        }
    }

    public void OnKnockdownCursorUpdate()
    {
        SwayBoundsX = (playerCamera.pixelWidth / 1.8f) - (playerCamera.pixelWidth / 2.2f);
        
        mousePosition += MouseAxis * cursorSensitivity;

        if (mousePosition.y > playerCamera.pixelHeight)
            playerStateMachine.XRotaion -= MouseAxis.y * CurrentSensitivity;
        else if (mousePosition.y < 0)
            playerStateMachine.XRotaion -= MouseAxis.y * CurrentSensitivity;

        mousePosition.x = Mathf.Clamp(mousePosition.x, 0f, playerCamera.pixelWidth);
        mousePosition.y = Mathf.Clamp(mousePosition.y, 0f, playerCamera.pixelHeight);
    }

    public static void ResetSensitivity() => CurrentSensitivity = aimSensitivity;
    public static void MultiplySensitivity(float multiplier) => CurrentSensitivity = aimSensitivity * multiplier;
    public static void SetInteractableActions(InteractableActions action = null, bool lockCursor = false)
    {
        interactableActions = action;
        _lockCursor = lockCursor;
    }
    public static void EnableCursor(bool enabled = true) => IsCursorEnabled = enabled;
    public static void SetState(ECursorState? state) => stateMachine.SetState(state);
}
