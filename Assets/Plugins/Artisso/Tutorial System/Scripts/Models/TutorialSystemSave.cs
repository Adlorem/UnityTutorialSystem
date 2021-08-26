using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artisso.TutorialSystem
{
    [Serializable]
    public struct TutorialSystemSave
    {
        public int tutorialNumber;
        public int stepNumber;
        [Tooltip("Tutorial save method")]
        public TutorialSystemSaveMethod saveMethod;
    }
}