using System;
using UnityEngine;

namespace Artisso.TutorialSystem
{
    [Serializable]
    public class TutorialSystemOptionalParameters
    {
        [Tooltip("If true shows skip tutorial button")]
        public bool allowToSkip;

        [Tooltip("Destroys tutorial system when current tutorial is finished. Please note that this will break every reference to current tutorial system")]
        public bool destroyOnEnd;

        [Tooltip("Specify canvas overlay color when disabling UI")]
        public Color overlayColor = Color.clear;

        [Tooltip("Tutorial saving system")]
        public TutorialSystemSaveMethod saveMethod;

        [Tooltip("The higher the value, the slower the speed. 0 means no typing effect")]
        public float textDisplaySpeed;

        [Tooltip("Enable this if you want to display an image in dialog window")]
        public bool displayImage;

        [Tooltip("The sprite of default image")]
        public Sprite defaultImage;
    }
}