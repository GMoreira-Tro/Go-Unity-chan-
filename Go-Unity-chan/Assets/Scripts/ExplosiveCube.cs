using System.Collections;
using System.Threading;
using UnityEngine;

public class ExplosiveCube : MonoBehaviour
{
    public Transform playerPosition;
    public Transform position;
    Thread tick;
    bool katsu;
    // Update is called once per frame
    void Update()
    {
        if (
        Mathf.RoundToInt(playerPosition.position.x) == position.position.x &&
        Mathf.RoundToInt(playerPosition.position.y) - 1 == position.position.y &&
        Mathf.RoundToInt(playerPosition.position.z) == position.position.z)
        {
            tick = new Thread(explode);
            tick.Start();
        }
        if (katsu)
        {
            Destroy(gameObject);
        }
    }

    void explode()
    {
        Thread.Sleep(3000);
        katsu = true;
    }
}
