using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace Squared
{
    [AddComponentMenu("Squared/Scene Transition")]
    [DisallowMultipleComponent]
    public class SceneTransition : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField] private RectTransform _rectTransform = null;
        [SerializeField] private float _slideAnimationDuration = 0.15f;
        #endregion

        #region Public Methods
        public void Transition(TweenCallback onTransitionDone, bool fromOffscreen = true)
        {
            gameObject.SetActive(true);
            int direction = Random.Range(0, 4);
            Vector2 onscreenPosition = new Vector2();
            Vector2 offscreenPosition = new Vector2(direction < 2 ? ((direction == 0 ? 1 : -1) * _rectTransform.rect.width) : 0,
                direction > 1 ? ((direction == 2 ? 1 : -1) * _rectTransform.rect.height) : 0);
            _rectTransform.DOAnchorPos(fromOffscreen ? onscreenPosition : offscreenPosition, _slideAnimationDuration)
                .From(fromOffscreen ? offscreenPosition : onscreenPosition)
                .OnComplete(onTransitionDone);
        }

        public void TransitionTo(string sceneName)
        {
            Transition(() =>
            {
                DOTween.KillAll();
                SceneManager.LoadScene(sceneName);

                if (sceneName == "Menu") Audio.PlayMenu();
                else if (sceneName == "Game") Audio.PlayGame();
                else if (sceneName == "Level Completed") Audio.PlayLevelCompleted();
            });
        }
        #endregion
    }
}
