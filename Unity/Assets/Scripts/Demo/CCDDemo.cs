using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Demo script showing Continuous Collision Detection in action.
/// Creates fast-moving projectiles that would normally tunnel through thin objects
/// but are stopped by CCD.
/// </summary>
public class CCDDemo : MonoBehaviour 
{
    [Header("CCD Settings")]
    public bool enableCCD = true;
    public float velocityThreshold = 1.0f;
    public float projectileSpeed = 10.0f;
    public float fireRate = 2.0f;
    
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    
    [Header("Targets")]
    public GameObject[] targets;
    
    private float nextFireTime;
    private VoltWorld world;
    
    void Start()
    {
        // Find the world component
        VolatileWorld worldComponent = FindObjectOfType<VolatileWorld>();
        if (worldComponent != null)
        {
            world = worldComponent.World;
        }
        
        // Configure targets for thin collision detection
        foreach (GameObject target in targets)
        {
            VolatileBody volatileBody = target.GetComponent<VolatileBody>();
            if (volatileBody != null && world != null)
            {
                // Make sure static bodies can interact with CCD
                volatileBody.Body.EnableCCD = true;
            }
        }
        
        if (firePoint == null)
            firePoint = transform;
    }
    
    void Update()
    {
        // Fire projectiles when space is pressed or automatically
        if (Input.GetKey(KeyCode.Space) || Time.time >= nextFireTime)
        {
            FireProjectile();
            nextFireTime = Time.time + 1.0f / fireRate;
        }
        
        // Toggle CCD with C key
        if (Input.GetKeyDown(KeyCode.C))
        {
            enableCCD = !enableCCD;
            Debug.Log("CCD " + (enableCCD ? "Enabled" : "Disabled"));
        }
        
        // Adjust speed with + and - keys
        if (Input.GetKey(KeyCode.Equals) || Input.GetKey(KeyCode.Plus))
        {
            projectileSpeed += 5.0f * Time.deltaTime;
            Debug.Log("Projectile Speed: " + projectileSpeed);
        }
        if (Input.GetKey(KeyCode.Minus))
        {
            projectileSpeed = Mathf.Max(1.0f, projectileSpeed - 5.0f * Time.deltaTime);
            Debug.Log("Projectile Speed: " + projectileSpeed);
        }
    }
    
    void FireProjectile()
    {
        if (projectilePrefab == null) return;
        
        // Create projectile
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        VolatileBody volatileBody = projectile.GetComponent<VolatileBody>();
        
        if (volatileBody != null && world != null)
        {
            // Enable CCD for the projectile
            if (enableCCD)
            {
                world.EnableCCD(volatileBody.Body, (FixMath.NET.Fix64)velocityThreshold);
            }
            else
            {
                world.DisableCCD(volatileBody.Body);
            }
            
            // Apply initial velocity
            Vector2 direction = firePoint.right; // Fire in the right direction
            volatileBody.Body.LinearVelocity = new Volatile.VoltVector2(
                (FixMath.NET.Fix64)direction.x * (FixMath.NET.Fix64)projectileSpeed,
                (FixMath.NET.Fix64)direction.y * (FixMath.NET.Fix64)projectileSpeed
            );
        }
        
        // Auto-destroy projectile after some time
        Destroy(projectile, 5.0f);
    }
    
    void OnGUI()
    {
        float x = 10;
        float y = 10;
        float width = 300;
        float height = 20;
        
        GUI.Label(new Rect(x, y, width, height), "CCD Demo Controls:");
        y += height + 5;
        
        GUI.Label(new Rect(x, y, width, height), "Space: Fire Projectile");
        y += height;
        
        GUI.Label(new Rect(x, y, width, height), "C: Toggle CCD (" + (enableCCD ? "ON" : "OFF") + ")");
        y += height;
        
        GUI.Label(new Rect(x, y, width, height), "+/-: Adjust Speed (" + projectileSpeed.ToString("F1") + ")");
        y += height + 10;
        
        GUI.Label(new Rect(x, y, width, height), "CCD Status: " + (enableCCD ? "ENABLED" : "DISABLED"));
        y += height;
        
        if (!enableCCD)
        {
            GUI.color = Color.red;
            GUI.Label(new Rect(x, y, width, height), "Fast projectiles may tunnel through thin objects!");
            GUI.color = Color.white;
        }
        else
        {
            GUI.color = Color.green;
            GUI.Label(new Rect(x, y, width, height), "CCD will prevent tunneling through thin objects");
            GUI.color = Color.white;
        }
    }
}
