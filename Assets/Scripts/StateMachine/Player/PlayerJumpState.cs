using UnityEngine;

/*public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerController currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
        InitializeSubState();
    }
    public override void CheckSwitchStates()
    {
        if (Ctx.IsGrounded && !Ctx.animator.GetBool("isJumping"))
        {
            SwitchState(Factory.Grounded());
        }
        else if (!Ctx.IsGrounded && Ctx.controller.velocity.y < -2f)
        {
            SwitchState(Factory.Falling());
        }
    }

    public override void EnterState()
    {
        Ctx.animator.SetBool("isJumping", true);
        Ctx.GravityVelocity = Mathf.Sqrt(Ctx.jumpHeight * -2f * Ctx.gravity);
    }

    public override void ExitState()
    {
        Ctx.animator.SetBool("isJumping", false);
        Ctx.TargetSpeed = Ctx.IsRunPressed ? Ctx.sprintSpeed : Ctx.defaultSpeed;
    }

    public override void InitializeSubState()
    {
        
    }

    public override void UpdateState()
    {
        Ctx.GravityVelocity += Ctx.gravity * Time.deltaTime;
        Ctx.controller.Move(Ctx.VelocityDirection * Time.deltaTime);
        CheckSwitchStates();
    }
}*/
