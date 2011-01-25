﻿using System;
using System.Collections.Generic;
using System.Text;
using ARDrone.Input.InputControls;
using ARDrone.Input.Utility;

namespace ARDrone.Input.InputMappings
{
    public abstract class ValidatedInputMapping : InputMapping
    {
        protected List<String> validBooleanInputValues = null;
        protected List<String> validContinuousInputValues = null;

        protected ValidatedInputMapping(List<String> validBooleanInputValues, List<String> validContinuousInputValues, InputControl controls)
            : base()
        {
            InitializeValidation(validBooleanInputValues, validContinuousInputValues);
            InitializeControls(controls);
        }

        private void InitializeValidation(List<String> validBooleanInputValues, List<String> validContinuousInputValues)
        {
            this.validBooleanInputValues = new List<String>();
            this.validContinuousInputValues = new List<String>();

            for (int i = 0; i < validBooleanInputValues.Count; i++)
            {
                if (validBooleanInputValues[i].Contains("-")) { throw new Exception("'-' is not allowed within button names (button name '" + validBooleanInputValues[i] + "')"); }
                if (validBooleanInputValues[i] == null) { throw new Exception("Null is not allowed as a button name"); }
                this.validBooleanInputValues.Add(validBooleanInputValues[i]);
            }
            for (int i = 0; i < validContinuousInputValues.Count; i++)
            {
                if (validContinuousInputValues[i].Contains("-")) { throw new Exception("'-' is not allowed within axis names (axis name '" + validBooleanInputValues[i] + "')"); }
                if (validContinuousInputValues[i] == null) { throw new Exception("Null is not allowed as an axis name"); }
                this.validContinuousInputValues.Add(validContinuousInputValues[i]);
            }

            if (!this.validBooleanInputValues.Contains("")) { this.validBooleanInputValues.Add(""); }
            if (!this.validContinuousInputValues.Contains("")) { this.validContinuousInputValues.Add(""); }

            if (this.validBooleanInputValues == null) { this.validBooleanInputValues = new List<String>(); }
            if (this.validContinuousInputValues == null) { this.validContinuousInputValues = new List<String>(); }
        }

        private void InitializeControls(InputControl controls)
        {
            CheckControls(controls);
            this.controls = InputFactory.CloneInputControls(controls);
        }

        public void CopyValidInputValuesFrom(ButtonBasedInputMapping mappingToCopyFrom)
        {
            validBooleanInputValues = mappingToCopyFrom.ValidBooleanInputValues;
            validContinuousInputValues = mappingToCopyFrom.ValidContinuousInputValues;
        }

        protected override void CheckControls(InputControl controls)
        {
            base.CheckControls(controls);

            Dictionary<String, String> mappings = controls.Mappings;

            foreach (KeyValuePair<String, String> keyValuePair in mappings)
            {
                String name = keyValuePair.Key;
                String value = keyValuePair.Value;

                if (controls.IsContinuousMapping(name) && !isValidContinuousInputValue(value))
                    throw new Exception("The input element '" + name + "' is no valid axis.");
                else if (controls.IsBooleanMapping(name) && !isValidBooleanInputValue(value))
                    throw new Exception("The input element '" + name + "' is no valid button.");
                else if (!controls.IsContinuousMapping(name) && !controls.IsBooleanMapping(name))
                    throw new Exception("The input element '" + name + "' is neither marked as button nor as axis");
            }
        }

        public bool isValidBooleanInputValue(String buttonValue)
        {
            return validBooleanInputValues.Contains(buttonValue);
        }

        public bool isValidContinuousInputValue(String axisValue)
        {
            if (validContinuousInputValues.Contains(axisValue))     // Continuous input values
            {
                return true;
            }
            else                                                    // Two boolean input values, separated by a "-"
            {
                String[] axisValues = axisValue.Split('-');
                return (axisValues.Length == 2 && validBooleanInputValues.Contains(axisValues[0]) && validBooleanInputValues.Contains(axisValues[1]));
            }
        }

        public List<String> ValidBooleanInputValues
        {
            get { return validBooleanInputValues; }
        }

        public List<String> ValidContinuousInputValues
        {
            get { return validContinuousInputValues; }
        }
    }
}