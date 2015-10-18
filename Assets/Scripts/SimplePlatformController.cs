using UnityEngine;
using System.Collections;

public class SimplePlatformController : MonoBehaviour {

    [HideInInspector] public bool facingRight = true;           // sprite facing right
    [HideInInspector] public bool jump = true;                  // true for key pressed which is spacebar

    public float moveForce = 365f;                              // ?
    public float maxSpeed = 5f;                                 // clamp velocity
    public float jumpForce = 1000f;
    public Transform groundCheck;                               // line casting(2d) instead of ray casting (3d) to check player on ground
                                                                // this appears in the ground and we will check for no contact i.e. in the air

    private bool grounded = false;
    private Animator anim;                                      // store ref to anim component
    private Rigidbody2D rb2d;                                   // store ref to rigidbody component


	// Use this for initialization
	void Awake ()
	{
        // Get references to components
	    anim = GetComponent<Animator>();
	    rb2d = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        /* cast line to see if grounded, set to return value of LineCast ( if hit anything )
        * do it from start ( vector2 ) to transform at our feet, use 1 and bitwise left shift 
        * baseically only layer casting against
        */ 
	    grounded = Physics2D.Linecast( transform.position, groundCheck.position, 1 << LayerMask.NameToLayer( "Ground" ) );

        // check we are on the ground and jummp button pressed i.e. cannot double jump
	    if (Input.GetButtonDown( "Jump" ) && grounded)
	    {
	        jump = true;
	    }
    }

    void FixedUpdate()
    {
        // get horizontal axis
        float h = Input.GetAxis( "Horizontal" );

        // whether moving left or right , we will use the positive to set the speed
        anim.SetFloat( "Speed", Mathf.Abs( h ) );

        // Check for under speed limit
        if (h * rb2d.velocity.x < maxSpeed)
        {
            // Add force in the right direction, if -h then go left
            rb2d.AddForce( Vector2.right * h * moveForce );
        }

        // Check for max speed
        if (Mathf.Abs( rb2d.velocity.x ) > maxSpeed)
        {
            // set velocity directly, get direction 
            rb2d.velocity = new Vector2( Mathf.Sign( rb2d.velocity.x ) * maxSpeed, rb2d.velocity.y );
        }

        // flip sprite based on direction of movement
        if (h > 0 && !facingRight)
        {
            Flip();
        } else if (h < 0 && facingRight)
        {
            Flip();
        }

        // add force upwards on jump
        if (jump)
        {
            anim.SetTrigger( "Jump" );
            rb2d.AddForce( new Vector2( 0f, jumpForce ) );
            jump = false;   // no double jump
        }
        
    }

    // flip the sprite around on movement left, right
    void Flip()
	    {
	        facingRight = !facingRight;
            Vector3 theScale = transform.localScale;
	        theScale.x = -1;
            transform.localScale = theScale;

	    }
	
}
