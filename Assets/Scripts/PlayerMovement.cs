using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    //Public variables to define player movement feel
    public bool hasInvertedControls = false;
    public float runSpeed = 40f;
    public int numOfJumps = 2;
    public float dashSpeed = 10f;
    public float dashDistance = 5f;
    public float jumpForce = 100f;

    //Vars to handle movement smoothing
    [Range(0, .3f)] private float moveSmoothing = 0.05f;
    private Vector3 charVelocity = Vector3.zero;

    //Private vars to determine what the player is currently doing
    private int jumpCount = 0;
    private bool canJump;
    private bool dashing = false;
    private Vector3 finalDashLocation;
    private bool facingRight = true;
    private bool grounded = true;

    float horizontalMove = 0f;
    bool jump = false;
    bool crouch = false;
    bool dash = false;

    Rigidbody2D rb;
    PolygonCollider2D playerCollider;
    


    void Start() {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<PolygonCollider2D>();
    }

    private void Update() {

        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        if (hasInvertedControls) {
            horizontalMove *= -1;
        }

        if (Input.GetButtonDown("Jump")) {
            jump = true;
        }

    }

    void FixedUpdate() {
        canJump = jumpCount <= numOfJumps && grounded;

        Move(horizontalMove * Time.fixedDeltaTime, jump, canJump);
        jump = false;

    }

    private void Move(float move, bool jump, bool canJump) {

        //Find target velocity based on input move amount
        Vector3 targetVelocity = new Vector2(move * 10f, rb.velocity.y);
        //Smooth movement and apply to character
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref charVelocity, moveSmoothing);


        //Runs Flip() to keep player facing correct direction
        if(move > 0 && !facingRight) {
            Flip();
        }else if (move < 0 && facingRight) {
            Flip();
        }
        
        //If player can and should Jump
        if(canJump && jump) {
            //Remove any vertical momentum and add jump force
            grounded = false;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0f, jumpForce));
        }

    }

    private void Flip() {
        //Keeps facingRight var accurate
        facingRight = !facingRight;

        //Multiplies character x scale by -1 to turn it in the opposite direction
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        Debug.Log("touching " + collision.gameObject.layer);

        //Check if player is touching layer 8 (ground)
        if (collision.gameObject.layer == 8) {
            grounded = true;
            Debug.Log("touching ground");
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        Debug.Log("leaving " + collision.gameObject.layer);
        //Check if player is leaving layer 8 (ground)
        if (collision.gameObject.layer == 8) {
            grounded = false;
            Debug.Log("leaving ground");
        }
    }
}
