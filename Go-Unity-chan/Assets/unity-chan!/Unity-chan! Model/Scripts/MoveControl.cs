using System;
using System.Collections;
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
     [Flags]
    public enum PlayerStates
    {
        idling = 0,
        running = 1,
        jumping = 2,
        grabing = 4,
        pushing = 8
    }
    PlayerStates machineStates;

    float move;
    float physicalMovement;
    float angleYDeg;
    float angleYRad;
    bool canFlip = true;
    bool canRightFlip = true;
    bool canLeftFlip = true;
    public static bool isGrounded = true;

    private float cameraPitch;
    private float cameraYaw;
    void Start()
    {
        cameraPitch = 30;
        cameraYaw = -180;
        audioSource.clip = clips[3];
        audioSource.Play();
    }

    void Update()
    {
        //Debug.Log("Front " + cubeInFrontType());
        // Debug.Log("Back " + cubeBehindType());
        //Debug.Log(Mathf.RoundToInt(XDirectionFront()) + "..." + Mathf.RoundToInt(ZDirectionFront()));
        //Debug.Log(machineStates + " - " + Input.GetKey("p"));

        //Anda pra direção apontada
        if ((Mathf.Abs(move) > 0.15f && machineStates == 0))
        {
            StartCoroutine(MovingRoutine());
        }

        neck.position = new Vector3(transform.position.x, transform.position.y + 4, transform.position.z + 5);

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
        {
            SceneManager.LoadScene("Level" + LevelManager.level);
        }

        //Arredonda a posição porque o ângulo zoa, às vezes, por conta do corpo dela
        RoundingPosition();
    }

    private void FixedUpdate()
    {
        if (!machineStates.HasFlag(PlayerStates.jumping) && Input.GetKey("space"))
        {
            audioSource.clip =  clips[System.DateTime.Now.Millisecond % 3];
            audioSource.Play();
            animator.SetBool("Jump", true);
            StartCoroutine(JumpRoutine());
        }

        if (machineStates == 0 && Input.GetKey("p") && cubeInFrontType() != 0 && cubeInFrontType() !=
            LevelManager.CubesTypes.rigid
            )
        {
            audioSource.clip = clips[4];
            audioSource.Play();
            StartCoroutine(GrabingRoutine());
        }

        if (machineStates == 0)
        {
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

    IEnumerator JumpRoutine()
    {
        machineStates = PlayerStates.jumping;

        float timer = 0;
        float jumpDuration = 1f; //Duração da animação do pulo
        Vector3 initialPos = transform.position;
        Vector3 finalPos = new Vector3(transform.position.x
                    + Mathf.Sin(angleYRad),
                    transform.position.y + 1,
                    transform.position.z + Mathf.Cos(angleYRad));

        while (timer < jumpDuration)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(initialPos, 
                    finalPos,
                    timer / jumpDuration
                    );
            /*transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x
                    + Mathf.Sin(angleYRad),
                    transform.position.y + 1,
                    transform.position.z + Mathf.Cos(angleYRad)),
                    jumpPower * Time.deltaTime);*/
            yield return null;
        }
        animator.SetBool("Jump", false);
        machineStates = 0;

        //Checar se tenho um tile abaixo
    }

    IEnumerator MovingRoutine()
    {
        machineStates = PlayerStates.running;

        float timer = 0;
        float runningDuration = 0.5f; //Duração da animação do pulo
        Vector3 initialPos = transform.position;
        Vector3 finalPos = moveFront();

        while (timer < runningDuration)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(initialPos,
                    finalPos,
                    timer / runningDuration
                    );
            yield return null;
        }
        machineStates = 0;
    }

    IEnumerator GrabingRoutine()
    {
        machineStates = PlayerStates.grabing;
        GameObject cube = cubeInFront().getCube();
        Debug.Log("is grabing");

        while(machineStates == PlayerStates.grabing)
        {
            if (Input.GetKeyDown("p"))
            {
                machineStates = 0;
            }
            else if(Input.GetKey("down") || Input.GetKey("s"))
            {
                if(cubeBehindType() == 0)
                {
                    float timer = 0;
                    float pullingDuration = 0.5f;
                    Vector3 initialPos = transform.position;
                    Vector3 finalPos = moveBack();
                    Vector3 cubeInitialPos = moveFront();

                    while (timer < pullingDuration)
                    {
                        float tim = timer / pullingDuration;
                        timer += Time.deltaTime;
                        transform.position = Vector3.Lerp(initialPos,
                                finalPos,
                                tim
                                );
                        cube.transform.position = Vector3.Lerp(cubeInitialPos,
                            initialPos,
                            tim
                            );
                        yield return null;
                    }
                }
            }
            yield return null;
        }
    }

    void RoundingPosition()
    {
        transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y),
            Mathf.RoundToInt(transform.position.z));
    }

    float XDirectionFront()
    {
        return transform.position.x + Mathf.Sin(angleYRad);
    }

    float ZDirectionFront()
    {
        return transform.position.z + Mathf.Cos(angleYRad);
    }

    float XDirectionBack()
    {
        return transform.position.x - Mathf.Sin(angleYRad);
    }

    float ZDirectionBack()
    {
        return transform.position.z - Mathf.Cos(angleYRad);
    }

    LevelManager.CubesTypes cubeInFrontType()
    {
        try
        {
            return LevelManager.map[Mathf.RoundToInt(XDirectionFront()), (int)transform.position.y, -Mathf.RoundToInt(ZDirectionFront())]
                    .getCubeType();
        }
        catch (IndexOutOfRangeException)
        {
            return 0;
        }
    }
    LevelManager.CubesTypes cubeBehindType()
    {
        try
        {
            return LevelManager.map[Mathf.RoundToInt(XDirectionBack()), (int)transform.position.y, -Mathf.RoundToInt(ZDirectionBack())]
                .getCubeType();
        }
        catch (IndexOutOfRangeException)
        {
            return 0;
        }
    }

    LevelManager.mapObject cubeInFront()
    {
        try
        {
            return LevelManager.map[Mathf.RoundToInt(XDirectionFront()), (int)transform.position.y, -Mathf.RoundToInt(ZDirectionFront())];
        }
        catch (IndexOutOfRangeException)
        {
            return new LevelManager.mapObject(0,null);
        }
    }

    LevelManager.mapObject cubeBehind()
    {
        try
        {
            return LevelManager.map[Mathf.RoundToInt(XDirectionBack()), (int)transform.position.y, -Mathf.RoundToInt(ZDirectionBack())];
        }
        catch (IndexOutOfRangeException)
        {
            return new LevelManager.mapObject(0, null);
        }
    }

    Vector3 moveFront()
    {
        return new Vector3(XDirectionFront(),
                    transform.position.y,
                    ZDirectionFront()); ;
    }

    Vector3 moveBack()
    {
        return new Vector3(XDirectionBack(),
                    transform.position.y,
                    ZDirectionBack());
    }
}
