using UnityEngine;

public class MoveControl : MonoBehaviour
{
    public Transform neck;
    public float cameraHorizontalSpeed;
    public float cameraVerticalSpeed;
    public Animator animator;
    public float moveSpeed = 1.5f;
    public float rotateSpeed = 5f;
    public short jumpForce;

    float move;
    float physicalMovement;
    float direction;
    float angleYDeg;
    float angleYRad;
    bool canFlip = true;
    bool isGrounded;

    private Rigidbody rigidbody;
    private float cameraPitch;
    private float cameraYaw;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        cameraPitch = -90;
        cameraYaw = 180;
    }

    // Update is called once per frame
    void Update()
    {
        //Anda pra direção apontada
        if (Mathf.Abs(move) > 0.15f)
        {
            if (animator.GetBool("Jump"))
                physicalMovement = moveSpeed / 100f;
            else
                physicalMovement = moveSpeed / 50f;
            transform.localPosition = new Vector3(transform.position.x + Mathf.Sin(angleYRad) * physicalMovement
                , transform.position.y,
                transform.position.z + Mathf.Cos(angleYRad) * physicalMovement);
        }

        //Rotaciona
        cameraYaw += direction;

        //Verifica se a personagem deve virar de costas
        

        //Seta a velocidade pelo eixo vertical
        animator.SetFloat("Speed", Mathf.Abs(move));

        //Rotação da câmera
        MoveCamera();
        if (Input.GetMouseButtonDown(1))
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            cameraPitch = -90;
            cameraYaw = 180;
        }        
    }

    private void FixedUpdate()
    {
        if (isGrounded && Input.GetKey("j"))
        {
            animator.SetBool("Jump", true);
            rigidbody.AddForce(Vector3.up * jumpForce);
            isGrounded = false;
        }

        if (canFlip)
            move = Input.GetAxis("Vertical") * moveSpeed;

        if (Input.GetKey("left") || Input.GetKey("a"))
            direction = -rotateSpeed;
        else if (Input.GetKey("right") || Input.GetKey("d"))
            direction = rotateSpeed;
        else
            direction = 0;

        angleYDeg = transform.eulerAngles.y;
        angleYRad = angleYDeg * Mathf.Deg2Rad;

        if ((Input.GetKeyDown("down") || Input.GetKeyDown("s")) && canFlip)
        {
            cameraYaw += 180;
            canFlip = false;
            move = 0;
        }
        if (!Input.GetKeyDown("down") && !Input.GetKeyDown("s"))
            canFlip = true;
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

    void MoveCamera()
    {
        cameraPitch -= cameraVerticalSpeed * Input.GetAxis("Mouse Y");
        cameraYaw += cameraHorizontalSpeed * Input.GetAxis("Mouse X");

        if (cameraPitch > -60)
            cameraPitch = -60;
        else if (cameraPitch < -120)
            cameraPitch = -120;

        transform.eulerAngles = new Vector3(0, cameraYaw, 0);

        neck.eulerAngles = new Vector3(neck.eulerAngles.x,
           neck.eulerAngles.y, 
            -180 - cameraPitch);
    }
}
