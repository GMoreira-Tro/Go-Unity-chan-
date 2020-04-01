using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveControl : MonoBehaviour
{
    public Transform camera;
    public Animator animator;
    public float moveSpeed = 1.5f;
    public float rotateSpeed = 5f;
    public short jumpForce;

    float move;
    float physicalMovement;
    float direction;
    bool back;
    bool isGrounded;

    private Rigidbody rigidbody;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGrounded && Input.GetKeyDown("space"))
        {
            animator.SetBool("Jump", true);
            rigidbody.AddForce(Vector3.up * jumpForce);
        }

        move = Input.GetAxis("Vertical") * moveSpeed;
        direction = Input.GetAxis("Horizontal") * rotateSpeed;
        float angleYDeg = transform.eulerAngles.y;
        float angleYRad = angleYDeg * Mathf.Deg2Rad;

        //Anda pra direção apontada
        if (Mathf.Abs(move) > 0.15f)
        {
            if (animator.GetBool("Jump"))
                physicalMovement = moveSpeed / 200f;
            else
                physicalMovement = moveSpeed / 50f;
            transform.localPosition = new Vector3(transform.position.x + Mathf.Sin(angleYRad) * physicalMovement
                , transform.position.y,
                transform.position.z + Mathf.Cos(angleYRad) * physicalMovement);
        }

        //Rotaciona
        transform.Rotate(0, direction, 0);

        //Verifica se a personagem deve virar de costas
        if (move < -0.15f && !back)
        {
            transform.Rotate(0, -180, 0);
            back = true;
        }

        else if (move > 0.15f && back)
        {
            transform.Rotate(0, 180, 0);
            back = false;
        }

        //Seta a velocidade pelo eixo vertical
        animator.SetFloat("Speed", Mathf.Abs(move));

    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ground")
        {
            isGrounded = true;
            animator.SetBool("Jump", false);
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }
    }
}
