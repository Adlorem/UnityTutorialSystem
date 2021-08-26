using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Artisso.TutorialSystem
{
    [DisallowMultipleComponent]
    public class MobileInputEventListener : MonoBehaviour
    {

        public TutorialSystemStepType stepType;
        private InputField _inputField = null;
        private TutorialSystem _tutorial = null;

        void Start()
        {
            _tutorial = TutorialSystem.Instance;
    
            _inputField = FindInput();

            if (_inputField != null && stepType == TutorialSystemStepType.KeyInput)
            {
                AddInputListener();
            }
            else
            {
                DestroyListener();
            }

        }

        private void AddInputListener()
        {
            if (_tutorial.currentStep.inputToWait == KeyCode.Return)
            {
                _inputField.onEndEdit.AddListener((data) => { OnEndEditEventDelegate(); });
            }
            else
            {
                _inputField.onValueChanged.AddListener((data) => { OnValueChangedEventDelegate(data); });
            }         
        }


        private void OnEndEditEventDelegate()
        {
            _inputField.onEndEdit.RemoveAllListeners();
            DestroyListener();
        }

        private void OnValueChangedEventDelegate(string data)
        {
            if (data.Equals(_tutorial.currentStep.inputToWait.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                _inputField.onValueChanged.RemoveAllListeners();
                DestroyListener();
            }
        }


        private InputField FindInput()
        {
            return _tutorial.currentStep.target.gameObject.GetComponent<InputField>();
        }

        private void DestroyListener()
        {
            _tutorial.GoToNextTutorialStep();
            Destroy(this);
        }

    }
}


