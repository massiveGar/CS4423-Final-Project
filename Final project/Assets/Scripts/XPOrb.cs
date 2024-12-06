using UnityEngine;

public class XPOrb : MonoBehaviour
{
    public float maxForce = 5f;        // Maximum force for the fling
    public float angleRange = 45f;     // Range of random angles (degrees)
    public float bounceDamping = 0.8f; // How much velocity is retained after bouncing
    public float floorHeight;   // The Y position of the "floor" to bounce on
    public float groundCheckDistance = 0.2f;  // Distance to detect the virtual floor

    private Rigidbody2D rb;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        floorHeight = transform.position.y - 1f;

        FlingOrb();
    }

    void FlingOrb() {
        // Randomize the force strength between 0 and maxForce
        float forceStrength = Random.Range(0, maxForce);

        // Randomize the direction within the specified angle range
        float randomAngle = Random.Range(-angleRange, angleRange);
        Vector3 flingDirection = Quaternion.Euler(0, randomAngle, 0) * Vector3.up;

        // Apply the force to fling the object
        rb.velocity = Vector3.zero; // Reset velocity before applying new force
        rb.AddForce(flingDirection * forceStrength, ForceMode2D.Impulse);
    }

    void Update() {
        // Simulate bounce when below the virtual floor height
        if(transform.position.y <= floorHeight) {
            // Reverse the Y velocity to simulate bounce and apply damping
            Vector3 velocity = rb.velocity;
            velocity.y = Mathf.Abs(velocity.y) * bounceDamping;  // Ensure upward direction with damping
            rb.velocity = velocity;
        }
    }
}