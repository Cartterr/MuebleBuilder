using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class SceneUtility : MonoBehaviour
    {
        [SerializeField]
        InputActionReference resetAction;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (resetAction != null)
            {
                resetAction.action.performed += OnResetActionPerformed;
            }
        }

        void OnResetActionPerformed(InputAction.CallbackContext context)
        {
            ResetScene();
        }

        void ResetScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        void OnEnable()
        {
            if (resetAction != null)
                resetAction.action.Enable();
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        void OnDisable()
        {
            if (resetAction != null)
                resetAction.action.Disable();
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        void OnDestroy()
        {
            if (resetAction != null)
                resetAction.action.performed -= OnResetActionPerformed;
        }

        void OnSceneUnloaded(Scene current)
        {
            if (current == SceneManager.GetActiveScene())
            {
                LoaderUtility.Deinitialize();
                LoaderUtility.Initialize();
            }
        }
    }
}
