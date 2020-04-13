using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveControl : MonoBehaviour
{
    public Transform neck;
    public float cameraHorizontalSpeed;
    public float cameraVerticalSpeed;
    public Animator animator;
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
        pushing = 8,
        pulling = 16,
        falling = 32
    }
    public PlayerStates machineStates;

    float physicalMovement;
    float angleYDeg;
    float angleYRad;
    bool canFlip = true;
    bool canRightFlip = true;
    bool canLeftFlip = true;
    public bool isGrounded = true;

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
        if (animator.GetBool("Run") && machineStates == PlayerStates.idling)
        {
            StartCoroutine(MovingRoutine());
        }

        neck.position = new Vector3(transform.position.x, transform.position.y + 4, transform.position.z + 5);

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

    }

    private void FixedUpdate()
    {
        if (!machineStates.HasFlag(PlayerStates.jumping) && Input.GetKey("space") && CubeUnder(RoundingPosition()).getCubeType() != 0)
        {
            audioSource.clip =  clips[System.DateTime.Now.Millisecond % 3];
            audioSource.Play();
            animator.SetBool("Jump", true);
            StartCoroutine(JumpRoutine());
        }
            
        if (machineStates == PlayerStates.idling)
        {
            RoundingPosition();
            if (!animator.GetBool("Run") &&
            canFlip && Input.GetKey("w") || Input.GetKey("up") && cubeInFront(RoundingPosition()).getCubeType() == 0) { 
                    animator.SetBool("Run", true);
            }

            else if ((Input.GetKey("left") || Input.GetKey("a")) && canLeftFlip)
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
            }

            else if (!Input.GetKey("down") && !Input.GetKey("s"))
                canFlip = true;

            else if (Input.GetKey("p") && cubeInFront(RoundingPosition()).getCubeType() != 0 &&
            cubeInFront(RoundingPosition()).getCubeType() !=
            LevelManager.CubesTypes.rigid
            )
            {
                audioSource.clip = clips[4];
                audioSource.Play();
                StartCoroutine(GrabingRoutine());
            }
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
        float jumpDuration = 0.6f; //Duração da animação do pulo
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
        machineStates = PlayerStates.idling;

        //Checar se tenho um tile abaixo
    }

    public IEnumerator FallingRoutine()
    {
        machineStates = PlayerStates.falling;

        float timer = 0;
        float jumpDuration = 0.2f;
        Vector3 initialPos = transform.position;
        Vector3 finalPos = new Vector3(transform.position.x,
                    transform.position.y - 1,
                    transform.position.z);

        while (timer < jumpDuration)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(initialPos,
                    finalPos,
                    timer / jumpDuration
                    );

            yield return null;
        }

        machineStates = 0;
    }

    IEnumerator MovingRoutine()
    {
        machineStates = PlayerStates.running;

        float timer = 0;
        float runningDuration = 0.5f;
        Vector3 initialPos = transform.position;
        Vector3 finalPos = moveFront(transform);

        while (timer < runningDuration)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(initialPos,
                    finalPos,
                    timer / runningDuration
                    );
            yield return null;
        }

        animator.SetBool("Run", false);
        machineStates = PlayerStates.idling;
    }

    IEnumerator GrabingRoutine()
    {
        machineStates = PlayerStates.grabing;
        GameObject cube = cubeInFront(transform).getCube();

        while(machineStates == PlayerStates.grabing)
        {
            if (Input.GetKeyDown("p"))
            {
                machineStates = PlayerStates.idling;
            }
            else if(Input.GetKey("down") || Input.GetKey("s"))
            {
                if(cubeBehind(transform).getCubeType() == 0)
                {
                    float timer = 0;
                    float pullingDuration = 0.5f;
                    Vector3 initialPos = transform.position;
                    Vector3 finalPos = moveBack(transform);
                    Vector3 cubeInitialPos = moveFront(transform);

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

    public Transform RoundingPosition()
    {
        transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y),
            Mathf.RoundToInt(transform.position.z));
        return transform;
    }

    public float XDirectionFront(Transform position)
    {
        return position.position.x + Mathf.Sin(angleYRad);
    }

    public float ZDirectionFront(Transform position)
    {
        return position.position.z + Mathf.Cos(angleYRad);
    }

    public float XDirectionBack(Transform position)
    {
        return position.position.x - Mathf.Sin(angleYRad);
    }

    public float ZDirectionBack(Transform position)
    {
        return position.position.z - Mathf.Cos(angleYRad);
    }

    public LevelManager.mapObject cubeInFront(Transform position)
    {
        try
        {
            return LevelManager.map[(int)XDirectionFront(position), (int)position.position.y,
                (int)-ZDirectionFront(position)];
        }
        catch (IndexOutOfRangeException)
        {
            return new LevelManager.mapObject(0,null);
        }
    }
    public LevelManager.mapObject cubeBehind(Transform position)
    {
        try
        {
            return LevelManager.map[(int)XDirectionBack(position), 
                (int)position.position.y, (int)(-ZDirectionBack(position))];
        }
        catch (IndexOutOfRangeException)
        {
            return new LevelManager.mapObject(0, null);
        }
    }
    public LevelManager.mapObject CubeUnder(Transform reference)
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 21; j++)
            {
                for (int k = 0; k < 7; k++)
                {
                    GameObject cube = LevelManager.map[i, j, k].getCube();
                    if (cube != null)
                    {
                        if (IsOnTheCube(i,j,k, reference))
                        {
                            return LevelManager.map[i, j, k]; 
                        }
                    }
                }
            }
        }
        return new LevelManager.mapObject(0, null);
    }

    public bool IsOnTheCube(int x, int y, int z, Transform reference)
    {
        return  reference.position.x == x &&
                        reference.position.y - 1 == y &&
                        -reference.position.z == z;

    }

    Vector3 moveFront(Transform reference)
    {
        return new Vector3(XDirectionFront(reference),
                    transform.position.y,
                    ZDirectionFront(reference)); ;
    }

    Vector3 moveBack(Transform reference)
    {
        return new Vector3(XDirectionBack(reference),
                    transform.position.y,
                    ZDirectionBack(reference));
    }
}
