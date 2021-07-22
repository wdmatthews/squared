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
        [SerializeField] private Button _playButton = null;
        #endregion

        #region Unity Methods
        private void Awake()
        {
            
        }
        #endregion
    }
}
