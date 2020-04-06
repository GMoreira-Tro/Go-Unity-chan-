using UnityEngine;

public class FootColision : MonoBehaviour
{
    public Animator animator;
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ground")
        {
            MoveControl.isGrounded = true;
            //MoveControl.rigidbody.useGravity = true;
            animator.SetBool("Jump", false);
        }
    }
}
