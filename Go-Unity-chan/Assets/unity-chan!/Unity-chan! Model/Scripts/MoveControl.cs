using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveControl : MonoBehaviour
{
    public Transform neck;
    public float cameraHorizontalSpeed;
    public float cameraVerticalSpeed;
    public Animator animator;
    public float moveSpeed = 1.5f;
    public float jumpPower;
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
    public static bool isGrounded = true;

    private Rigidbody rigidbody;
    private float cameraPitch;
    private float cameraYaw;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        cameraPitch = 30;
        cameraYaw = -180;
        audioSource.clip = clips[3];
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        //Anda pra direção apontada
        if (Mathf.Abs(move) > 0.15f && isGrounded)
        {
            physicalMovement = moveSpeed / 50f;

        transform.position = new Vector3(transform.position.x + Mathf.Sin(angleYRad) * physicalMovement
            , transform.position.y,
            transform.position.z + Mathf.Cos(angleYRad) * physicalMovement);
        }

        neck.position = new Vector3(transform.position.x, transform.position.y + 4, transform.position.z + 5);

        //Verifica se a personagem deve virar de costas
        

        //Seta a velocidade pelo eixo vertical
        animator.SetFloat("Speed", Mathf.Abs(move));

        //Rotação da câmera
        MoveNeck();
        if (Input.GetMouseButtonDown(1))
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            cameraPitch = 30;
            cameraYaw = -180;
        }

        if (transform.position.y < -5)
            SceneManager.LoadScene("Level" + LevelManager.level);
    }

    private void FixedUpdate()
    {
        if (isGrounded && Input.GetKey("space"))
        {
            audioSource.clip =  clips[System.DateTime.Now.Millisecond % 3];
            audioSource.Play();
            animator.SetBool("Jump", true);
        }

        else if(!isGrounded)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x
                + Mathf.Sin(angleYRad),
                transform.position.y + 1,
                transform.position.z + Mathf.Cos(angleYRad)),
                jumpPower * Time.deltaTime);
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

    void MoveNeck()
    {
        cameraPitch -= cameraVerticalSpeed * Input.GetAxis("Mouse Y");
        cameraYaw += cameraHorizontalSpeed * Input.GetAxis("Mouse X");

        if (cameraYaw > -150)
            cameraYaw = -150;
        else if (cameraYaw < -210)
            cameraYaw = -210;
        if (cameraPitch < 0)
            cameraPitch = 0;
        else if (cameraPitch > 60)
            cameraPitch = 60;

        neck.eulerAngles = new Vector3(cameraPitch,
           cameraYaw,
            neck.eulerAngles.z);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ground")
        {
            isGrounded = true;
            animator.SetBool("Jump", false);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        { 
            isGrounded = false;
        }
    }
}
