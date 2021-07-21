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
        [SerializeField] protected float _moveAnimationDuration = 1;
        [SerializeField] protected float _mergeAnimationDuration = 1;
        [SerializeField] protected float _mergeAnimationScale = 1.1f;
        [SerializeField] protected float _removeAnimationDuration = 1;
        #endregion

        #region Runtime Fields
        protected TileSO _data = null;
        #endregion

        #region Properties
        public TileSO Data
        {
            get => _data;
            protected set
            {
                _data = value;
                Power = 1;
                NextPower = 1;
                _renderer.color = _data.Color;
                _label.text = $"{_data.BaseNumber}";
            }
        }
        public Vector2Int BoardPosition { get; protected set; }
        public Vector2Int NextBoardPosition { get; set; }
        public int Power { get; protected set; } = 1;
        public int NextPower { get; set; } = 1;
        #endregion

        #region Protected Methods
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

        public void Move(Vector3 worldPosition, Vector2Int boardPosition)
        {
            BoardPosition = boardPosition;
            transform.DOMove(worldPosition, _moveAnimationDuration);
        }

        public bool SetPower(int power)
        {
            if (power >= _powerSprites.Length)
            {
                Remove();
                return true;
            }

            Power = power;
            _renderer.sprite = _powerSprites[Power - 1];
            _label.text = $"{Pow(_data.BaseNumber, Power)}";
            transform.DOScale(_mergeAnimationScale, _mergeAnimationDuration / 2)
                    .OnComplete(() => transform.DOScale(1, _mergeAnimationDuration / 2));
            return false;
        }

        public void Remove()
        {
            transform.DOScale(0, _removeAnimationDuration)
                .OnComplete(() => gameObject.SetActive(false));
        }
        #endregion
    }
}
