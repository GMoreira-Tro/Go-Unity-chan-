using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class UIGameControl : MonoBehaviour
{
    public Text timeText;

    int[] hoursMinsAndSeconds = { 0, 0, 0 };
    static Thread timer;
    bool setTime;
    void Start()
    {
        //Seta o timer para a contagem do tempo
        timer = new Thread(TimerTick);
        timer.Start();
    }

    // Update is called once per frame
    private void Update()
    {
        if (setTime)
        {
            setTimeText();
            setTime = false;
        }
    }
    void TimerTick()
    {
        while(true)
        {
            Thread.Sleep(1000);

            incrementaHorario(2);
            setTime = true;
        }
      
    }

    void incrementaHorario(int i)
    {
        if (i != 0)
            hoursMinsAndSeconds[i] = (hoursMinsAndSeconds[i] + 1) % 60;
        else
            hoursMinsAndSeconds[i] = (hoursMinsAndSeconds[i] + 1) % 24;

        if (hoursMinsAndSeconds[i] == 0)
        {
            if (i != 0)
                incrementaHorario(--i);
            else
            {
                hoursMinsAndSeconds[0] = 24;
                timer.Abort();
            }
        }

    }

    void setTimeText()
    {
        if (hoursMinsAndSeconds[0] < 10)
            timeText.text = "0" + hoursMinsAndSeconds[0].ToString() + ":";
        else
            timeText.text = hoursMinsAndSeconds[0].ToString() + ":";

        if (hoursMinsAndSeconds[1] < 10)
            timeText.text += "0" + hoursMinsAndSeconds[1].ToString() + ":";
        else
            timeText.text += hoursMinsAndSeconds[1].ToString() + ":";

        if (hoursMinsAndSeconds[2] < 10)
            timeText.text += "0" + hoursMinsAndSeconds[2].ToString();
        else
            timeText.text += hoursMinsAndSeconds[2].ToString();
    }
}
