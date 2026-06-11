using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace dev.xrlab.NarraGol.Core
{
    public class AppShortcutManager : MonoBehaviour
    {
        private static AppShortcutManager instance;

        [Header("Scene Settings")]
        [SerializeField] private int mainSceneIndex = 0;

        [Header("Shortcuts")]
        [SerializeField] private bool enableShortcuts = true;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            DontDestroyOnLoad(gameObject);

            Debug.Log("AppShortcutManager initialized.");
        }

        private void Update()
        {
            if (!enableShortcuts)
                return;

            Keyboard keyboard = Keyboard.current;

            if (keyboard == null)
                return;

            if (IsReloadShortcutPressed(keyboard))
                ReloadMainScene();

            if (keyboard.escapeKey.wasPressedThisFrame)
                QuitApplication();

            if (keyboard.f11Key.wasPressedThisFrame)
                ToggleFullscreen();
        }

        private bool IsReloadShortcutPressed(Keyboard keyboard)
        {
            bool controlPressed =
                keyboard.leftCtrlKey.isPressed ||
                keyboard.rightCtrlKey.isPressed;

            return controlPressed &&
                   keyboard.rKey.wasPressedThisFrame;
        }

        public void ReloadMainScene()
        {
            Debug.Log($"Reloading main scene: {mainSceneIndex}");

            SceneManager.LoadScene(mainSceneIndex);
        }

        public void QuitApplication()
        {
            Debug.Log("Quitting application.");

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void ToggleFullscreen()
        {
            if (Screen.fullScreenMode == FullScreenMode.Windowed)
            {
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                Screen.fullScreen = true;
            }
            else
            {
                Screen.fullScreenMode = FullScreenMode.Windowed;
                Screen.fullScreen = false;
            }

            Debug.Log(
                $"Fullscreen Mode: {Screen.fullScreenMode}"
            );
        }
    }
}