using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 15;

    private bool isTravelling;
    private Vector3 travelDirection;
    private Vector3 nextCollisionPosition;

    public int minSwipeRecognition = 500;
    private Vector2 swipePosLastFrame;
    private Vector2 swipePosCurrentFrame;
    private Vector2 currentSwipe;

    private Color solveColor;

    private void Start()
    {
        solveColor = Random.ColorHSV(0.5f, 1);
        GetComponent<MeshRenderer>().material.color = solveColor;
    }

    public void FixedUpdate()
    {
        if (isTravelling)
        {
            rb.velocity = speed * travelDirection;
        }

        Collider[] hitcollider = Physics.OverlapSphere(transform.position - (Vector3.up / 2), 0.05f);
        int i = 0;
        while (i < hitcollider.Length)
        {
            GroundPiece ground = hitcollider[i].transform.GetComponent<GroundPiece>();
            if (ground && !ground.isColored)
            {
                ground.ChangeColor(solveColor);
            }
            i++;
        }

        if (nextCollisionPosition != Vector3.zero)
        {
            if (Vector3.Distance(transform.position ,nextCollisionPosition) < 1)
            {
                isTravelling = false;
                travelDirection = Vector3.zero;
                nextCollisionPosition = Vector3.zero;
            }
        }

        if (isTravelling)
            return;

        if (Input.GetMouseButton(0))
        {
            swipePosCurrentFrame = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            if (swipePosLastFrame != Vector2.zero)
            {
                currentSwipe = swipePosCurrentFrame - swipePosLastFrame;
            }

            if (currentSwipe.sqrMagnitude < minSwipeRecognition)
            {
                return;
            }

            currentSwipe.Normalize();

            //Up/Down
            if(currentSwipe.x > -.5f && currentSwipe.x < .5f)
            {
                //Go up/down
                SetDestination(currentSwipe.y > 0 ? Vector3.forward : Vector3.back);
            }

            if (currentSwipe.y > -.5f && currentSwipe.y < .5f)
            {
                //Go left/right
                SetDestination(currentSwipe.x > 0  ? Vector3.right : Vector3.left);
            }

        }
        swipePosLastFrame = swipePosCurrentFrame;

        if (Input.GetMouseButtonUp(0))
        {
            swipePosLastFrame = Vector2.zero;
            currentSwipe = Vector2.zero;
        }
    }

    private void SetDestination(Vector3 direction)
    {
        travelDirection = direction;
        
        RaycastHit hit;
        if(Physics.Raycast(transform.position,direction, out hit, 100f))
        {
            nextCollisionPosition = hit.point;
        }
        isTravelling = true;
    }
}
