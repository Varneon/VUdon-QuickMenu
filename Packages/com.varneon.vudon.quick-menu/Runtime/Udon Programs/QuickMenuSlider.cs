﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Varneon.VUdon.Menus.Abstract;
using Varneon.VUdon.QuickMenu.Abstract;

namespace Varneon.VUdon.QuickMenu
{
    [AddComponentMenu("")]
    [ExcludeFromPreset]
    [DisallowMultipleComponent]
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
                    currentStep = GetCurrentStep(value);

                    _value = _fraction * currentStep;

                    OnValueChanged();
                }
            }
        }

        [SerializeField, HideInInspector]
        private float _value;

        [SerializeField, HideInInspector]
        private int currentStep;

        public void SetValueWithoutNotify(float value)
        {
            currentStep = GetCurrentStep(value);

            _value = _fraction * currentStep;

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

            _value = _fraction * ++currentStep;

            OnValueChanged();

            return true;
        }

        public override bool OnClickLeft()
        {
            if (Value <= _minValue) { return false; }

            _value = _fraction * --currentStep;

            OnValueChanged();

            return true;
        }

        private void OnValueChanged()
        {
            UpdateValueLabel();

            RefreshGraphics();

            if (_callbackReceiver) { _callbackReceiver.OnMenuSliderValueChanged(_path, Value); }
        }

        public void OnBeginValueEdit()
        {
            if (_callbackReceiver) { _callbackReceiver.OnMenuSliderBeginValueEdit(_path); }
        }

        public void OnEndValueEdit()
        {
            if (_callbackReceiver) { _callbackReceiver.OnMenuSliderEndValueEdit(_path); }
        }

        private void UpdateValueLabel()
        {
            valueLabel.text = $"{Value} {_unit}";
        }

        private void RefreshGraphics()
        {
            slider.value = Value;

            sliderFill.color = ItemEnabled ? (Selected ? highlightedOptionColor : activeOptionColor) : (Selected ? disabledHighlightedOptionColor : disabledOptionColor);
        }

        private int GetCurrentStep(float value)
        {
            return Mathf.FloorToInt(Mathf.Clamp(value, _minValue, _maxValue) / _fraction);
        }

        protected override void OnSelectedStateChanged(bool selected)
        {
            RefreshGraphics();
        }

        protected override void OnEnabledStateChanged(bool enabled)
        {
            RefreshGraphics();

            valueLabel.color = enabled ? defaultContentColor : disabledContentColor;
        }

        internal void Initialize(string itemPath, string label, MenuEventCallbackReceiver callbackReceiver, float minValue = 0f, float maxValue = 100f, int steps = 10, string unit = "%", float defaultValue = 0f, string tooltip = "")
        {
            RegisterAbstractProperties(itemPath, callbackReceiver, tooltip);

            _value = defaultValue;

            _minValue = minValue;

            _maxValue = maxValue;

            _fraction = (maxValue - minValue) / (steps - 1);

            _unit = unit;

            slider.minValue = minValue;

            slider.maxValue = maxValue;

            SetValueWithoutNotify(_value);

            SetLabel(label);
        }
    }
}
