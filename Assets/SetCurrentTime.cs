using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class SetCurrentTime : MonoBehaviour
{
    [SerializeField] private GameController game;
    private TMP_Text text;

    public String time;

    private int seconds, minutes, hours;

    private float SECOND_UNIT = 1f;
    private float currentTime = 0f;

    private readonly WaitForSeconds waitForSeconds = new WaitForSeconds(1f);

    private void Start()
    {
        text = GetComponent<TMP_Text>();

        time = "00:00:00";

        StartCoroutine("SetTime");
    }

    private void Update()
    {
        text.text = time;
    }

    private IEnumerator SetTime() {
        while (true)
        {
            if (game.state == GameController.RUNNING)
            {
                seconds += 1;
                minutes += seconds % 59;
                hours += minutes % 59;

                seconds = seconds > 59 ? 0 : seconds;
                minutes = minutes > 59 ? 0 : minutes;

                time = GetFormatedTime(hours) + ":" + GetFormatedTime(minutes) + ":" + GetFormatedTime(seconds);
            }
        }
    }

    private String GetFormatedTime(int unformattedTime)
    {
        return unformattedTime < 10 ? "0" + unformattedTime : "" + unformattedTime;
    }
}
