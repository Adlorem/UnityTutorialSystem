using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
//using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Artisso.TutorialSystem
{

    [DisallowMultipleComponent]
    [Serializable]
    public class TutorialSystem : MonoBehaviour
    {
        public static TutorialSystem Instance;

        [Tooltip("If set to true starts tutorial automatically. To start manually use prefab event : TutorialSystem.StartTutorial() with tutorial number")]
        public bool autoStart = true;

        [Tooltip("Starts specific tutorial from tutorials list")]
        public int tutorialNumber;

        [Tooltip("Canvas that will be used to display tutorial window")]
        public Canvas defaultCanvas;

        [Tooltip("Tutorial dialog window prefab")]
        public TutorialSystemDialogTemplate dialogTemplate;

        [Header("Additional Parameters")]
        public TutorialSystemOptionalParameters optionalParameters;

        [Header("Tutorial Editor")]
        public List<TutorialSystemTutorials> tutorialList = new List<TutorialSystemTutorials>();

        [HideInInspector]
        public TutorialSystemStep currentStep = new TutorialSystemStep();

        // private variables
        private int _currentStepIndex = 0;
        private GameObject _selectedObject;
        private List<TutorialSystemStep> _currentTutorial;
        private Coroutine _textTypingCorouitune;
        private GameObject _canvasOverlay;
        private Texture2D _outlineTexture;
        private TutorialStepTargetType _currentStepTargetType;

        void Awake()
        {
            CreateTutorialInstance();
        }

        void Start()
        {
            if (autoStart)
            {
                if (optionalParameters.saveMethod == TutorialSystemSaveMethod.PlayerPrefs)
                {
                    if (PlayerPrefs.GetInt($"TutorialNumber_{tutorialNumber}") == 1)
                    {
                        EndTutorial();
                        return;
                    }
                }

                StartTutorial(tutorialNumber);
            }
        }


        void Update()
        {
            if (!IsTutorialFinished())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    switch (_currentStepTargetType)
                    {
                        case TutorialStepTargetType.TargetUi:
                            _selectedObject = GetUiElementAtCursor();
                            break;
                        case TutorialStepTargetType.TargetSprite:
                            _selectedObject = GetSpriteObjectAtCursor();
                            break;
                        case TutorialStepTargetType.Target3d:
                            _selectedObject = Get3dObjectAtCursor();
                            break;
                        default:
                            break;
                    }
                }

                CheckTutorialActionType();
            }
        }

        private void CheckTutorialActionType()
        {
            switch (currentStep.actionType)
            {
                case TutorialSystemStepType.TargetClick:

                    if (_selectedObject == currentStep.target.gameObject)
                    {
                        GoToNextTutorialStep();
                    }
                    break;

                case TutorialSystemStepType.KeyInput:
                    if (Input.GetKeyDown(currentStep.inputToWait))
                    {
                        GoToNextTutorialStep();
                    }
                    break;

                case TutorialSystemStepType.TimeDelay:

                    currentStep.timer += Time.deltaTime;
                    dialogTemplate.timerText.text = (currentStep.timeToWait - currentStep.timer).ToString("0");
                    if (currentStep.timer >= currentStep.timeToWait)
                    {
                        GoToNextTutorialStep();
                    }
                    break;
                default:
                    break;
            }
        }

        public void StartTutorial(int tutorialNumber)
        {
            InitializeParameters(tutorialNumber);
            PlayTutorialStep(_currentStepIndex);
        }

        private void InitializeParameters(int tutorialNumber)
        {
            //initialize start parameters
            _currentTutorial = tutorialList[tutorialNumber].stepList;
            this.enabled = true;
            dialogTemplate.gameObject.SetActive(true);
            _canvasOverlay.gameObject.SetActive(true);
            _selectedObject = null;


            if (optionalParameters.allowToSkip)
            {
                dialogTemplate.skipTutorialButton.gameObject.SetActive(true);
            }
            else
            {
                dialogTemplate.skipTutorialButton.gameObject.SetActive(false);
            }

           
            if (optionalParameters.saveMethod == TutorialSystemSaveMethod.PlayerPrefs)
            {
                var stepIndex = PlayerPrefs.GetInt($"Tutorial_{tutorialNumber}_Step");

                if (stepIndex != 0)
                {
                    _currentStepIndex = stepIndex;
                }
            }
            else
            {
                if (IsTutorialFinished())
                {
                    _currentStepIndex = 0;
                }
            }
        }

        private void OnSkiptButtonClicked()
        {
            EndTutorial();
        }

        private void OnDialogNextButtonClicked()
        {
            GoToNextTutorialStep();
        }

        private void OnDialogPreviousButtonClicked()
        {
            GoToPreviousTutorialStep();
        }

        private void CreateTutorialInstance()
        {
            if (Instance == null)
            {
                Instance = this;

                string errors = ValidateInstance();

                if (string.IsNullOrEmpty(errors))
                {
                    var dc = defaultCanvas.GetComponent<RectTransform>();
                    _canvasOverlay = CreateUiOverlay("TutorialSystemCanvasOverlay", dc.sizeDelta);
                    dialogTemplate = Instantiate(dialogTemplate, defaultCanvas.transform);
                    dialogTemplate.gameObject.SetActive(false);
                    //add dialog buttons listeners
                    dialogTemplate.nextStepButton.onClick.AddListener(delegate { OnDialogNextButtonClicked(); });
                    dialogTemplate.previousStepButton.onClick.AddListener(delegate { OnDialogPreviousButtonClicked(); });
                    dialogTemplate.skipTutorialButton.onClick.AddListener(delegate { OnSkiptButtonClicked(); });
                }
                else
                {
                    Debug.LogError(errors);
                    this.enabled = false;
                }
            }
        }



        private string ValidateInstance()
        {
            StringBuilder sb = new StringBuilder();

            if (defaultCanvas == null)
            {
                sb.Append("ERROR: No default canvas defined\r\n");
            }
            if (dialogTemplate == null)
            {
                sb.Append("ERROR: No dialog tempate defined\r\n");
            }
            if (dialogTemplate.nextStepButton == null)
            {
                sb.Append("ERROR: No next step button defined in dialog template\r\n");
            }
            if (dialogTemplate.previousStepButton == null)
            {
                sb.Append("ERROR: No previous step button defined in dialog template\r\n");
            }
            if (dialogTemplate.skipTutorialButton == null)
            {
                sb.Append("ERROR: No skip tutorial button defined in dialog template\r\n");
            }
            if (dialogTemplate.timer == null)
            {
                sb.Append("ERROR: No timer field defined in dialog template\r\n");
            }
            if (dialogTemplate.timerText == null)
            {
                sb.Append("ERROR: No timer text field defined in dialog template\r\n");
            }
            if (tutorialList.ElementAtOrDefault(tutorialNumber) == null)
            {
                sb.Append("ERROR: No tutorials. Crete tutorial first\r\n");
            }
            return sb.ToString();
        }

        private void PlayTutorialStep(int stepId)
        {
            if (!IsTutorialFinished())
            {
                if (_textTypingCorouitune != null)
                {
                    StopCoroutine(_textTypingCorouitune);
                }

                _currentStepIndex = stepId;
                currentStep = _currentTutorial[stepId];
                _currentStepTargetType = GetCurrentStepTargetType();
                currentStep.timer = 0f;


                SetDialogForCurrentStep();
                DisplayDialogAtPosition(currentStep.target.gameObject);

                if (currentStep.actionType == TutorialSystemStepType.EventTrigger)
                {
                    AddConditionalListener();
                }

#if UNITY_ANDROID && !UNITY_EDITOR
                if (currentStep.actionType == TutorialSystemStepType.KeyInput)
                {
                    AddMobileKeybordListener();
                }
#endif

                if (currentStep.lockInterface)
                {
                    LockInterface();
                }

                // outline after lock interface to be able to swap target dynamically
                if (currentStep.outlineTargetObject)
                {
                    OutlineStepTarget();
                }

                if (currentStep.useImage)
                {
                    AddImageToDialogTemplate();
                }

                if (currentStep.freezeTime)
                {
                    Time.timeScale = 0;
                }
                else
                {
                    Time.timeScale = 1;
                }

                // uncomment this line to use unity Localization Component
                //currentStep.textToDisplay = LocalizationSettings.StringDatabase.GetLocalizedString(currentStep.textToDisplay);

                _textTypingCorouitune = StartCoroutine(DisplayTutorialText(currentStep.textToDisplay));
            }
            else
            {
                EndTutorial();
            }
        }

        private void SetDialogForCurrentStep()
        {
            dialogTemplate.gameObject.SetActive(true);

            dialogTemplate.previousStepButton.gameObject.SetActive(currentStep.allowBackStep);

            bool isNextButton = currentStep.actionType == TutorialSystemStepType.NextButtonClick;
            dialogTemplate.nextStepButton.gameObject.SetActive(isNextButton);

            bool isTimeButton = currentStep.actionType == TutorialSystemStepType.TimeDelay;
            dialogTemplate.timer.SetActive(isTimeButton);

        }

        private void AddImageToDialogTemplate()
        {
            if (dialogTemplate.portrait != null && optionalParameters.defaultImage != null)
            {
                dialogTemplate.portrait.enabled = true;

                if (currentStep.specificImage != null)
                {
                    dialogTemplate.portrait.sprite = currentStep.specificImage;
                }
                else
                {
                    dialogTemplate.portrait.sprite = optionalParameters.defaultImage;
                }
            }
        }

        private void OutlineStepTarget()
        {
            switch (_currentStepTargetType)
            {
                case TutorialStepTargetType.TargetUi:
                    if (currentStep.target != null)
                    {
                        GenerateUiOutline(currentStep.target.gameObject);
                    }
                    break;

                case TutorialStepTargetType.Target3d:
                    if (currentStep.target != null && currentStep.target.gameObject.GetComponent<TutorialSystem3dObjectHighlighter>() == null)
                    {
                        var outlineTarget = currentStep.target.gameObject.AddComponent<TutorialSystem3dObjectHighlighter>();
                        outlineTarget.OutlineColor = currentStep.outlineColor;
                        outlineTarget.OutlineWidth = currentStep.outlineDistance;
                    }
                    break;

                case TutorialStepTargetType.TargetSprite:
                    if (currentStep.target != null && currentStep.target.gameObject.GetComponent<TutorialSystemSpriteHightlighter>() == null)
                    {
                        var outlineTarget = currentStep.target.gameObject.AddComponent<TutorialSystemSpriteHightlighter>();
                        outlineTarget.outlineColor = currentStep.outlineColor;
                        outlineTarget.outlineDistance = (int)currentStep.outlineDistance;
                    }
                    break;
                default:
                    break;
            }

        }

        private void DestroyTargetOutline()
        {
            switch (_currentStepTargetType)
            {
                case TutorialStepTargetType.TargetUi:
                    break;

                case TutorialStepTargetType.Target3d:
                    if (currentStep.target != null)
                    {
                        var outlineTarget = currentStep.target.gameObject.GetComponent<TutorialSystem3dObjectHighlighter>();
                        if (outlineTarget != null) Destroy(outlineTarget);
                    }
                    break;

                case TutorialStepTargetType.TargetSprite:
                    if (currentStep.target != null)
                    {
                        var outlineTarget = currentStep.target.gameObject.GetComponent<TutorialSystemSpriteHightlighter>();
                        if (outlineTarget != null) Destroy(outlineTarget);
                    }
                    break;
                default:
                    break;
            }

        }

        public void GoToNextTutorialStep()
        {

            ResetCanvasOverlay();

            if (currentStep.outlineTargetObject)
            {
                DestroyTargetOutline();
            }

            if (optionalParameters.saveMethod != TutorialSystemSaveMethod.None)
            {
                if (currentStep.saveThisStep)
                {
                    SaveTutorialStep();
                }
            }

            if (currentStep.onStepEnd != null)
            {
                currentStep.onStepEnd.Invoke();
            }

            if (!IsTutorialFinished())
            {
                _currentStepIndex += 1;
                _selectedObject = null;

                StartCoroutine(GoToNextTutorialStepCoroutine());
            }

        }

        private void GoToPreviousTutorialStep()
        {
            if (_currentStepIndex > 0)
            {
                PlayTutorialStep(_currentStepIndex - 1);
            }
        }

        private IEnumerator GoToNextTutorialStepCoroutine()
        {
            yield return new WaitForEndOfFrame();
            PlayTutorialStep(_currentStepIndex);
            yield return null;
        }

        private IEnumerator DisplayTutorialText(string tutorialText)
        {
            dialogTemplate.textToDisplay.text = string.Empty;

            if (optionalParameters.textDisplaySpeed > 0 && !currentStep.freezeTime)
            {
                foreach (var c in tutorialText)
                {
                    dialogTemplate.textToDisplay.text += c;
                    yield return new WaitForSeconds(optionalParameters.textDisplaySpeed);
                }
            }
            else
            {
                dialogTemplate.textToDisplay.text = tutorialText;
            }

            yield return null;
        }

        private void DisplayDialogAtPosition(GameObject target)
        {
            var dt = dialogTemplate.GetComponent<RectTransform>();
            var dc = defaultCanvas.GetComponent<RectTransform>();
            var position = Vector3.zero;
            Vector3[] v = new Vector3[4];

            switch (currentStep.dialogPosition)
            {
                case TutorialSystemDialogPosition.Default:
                    if (target != null)
                        position = CalculateDialogPosition(target);
                    break;
                case TutorialSystemDialogPosition.Middle:
                    position = Vector3.zero;
                    break;
                case TutorialSystemDialogPosition.TopLeft:
                    dc.GetLocalCorners(v);
                    position = new Vector3((v[1].x) + ((dt.rect.width / 2) + currentStep.dialogPadding.x), v[1].y - ((dt.rect.height / 2) + currentStep.dialogPadding.y), v[1].z);
                    break;
                case TutorialSystemDialogPosition.BottomLeft:
                    dc.GetLocalCorners(v);
                    position = new Vector3((v[0].x) + ((dt.rect.width / 2) + currentStep.dialogPadding.x), v[0].y + ((dt.rect.height / 2) + currentStep.dialogPadding.y), v[0].z);
                    break;
                case TutorialSystemDialogPosition.TopRight:
                    dc.GetLocalCorners(v);
                    position = new Vector3((v[2].x) - ((dt.rect.width / 2) + currentStep.dialogPadding.x), v[2].y - ((dt.rect.height / 2) + currentStep.dialogPadding.y), v[2].z);
                    break;
                case TutorialSystemDialogPosition.BottomRight:
                    dc.GetLocalCorners(v);
                    position = new Vector3((v[3].x) - ((dt.rect.width / 2) + currentStep.dialogPadding.x), v[3].y + ((dt.rect.height / 2) + currentStep.dialogPadding.y), v[3].z);
                    break;
                default:
                    break;
            }

            dt.anchoredPosition = position;
        }

        private Vector3 CalculateDialogPosition(GameObject target)
        {
            var position = Vector3.zero;
            var cr = defaultCanvas.GetComponent<RectTransform>();

            if (_currentStepTargetType == TutorialStepTargetType.TargetUi)
            {
                var dr = dialogTemplate.GetComponent<RectTransform>();
                var tr = target.GetComponent<RectTransform>();

                Vector3[] twc = new Vector3[4];
                tr.GetWorldCorners(twc);

                Vector3[] cwc = new Vector3[4];
                cr.GetLocalCorners(cwc);

                var offsetPos = cr.InverseTransformPoint(twc[1]);
                var posX = offsetPos.x + (dr.rect.width / 2);
                var maxX = offsetPos.x + dr.rect.width;
                var maxY = offsetPos.y + dr.rect.height;
                if (maxX > cwc[2].x)
                {
                    posX = cwc[2].x - (dr.rect.width / 2);
                }
                var posY = offsetPos.y + (dr.rect.height / 2);
                if (maxY > cwc[1].y)
                {
                    offsetPos = cr.InverseTransformPoint(twc[3]);
                    posY = offsetPos.y - (dr.rect.height / 2);
                }

                position = new Vector3(posX, posY, 0f);
            }
            else
            {
                // calculate offset
                float offsetPosY = target.transform.position.y + 1.5f;
                // final position of marker above gameobject in world space
                Vector3 offsetPos = new Vector3(target.transform.position.x, offsetPosY, target.transform.position.z);
                // calculate *screen* position (note, not a canvas/recttransform position)
                Vector2 screenPoint = Camera.main.WorldToScreenPoint(offsetPos);
                // convert screen position to Canvas
                var screenPosition = Vector2.zero;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(cr, screenPoint, null, out screenPosition);
                position = new Vector3(screenPosition.x, screenPosition.y, 0);
            }

            return position;
        }

        private void AddConditionalListener()
        {
            if (currentStep.target != null)
            {
                switch (currentStep.eventType)
                {
                    case TutorialSystemEventTriggerType.OnButtonClick:
                        AddButtonClickListener();
                        break;
                    case TutorialSystemEventTriggerType.OnColliderTriggerEnter:
                        AddCollinderEnterListener();
                        break;
                    default:
                        AddEventTriggerListener();
                        break;
                }

            }
        }

        private void AddEventTriggerListener()
        {
            currentStep.target.gameObject.AddComponent<EventTriggerListener>();
            currentStep.target.gameObject.GetComponent<EventTriggerListener>().eventType = currentStep.eventType;
        }

        private void AddButtonClickListener()
        {
            currentStep.target.gameObject.AddComponent<OnButtonClickListener>();
        }

        private void AddCollinderEnterListener()
        {
            currentStep.target.gameObject.AddComponent<ColliderOnTriggerEnterListener>();
        }

        // in case on mobile keydown is enter we must add specific input listener
