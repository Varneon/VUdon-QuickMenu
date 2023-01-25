using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using Varneon.VUdon.ArrayExtensions;
using Varneon.VUdon.Menus.Abstract;
using Varneon.VUdon.QuickMenu.Abstract;
using VRC.SDKBase;
using VRC.Udon.Common;

namespace Varneon.VUdon.QuickMenu
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class QuickMenu : MenuProvider
    {
        [Header("Settings")]
        [SerializeField]
        private int maxItemsOnPage = 10;

        [Header("References")]
        [SerializeField]
        private GameObject canvas;

        [SerializeField]
        private Transform vrTogglePoint;

        [SerializeField]
        private TextMeshProUGUI greetingText;

        [SerializeField]
        private TextMeshProUGUI dateTimeText;

        [SerializeField]
        private TextMeshProUGUI fpsText;

        [SerializeField]
        private TextMeshProUGUI menuHeaderText;

        [SerializeField]
        private TextMeshProUGUI elementIndexText;

        [SerializeField]
        private TextMeshProUGUI tooltipText;

        [SerializeField]
        private RectTransform itemContainer;

        [SerializeField]
        private ScrollRect scrollRect;

        [SerializeField]
        private Scrollbar scrollbar;

        [SerializeField]
        private AudioSource sfxSource;

        [SerializeField]
        private AudioClip
            audioOpen,
            audioClose,
            audioClick,
            audioToggle,
            audioSelect,
            audioAdjust,
            audioBack;

        [SerializeField]
        private QuickMenuFolderContainer folderContainer;

        [SerializeField]
        private QuickMenuFolderItem folderItem;

        [SerializeField]
        private QuickMenuButton buttonItem;

        [SerializeField]
        private QuickMenuToggle toggleItem;

        [SerializeField]
        private QuickMenuSlider sliderItem;

        [SerializeField]
        private QuickMenuOption optionItem;

        [SerializeField, HideInInspector]
        internal QuickMenuItem[][] items;

        [SerializeField, HideInInspector]
        internal string[] folderPaths;

        private QuickMenuFolderContainer defaultFolderContainer => folders.FirstOrDefault();

        [SerializeField, HideInInspector]
        internal QuickMenuFolderContainer[] folders;

        private QuickMenuFolderContainer currentFolderContainer;

        private string currentFolderPath;

        private bool isCurrentFolderEmpty;

        private int folderIndex = 0;

        private QuickMenuItem[] folderItems;

        private QuickMenuItem selectedItem;

        private ItemType selectedItemType;

        private bool open;

        private int SelectedItemIndex
        {
            get => selectedItemIndex;
            set
            {
                if (selectedItem) { selectedItem.OnDeselect(); }

                selectedItemIndex = value;

                if (selectedItemIndex >= folderItemCount) { selectedItemIndex = 0; }
                else if (selectedItemIndex < 0) { selectedItemIndex = folderItemCount - 1; }

                selectedItem = isCurrentFolderEmpty ? null : folderItems[selectedItemIndex];

                SendCustomEventDelayedFrames(nameof(RebuildCompleteLayout), 0);

                if (selectedItem)
                {
                    selectedItem.OnSelect();

                    selectedItemType = selectedItem.Type;

                    tooltipText.text = selectedItem.Tooltip;
                }
                else
                {
                    selectedItemType = ItemType.None;

                    tooltipText.text = string.Empty;
                }

                elementIndexText.text = string.Format("{0} <color=#888>/</color> {1}", selectedItemIndex + 1, folderItemCount);

                if (scrollbarActive)
                {
                    if(selectedItemIndex < visibleItemRangeStartIndex)
                    {
                        int delta = selectedItemIndex - visibleItemRangeStartIndex;

                        scrollbar.value -= scrollFraction * delta;

                        visibleItemRangeStartIndex += delta;
                    }
                    else if(selectedItemIndex >= visibleItemRangeStartIndex + maxItemsOnPage)
                    {
                        int delta = selectedItemIndex - visibleItemRangeStartIndex - maxItemsOnPage + 1;

                        scrollbar.value -= scrollFraction * delta;

                        visibleItemRangeStartIndex += delta;
                    }
                }
            }
        }

        private int selectedItemIndex;

        private float NavigationDirection
        {
            get => navigationDirection;
            set
            {
                if (navigationDirection != value)
                {
                    navigationDirection = value;

                    switch (navigationDirection)
                    {
                        case -1:
                            NavigateDown();
                            break;
                        case 1:
                            NavigateUp();
                            break;
                    }
                }
            }
        }

        private float navigationDirection;

        private bool scrollbarActive;

        private int visibleItemRangeStartIndex;

        private float scrollFraction;

        private int folderItemCount;

        private RectTransform scrollRectTransform;

        private RectTransform canvasRectTransform;

        private bool editingVRValue;

        private float horizontalBuildup;

        private float linearInputIncrement = 0.02f;

        private VRC_Pickup.PickupHand dominantPickupHand = VRC_Pickup.PickupHand.Right;

        private HandType dominantHandType = HandType.RIGHT;

        private VRCPlayerApi.TrackingDataType dominantTrackingDataType = VRCPlayerApi.TrackingDataType.RightHand;

        private bool dominantRightHand = true;

        private Vector3 localHandPos;

        private Vector3 lastLocalHandPos;

        private VRCPlayerApi localPlayer;

        private bool vrEnabled;

        private void Start()
        {
            vrEnabled = (localPlayer = Networking.LocalPlayer).IsUserInVR();

            canvasRectTransform = GetComponentInChildren<Canvas>(true).GetComponent<RectTransform>();

            if (vrEnabled)
            {
                canvasRectTransform.localPosition = new Vector3(0f, -0.1f, 1f);
            }

            scrollRectTransform = scrollRect.GetComponent<RectTransform>();

            greetingText.text = string.Format("Hello, <color=#FFF>{0}</color>", Networking.LocalPlayer.displayName);

            // Open home folder
            OpenFolder(string.Empty);

            UpdateOnSecond();
        }

        private void Update()
        {
            if (vrEnabled)
            {
                if (editingVRValue) { HandleVRValueEditingInput(); }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Q)) { ToggleQuickMenu(); }

                if (open) { HandleDesktopInput(); }
            }
        }

        private void HandleDesktopInput()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                NavigateUp();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                NavigateDown();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                switch (selectedItemType)
                {
                    case ItemType.Toggle:
                        TryClickRight();
                        break;
                    case ItemType.Slider:
                    case ItemType.Option:
                        TryAdjustRight();
                        break;
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                switch (selectedItemType)
                {
                    case ItemType.Toggle:
                        TryClickLeft();
                        break;
                    case ItemType.Slider:
                    case ItemType.Option:
                        TryAdjustLeft();
                        break;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                NavigateForward();
            }
            else if (Input.GetKeyDown(KeyCode.Backspace))
            {
                NavigateBack();
            }
        }

        private void NavigateUp()
        {
            SelectedItemIndex--;

            sfxSource.PlayOneShot(audioSelect);

            TriggerAdjustHaptics();
        }

        private void NavigateDown()
        {
            SelectedItemIndex++;

            sfxSource.PlayOneShot(audioSelect);

            TriggerAdjustHaptics();
        }

        private void NavigateForward()
        {
            switch (selectedItemType)
            {
                case ItemType.Folder:
                    OpenFolder(((QuickMenuFolderItem)selectedItem).FolderPath);

                    sfxSource.PlayOneShot(audioClick);

                    TriggerAdjustHaptics();
                    break;
                case ItemType.Button:
                case ItemType.Toggle:
                    if (selectedItem.OnClick())
                    {
                        sfxSource.PlayOneShot(audioToggle);

                        TriggerSwitchHaptics();
                    }
                    break;
                case ItemType.Slider:
                case ItemType.Option:
                    if (vrEnabled)
                    {
                        BeginVRValueEditing();
                    }
                    break;
            }
        }

        private void NavigateBack()
        {
            if (folderIndex > 0)
            {
                OpenFolder(currentFolderPath.Contains("/") ? currentFolderPath.Substring(0, currentFolderPath.LastIndexOf('/')) : string.Empty);

                sfxSource.PlayOneShot(audioBack);

                TriggerAdjustHaptics();
            }
            else
            {
                ToggleQuickMenu();
            }
        }

        private void TryClickRight()
        {
            if (selectedItem.OnClickRight())
            {
                sfxSource.PlayOneShot(audioToggle);
            }
        }

        private void TryClickLeft()
        {
            if (selectedItem.OnClickLeft())
            {
                sfxSource.PlayOneShot(audioToggle);
            }
        }

        private void TryAdjustRight()
        {
            if (selectedItem.OnClickRight())
            {
                sfxSource.PlayOneShot(audioAdjust);

                TriggerAdjustHaptics();
            }
        }

        private void TryAdjustLeft()
        {
            if (selectedItem.OnClickLeft())
            {
                sfxSource.PlayOneShot(audioAdjust);

                TriggerAdjustHaptics();
            }
        }

        private void TriggerAdjustHaptics()
        {
            if (vrEnabled)
            {
                localPlayer.PlayHapticEventInHand(dominantPickupHand, 0.05f, 0.25f, 200f);
            }
        }

        private void TriggerSwitchHaptics()
        {
            if (vrEnabled)
            {
                localPlayer.PlayHapticEventInHand(dominantPickupHand, 0.25f, 0.5f, 40f);
            }
        }

        private void BeginVRValueEditing()
        {
            lastLocalHandPos = transform.InverseTransformPoint(Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position);

            editingVRValue = true;

            linearInputIncrement = selectedItemType == ItemType.Slider ? 0.02f : 0.1f;
        }

        private void HandleVRValueEditingInput()
        {
            Vector3 localHandPos = transform.InverseTransformPoint(localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position);

            horizontalBuildup = (localHandPos - lastLocalHandPos).x;

            if (horizontalBuildup > linearInputIncrement) { TryAdjustRight(); lastLocalHandPos = localHandPos; }
            if (horizontalBuildup < -linearInputIncrement) { TryAdjustLeft(); lastLocalHandPos = localHandPos; }
        }

        private void OpenFolder(string path)
        {
            int index = folderPaths.IndexOf(path);

            if(index < 0) { Debug.LogWarning($"Folder ({path}) doesn't exist!"); return; }

            folderIndex = index;

            if (currentFolderContainer) { currentFolderContainer.gameObject.SetActive(false); }

            currentFolderContainer = folders[folderIndex];

            currentFolderContainer.gameObject.SetActive(true);

            folderItems = items[folderIndex];

            folderItemCount = folderItems.Length;

            isCurrentFolderEmpty = folderItemCount == 0;

            currentFolderPath = folderPaths[folderIndex];

            SelectedItemIndex = 0;

            menuHeaderText.text = string.IsNullOrEmpty(currentFolderPath) ? "Quick Menu" : currentFolderPath.Replace("/", " <color=#888>></color> ");

            scrollbarActive = folderItemCount > maxItemsOnPage;

            if (scrollbarActive)
            {
                int exceedingItemCount = folderItemCount - maxItemsOnPage;

                scrollbar.numberOfSteps = exceedingItemCount + 1;

                scrollFraction = 1f / exceedingItemCount;
            }

            visibleItemRangeStartIndex = 0;

            UpdateScrollRectSize();
        }

        private void UpdateScrollRectSize()
        {
            int minSize = Mathf.Min(maxItemsOnPage, folderIndex == -1 ? defaultFolderContainer.transform.childCount : folders[folderIndex].transform.childCount);

            scrollRectTransform.sizeDelta = new Vector2(scrollRectTransform.sizeDelta.x, 60f * minSize + 4f * (minSize - 1));

            RebuildItemLayout();
        }

        public void UpdateOnSecond()
        {
            dateTimeText.text = DateTime.UtcNow.ToLocalTime().ToString("yyyy.MM.dd HH:mm:ss");

            UpdateFPSText();

            SendCustomEventDelayedSeconds(nameof(UpdateOnSecond), 1f);
        }

        private void UpdateFPSText()
        {
            float fps = Mathf.Floor(1f / Time.deltaTime);

            fpsText.color = new Color(Mathf.Cos(Mathf.Clamp(fps, 0f, 90f) * Mathf.Deg2Rad), Mathf.Sin(Mathf.Clamp(fps, 0f, 90f) * Mathf.Deg2Rad), 0f);

            fpsText.text = $"<color=#AAAAAA>FPS:</color> {fps}";
        }

        public void RebuildItemLayout()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(itemContainer);
        }

        public void RebuildCompleteLayout()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(canvasRectTransform);
        }

        private string[] PreRegisterItemPath(string path, out string currentPath)
        {
            string[] pathElements = path.Split('/');

            int pathElementCount = pathElements.Length;

            currentPath = string.Empty;

            if (pathElementCount > 1)
            {
                for (int i = 0; i < pathElementCount; i++)
                {
                    currentPath = string.Join("/", pathElements, 0, i);

                    if (!folderPaths.Contains(currentPath))
                    {
                        AddFolder(currentPath);
                    }
                }
            }

            return pathElements;
        }

        public override bool TryRegisterButton(string path, MenuEventCallbackReceiver callbackReceiver, string tooltip = "")
        {
            string[] pathElements = PreRegisterItemPath(path, out string currentPath);

            QuickMenuButton newButton = Instantiate(buttonItem.gameObject, GetFolderContainer(currentPath, out int folderIndex).transform, false).GetComponent<QuickMenuButton>();

            items[folderIndex] = items[folderIndex].Add(newButton);

            newButton.Initialize(path, pathElements.LastOrDefault(), callbackReceiver, tooltip);

            return true;
        }

        public override bool TryRegisterToggle(string path, MenuEventCallbackReceiver callbackReceiver, bool defaultValue, string offOptionName = "Off", string onOptionName = "On", string tooltip = "")
        {
            string[] pathElements = PreRegisterItemPath(path, out string currentPath);

            QuickMenuToggle newOption = Instantiate(toggleItem.gameObject, GetFolderContainer(currentPath, out int folderIndex).transform, false).GetComponent<QuickMenuToggle>();

            items[folderIndex] = items[folderIndex].Add(newOption);

            newOption.Initialize(path, pathElements.LastOrDefault(), callbackReceiver, defaultValue, tooltip);

            return true;
        }

        public override bool TryRegisterOption(string path, MenuEventCallbackReceiver callbackReceiver, string[] optionNames, int defaultValue, string tooltip = "")
        {
            string[] pathElements = PreRegisterItemPath(path, out string currentPath);

            QuickMenuOption newToggle = Instantiate(optionItem.gameObject, GetFolderContainer(currentPath, out int folderIndex).transform, false).GetComponent<QuickMenuOption>();

            items[folderIndex] = items[folderIndex].Add(newToggle);

            newToggle.Initialize(path, pathElements.LastOrDefault(), optionNames, callbackReceiver, defaultValue, tooltip);

            return true;
        }

        public override bool TryRegisterSlider(string path, MenuEventCallbackReceiver callbackReceiver, float defaultValue, float minValue = 0, float maxValue = 1, int steps = 10, string unit = "%", string tooltip = "")
        {
            string[] pathElements = PreRegisterItemPath(path, out string currentPath);

            QuickMenuSlider newSlider = Instantiate(sliderItem.gameObject, GetFolderContainer(currentPath, out int folderIndex).transform, false).GetComponent<QuickMenuSlider>();

            items[folderIndex] = items[folderIndex].Add(newSlider);

            newSlider.Initialize(path, pathElements.LastOrDefault(), callbackReceiver, minValue, maxValue, steps, unit, defaultValue, tooltip);

            return true;
        }

        public override bool TrySetToggleValue(string path, bool value)
        {
            if (!TryGetMenuItem(path, out QuickMenuItem menuItem)) { return false; }

            ((QuickMenuToggle)menuItem).Value = value;

            return true;
        }

        public override bool TrySetOptionValue(string path, int value)
        {
            if (!TryGetMenuItem(path, out QuickMenuItem menuItem)) { return false; }

            ((QuickMenuOption)menuItem).Value = value;

            return true;
        }

        public override bool TrySetSliderValue(string path, float value)
        {
            if (!TryGetMenuItem(path, out QuickMenuItem menuItem)) { return false; }

            ((QuickMenuSlider)menuItem).Value = value;

            return true;
        }

        public override bool TrySetToggleValueWithoutNotify(string path, bool value)
        {
            if (!TryGetMenuItem(path, out QuickMenuItem menuItem)) { return false; }

            ((QuickMenuToggle)menuItem).SetValueWithoutNotify(value);

            return true;
        }

        public override bool TrySetOptionValueWithoutNotify(string path, int value)
        {
            if (!TryGetMenuItem(path, out QuickMenuItem menuItem)) { return false; }

            ((QuickMenuOption)menuItem).SetValueWithoutNotify(value);

            return true;
        }

        public override bool TrySetSliderValueWithoutNotify(string path, float value)
        {
            if (!TryGetMenuItem(path, out QuickMenuItem menuItem)) { return false; }

            ((QuickMenuSlider)menuItem).SetValueWithoutNotify(value);

            return true;
        }

        private QuickMenuFolderContainer GetFolderContainer(string path, out int folderIndex)
        {
            if (string.IsNullOrEmpty(path)) { folderIndex = 0; return defaultFolderContainer; }

            folderIndex = folderPaths.IndexOf(path);

            if(folderIndex == 0) { return null; }

            return folders[folderIndex];
        }

        public override bool TryRegisterPage(string path, string tooltip = "")
        {
            return TryRegisterFolder(path, tooltip);
        }

        public bool TryRegisterFolder(string path, string tooltip = "")
        {
            AddFolder(path, tooltip);

            return true;
        }

        private bool TryGetMenuItem(string path, out QuickMenuItem menuItem)
        {
            string folderPath = path.Contains("/") ? path.Substring(0, path.LastIndexOf('/')) : string.Empty;

            int folderIndex = folderPaths.IndexOf(folderPath);

            if (folderIndex < 0) { menuItem = null; return false; }

            QuickMenuItem[] folderItems = items[folderIndex];

            for (int i = 0; i < folderItems.Length; i++)
            {
                QuickMenuItem item = folderItems[i];

                if (item.Path == path)
                {
                    menuItem = item;

                    return true;
                }
            }

            menuItem = null;

            return false;
        }

        private void AddFolder(string path, string tooltip = "")
        {
            items = items.Add(new QuickMenuItem[0]);

            folderPaths = folderPaths.Add(path);

            string parentPath = path.Contains("/") ? path.Substring(0, path.LastIndexOf('/')) : string.Empty;

            QuickMenuFolderContainer parentContainer = GetFolderContainer(parentPath, out int folderIndex);

            QuickMenuFolderItem newFolderItem = Instantiate(folderItem.gameObject, parentContainer.transform, false).GetComponent<QuickMenuFolderItem>();

            newFolderItem.SetFolderPath(path);

            newFolderItem.SetTooltip(string.IsNullOrWhiteSpace(tooltip) ? path.Contains("/") ? path.Substring(path.LastIndexOf('/') + 1) : path : tooltip);

            items[folderIndex] = items[folderIndex].Add(newFolderItem);

            QuickMenuFolderContainer newFolderContainer = Instantiate(folderContainer.gameObject, itemContainer, false).GetComponent<QuickMenuFolderContainer>();

            folders = folders.Add(newFolderContainer);

            newFolderContainer.SetPath(path);
        }

        public override void InputUse(bool value, UdonInputEventArgs args)
        {
            if (vrEnabled)
            {
                if (value)
                {
                    if(args.handType == dominantHandType)
                    {
                        if (!TryToggleVRQuickMenu() && open)
                        {
                            NavigateForward();
                        }
                    }
                    else
                    {
                        if (open) { NavigateBack(); }
                    }

                    //switch (args.handType)
                    //{
                    //    case HandType.LEFT:
                    //        if (open) { NavigateBack(); }
                    //        break;
                    //    case HandType.RIGHT:
                    //        if (!TryToggleVRQuickMenu() && open)
                    //        {
                    //            NavigateForward();
                    //        }
                    //        break;
                    //}
                }
                else
                {
                    if(args.handType == dominantHandType)
                    {
                        if (editingVRValue) { editingVRValue = false; }
                    }

                    //switch (args.handType)
                    //{
                    //    case HandType.LEFT:
                            
                    //        break;
                    //    case HandType.RIGHT:
                    //        if (editingVRValue) { editingVRValue = false; }
                    //        break;
                    //}
                }
            }
        }

        public override void InputLookVertical(float value, UdonInputEventArgs args)
        {
            if (vrEnabled && open)
            {
                NavigationDirection = Mathf.Round(value);
            }
        }

        public override bool IsPathRegistered(string path)
        {
            return false;
        }

        private bool TryToggleVRQuickMenu()
        {
            Vector3 handPos = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position;

            localHandPos = transform.InverseTransformPoint(handPos);

            if (Vector3.Magnitude(vrTogglePoint.position - handPos) < 0.15f)
            {
                ToggleQuickMenu();

                return true;
            }
            else
            {
                return false;
            }
        }

        private void ToggleQuickMenu()
        {
            open ^= true;

            canvas.SetActive(open);

            sfxSource.PlayOneShot(open ? audioOpen : audioClose);

            TriggerHaptics();
        }

        private void TriggerHaptics()
        {
            if (vrEnabled)
            {
                localPlayer.PlayHapticEventInHand(dominantPickupHand, 0.25f, 0.5f, 100f);
            }
        }
    }
}
