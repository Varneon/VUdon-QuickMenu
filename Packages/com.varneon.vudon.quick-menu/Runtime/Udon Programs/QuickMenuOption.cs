using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Varneon.VUdon.Menus.Abstract;
using Varneon.VUdon.QuickMenu.Abstract;

namespace Varneon.VUdon.QuickMenu
{
    public class QuickMenuOption : QuickMenuItem
    {
        public override ItemType Type => ItemType.Option;

        [SerializeField]
        private Image leftArrow, rightArrow, optionPanel;

        [SerializeField]
        private TextMeshProUGUI optionLabel;

        [SerializeField]
        private string[] _options;

        [SerializeField]
        private int _optionCount;

        public int Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;

                    OnValueChanged();
                }
            }
        }

        [SerializeField, HideInInspector]
        private int _value;

        public override bool OnClickRight()
        {
            if (Value >= _optionCount - 1) { return false; }

            Value++;

            return true;
        }

        public override bool OnClickLeft()
        {
            if (Value <= 0) { return false; }

            Value--;

            return true;
        }

        private void OnValueChanged()
        {
            UpdateOptionLabel();

            RefreshGraphics();

            if (_callbackReceiver) { _callbackReceiver.OnMenuOptionValueChanged(_path, Value); }
        }

        private void UpdateOptionLabel()
        {
            optionLabel.text = _options[Value];
        }

        private void RefreshGraphics()
        {
            leftArrow.color = Value > 0 ? new Color(0.8f, 0.8f, 0.8f) : new Color(0.3f, 0.3f, 0.3f);
            rightArrow.color = (Value < _optionCount - 1) ? new Color(0.8f, 0.8f, 0.8f) : new Color(0.3f, 0.3f, 0.3f);
            optionPanel.color = Selected ? highlightedOptionColor : activeOptionColor;
        }

        protected override void OnSelectedStateChanged(bool selected)
        {
            RefreshGraphics();
        }

        internal void Initialize(string itemPath, string label, string[] options, MenuEventCallbackReceiver callbackReceiver, int defaultValue, string tooltip = "")
        {
            RegisterAbstractProperties(itemPath, callbackReceiver, tooltip);

            _options = options;

            _optionCount = _options.Length;

            _value = defaultValue;

            UpdateOptionLabel();

            RefreshGraphics();

            SetLabel(label);
        }
    }
}
