using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rb;
    [SerializeField]
    private LayerMask groundLayer;

    private readonly Vector3 ROTATED_LEFT = new Vector3(0, 360, 0); //180 on y 360 for testing with placeholder
    private readonly Vector3 ROTATED_RIGHT = Vector3.zero;

    private float speed = 0f;
    private float maxSpeed = 3f;
    private float jumpingSpeed = 6f;
    private float direction;

    private bool isJumping = false;
    private bool isFacingRight = true;
    private bool grounded = true;

    public void Move(float dir)
    {
        // dir -> -1 = left, 1 = right, 0 = none
        if (dir == 0)
        {
            this.StopMoving();
            return;
        }

        if (this.direction != dir && dir != 0)
        {
            this.speed = 0;
        }

        if (dir < 0 && this.isFacingRight)
        {
            this.isFacingRight = false;
            this.transform.eulerAngles = this.ROTATED_LEFT;
        }
        else if (dir > 0 && !this.isFacingRight)
        {
            this.isFacingRight = true;
            this.transform.eulerAngles = this.ROTATED_RIGHT;
        }

        this.direction = dir;
        
        float acceleration = 10;
        this.speed = Mathf.Clamp(this.speed + Time.deltaTime * acceleration * this.direction, -this.maxSpeed, this.maxSpeed);
        this.rb.velocity = new Vector2(this.speed, this.rb.velocity.y);
    }

    public void StopMoving()
    {
        if (this.speed != 0)
        {
            float min = this.direction == 1 ? 0 : -this.maxSpeed;
            float max = this.direction == -1 ? 0 : this.maxSpeed;

            this.speed = Mathf.Clamp(this.speed + Time.deltaTime * 20 * this.direction * -1, min, max);
        }
        this.rb.velocity = new Vector2(this.speed, this.rb.velocity.y);
    }

    public void Jump()
    {
        if (this.isJumping)
        {
            this.rb.velocity = new Vector2(rb.velocity.x, this.rb.velocity.y - Time.deltaTime * 2);
        }
        else
        {
            this.rb.velocity = new Vector2(rb.velocity.x, this.jumpingSpeed);
            this.isJumping = true;
            this.grounded = false;
        }
    }

    public bool PlayerIsGrounded()
    {
        if (this.grounded)
        {
            this.isJumping = false;
            return true;
        }

        return false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (1 << collision.collider.gameObject.layer == this.groundLayer.value)
        {
            this.grounded = true;
            ContactPoint2D[] allPoints = new ContactPoint2D[collision.contactCount];
            collision.GetContacts(allPoints);

            foreach (var i in allPoints)
            {
                if (i.point.y >= transform.position.y)
                {
                    this.grounded = false;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (1 << collision.collider.gameObject.layer == this.groundLayer.value)
        {
            this.grounded = false;
        }
    }
}
