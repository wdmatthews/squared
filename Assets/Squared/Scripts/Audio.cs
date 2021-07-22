using UnityEngine;
using DG.Tweening;

namespace Squared
{
    [AddComponentMenu("Squared/Audio")]
    [DisallowMultipleComponent]
    public class Audio : MonoBehaviour
    {
        #region Static Fields
        private static Audio _instance = null;
        #endregion

        #region Inspector Fields
        [SerializeField] private AudioSource _source = null;
        [SerializeField] private AudioClip _menuAudio = null;
        [SerializeField] private AudioClip _gameAudio = null;
        [SerializeField] private AudioClip _levelCompletedAudio = null;
        [SerializeField] private float _volume = 1;
        [SerializeField] private float _volumeFadeDuration = 0.15f;
        #endregion

        #region Unity Methods
        private void Start()
        {
            if (_instance)
            {
                if (_instance != this) Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
            _source.clip = _menuAudio;
            _source.volume = _volume;
            _source.Play();
        }
        #endregion

        #region Private Methods
        private void SwitchAudio(AudioClip newClip)
        {
            _source.DOFade(0, _volumeFadeDuration).OnComplete(() =>
            {
                _source.Stop();
                _source.clip = newClip;
                _source.Play();
                _source.DOFade(_volume, _volumeFadeDuration);
            });
        }
        #endregion

        #region Public Methods
        public static void PlayMenu() => _instance?.SwitchAudio(_instance?._menuAudio);
        public static void PlayGame() => _instance?.SwitchAudio(_instance?._gameAudio);
        public static void PlayLevelCompleted() => _instance?.SwitchAudio(_instance?._levelCompletedAudio);
        #endregion
    }
}
