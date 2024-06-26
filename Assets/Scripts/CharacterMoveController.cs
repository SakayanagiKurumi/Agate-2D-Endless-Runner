﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveController : MonoBehaviour
{
    [Header("Movement")]
    public float moveAccel;
    public float maxSpeed;

    [Header("Jump")]
    public float jumpAccel;

    [Header("Ground Raycast")]
    public float groundRaycastDistance;
    public LayerMask groundLayerMask;

    [Header("Scoring")]
    public ScoreController score;
    public float scoringRatio;
    public int scoreItem;
    private float lastPositionX;

    [Header("Game Over")]
    public GameObject gameOverScreen;
    public float fallPositionY;

    [Header("Camera")]
    public CameraMoveController gameCamera;

    public TerrainGeneratorController terrain;

    private bool isJumping;
    private bool isOnGround;

    private Rigidbody2D rig;
    private Animator anim;
    private CharacterSoundController sound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Item"))
        {
            Debug.Log("Player catch item!");
            score.increaseCurrentScore(scoreItem);
            terrain.spawnedItem.Remove(collision.gameObject);
            Destroy(collision.gameObject);
        }
    }

    private void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sound = GetComponent<CharacterSoundController>();
    }

    private void Update()
    {
        // Read input
        if(Input.GetMouseButtonDown(0))
        {
            if (isOnGround)
            {
                isJumping = true;
                sound.PlayJump();
            }
        }

        // Change animation
        anim.SetBool("isOnGround", isOnGround);

        // Calculate score
        int distancePassed = Mathf.FloorToInt(transform.position.x - lastPositionX);
        int scoreIncrement = Mathf.FloorToInt(distancePassed / scoringRatio);

        if(scoreIncrement > 0)
        {
            score.increaseCurrentScore(scoreIncrement);
            lastPositionX += distancePassed;
        }

        // Game Over
        if (transform.position.y < fallPositionY)
            GameOver();
    }

    private void GameOver()
    {
        // Set high score
        score.finishScoring();

        // Stop camera movement
        gameCamera.enabled = false;

        // Show game over
        gameOverScreen.SetActive(true);

        // Disable this too
        this.enabled = false;
    }

    private void FixedUpdate()
    {
        // Raycasat ground
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundRaycastDistance, groundLayerMask);

        if(hit)
        {
            if (!isOnGround && rig.velocity.y <= 0)
                isOnGround = true;
        }
        else
        {
            isOnGround = false;
        }

        // Calculate velocity vector
        Vector2 velocityVector = rig.velocity;

        if(isJumping)
        {
            velocityVector.y += jumpAccel;
            isJumping = false;
        }

        velocityVector.x = Mathf.Clamp(velocityVector.x + moveAccel * Time.deltaTime, 0f, maxSpeed);
        rig.velocity = velocityVector;
    }

    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, transform.position + (Vector3.down * groundRaycastDistance), Color.white);
    }
}
