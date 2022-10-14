// using UnityEngine;
// using UnityEngine.UIElements;
//
// public class PlayerDisplay : MonoBehaviour
// {
//     private Label _ppMoney;
//
//     private Label _ppName;
//     private Label _ppPopulation;
//
//     private void Start()
//     {
//         var root = GetComponent<UIDocument>().rootVisualElement;
//         _ppMoney = root.Q<Label>("PPMoney");
//         _ppName = root.Q<Label>("PPName");
//         _ppPopulation = root.Q<Label>("PPPopulation");
//     }
//
//     public void UsePlayerCountryData(CountryData countryData)
//     {
//         if (countryData == null)
//         {
//             Debug.LogError("Country data is null");
//             return;
//         }
//
//         _ppName.text = countryData.Name;
//         _ppMoney.text = countryData.Money.ToString("F1");
//         _ppPopulation.text = countryData.Population.ToString("F1");
//     }
// }
