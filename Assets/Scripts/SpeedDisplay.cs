// using System;
// using UnityEngine;
// using UnityEngine.UIElements;
//
// public class SpeedDisplay : MonoBehaviour
// {
//     private VisualElement _sdFast;
//     private VisualElement _sdNormal;
//     private VisualElement _sdPaused;
//     private VisualElement _sdSlow;
//     private VisualElement _sdSuperFast;
//     private VisualElement[] _visualElements;
//
//     public void Awake()
//     {
//         GameSpeedManager.Instance.OnGameSpeedChanged += OnGameSpeedChanged;
//     }
//
//     public void Start()
//     {
//         var root = GetComponent<UIDocument>().rootVisualElement;
//         _sdPaused = root.Q<VisualElement>("SDPaused");
//         _sdSlow = root.Q<VisualElement>("SDSlow");
//         _sdNormal = root.Q<VisualElement>("SDNormal");
//         _sdFast = root.Q<VisualElement>("SDFast");
//         _sdSuperFast = root.Q<VisualElement>("SDSuperFast");
//
//         _visualElements = new[] { _sdPaused, _sdSlow, _sdNormal, _sdFast, _sdSuperFast };
//     }
//
//     public void OnDestroy()
//     {
//         GameSpeedManager.Instance.OnGameSpeedChanged -= OnGameSpeedChanged;
//     }
//
//     private void OnGameSpeedChanged(EGameSpeed newGameSpeed)
//     {
//         UseSpeedDisplay();
//         SpeedController.SwitchGameSpeed(newGameSpeed);
//     }
//
//     private void UseSpeedDisplay()
//     {
//         foreach (var speedDisplay in _visualElements)
//             speedDisplay.style.opacity = 0;
//
//         //set the opacity of the image whose name matches the current game speed to 1
//         switch (GameSpeedManager.Instance.CurrentGameSpeed)
//         {
//             case EGameSpeed.Paused:
//                 _sdPaused.style.opacity = 1;
//                 break;
//             case EGameSpeed.Slow:
//                 _sdSlow.style.opacity = 1;
//                 break;
//             case EGameSpeed.Normal:
//                 _sdNormal.style.opacity = 1;
//                 break;
//             case EGameSpeed.Fast:
//                 _sdFast.style.opacity = 1;
//                 break;
//             case EGameSpeed.Superfast:
//                 _sdSuperFast.style.opacity = 1;
//                 break;
//             default:
//                 throw new ArgumentOutOfRangeException();
//         }
//     }
// }
