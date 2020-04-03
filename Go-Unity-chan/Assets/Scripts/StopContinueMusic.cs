using UnityEngine;
using UnityEngine.UI;

public class StopContinueMusic : MonoBehaviour
{

    public AudioSource music;
    public Image buttonImage;
    public Sprite[] sprites;
    private bool isPlaying = true;
    // Start is called before the first frame update
    public void toggleMusicState()
    {
        isPlaying = !isPlaying;
        if (isPlaying)
        {
            music.Play();
            buttonImage.sprite = sprites[0];
        }
        else
        {
            music.Stop();
            buttonImage.sprite = sprites[1];
        }
    }

    void Start()
    {
        if (isPlaying)
        {
            buttonImage.sprite = sprites[0];
            music.Play();
        }
        else
            buttonImage.sprite = sprites[1];

    }
}
