using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Varneon.VUdon.Menus.Abstract;
using Varneon.VUdon.QuickMenu.Abstract;

namespace Varneon.VUdon.QuickMenu
{
    public class QuickMenuToggle : QuickMenuItem
    {
        public override ItemType Type => ItemType.Toggle;

        [SerializeField]
        private Image leftElement, rightElement;

        [SerializeField]
        private TextMeshProUGUI leftLabel, rightLabel;

        public bool State
        {
            get => state;
            set
            {
                if(state != value)
                {
                    state = value;

                    OnValueChanged();
                }
            }
        }

        [SerializeField, HideInInspector]
        private bool state;

        public override bool OnClick()
        {
            State ^= true;

            return true;
        }

        public override bool OnClickRight()
        {
            if (State) { return false; }

            State = true;

            return true;
        }

        public override bool OnClickLeft()
        {
            if (!State) { return false; }

            State = false;

            return true;
        }

        private void OnValueChanged()
        {
            RefreshHighlights();

            if (_callbackReceiver) { _callbackReceiver.OnMenuToggleValueChanged(_path, State); }
        }

        private void RefreshHighlights()
        {
            leftElement.color = State ? defaultOptionColor : (Selected ? highlightedOptionColor : activeOptionColor);
            rightElement.color = State ? (Selected ? highlightedOptionColor : activeOptionColor) : defaultOptionColor;
        }

        protected override void OnSelectedStateChanged(bool selected)
        {
            RefreshHighlights();
        }

        internal void Initialize(string itemPath, string label, MenuEventCallbackReceiver callbackReceiver, bool defaultValue, string tooltip = "")
        {
            RegisterAbstractProperties(itemPath, callbackReceiver, tooltip);

            state = defaultValue;

            RefreshHighlights();

            SetLabel(label);
        }
    }
}
