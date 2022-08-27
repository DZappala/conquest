using System;
using TMPro;
using UnityEngine;

public class DateDisplay : MonoBehaviour
{
    public TMP_Text DP_Date;

    public void UseDateDisplay(DateTime date)
    {
        DP_Date.text = date.ToString("dd/MM/yyyy");
    }
}
