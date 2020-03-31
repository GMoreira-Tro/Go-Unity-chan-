using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveControl : MonoBehaviour
{
    public CharacterController controller;
    public Animator animator;
    public float MoveSpeed = 1.5f;
    public float RotateSpeed = 5f;
    public Collider leftFoot;
    public Collider rightFoot;

    float move;
    float direction;
    bool jump;
    bool back;
    bool flipDirect;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
            animator.SetBool("Jump", true);

        move = Input.GetAxis("Vertical") * MoveSpeed;
        direction = Input.GetAxis("Horizontal") * RotateSpeed;
        float angleYDeg = 180 - Mathf.Acos(transform.rotation.y) * Mathf.Rad2Deg * 2;

        //Debug.Log(angleYDeg);
        if (flipDirect)
        {
            flipDirect = false;
        }
        float angleYRad = angleYDeg * Mathf.Deg2Rad;
        //Anda pra direção apontada
        if (Mathf.Abs(move) > 0.15f)
        {
            transform.localPosition = new Vector3(transform.position.x + Mathf.Sin(angleYRad) *MoveSpeed/50f
                , transform.position.y,
                transform.position.z + Mathf.Cos(angleYRad) *MoveSpeed / 50f);
        }

        //Rotaciona
        transform.Rotate(0, direction, 0);

        //Verifica se a personagem deve virar de costas
        if (move < -0.15f && !back)
        {
            transform.Rotate(0, 180, 0);
            back = true;
            flipDirect = true;
        }
            
        else if (move > 0.15f && back)
        {
            transform.Rotate(0, 180, 0);
            back = false;
            flipDirect = true;
        }

        //Seta a velocidade pelo eixo vertical
        animator.SetFloat("Speed", Mathf.Abs(move));

        //Se os dois pés colidiram, para de pular
        if (rightFoot.isTrigger && leftFoot.isTrigger)
            animator.SetBool("Jump", false);
    }

    void flip()
    {

    }
}
