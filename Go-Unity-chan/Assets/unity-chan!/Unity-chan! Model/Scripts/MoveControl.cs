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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
            animator.SetBool("Jump", true);

        move = Input.GetAxis("Vertical") * MoveSpeed;
        //direction = Input.GetAxis("Horizontal") * RotateSpeed;
        float angleY = transform.rotation.y * 180f;
        //Anda pra direção apontada
        if (Mathf.Abs(move) > 0.15f)
        {
            transform.localPosition = new Vector3(transform.position.x +  Mathf.Sin(angleY * Mathf.Deg2Rad) *MoveSpeed/50f
                , transform.position.y,
                transform.position.z + Mathf.Cos(angleY * Mathf.Deg2Rad) *MoveSpeed / 50f);
        }

        //Rotaciona
        Debug.Log(transform.rotation.y + " - " + direction);
        transform.localRotation = Quaternion.Euler(transform.position.x, transform.position.y - 5, transform.position.z);

        //Verifica se a personagem deve virar de costas
        if (move < 0 && !back)
        {
            transform.localRotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y*180 - 180, transform.rotation.z);
            back = true;
        }
            
        else if (move > 0.15f && back)
        {
            transform.localRotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y*180 + 180, transform.rotation.z);
            back = false;
        }

        //Seta a velocidade pelo eixo vertical e a direção pelo eixo horizontal
        animator.SetFloat("Speed", Mathf.Abs(move));
        animator.SetFloat("Direction", direction);

        //Se os dois pés colidiram, para de pular
        if (rightFoot.isTrigger && leftFoot.isTrigger)
            animator.SetBool("Jump", false);
    }
}
