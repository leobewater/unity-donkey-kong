using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private new Rigidbody2D rigidbody;
    private new Collider2D collider;

	// max 4 type of game objects going to be overlapping with the player
    private Collider2D[] overlaps = new Collider2D[4];
    private Vector2 direction;

    public float moveSpeed = 1f;
    public float jumpStrength = 1f;

    private bool grounded;

    private void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        

    }

    private void CheckCollision() {
        grounded = false;

        // Get player collider size and make it slightly taller and narrower for better detection
        Vector3 size = collider.bounds.size;
        size.y += 0.1f;
        size.x /= 2f;
        int amount = Physics2D.OverlapBoxNonAlloc(transform.position, size, 0f, overlaps);

        for (int i = 0; i < amount; i++) {
            // What did the player overlap with?
            GameObject hit = overlaps[i].gameObject;

            // When player collides with the ground
            if (hit.layer == LayerMask.NameToLayer("Ground")) {
                // Only true when colliding platform belows the player
                grounded = hit.transform.position.y < (transform.position.y - 0.5f);
                // When jumping ignore the collision with object is above the player
                Physics2D.IgnoreCollision(overlaps[i], collider, !grounded);
            }
        }
    }

    private void Update() {
        // Check if player is grounded or jumping
        CheckCollision();

        // Handle jumping - Project Settings -> Input Manager
        // No double jumps
        if (grounded && Input.GetButtonDown("Jump")) {
            direction = Vector2.up * jumpStrength;
        } else {
            // Apply gravity when not jumping
            direction += Physics2D.gravity * Time.deltaTime;
        }

        // Project Settings -> Input Manager
        direction.x = Input.GetAxis("Horizontal") * moveSpeed;
        // Apply gravity forces only when player is grounded
        if (grounded) {
            direction.y = Mathf.Max(direction.y, -1f);
        }

        // Flip player
        if (direction.x > 0f) {
            // Moving to right
            transform.eulerAngles = Vector3.zero;
        } else if (direction.x < 0f) {
            // Moving to left
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }

    private void FixedUpdate() {
        rigidbody.MovePosition(rigidbody.position + direction * Time.fixedDeltaTime);
    }
}
