using UnityEngine;
using UnityEngine.UI;

namespace Squared
{
    [AddComponentMenu("Squared/Level Completed UI")]
    [DisallowMultipleComponent]
    public class LevelCompletedUI : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField] private Button _menuButton = null;
        [SerializeField] private Button _nextButton = null;
        [SerializeField] private SceneTransition _sceneTransition = null;
        [SerializeField] private LevelSO[] _levelSOs = { };
        #endregion

        #region Unity Methods
        private void Awake()
        {
            _menuButton.onClick.AddListener(() => _sceneTransition.TransitionTo("Menu"));
            _nextButton.onClick.AddListener(() =>
            {
                Board.LevelSOIndex++;
                _sceneTransition.TransitionTo("Game");
            });

            if (Board.LevelSOIndex == _levelSOs.Length - 1) _nextButton.interactable = false;
        }
        #endregion
    }
}
