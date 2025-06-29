using UnityEngine;

/// <summary>
/// Simple projectile script that demonstrates CCD behavior.
/// Shows visual effects when CCD collisions occur.
/// </summary>
public class CCDProjectile : MonoBehaviour
{
    [Header("Visual Effects")]
    public GameObject impactEffect;
    public TrailRenderer trail;
    
    [Header("Settings")]
    public bool destroyOnImpact = true;
    public float lifetime = 5.0f;
    
    private VolatileBody volatileBody;
    private bool hasCollided = false;
    
    void Start()
    {
        volatileBody = GetComponent<VolatileBody>();
        
        // Auto-destroy after lifetime
        Destroy(gameObject, lifetime);
        
        // Set up trail if available
        if (trail == null)
            trail = GetComponent<TrailRenderer>();
    }
    
    void Update()
    {
        // Check if we've stopped moving (likely due to collision)
        if (volatileBody != null && !hasCollided)
        {
            float speed = volatileBody.Body.LinearVelocity.magnitude;
            if (speed < 0.1f) // Very slow, likely collided
            {
                OnImpact();
            }
        }
    }
    
    void OnImpact()
    {
        if (hasCollided) return;
        
        hasCollided = true;
        
        // Create impact effect
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }
        
        // Stop the trail
        if (trail != null)
        {
            trail.emitting = false;
        }
        
        // Destroy or stop the projectile
        if (destroyOnImpact)
        {
            Destroy(gameObject, 0.1f); // Small delay to show impact
        }
        else
        {
            // Just stop moving
            if (volatileBody != null)
            {
                volatileBody.Body.LinearVelocity = Volatile.VoltVector2.zero;
                volatileBody.Body.AngularVelocity = FixMath.NET.Fix64.Zero;
            }
        }
        
        Debug.Log("Projectile impact detected! CCD working properly.");
    }
    
    void OnDrawGizmos()
    {
        // Draw velocity vector
        if (volatileBody != null)
        {
            Gizmos.color = Color.red;
            Vector3 velocity = new Vector3(
                (float)volatileBody.Body.LinearVelocity.x,
                (float)volatileBody.Body.LinearVelocity.y,
                0
            );
            Gizmos.DrawLine(transform.position, transform.position + velocity * 0.1f);
        }
        
        // Draw CCD status indicator
        if (volatileBody != null && volatileBody.Body.EnableCCD)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.1f);
        }
    }
}
