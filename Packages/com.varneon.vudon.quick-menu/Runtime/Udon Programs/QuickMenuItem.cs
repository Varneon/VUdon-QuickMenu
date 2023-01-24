using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using Varneon.VUdon.Menus.Abstract;

namespace Varneon.VUdon.QuickMenu.Abstract
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public abstract class QuickMenuItem : UdonSharpBehaviour
    {
        public abstract ItemType Type { get; }

        public bool Selected => selected;

        public string Path => _path;

        public string Tooltip => _tooltip;

        [SerializeField]
        private Image panelImage;

        [SerializeField]
        private TextMeshProUGUI label;

        [SerializeField, HideInInspector]
        protected MenuEventCallbackReceiver _callbackReceiver;

        [SerializeField, HideInInspector]
        protected string _path;

        [SerializeField, HideInInspector]
        protected string _tooltip;

        private readonly Color
            defaultPanelColor = new Color(0f, 0f, 0f, 0.8f),
            highlightedPanelColor = new Color(0f, 0.4f, 0.4f, 1f);

        protected readonly Color
            defaultOptionColor = new Color(0f, 0f, 0f, 0.5f),
            activeOptionColor = new Color(0.4f, 0.4f, 0.4f, 0.5f),
            highlightedOptionColor = new Color(0.3f, 0.6f, 0.6f, 0.5f);

        private bool selected;

        public virtual bool OnClick() { return false; }

        public virtual bool OnClickRight() { return false; }

        public virtual bool OnClickLeft() { return false; }

        public virtual bool Adjust(float delta) { return false; }

        internal void OnSelect()
        {
            selected = true;

            OnSelectedStateChanged(true);

            SetPanelHighlightedState(true);
        }

        internal void OnDeselect()
        {
            selected = false;

            OnSelectedStateChanged(false);

            SetPanelHighlightedState(false);
        }

        private void SetPanelHighlightedState(bool highlighted)
        {
            panelImage.color = highlighted ? highlightedPanelColor : defaultPanelColor;
        }

        protected virtual void OnSelectedStateChanged(bool selected) { }

        protected void RegisterAbstractProperties(string path, MenuEventCallbackReceiver callbackReceiver, string tooltip)
        {
            _path = path;

            _callbackReceiver = callbackReceiver;

            _tooltip = tooltip;
        }

        internal void SetLabel(string text)
        {
            label.text = text;
        }
    }
}
