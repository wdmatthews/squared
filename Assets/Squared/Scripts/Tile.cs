using UnityEngine;
using TMPro;
using DG.Tweening;

namespace Squared
{
    [AddComponentMenu("Squared/Tile")]
    [DisallowMultipleComponent]
    public class Tile : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField] protected SpriteRenderer _renderer = null;
        [SerializeField] protected TextMeshPro _label = null;
        [SerializeField] protected Sprite[] _powerSprites = { };
        [SerializeField] protected float _placeAnimationDuration = 1;
        [SerializeField] protected float _placeAnimationScale = 1.1f;
        #endregion

        #region Runtime Fields
        protected TileSO _data = null;
        protected int _power = 1;
        #endregion

        #region Properties
        public TileSO Data
        {
            get => _data;
            protected set
            {
                _data = value;
                _power = 1;
                _renderer.color = _data.Color;
                _label.text = $"{_data.BaseNumber}";
            }
        }
        public Vector2Int BoardPosition { get; protected set; }
        #endregion

        #region Protected Methods
        protected void IncreasePower()
        {
            _renderer.sprite = _powerSprites[_power];
            _power++;
            _label.text = $"{Pow(_data.BaseNumber, _power)}";
        }

        protected int Pow(int baseNumber, int power)
        {
            int result = baseNumber;

            for (int i = 1; i < power; i++)
            {
                result *= baseNumber;
            }

            return result;
        }
        #endregion

        #region Public Methods
        public void Place(Vector3 worldPosition, TileSO data, Vector2Int boardPosition)
        {
            transform.position = worldPosition;
            Data = data;
            BoardPosition = boardPosition;
            gameObject.SetActive(true);
            transform.DOScale(1, _placeAnimationDuration).From(_placeAnimationScale);
        }
        #endregion
    }
}
