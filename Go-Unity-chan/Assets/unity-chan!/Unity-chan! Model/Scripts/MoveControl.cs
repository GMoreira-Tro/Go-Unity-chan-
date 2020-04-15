using System;
using System.Collections;
using System.Collections.Generic;
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
        climbing = 16,
        falling = 32
    }
    public PlayerStates machineStates;

    float physicalMovement;
    float angleYDeg;
    float angleYRad;
    bool canFlip = true;
    bool canRightFlip = true;
    bool canLeftFlip = true;
    bool isGrounded = true;

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

        Update2();

    }

    private void Update2()
    {
        if (machineStates == PlayerStates.idling && Input.GetKey("space") && cubeUpAndInFront(RoundingPosition()).getCubeType() == 0)
        {
            audioSource.clip =  clips[DateTime.Now.Millisecond % 3];
            audioSource.Play();
            animator.SetBool("Jump", true);
            StartCoroutine(JumpRoutine());
        }
            
        if (machineStates == PlayerStates.idling)
        {
            RoundingPosition();
            if (!animator.GetBool("Run") &&
            canFlip && Input.GetKey("w") || Input.GetKey("up") && cubeInFront(RoundingPosition()).getCube() == null) { 
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

            if (Input.GetKey("p") && cubeInFront(RoundingPosition()).getCube() != null &&
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

            yield return null;
        }
        animator.SetBool("Jump", false);
        machineStates = PlayerStates.idling;
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
        animator.SetBool("ArmsUp", true);
        machineStates = PlayerStates.grabing;
        GameObject cube = cubeInFront(transform).getCube();

        while(machineStates == PlayerStates.grabing)
        {
            if (Input.GetKeyDown("p"))
            {
                machineStates = PlayerStates.idling;
                animator.SetBool("ArmsUp", false);
            }
            else if(Input.GetKey("down") || Input.GetKey("s"))
            {
                if(cubeBehind(transform).getCube() == null)
                {
                    float timer = 0;
                    float pullingDuration = 0.5f;
                    Vector3 initialPos = transform.position;
                    Vector3 finalPos = moveBack(transform);
                    Vector3 cubeInitialPos = moveFront(transform);

                    LevelManager.map[(int)cube.transform.position.x, (int)cube.transform.position.y, (int)cube.transform.position.z] = 
                        new LevelManager.mapObject(0,null);
                    try
                    {
                            LevelManager.map[(int)transform.position.x, (int)transform.position.y,
                    (int)-transform.position.z] = new LevelManager.mapObject(cubeInFront(transform).getCubeType(),
                     cube);
                    }
                    catch (Exception) { }

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

            else if (Input.GetKey("up") || Input.GetKey("w"))
            {
                if (cubeInFront(cube.transform).getCubeType() != LevelManager.CubesTypes.rigid)
                {
                    List<GameObject> cubes = new List<GameObject>();
                    while (true)
                    {
                        cubes.Add(cube);
                        LevelManager.mapObject behind = cubeBehind(cube.transform);
                        LevelManager.mapObject inFront = cubeInFront(cube.transform);
                        LevelManager.map[(int)cube.transform.position.x, (int)cube.transform.position.y, (int)cube.transform.position.z] =
                            new LevelManager.mapObject(behind.getCubeType(), behind.getCube());

                        if (inFront.getCubeType() == LevelManager.CubesTypes.rigid || inFront.getCubeType() == LevelManager.CubesTypes.anyBlock)
                        {
                            Debug.Log("chega");
                            break;
                        }

                        cube = inFront.getCube();
                    }
                    float timer = 0;
                    float pushingDuration = 0.5f;
                    Vector3 initialPos = transform.position;
                    Vector3 finalPos = moveFront(transform);

                    while (timer < pushingDuration)
                    {
                        float tim = timer / pushingDuration;
                        timer += Time.deltaTime;
                        transform.position = Vector3.Lerp(initialPos,
                                finalPos,
                                tim
                                );
                        for(int i = 0; i < cubes.Count; i++) {
                            LerpCube(cubes[i], cubes[i].transform.position, moveFront(cubes[i].transform), tim);
                        }
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
        return position.position.z + Mathf.Cos(angleYRad);
    }

    public float XDirectionRight(Transform position)
    {
        return position.position.x - Mathf.Cos(angleYRad);
    }

    public float XDirectionLeft(Transform position)
    {
        return position.position.x + Mathf.Cos(angleYRad);
    }
    public float ZDirectionLeft(Transform position)
    {
        return position.position.z + Mathf.Sin(angleYRad);
    }

    public float ZDirectionRight(Transform position)
    {
        return position.position.z - Mathf.Sin(angleYRad);
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

    public LevelManager.mapObject cubeUpAndInFront(Transform position)
    {
        try
        {
            return LevelManager.map[(int)XDirectionFront(position), (int)position.position.y+1,
                (int)-ZDirectionFront(position)];
        }
        catch (IndexOutOfRangeException)
        {
            return new LevelManager.mapObject(0, null);
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

    public LevelManager.mapObject cubeRight(Transform position)
    {
        try
        {
            return LevelManager.map[(int)XDirectionRight(position), (int)position.position.y,
                (int)ZDirectionRight(position)];
        }
        catch (IndexOutOfRangeException)
        {
            return new LevelManager.mapObject(0, null);
        }
    }

    public LevelManager.mapObject cubeLeft(Transform position)
    {
        try
        {
            return LevelManager.map[(int)XDirectionLeft(position), (int)position.position.y,
                (int)ZDirectionLeft(position)];
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
                    GameObject cube = null;
                    try
                    {
                        cube = LevelManager.map[i, j, k].getCube();
                    }
                    catch (NullReferenceException) { }

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

    void LerpCube(GameObject cube, Vector3 cubeInitialPos, Vector3 cubeFinalPos, float tim)
    {
        cube.transform.position = Vector3.Lerp(cubeInitialPos,
                            cubeFinalPos,
                            tim
                            );
    }
}
