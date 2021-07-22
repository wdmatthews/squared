using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Squared
{
    [AddComponentMenu("Squared/Level Choice")]
    [DisallowMultipleComponent]
    public class LevelChoice : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField] private TextMeshProUGUI _numberLabel = null;
        [SerializeField] private Button _selectButton = null;
        #endregion

        #region Runtime Fields
        private int _levelIndex = 0;
        #endregion

        #region Public Methods
        public void Initialize(int levelIndex, System.Action<int> onSelect)
        {
            _levelIndex = levelIndex;
            _numberLabel.text = $"{_levelIndex + 1}";
            _selectButton.onClick.AddListener(() => onSelect(_levelIndex));
        }
        #endregion
    }
}
