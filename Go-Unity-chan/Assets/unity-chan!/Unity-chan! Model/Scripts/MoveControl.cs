using UnityEngine;

public class MoveControl : MonoBehaviour
{
    public Transform neck;
    public float cameraHorizontalSpeed;
    public float cameraVerticalSpeed;
    public Animator animator;
    public float moveSpeed = 1.5f;
    public float rotateSpeed = 5f;
    public AudioSource audioSource;
    public AudioClip[] clips;
    /*0,1,2 Jump sounds
     * 3 start sound
     * */

    float move;
    float physicalMovement;
    float angleYDeg;
    float angleYRad;
    bool canFlip = true;
    bool canRightFlip = true;
    bool canLeftFlip = true;
    bool isGrounded;

    private Rigidbody rigidbody;
    private float cameraPitch;
    private float cameraYaw;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        cameraPitch = -90;
        cameraYaw = 0;
        audioSource.clip = clips[3];
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        //Anda pra direção apontada
        if (Mathf.Abs(move) > 0.15f && !(animator.GetBool("Jump")))
        {
            physicalMovement = moveSpeed / 50f;
        transform.localPosition = new Vector3(transform.position.x + Mathf.Sin(angleYRad) * physicalMovement
            , transform.position.y,
            transform.position.z + Mathf.Cos(angleYRad) * physicalMovement);
        }

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
        if (isGrounded && Input.GetKey("space"))
        {
            animator.SetBool("Jump", true);
            transform.localPosition = new Vector3(transform.position.x + Mathf.Sin(angleYRad)
                , transform.position.y + 1,
                transform.position.z + Mathf.Cos(angleYRad));
            isGrounded = false;
            audioSource.clip =  clips[System.DateTime.Now.Millisecond % 3];
            audioSource.Play();
        }

        if (canFlip)
            move = Input.GetAxis("Vertical") * moveSpeed;

        if ((Input.GetKey("left") || Input.GetKey("a")) && canLeftFlip)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x,
                transform.eulerAngles.y - 90,
                transform.eulerAngles.z);
            canLeftFlip = false;
        }
        else if ((Input.GetKey("right") || Input.GetKey("d")) && canRightFlip)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x,
                transform.eulerAngles.y + 90,
                transform.eulerAngles.z);
            canRightFlip = false;
        }

        if (!Input.GetKey("left") && !Input.GetKey("a"))
            canLeftFlip = true;
        if (!Input.GetKey("right") && !Input.GetKey("d"))
            canRightFlip = true;

        angleYDeg = transform.eulerAngles.y;
        angleYRad = angleYDeg * Mathf.Deg2Rad;

        if ((Input.GetKey("down") || Input.GetKey("s")) && canFlip)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x,
                transform.eulerAngles.y + 180,
                transform.eulerAngles.z);
            canFlip = false;
            move = 0;
        }
        if (!Input.GetKey("down") && !Input.GetKey("s"))
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
        if (cameraYaw < -30)
            cameraYaw = -30;
        else if (cameraYaw > 30)
            cameraYaw = 30;

        neck.eulerAngles = new Vector3(cameraYaw*-1,
           neck.eulerAngles.y, 
            -180 - cameraPitch);
    }
}
