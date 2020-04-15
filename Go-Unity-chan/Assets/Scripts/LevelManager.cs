using System;
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
    public MoveControl unityChan;

    Thread tick;
    bool katsu;
    bool C4;

    GameObject GO;
    List<mapObject> explosives;
    mapObject DeidaraCube;

    public enum CubesTypes
    {
        anyBlock = 0,
        standard = 1,
        rigid = 2,
        breakable = 4,
        explosive = 8,
        GG = 16
    }
    public class mapObject
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
    public static mapObject[,,] map = new mapObject[14,21,14];
    void Start()
    {
        for(int i = 0; i < 14; i++)
        {
            for (int j = 0; j < 21; j++)
            {
                for(int k = 0; k < 14; k++)
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

        tick = new Thread(explode);
    }
    void Update()
    {
        Debug.Log(unityChan.cubeBehind(unityChan.RoundingPosition()).getCube());
        mapObject cube = unityChan.CubeUnder(unityChan.RoundingPosition());

        if (cube.getCube() == null)
        {
            GameObject underBehind = unityChan.cubeBehind(unityChan.RoundingPosition()).getCube();
            if (underBehind == null && unityChan.machineStates != MoveControl.PlayerStates.falling)
            {
                StopAllCoroutines();
                unityChan.animator.SetBool("ArmsUp", false);
                unityChan.animator.SetBool("ClimbUp", false);
                StartCoroutine(unityChan.FallingRoutine());
            }
            
            else if (underBehind != null && unityChan.machineStates != MoveControl.PlayerStates.climbing)
            {
                StopAllCoroutines();
                unityChan.animator.SetBool("ClimbUp", true);
            }
        }

        if (unityChan.machineStates == MoveControl.PlayerStates.idling)
        {
            if (cube.getCubeType() != 0)
            {
                switch (cube.getCubeType())
                {
                    case CubesTypes.explosive:
                        if (!tick.IsAlive)
                        {
                            DeidaraCube = cube;
                            tick = new Thread(explode);
                            tick.Start();
                        }
                        break;
                        
                        
                }
            }

            if (katsu && DeidaraCube.getCube() != null)
            {
                Transform pos = DeidaraCube.getCube().transform;
                mapObject[] adjacents = {unityChan.cubeLeft(pos),
                            unityChan.cubeRight(pos),
                            unityChan.cubeInFront(pos),
                            unityChan.cubeBehind(pos)};

                DeidaraCube.setCubeType(CubesTypes.anyBlock);
                Destroy(DeidaraCube.getCube().gameObject);

                explosives = ChainExplosion(adjacents, new List<mapObject>());
                tick = new Thread(BigBang);
                tick.Start();
                C4 = true;
                katsu = false;
            }

            else if(C4 && !tick.IsAlive)
            {
                for (int i = 0; i < explosives.Count; i++)
                {
                    Destroy(explosives[i].getCube().gameObject);
                    Debug.Log("dmka");
                }
                C4 = false;
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
        Instantiate(explosiveCube, new Vector3(0, 2, 0), Quaternion.identity);
        map[0, 2, 0] = new mapObject(CubesTypes.explosive, GO);

    }
    void explode()
    {
        Thread.Sleep(3000);
        katsu = true;

    }

    void BigBang()
    {
        Thread.Sleep(3000);
    }

    List<mapObject> ChainExplosion(mapObject[] adjacents, List<mapObject> explosives)
    {
        for (int i = 0; i < adjacents.Length; i++)
        {
            try
            {

                Transform pos = adjacents[i].getCube().transform;

                if (adjacents[i].getCubeType() == CubesTypes.explosive)
                {
                    adjacents[i].setCubeType(CubesTypes.anyBlock);
                    explosives.Add(adjacents[i]);

                    mapObject[] newAdjacents = {
                        unityChan.cubeLeft(pos),
                        unityChan.cubeRight(pos),
                        unityChan.cubeInFront(pos),
                        unityChan.cubeBehind(pos)
                    };

                    ChainExplosion(newAdjacents, explosives);
                }

                else if (adjacents[i].getCubeType() == CubesTypes.standard)
                {
                    adjacents[i].setCubeType(CubesTypes.breakable);
                }

                else if (adjacents[i].getCubeType() == CubesTypes.breakable)
                {
                    Destroy(adjacents[i].getCube().gameObject);
                    adjacents[i].setCubeType(CubesTypes.anyBlock);
                }

            }
            catch (NullReferenceException)
            {
            }
        }
        
        return explosives;
    }
}
