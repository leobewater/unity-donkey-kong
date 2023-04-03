using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private new Rigidbody2D rigidbody;
    private Vector2 direction;
    public float moveSpeed = 1f;
    public float jumpStrength = 1f;

    private void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        // Handle jumping - Project Settings -> Input Manager
        if (Input.GetButtonDown("Jump")) {
            direction = Vector2.up * jumpStrength;
        } else {
            // Apply gravity when not jumping
            direction += Physics2D.gravity * Time.deltaTime;
        }

        // Project Settings -> Input Manager
        direction.x = Input.GetAxis("Horizontal") * moveSpeed;
        // Control gravity forces
        direction.y = Mathf.Max(direction.y, -1f);

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
