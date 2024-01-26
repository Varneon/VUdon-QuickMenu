using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Varneon.VUdon.Menus.Abstract;
using Varneon.VUdon.QuickMenu.Abstract;

namespace Varneon.VUdon.QuickMenu
{
    [AddComponentMenu("")]
    [ExcludeFromPreset]
    [DisallowMultipleComponent]
    public class QuickMenuToggle : QuickMenuItem
    {
        public override ItemType Type => ItemType.Toggle;

        [SerializeField]
        private Image leftElement, rightElement;

        [SerializeField]
        private TextMeshProUGUI leftLabel, rightLabel;

        public bool Value
        {
            get => _value;
            set
            {
                if(_value != value)
                {
                    _value = value;

                    OnValueChanged();
                }
            }
        }

        [SerializeField, HideInInspector]
        private bool _value;

        public void SetValueWithoutNotify(bool value)
        {
            _value = value;

            RefreshHighlights();
        }

        public override bool OnClick()
        {
            Value ^= true;

            return true;
        }

        public override bool OnClickRight()
        {
            if (Value) { return false; }

            Value = true;

            return true;
        }

        public override bool OnClickLeft()
        {
            if (!Value) { return false; }

            Value = false;

            return true;
        }

        private void OnValueChanged()
        {
            RefreshHighlights();

            if (_callbackReceiver) { _callbackReceiver.OnMenuToggleValueChanged(_path, Value); }
        }

        private void RefreshHighlights()
        {
            leftElement.color = Value ? defaultOptionColor : (Selected ? highlightedOptionColor : activeOptionColor);
            rightElement.color = Value ? (Selected ? highlightedOptionColor : activeOptionColor) : defaultOptionColor;
        }

        protected override void OnSelectedStateChanged(bool selected)
        {
            RefreshHighlights();
        }

        protected override void OnEnabledStateChanged(bool enabled)
        {
            RefreshHighlights();

            Color color = enabled ? defaultContentColor : disabledContentColor;

            leftLabel.color = color;

            rightLabel.color = color;
        }

        internal void Initialize(string itemPath, string label, MenuEventCallbackReceiver callbackReceiver, bool defaultValue, string tooltip = "")
        {
            RegisterAbstractProperties(itemPath, callbackReceiver, tooltip);

            _value = defaultValue;

            RefreshHighlights();

            SetLabel(label);
        }
    }
}
