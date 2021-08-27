using System;
using UnityEngine;
using UnityEngine.UI;

namespace Artisso.TutorialSystem
{
    [DisallowMultipleComponent]
    [Serializable]
    public class TutorialSystemDialogTemplate: MonoBehaviour
    {
        public Text textToDisplay;
        public Button nextStepButton;
        public Button previousStepButton;
        public Button skipTutorialButton;
        public GameObject timer;
        public Text timerText;
        public Image portrait;

    }
}

