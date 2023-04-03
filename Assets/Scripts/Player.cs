using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite[] runSprites;
    public Sprite climbSprite;
    private int spriteIndex;

    private new Rigidbody2D rigidbody;
    private new Collider2D collider;

    // max 4 type of game objects going to be overlapping with the player
    private Collider2D[] overlaps = new Collider2D[4];
    private Vector2 direction;

    private bool grounded;
    private bool climbing;

    public float moveSpeed = 3f;
    public float jumpStrength = 4f;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
    }

    private void OnEnable() {
        // Set player animation
        InvokeRepeating(nameof(AnimateSprite), 1f / 12f, 1f / 12f);
    }

    private void OnDisable() {
        CancelInvoke();
    }

    private void Update() {
        // Check if player is grounded or jumping
        CheckCollision();
        SetDirection();
    }

    private void CheckCollision() {
        grounded = false;
        climbing = false;

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
            } else if (hit.layer == LayerMask.NameToLayer("Ladder")) {
                // When touching the ladder
                climbing = true;
            }
        }
    }

    private void SetDirection() {
        if (climbing) {
            // Climbing
            direction.y = Input.GetAxis("Vertical") * moveSpeed;
        } else if (grounded && Input.GetButtonDown("Jump")) {
            // Handle jumping - Project Settings -> Input Manager
            // No double jumps
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

    private void AnimateSprite() {
        if (climbing) {
            spriteRenderer.sprite = climbSprite;
        } else if (direction.x != 0f) {
            // Only animate the player when moving
            spriteIndex++;

            if (spriteIndex >= runSprites.Length) {
                spriteIndex = 0;
            }

            spriteRenderer.sprite = runSprites[spriteIndex];
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Objective")) {
            enabled = false;
            FindObjectOfType<GameManager>().LevelComplete();
        } else if (collision.gameObject.CompareTag("Obstacle")) {
            enabled = false;
            FindObjectOfType<GameManager>().LevelFailed();
        }
    }
}
