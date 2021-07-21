using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Squared
{
    [AddComponentMenu("Squared/Input")]
    [DisallowMultipleComponent]
    public class Input : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField] private Board _board = null;
        [SerializeField] private float _pointerMoveThreshold = 64;
        [SerializeField] private Button _menuButton = null;
        [SerializeField] private Button _retryButton = null;
        [SerializeField] private Button _slideLeftButton = null;
        [SerializeField] private Button _slideRightButton = null;
        [SerializeField] private Button _slideDownButton = null;
        [SerializeField] private Button _slideUpButton = null;
        #endregion

        #region Runtime Fields
        private bool _aKeyWasPressed = false;
        private bool _dKeyWasPressed = false;
        private bool _sKeyWasPressed = false;
        private bool _wKeyWasPressed = false;
        private bool _leftArrowKeyWasPressed = false;
        private bool _rightArrowKeyWasPressed = false;
        private bool _downArrowKeyWasPressed = false;
        private bool _upArrowKeyWasPressed = false;
        private bool _rKeyWasPressed = false;
        private bool _mouseWasPressed = false;
        private bool _screenWasTouched = false;
        private Vector2 _lastMousePosition = new Vector2();
        private Vector2 _lastTouchPosition = new Vector2();
        #endregion

        #region Unity Methods
        private void Awake()
        {
            // TODO Send to menu
            _menuButton.onClick.AddListener(() => Debug.Log("Menu"));
            _retryButton.onClick.AddListener(() => _board.Retry());
            _slideLeftButton.onClick.AddListener(() => _board.SlideTiles(new Vector2Int(-1, 0)));
            _slideRightButton.onClick.AddListener(() => _board.SlideTiles(new Vector2Int(1, 0)));
            _slideDownButton.onClick.AddListener(() => _board.SlideTiles(new Vector2Int(0, -1)));
            _slideUpButton.onClick.AddListener(() => _board.SlideTiles(new Vector2Int(0, 1)));
        }

        private void Update()
        {
            bool aKeyIsPressed = Keyboard.current != null ? Keyboard.current.aKey.isPressed : false;
            bool dKeyIsPressed = Keyboard.current != null ? Keyboard.current.dKey.isPressed : false;
            bool sKeyIsPressed = Keyboard.current != null ? Keyboard.current.sKey.isPressed : false;
            bool wKeyIsPressed = Keyboard.current != null ? Keyboard.current.wKey.isPressed : false;
            bool leftArrowKeyIsPressed = Keyboard.current != null ? Keyboard.current.leftArrowKey.isPressed : false;
            bool rightArrowKeyIsPressed = Keyboard.current != null ? Keyboard.current.rightArrowKey.isPressed : false;
            bool downArrowKeyIsPressed = Keyboard.current != null ? Keyboard.current.downArrowKey.isPressed : false;
            bool upArrowKeyIsPressed = Keyboard.current != null ? Keyboard.current.upArrowKey.isPressed : false;
            bool rKeyIsPressed = Keyboard.current != null ? Keyboard.current.rKey.isPressed : false;
            bool mouseIsPressed = Mouse.current != null ? Mouse.current.press.isPressed : false;
            bool screenIsTouched = Touchscreen.current != null ? Touchscreen.current.press.isPressed : false;
            Vector2 mousePosition = Mouse.current != null ? Mouse.current.position.ReadValue() : new Vector2();
            Vector2 touchPosition = Touchscreen.current != null ? Touchscreen.current.position.ReadValue() : new Vector2();
            Vector2Int slideDirection = new Vector2Int();

            if (aKeyIsPressed && !_aKeyWasPressed
                || leftArrowKeyIsPressed && !_leftArrowKeyWasPressed
                || !mouseIsPressed && _mouseWasPressed
                    && mousePosition.x < _lastMousePosition.x - _pointerMoveThreshold
                || !screenIsTouched && _screenWasTouched
                    && touchPosition.x < _lastTouchPosition.x - _pointerMoveThreshold)
            {
                slideDirection = new Vector2Int(-1, 0);
            }
            else if (dKeyIsPressed && !_dKeyWasPressed
                || rightArrowKeyIsPressed && !_rightArrowKeyWasPressed
                || !mouseIsPressed && _mouseWasPressed
                    && mousePosition.x > _lastMousePosition.x + _pointerMoveThreshold
                || !screenIsTouched && _screenWasTouched
                    && touchPosition.x > _lastTouchPosition.x + _pointerMoveThreshold)
            {
                slideDirection = new Vector2Int(1, 0);
            }
            else if (sKeyIsPressed && !_sKeyWasPressed
                || downArrowKeyIsPressed && !_downArrowKeyWasPressed
                || !mouseIsPressed && _mouseWasPressed
                    && mousePosition.y < _lastMousePosition.y - _pointerMoveThreshold
                || !screenIsTouched && _screenWasTouched
                    && touchPosition.y < _lastTouchPosition.y - _pointerMoveThreshold)
            {
                slideDirection = new Vector2Int(0, -1);
            }
            else if (wKeyIsPressed && !_wKeyWasPressed
                || upArrowKeyIsPressed && !_upArrowKeyWasPressed
                || !mouseIsPressed && _mouseWasPressed
                    && mousePosition.y > _lastMousePosition.y + _pointerMoveThreshold
                || !screenIsTouched && _screenWasTouched
                    && touchPosition.y > _lastTouchPosition.y + _pointerMoveThreshold)
            {
                slideDirection = new Vector2Int(0, 1);
            }

            if (slideDirection.x != 0 || slideDirection.y != 0)
            {
                _board.SlideTiles(slideDirection);
            }

            if (rKeyIsPressed && !_rKeyWasPressed) _board.Retry();

            _aKeyWasPressed = aKeyIsPressed;
            _dKeyWasPressed = dKeyIsPressed;
            _sKeyWasPressed = sKeyIsPressed;
            _wKeyWasPressed = wKeyIsPressed;
            _leftArrowKeyWasPressed = leftArrowKeyIsPressed;
            _rightArrowKeyWasPressed = rightArrowKeyIsPressed;
            _downArrowKeyWasPressed = downArrowKeyIsPressed;
            _upArrowKeyWasPressed = upArrowKeyIsPressed;
            _rKeyWasPressed = rKeyIsPressed;
            if (mouseIsPressed != _mouseWasPressed) _lastMousePosition = mousePosition;
            if (screenIsTouched != _screenWasTouched) _lastTouchPosition = touchPosition;
            _mouseWasPressed = mouseIsPressed;
            _screenWasTouched = screenIsTouched;
        }
        #endregion
    }
}
