using TMPro;
using UnityEngine;

public class DateDisplay : MonoBehaviour
{
    public TMP_Text DP_Date;

    public void UseDateDisplay()
    {
        DP_Date.text = GameControl.Date.ToString("dd/MM/yyyy");
    }
}
