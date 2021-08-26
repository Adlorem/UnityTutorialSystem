using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Artisso.TutorialSystem
{
    [Serializable]
    public class TutorialSystemTutorials
    {
        public List<TutorialSystemStep> stepList = new List<TutorialSystemStep>();
    }

    [Serializable]
    public class TutorialSystemStep
    {
        [Tooltip("Text to display in current step dialog window")]
        [TextArea]
        public string textToDisplay;

        [Tooltip("Choose action required to finish this step")]
        public TutorialSystemStepType actionType;

        [Tooltip("Awaits specific keycode to be pressed before proceeding to the next step")]
        [ConditionalAttributeProperty("actionType", TutorialSystemStepType.KeyInput)]
        public KeyCode inputToWait;

        [Tooltip("Time to wait before proceeding to the next step")]
        [ConditionalAttributeProperty("actionType", TutorialSystemStepType.TimeDelay)]
        public float timeToWait;

        [Tooltip("Awaits target game object Event Trigger component to fire specified event type before proceeding to the next step")]
        [ConditionalAttributeProperty("actionType", TutorialSystemStepType.EventTrigger)]
        public TutorialSystemEventTriggerType eventType;

        [HideInInspector]
        public float timer = 0f;

        [Tooltip("Disables interface in current step, except target game object")]
        public bool lockInterface;

        [Tooltip("True = shows colored overlay\r\nFalse = transparent overlay")]
        [ConditionalAttributeProperty("lockInterface", true)]
        public bool showOverlay;

        [Tooltip("Sets time scale to 0 for in this step")]
        public bool freezeTime;

        [Tooltip("Places dialog window at specific screen position. Default option tries to attach dialog to target game object")]
        public TutorialSystemDialogPosition dialogPosition;

        [Tooltip("Adjusts dialog window position with specified padding")]
        public Vector2 dialogPadding;

        [Tooltip("Target object - can be any game object")]
        public GameObject target;

        [Tooltip("Outlines target object")]
        public bool outlineTargetObject;

        [Tooltip("Outline color")]
        public Color outlineColor;

        [Tooltip("Outline distance, determinates outline thickness")]
        [Range(0f, 10f)]
        public float outlineDistance;

        [Tooltip("If true to display default image")]
        public bool useImage;

        [Tooltip("Specify image for this step if different from default image")]
        public Sprite specificImage;

        [Tooltip("Event to be performed when current step is finished")]
        public UnityEvent onStepEnd;

        [Tooltip("Saves current step if saving system is enabled")]
        public bool saveThisStep;
    }
}