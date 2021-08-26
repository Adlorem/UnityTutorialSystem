using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Artisso.TutorialSystem
{
    [DisallowMultipleComponent]
    public class OnButtonClickListener : MonoBehaviour
    {
        private Button _button = null;

        void Start()
        {
            _button = GetButton();

            if (_button != null && TutorialSystem.Instance.currentStep.eventType == TutorialSystemEventTriggerType.OnButtonClick)
            {
                AddButtonListener();
            }
            else
            {
                DestroyListener();
            }

        }

        private void AddButtonListener()
        {
            _button.onClick.AddListener(() => { OnButtonClickDelegate(); });
        }

        private void OnButtonClickDelegate()
        {
            _button.onClick.RemoveAllListeners();
            DestroyListener();
        }


        private Button GetButton()
        {
            return TutorialSystem.Instance.currentStep.target.gameObject.GetComponent<Button>();
        }

        private void DestroyListener()
        {
            TutorialSystem.Instance.GoToNextTutorialStep();
            Destroy(this);
        }

    }
}


