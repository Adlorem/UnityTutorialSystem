using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Artisso.TutorialSystem
{
    [DisallowMultipleComponent]
    public class ColliderOnTriggerEnterListener : MonoBehaviour
    {

        private void OnTriggerEnter(Collider other)
        {
            DestroyListener();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            DestroyListener();
        }

        private void DestroyListener()
        {
            TutorialSystem.Instance.GoToNextTutorialStep();
            Destroy(this);
        }

    }
}


