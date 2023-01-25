using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Varneon.VUdon.Menus.Abstract;
using Varneon.VUdon.QuickMenu.Abstract;

namespace Varneon.VUdon.QuickMenu
{
    public class QuickMenuSlider : QuickMenuItem
    {
        public override ItemType Type => ItemType.Slider;

        [SerializeField]
        private Slider slider;

        [SerializeField]
        private Image sliderFill;

        [SerializeField]
        private TextMeshProUGUI valueLabel;

        public float Value
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
        private float _value;

        public void SetValueWithoutNotify(float value)
        {
            _value = value;

            UpdateValueLabel();

            RefreshGraphics();
        }

        [SerializeField, HideInInspector]
        private float _minValue, _maxValue;

        [SerializeField, HideInInspector]
        private float _fraction;

        [SerializeField, HideInInspector]
        private string _unit;

        public override bool OnClickRight()
        {
            if (Value >= _maxValue) { return false; }

            Value += _fraction;

            return true;
        }

        public override bool OnClickLeft()
        {
            if (Value <= _minValue) { return false; }

            Value -= _fraction;

            return true;
        }

        private void OnValueChanged()
        {
            UpdateValueLabel();

            RefreshGraphics();

            if (_callbackReceiver) { _callbackReceiver.OnMenuSliderValueChanged(_path, Value); }
        }

        private void UpdateValueLabel()
        {
            valueLabel.text = $"{Value} {_unit}";
        }

        private void RefreshGraphics()
        {
            slider.value = Value;

            sliderFill.color = Selected ? highlightedOptionColor : activeOptionColor;
        }

        protected override void OnSelectedStateChanged(bool selected)
        {
            RefreshGraphics();
        }

        internal void Initialize(string itemPath, string label, MenuEventCallbackReceiver callbackReceiver, float minValue = 0f, float maxValue = 100f, int steps = 10, string unit = "%", float defaultValue = 0f, string tooltip = "")
        {
            RegisterAbstractProperties(itemPath, callbackReceiver, tooltip);

            _value = defaultValue;

            _minValue = minValue;

            _maxValue = maxValue;

            _fraction = (maxValue - minValue) / steps;

            _unit = unit;

            slider.minValue = minValue;

            slider.maxValue = maxValue;

            UpdateValueLabel();

            RefreshGraphics();

            SetLabel(label);
        }
    }
}
