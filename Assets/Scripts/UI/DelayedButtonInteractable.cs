using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace dev.xrlab.NarraGol.UI
{
    public class DelayedButtonInteractable : MonoBehaviour
    {
        [Header("Button")]
        [SerializeField] private Button targetButton;

        [Header("Settings")]
        [SerializeField] private float delaySeconds = 3f;
        [SerializeField] private bool enableOnStart = true;
        [SerializeField] private bool disableButtonOnStart = true;

        private Coroutine enableRoutine;

        private void Reset()
        {
            targetButton = GetComponent<Button>();
        }

        private void Start()
        {
            if (targetButton == null)
            {
                Debug.LogError(
                    $"{nameof(DelayedButtonInteractable)}: Target Button not assigned."
                );
                return;
            }

            if (disableButtonOnStart)
            {
                targetButton.interactable = false;
            }

            if (enableOnStart)
            {
                EnableAfterDelay();
            }
        }

        public void EnableAfterDelay()
        {
            if (enableRoutine != null)
            {
                StopCoroutine(enableRoutine);
            }

            enableRoutine = StartCoroutine(
                EnableRoutine()
            );
        }

        public void EnableImmediately()
        {
            if (enableRoutine != null)
            {
                StopCoroutine(enableRoutine);
                enableRoutine = null;
            }

            targetButton.interactable = true;
        }

        public void DisableImmediately()
        {
            if (enableRoutine != null)
            {
                StopCoroutine(enableRoutine);
                enableRoutine = null;
            }

            targetButton.interactable = false;
        }

        private IEnumerator EnableRoutine()
        {
            yield return new WaitForSeconds(delaySeconds);

            targetButton.interactable = true;

            enableRoutine = null;
        }
    }
}