#if UNITY_ANDROID && !UNITY_EDITOR
        private void AddMobileKeybordListener()
        {
            if (currentStep.target != null)
            {
                if (currentStep.target.GetComponent<TutorialSystemMobileEventListener>() == null)
                    currentStep.target.AddComponent<TutorialSystemMobileEventListener>();
                currentStep.target.GetComponent<TutorialSystemMobileEventListener>().stepType = currentStep.actionType;
            }
        }
#endif

        public void EndTutorial()
        {
            SaveTutorialEnd();

            if (optionalParameters.destroyOnEnd)
            {
                DestroyTutorialSystem();
            }
            else
            {
                DeactivateTutorialSystem();
            }
        }

        private void DeactivateTutorialSystem()
        {
            Time.timeScale = 1;
            dialogTemplate.gameObject.SetActive(false);
            _canvasOverlay.gameObject.SetActive(false);
            DestroyGameObjectChildren(_canvasOverlay);
            this.enabled = false;
        }

        private TutorialStepTargetType GetCurrentStepTargetType()
        {
            if (currentStep.target != null && currentStep.target.gameObject.GetComponent<RectTransform>() != null)
            {
                 return TutorialStepTargetType.TargetUi;
            }
            else if (currentStep.target != null && currentStep.target.GetComponent<MeshRenderer>() != null)
            {
                return TutorialStepTargetType.Target3d;
            }
            else if (currentStep.target != null && currentStep.target.GetComponent<SpriteRenderer>() != null)
            {
                return TutorialStepTargetType.TargetSprite;
            }

            return TutorialStepTargetType.EmptyOrNotSupported;
        }

        private GameObject GetUiElementAtCursor()
        {

            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,
            };

            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            return results.FirstOrDefault(x => x.gameObject == currentStep.target).gameObject;

        }

        private GameObject GetSpriteObjectAtCursor()
        {
            GameObject result = null;

            RaycastHit2D[] rayHits = Physics2D.GetRayIntersectionAll(Camera.main.ScreenPointToRay(Input.mousePosition));

            if (rayHits.Count() > 0)
            {
                var isExist = rayHits.FirstOrDefault(x => x.collider.gameObject == currentStep.target);
                {
                    if (isExist)
                    {
                        result = isExist.collider.gameObject;
                    }
                }
            }
            return result;
        }

        private GameObject Get3dObjectAtCursor()
        {
            GameObject result = null;
            RaycastHit _raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out _raycastHit))
            {
                result = _raycastHit.collider.gameObject;
            }
            return result;
        }

        private bool IsTutorialFinished()
        {
            if (_currentTutorial != null)
            {
                if (_currentStepIndex >= _currentTutorial.Count)
                {
                    return true;
                }
            }

            return false;
        }


        private void SaveTutorialStep()
        {
            switch (optionalParameters.saveMethod)
            {
                case TutorialSystemSaveMethod.PlayerPrefs:
                    PlayerPrefs.SetInt($"Tutorial_{tutorialNumber}_Step", _currentStepIndex);
                    break;
                default:
                    break;
            }
        }

        private void SaveTutorialEnd()
        {
            switch (optionalParameters.saveMethod)
            {
                case TutorialSystemSaveMethod.PlayerPrefs:
                    PlayerPrefs.SetInt($"TutorialNumber_{tutorialNumber}", 1);
                    break;
                default:
                    break;
            }
        }

        private void DestroyTutorialSystem()
        {
            dialogTemplate.nextStepButton.onClick.RemoveAllListeners();
            dialogTemplate.skipTutorialButton.onClick.RemoveAllListeners();
            Destroy(dialogTemplate.gameObject);
            Destroy(_canvasOverlay);
            Destroy(this.gameObject);
        }

        private void LockInterface()
        {
            var image = _canvasOverlay.GetComponent<RawImage>();

            if (currentStep.target != null && (_currentStepTargetType == TutorialStepTargetType.TargetUi))
            {
                GenerateTargetOverlay(currentStep.target);
                image.raycastTarget = false;
            }
            else
            {
                image.raycastTarget = true;
                if (currentStep.showOverlay)
                {
                    image.color = Color.clear;
                }
            }
        }

        private void ResetCanvasOverlay()
        {
            var image = _canvasOverlay.GetComponent<RawImage>();
            image.color = Color.clear;
            image.raycastTarget = false;
            DestroyGameObjectChildren(_canvasOverlay);
        }

        private GameObject CreateUiOverlay(string name, Vector2 size)
        {
            var gameObject = new GameObject() { name = name };

            var transform = gameObject.AddComponent<RectTransform>();
            transform.anchoredPosition = defaultCanvas.transform.position;
            transform.sizeDelta = new Vector2( size.x * defaultCanvas.scaleFactor, size.y * defaultCanvas.scaleFactor);
            transform.anchorMin = Vector2.zero;
            transform.anchorMax = Vector2.one;
            transform.pivot = new Vector2(0.5f, 0.5f);

            gameObject.transform.SetParent(defaultCanvas.transform);
            var image = gameObject.AddComponent<RawImage>();
            image.color = Color.clear;
            image.raycastTarget = false;

            return gameObject;
        }

        private void GenerateTargetOverlay(GameObject target)
        {
            Color color = Color.clear;

            if (currentStep.showOverlay)
            {
                color = optionalParameters.overlayColor;
            }

            //var dc = defaultCanvas.GetComponent<RectTransform>();
            var co = _canvasOverlay.GetComponent<RectTransform>();
            var tr = target.GetComponent<RectTransform>();

            //right side
            Vector3[] twc = new Vector3[4];
            tr.GetWorldCorners(twc);

            Vector3[] owc = new Vector3[4];
            co.GetWorldCorners(owc);

            var offsetCanvas = co.InverseTransformPoint(owc[3]);
            var offsetTarget = co.InverseTransformPoint(twc[3]);

            var size = (offsetCanvas.x - offsetTarget.x);
            var anchor = (offsetCanvas.x - (size / 2));

            var childOverlay = new GameObject() { name = "RightOverlay" };

            var transform = childOverlay.AddComponent<RectTransform>();

            transform.sizeDelta = new Vector2(size, co.rect.height);
            transform.anchorMin = new Vector2(0.5f, 0f);
            transform.anchorMax = new Vector2(0.5f, 1f);

            transform.pivot = new Vector2(0.5f, 0.5f);

            childOverlay.transform.SetParent(_canvasOverlay.transform);
            transform.anchoredPosition = new Vector2(anchor, 0);
            transform.localScale = Vector3.one;

            var image = childOverlay.AddComponent<RawImage>();
            image.color = color;
            image.raycastTarget = true;

            // left side
            offsetCanvas = co.InverseTransformPoint(owc[1]);
            offsetTarget = co.InverseTransformPoint(twc[1]);

            size = (offsetTarget.x - offsetCanvas.x);
            anchor = (offsetCanvas.x + (size / 2));

            childOverlay = new GameObject() { name = "LefOverlay" };

            transform = childOverlay.AddComponent<RectTransform>();

            transform.sizeDelta = new Vector2(size, co.rect.height);
            transform.anchorMin = new Vector2(0.5f, 0f);
            transform.anchorMax = new Vector2(0.5f, 1f);

            transform.pivot = new Vector2(0.5f, 0.5f);

            childOverlay.transform.SetParent(_canvasOverlay.transform);
            transform.anchoredPosition = new Vector2(anchor, 0);
            transform.localScale = Vector3.one;

            image = childOverlay.AddComponent<RawImage>();
            image.color = color;
            image.raycastTarget = true;

            //top side
            offsetCanvas = co.InverseTransformPoint(owc[1]);
            offsetTarget = co.InverseTransformPoint(twc[1]);
            var offsetRight = co.InverseTransformPoint(twc[2]);
            var targetWidth = offsetRight.x - offsetTarget.x;

            size = (offsetCanvas.y - offsetTarget.y);
            anchor = (offsetCanvas.y - (size / 2));
            var anchorX = offsetTarget.x + (targetWidth / 2);

            childOverlay = new GameObject() { name = "TopOverlay" };

            transform = childOverlay.AddComponent<RectTransform>();

            transform.sizeDelta = new Vector2(targetWidth, size);
            transform.anchorMin = new Vector2(0.5f, 0.5f);
            transform.anchorMax = new Vector2(0.5f, 0.5f);
            transform.pivot = new Vector2(0.5f, 0.5f);

            childOverlay.transform.SetParent(_canvasOverlay.transform);
            transform.anchoredPosition = new Vector2(anchorX, anchor);
            transform.localScale = Vector3.one;

            image = childOverlay.AddComponent<RawImage>();
            image.color = color;
            image.raycastTarget = true;

            // down side
            offsetCanvas = co.InverseTransformPoint(owc[3]);
            offsetTarget = co.InverseTransformPoint(twc[3]);

            size = (offsetTarget.y - offsetCanvas.y);
            anchor = (offsetCanvas.y + (size / 2));
            anchorX = (offsetTarget.x - (targetWidth / 2));

            childOverlay = new GameObject() { name = "DownOverlay" };

            transform = childOverlay.AddComponent<RectTransform>();

            transform.sizeDelta = new Vector2(targetWidth, size);
            transform.anchorMin = new Vector2(0.5f, 0.5f);
            transform.anchorMax = new Vector2(0.5f, 0.5f);
            transform.pivot = new Vector2(0.5f, 0.5f);

            childOverlay.transform.SetParent(_canvasOverlay.transform);
            transform.anchoredPosition = new Vector2(anchorX, anchor);
            transform.localScale = Vector3.one;

            image = childOverlay.AddComponent<RawImage>();
            image.color = color;
            image.raycastTarget = true;
        }

        private void GenerateUiOutline(GameObject target)
        {
            //add custom targe overlay outline
            var targetOverlay = new GameObject() { name = "TargetOverlay" };

            var outlineDistance = currentStep.outlineDistance * 2f;

            var co = _canvasOverlay.GetComponent<RectTransform>();
            var tr = target.GetComponent<RectTransform>();


            Vector3[] twc = new Vector3[4];
            tr.GetWorldCorners(twc);

            var ld = co.InverseTransformPoint(twc[0]);
            var lt = co.InverseTransformPoint(twc[1]);
            var rt = co.InverseTransformPoint(twc[2]);

            var sizeX = rt.x - lt.x;
            var sizeY = lt.y - ld.y;
            var anchorX = (ld.x + (sizeX / 2));
            var anchorY = (ld.y + (sizeY / 2));

            var transform = targetOverlay.AddComponent<RectTransform>();

            transform.anchorMin = new Vector2(0.5f, 0.5f);
            transform.anchorMax = new Vector2(0.5f, 0.5f);
            transform.pivot = new Vector2(0.5f, 0.5f);

            targetOverlay.transform.SetParent(_canvasOverlay.transform);
            transform.sizeDelta = new Vector2(sizeX + outlineDistance, sizeY + outlineDistance);
            transform.anchoredPosition = new Vector2(anchorX, anchorY);
            transform.localScale = Vector3.one;

            var image = targetOverlay.AddComponent<RawImage>();
            image.texture = GenerateOutlineTexture((int)sizeX, (int)sizeY, currentStep.outlineColor, outlineDistance);
            image.color = currentStep.outlineColor;
            image.raycastTarget = false;
        }

        private Texture2D GenerateOutlineTexture(int width, int height, Color color, float outlineDistance)
        {
            var insideColor = Color.clear;

            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = insideColor;
            }
            // use private object to avoid memory leak
            _outlineTexture = new Texture2D(width, height);
            _outlineTexture.SetPixels(pix);
            _outlineTexture.Apply();

            for (int x = 0; x < _outlineTexture.width; x++)
            {
                for (int y = 0; y < _outlineTexture.height; y++)
                {
                    if (x < outlineDistance || x > _outlineTexture.width - 1 - outlineDistance) _outlineTexture.SetPixel(x, y, color);
                    else if (y < outlineDistance || y > _outlineTexture.height - 1 - outlineDistance) _outlineTexture.SetPixel(x, y, color);
                }
            }

            _outlineTexture.Apply();

            return _outlineTexture;
        }

        private void DestroyGameObjectChildren(GameObject target)
        {
            foreach (Transform child in target.transform)
            { 
                Destroy(child.gameObject);
            }
        }

    }

}