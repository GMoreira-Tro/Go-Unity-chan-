using UnityEngine;

public class BreakableCube : MonoBehaviour
{
    public Transform playerPosition;
    public Transform position;
    char passOnTimes;
    bool pass;
    void Update()
    {
        if (!pass)
        {
            if (
            Mathf.RoundToInt(playerPosition.position.x) == position.position.x &&
            Mathf.RoundToInt(playerPosition.position.y)-1 == position.position.y &&
            Mathf.RoundToInt(playerPosition.position.z) == position.position.z)
            {
                pass = true;
                passOnTimes++;
                Debug.Log("passou");
            }
        }
        else if(
            Mathf.RoundToInt(playerPosition.position.x) != position.position.x ||
            Mathf.RoundToInt(playerPosition.position.y)-1 != position.position.y ||
            Mathf.RoundToInt(playerPosition.position.z) != position.position.z) 
            {
            pass = false;
        }
        
        if(passOnTimes == 3)
        {
            Destroy(gameObject);
        }
    }
}
