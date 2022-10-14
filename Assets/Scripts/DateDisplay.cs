// using System;
// using UnityEngine;
// using UnityEngine.UIElements;
//
// public class DateDisplay : MonoBehaviour
// {
//     private Label _dpDate;
//
//     public void Start()
//     {
//         var root = GetComponent<UIDocument>().rootVisualElement;
//         _dpDate = root.Q<Label>("DPDate");
//     }
//
//     public void UseDateDisplay(DateTime date)
//     {
//         _dpDate.text = date.ToString("dd MMM yyyy");
//     }
// }
