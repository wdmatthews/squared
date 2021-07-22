using UnityEngine;
using UnityEngine.UI;

namespace Squared
{
    [AddComponentMenu("Squared/Menu")]
    [DisallowMultipleComponent]
    public class Menu : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField] private SceneTransition _sceneTransition = null;
        [SerializeField] private Transform _mainWindow = null;
        [SerializeField] private Button _playButton = null;
        [SerializeField] private Button _creditsButton = null;
        [SerializeField] private Transform _playWindow = null;
        [SerializeField] private Transform _levelScrollViewContent = null;
        [SerializeField] private LevelChoice _levelChoicePrefab = null;
        [SerializeField] private Button _playBackButton = null;
        [SerializeField] private Transform _creditsWindow = null;
        [SerializeField] private Button _creditsBackButton = null;
        [SerializeField] private LevelSO[] _levelSOs = { };
        #endregion

        #region Unity Methods
        private void Awake()
        {
            _mainWindow.gameObject.SetActive(true);
            _playWindow.gameObject.SetActive(false);
            _creditsWindow.gameObject.SetActive(false);
            _playButton.onClick.AddListener(() =>
            {
                _sceneTransition.Transition(() =>
                {
                    _mainWindow.gameObject.SetActive(false);
                    _playWindow.gameObject.SetActive(true);
                    _sceneTransition.gameObject.SetActive(false);
                });
            });
            _creditsButton.onClick.AddListener(() =>
            {
                _sceneTransition.Transition(() =>
                {
                    _mainWindow.gameObject.SetActive(false);
                    _creditsWindow.gameObject.SetActive(true);
                    _sceneTransition.gameObject.SetActive(false);
                });
            });
            _playBackButton.onClick.AddListener(() =>
            {
                _sceneTransition.Transition(() =>
                {
                    _playWindow.gameObject.SetActive(false);
                    _mainWindow.gameObject.SetActive(true);
                    _sceneTransition.gameObject.SetActive(false);
                });
            });
            _creditsBackButton.onClick.AddListener(() =>
            {
                _sceneTransition.Transition(() =>
                {
                    _creditsWindow.gameObject.SetActive(false);
                    _mainWindow.gameObject.SetActive(true);
                    _sceneTransition.gameObject.SetActive(false);
                });
            });

            int levelCount = _levelSOs.Length;

            for (int i = 0; i < levelCount; i++)
            {
                LevelChoice levelChoice = Instantiate(_levelChoicePrefab, _levelScrollViewContent);
                levelChoice.Initialize(i, SelectLevel);
            }
        }
        #endregion

        #region Public Methods
        public void SelectLevel(int levelIndex)
        {
            Board.LevelSOIndex = levelIndex;
            _sceneTransition.TransitionTo("Game");
        }
        #endregion
    }
}
