// using UnityEngine;
// using UnityEngine.UIElements;
//
// public class CountryDisplay : MonoBehaviour
// {
//     private CountryData _countryData;
//     private Label _cpMoney;
//     private Label _cpName;
//     private Label _cpPopulation;
//
//     public void Awake()
//     {
//         CountryDisplayManager.Instance.OnCountrySelected += OnCountrySelected;
//     }
//
//     public void Start()
//     {
//         gameObject.SetActive(false);
//         var root = GetComponent<UIDocument>().rootVisualElement;
//         _cpName = root.Q<Label>("CPName");
//         _cpMoney = root.Q<Label>("CPMoney");
//         _cpPopulation = root.Q<Label>("CPPopulation");
//     }
//
//     public void Update()
//     {
//         if (CountryDisplayManager.Instance.IsCountrySelected)
//             //HACK is there a way to invoke this through the instance instead?
//             OnCountrySelected(_countryData);
//     }
//
//     public void OnDestroy()
//     {
//         CountryDisplayManager.Instance.OnCountrySelected -= OnCountrySelected;
//     }
//
//     private void OnCountrySelected(CountryData countryDataFromEventParam)
//     {
//         if (countryDataFromEventParam == null)
//         {
//             Debug.LogError("Country data is null");
//             return;
//         }
//
//         if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
//
//         _countryData = countryDataFromEventParam;
//         UpdateCountryData();
//     }
//
//     private void UpdateCountryData()
//     {
//         _cpName.text = _countryData.Name;
//         _cpMoney.text = _countryData.Money.ToString("F1");
//         _cpPopulation.text = _countryData.Population.ToString("F1");
//     }
// }
