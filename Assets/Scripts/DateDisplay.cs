using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class DateDisplay : MonoBehaviour
{
    [FormerlySerializedAs("DP_Date")] public TMP_Text dpDate;

    public void UseDateDisplay(DateTime date)
    {
        dpDate.text = date.ToString("dd MMM yyyy");
    }
}
