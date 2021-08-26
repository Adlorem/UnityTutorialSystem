using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Artisso.TutorialSystem
{
    [DisallowMultipleComponent]
    public class EventTriggerListener : MonoBehaviour
    {
        public TutorialSystemEventTriggerType eventType;

        private EventTrigger _trigger;
        private EventTrigger.Entry _eventEntry = null;

        void Start()
        {
            _trigger = FindEventTrigger();

            if (_trigger != null)
            {
                AddEventListener();
            }
            else
            {
                DestroyListener();
            }
        }

        private void AddEventListener()
        {
            switch (eventType)
            {
                case TutorialSystemEventTriggerType.BeginDrag:
                    _eventEntry = _trigger.triggers.Find(x => x.eventID == EventTriggerType.BeginDrag);
                    break;
                case TutorialSystemEventTriggerType.Cancel:
                    _eventEntry = _trigger.triggers.Find(x => x.eventID == EventTriggerType.Cancel);
                    break;
                case TutorialSystemEventTriggerType.Deselect:
                    _eventEntry = _trigger.triggers.Find(x => x.eventID == EventTriggerType.Deselect);
                    break;
                case TutorialSystemEventTriggerType.Drag:
                    _eventEntry = _trigger.triggers.Find(x => x.eventID == EventTriggerType.Drag);
                    break;
                case TutorialSystemEventTriggerType.Drop:
                    _eventEntry = _trigger.triggers.Find(x => x.eventID == EventTriggerType.Drop);
                    break;
                case TutorialSystemEventTriggerType.EndDrag:
                    _eventEntry = _trigger.triggers.Find(x => x.eventID == EventTriggerType.EndDrag);
                    break;
                case TutorialSystemEventTriggerType.InitializePotentialDrag:
                    _eventEntry = _trigger.triggers.Find(x => x.eventID == EventTriggerType.InitializePotentialDrag);
                    break;
                case TutorialSystemEventTriggerType.Move:
                    _eventEntry = _trigger.triggers.Find(x => x.eventID == EventTriggerType.Move);
                    break;
                case TutorialSystemEventTriggerType.PointerClick:
                    _eventEntry = _trigger.triggers.Find(x => x.eventID == EventTriggerType.PointerClick);
                    break;
                case TutorialSystemEventTriggerType.PointerDown:
                    _eventEntry = _trigger.triggers.Find(x => x.eventID == EventTriggerType.PointerDown);
                    break;
                case TutorialSystemEventTriggerType.PointerEnter:
                    _eventEntry = _trigger.triggers.Find(x => x.eventID == EventTriggerType.PointerEnter);
                    break;
                case TutorialSystemEventTriggerType.PointerExit:
                    _eventEntry = _trigger.triggers.Find(x => x.eventID == EventTriggerType.PointerExit);
                    break;
                case TutorialSystemEventTriggerType.PointerUp:
                    _eventEntry = _trigger.triggers.Find(x => x.eventID == EventTriggerType.PointerUp);
                    break;
                case TutorialSystemEventTriggerType.Scroll:
                    _eventEntry = _trigger.triggers.Find(x => x.eventID == EventTriggerType.Scroll);
                    break;
                case TutorialSystemEventTriggerType.Select:
                    _eventEntry = _trigger.triggers.Find(x => x.eventID == EventTriggerType.Select);
                    break;
                case TutorialSystemEventTriggerType.Submit:
                    _eventEntry = _trigger.triggers.Find(x => x.eventID == EventTriggerType.Submit);
                    break;
                case TutorialSystemEventTriggerType.UpdateSelected:
                    _eventEntry = _trigger.triggers.Find(x => x.eventID == EventTriggerType.UpdateSelected);
                    break;
            }

            if (_eventEntry != null)
            {
                _eventEntry.callback.AddListener((data) => { OnPointerEventDelegate(); });
            }
            else
            {
                DestroyListener();
            }
        }

        private void OnPointerEventDelegate()
        {
            _eventEntry.callback.RemoveAllListeners();
            DestroyListener();
        }

        private EventTrigger FindEventTrigger()
        {
            return TutorialSystem.Instance.currentStep.target.gameObject.GetComponent<EventTrigger>();
        }

        private void DestroyListener()
        {
            TutorialSystem.Instance.GoToNextTutorialStep();
            Destroy(this);
        }

    }
}


