using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artisso.TutorialSystem
{
    public enum TutorialSystemEventTriggerType
    {
        OnButtonClick,
        OnColliderTriggerEnter,
        PointerEnter,
        PointerExit,
        PointerUp,
        PointerDown,
        PointerClick,
        Drag,
        Drop,
        Scroll,
        UpdateSelected,
        Select,
        Deselect,
        Move,
        InitializePotentialDrag,
        BeginDrag,
        EndDrag,
        Submit,
        Cancel
    }

    public enum TutorialSystemDialogPosition
    {
        Default,
        Middle,
        TopLeft,
        BottomLeft,
        TopRight,
        BottomRight,
    }

    public enum HighlightMode
    {
        OutlineAll,
        OutlineVisible,
        OutlineHidden,
        OutlineAndSilhouette,
        SilhouetteOnly
    }

    public enum TutorialSystemSaveMethod
    {
        None,
        PlayerPrefs,
    }

    public enum TutorialSystemStepType
    {
        NextButtonClick,
        TargetClick,
        KeyInput,
        EventTrigger,
        TimeDelay,
    }

    public enum TutorialStepTargetType
    {
        TargetUi,
        Target3d,
        TargetSprite,
        EmptyOrNotSupported
    }
}