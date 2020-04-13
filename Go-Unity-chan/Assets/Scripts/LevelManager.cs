using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject rigidCube;
    public GameObject standardCube;
    public GameObject explosiveCube;
    public GameObject BreakableCube;
    public GameObject ggCube;
    public Transform playerPosition;
    public MoveControl UnityChan;

    Thread tick;
    bool katsu;

    GameObject GO;

    public enum CubesTypes
    {
        anyBlock = 0,
        standard = 1,
        rigid = 2,
        breakable = 4,
        explosive = 8,
        GG = 16
    }
    public struct mapObject
    {
        CubesTypes cubeType;
        GameObject cube;

        public mapObject(CubesTypes ct, GameObject cube)
        {
            cubeType = ct;
            this.cube = cube;
        }
        public GameObject getCube()
        {
            return cube;
        }
        public CubesTypes getCubeType()
        {
            return cubeType;
        }
        public void setCube(GameObject cube)
        {
            this.cube = cube;
        }
        public void setCubeType(CubesTypes ct)
        {
            cubeType = ct;
        }
    }

    public static int level = 1;
    public static mapObject[,,] map = new mapObject[7,21,7];
    void Start()
    {
        for(int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 21; j++)
            {
                for(int k = 0; k < 7; k++)
                {
                    map[i,j,k] = new mapObject(0,null);
                }
            }
        }

        switch(level)
        {
            case 1:
                InstantiateLevel1();
                break;
        }
    }
    void FixedUpdate()
    {
        mapObject cube = UnityChan.CubeUnder(UnityChan.RoundingPosition());

        if (UnityChan.machineStates == MoveControl.PlayerStates.idling)
        {
            if (cube.getCubeType() != 0)
            {
                switch (cube.getCubeType())
                {
                    case CubesTypes.explosive:
                        tick = new Thread(explode);
                        tick.Start();

                        if (katsu)
                        {
                            Destroy(cube.getCube());
                        }
                        break;
                }
            }
            else 
            {
                {
                    StopAllCoroutines();
                    StartCoroutine(UnityChan.FallingRoutine());
                }
            }
        }
    }

    void InstantiateLevel1()
    {
        for (int i = 0; i < 7; i++)
        {
            GO = Instantiate(standardCube, new Vector3(i, 0, 0), Quaternion.identity);
            map[i, 0, 0] = new mapObject(CubesTypes.standard, GO);
        }
        for (int i = 0; i < 3; i++)
        {
           GO = Instantiate(rigidCube, new Vector3(i, 1, 0), Quaternion.identity);
            map[i, 1, 0] = new mapObject(CubesTypes.rigid, GO);
        }

        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                GO = Instantiate(rigidCube, new Vector3(i, j, -1), Quaternion.identity);
                map[i, j, 1] = new mapObject(CubesTypes.rigid,GO);
            }
        }

        for (int i = 2; i < 7; i++)
        {
            GO = Instantiate(rigidCube, new Vector3(i, 3, -1), Quaternion.identity);
            map[i, 3, 1] = new mapObject(CubesTypes.rigid, GO);
        }
        GO = Instantiate(standardCube, new Vector3(0, 3, -1), Quaternion.identity);
        map[0, 3, 1] = new mapObject(CubesTypes.standard,GO);
        GO = Instantiate(rigidCube, new Vector3(1, 3, -1), Quaternion.identity);
        map[1, 3, 1] = new mapObject(CubesTypes.rigid,GO);

        GO = Instantiate(explosiveCube, new Vector3(1, 2, 0), Quaternion.identity);
        map[1, 2, 0] = new mapObject(CubesTypes.explosive, GO);
        Instantiate(standardCube, new Vector3(0, 2, 0), Quaternion.identity);
        map[0, 2, 0] = new mapObject(CubesTypes.standard, GO);

    }
    void explode()
    {
        Thread.Sleep(3000);
        katsu = true;
    }
}
