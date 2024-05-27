using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    [SerializeField]
    private enum PLAYER_STATES
    {
        STANDING,
        WALKING,
        JUMPING,
        FOCUSING_SCREEN,
        FOCUSING_MOUSE
    }

    [SerializeField]
    private PLAYER_STATES state;
    
    [SerializeField]
    private PlayerMovement movement;

    [SerializeField]
    private FocusablesManager focusablesManager;

    private float inputX;
    private bool jump;

    void Start()
    {
        this.state = PLAYER_STATES.STANDING;    
    }

    void Update()
    {
        this.ChangeStateBasedOnInput();

        switch (this.state)
        {
            case PLAYER_STATES.STANDING:
                this.movement.StopMoving();
                break;

            case PLAYER_STATES.WALKING:
                this.movement.Move(this.inputX);
                break;
            case PLAYER_STATES.JUMPING:
                this.movement.Move(this.inputX);
                this.movement.Jump();
                break;
            
            case PLAYER_STATES.FOCUSING_SCREEN:    
                this.focusablesManager.StartFocusing();
                this.movement.StopMoving();
                break;
            
            case PLAYER_STATES.FOCUSING_MOUSE:
                this.focusablesManager.StartFocusingSpecific();
                this.movement.Move(this.inputX);
                if (this.jump)
                {
                    this.movement.Jump();
                }
                break;
        }
    }

    private void ChangeStateBasedOnInput()
    {
        PLAYER_STATES prevState = this.state;
        bool onGround = this.movement.PlayerIsGrounded();

        this.inputX = Input.GetAxisRaw("Horizontal");
        this.jump = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) && onGround;
        bool focus = Input.GetKey(KeyCode.Q) && onGround;
        bool focusSpecific = Input.GetKey(KeyCode.Space) && !focus;

        if (this.inputX == 0 && onGround)
        {
            this.state = PLAYER_STATES.STANDING;
        }

        if (this.inputX != 0 && onGround)
        {
            this.state = PLAYER_STATES.WALKING;
        }

        if (this.jump)
        {
            this.state = PLAYER_STATES.JUMPING;
        }

        if (focus)
        {
            this.state = PLAYER_STATES.FOCUSING_SCREEN;
        }
        else if (prevState == PLAYER_STATES.FOCUSING_SCREEN)
        {
            this.focusablesManager.StopFocusing();
        }

        if (focusSpecific)
        {
            this.state = PLAYER_STATES.FOCUSING_MOUSE;
        }
        else if (prevState == PLAYER_STATES.FOCUSING_MOUSE)
        {
            this.focusablesManager.StopFocusing();
            this.state = PLAYER_STATES.WALKING;
        }
    }
}
