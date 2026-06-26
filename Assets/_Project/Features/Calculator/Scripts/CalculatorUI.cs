using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;
using KBCore.Refs;

using MathEvaluation;
using MathEvaluation.Context;
using ProjectATLAS.UI;
using System.Linq;

namespace ProjectATLAS.Calculator
{
    public class CalculatorUI : MonoBehaviour
    {
        [Header("Values")]
        [SerializeField] private string inputText;
        [SerializeField] private string previewText;
        
        [Header("Components")]
        [SerializeField] private TMP_InputField inputComponent;
        [SerializeField] private TMP_Text previewComponent;
        [SerializeField, Child] private CalculatorKey[] calculatorKeys;
        [SerializeField] private UIToggleButton shiftToggle;
        [SerializeField] private UIToggleButton altToggle;
        
        [Header("Debug")]
        [SerializeField] private bool logKeyPress;
        
        private const StringComparison IGNORE_CASE = StringComparison.CurrentCultureIgnoreCase;
        private static readonly string[] constants = new[] { "π", "τ", "e" };
        
        private List<CalculatorKey> shiftableKeys;
        private List<CalculatorKey> altableKeys;
        private List<string> keywords;
        
        private int lastCaretPos = 0;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            shiftableKeys = new();
            altableKeys = new();
            keywords = new() { "Error", "NaN", "Infinity" };
            
            foreach (CalculatorKey key in calculatorKeys)
            {
                key.OnPressed += CalculatorKey_OnPressed;
                
                if (key.IsShiftable) shiftableKeys.Add(key);
                if (key.IsAltable) altableKeys.Add(key);
                
                keywords.AddRange(key.GetKeywords());
            }
            
            shiftToggle.OnValueChanged += ShiftToggle_OnValueChanged;
            altToggle.OnValueChanged += AltToggle_OnValueChanged;
            
            inputComponent.onValueChanged.AddListener(InputComponent_onValueChanged);
        }
        
        private void OnValidate()
        {
            this.ValidateRefs();
            
            if (inputComponent) inputComponent.text = inputText;
            if (previewComponent) previewComponent.text = previewText;
        }
        
        
        // PUBLIC METHODS
        public void Type(string value)
        {
            inputText = InsertAtCaret(inputText, value);
            previewText = CalculateAndGetString(inputText);
        }
        
        public void Shift(bool value)
        {
            foreach (var key in shiftableKeys)
            {
                key.SetShift(value);
            }
        }
        
        public void Alt(bool value)
        {
            foreach (var key in altableKeys)
            {
                key.SetAlt(value);
            }
        }
        
        public void Flip()
        {
            if (string.IsNullOrEmpty(inputText))
            {
                return;
            }
            
            // 1) If the input already ends with -( ... ), unwrap it completely.
            //    This matches "-(something)" at the very end and captures "something".
            var negativeWrap = new Regex(@"-\(([^)]*)\)$");
            var negMatch = negativeWrap.Match(inputText);
            if (negMatch.Success)
            {
                // Replace the entire "-(inner)" with "inner"
                inputText = inputText.Substring(0, negMatch.Index) + negMatch.Groups[1].Value;
                return;
            }
            
            // 2) Otherwise find the last number or parenthesized expression and wrap it.
            var target = new Regex(@"(\([^\)]+\)|\d+(\.\d+)?)$");
            var match = target.Match(inputText);
            if (!match.Success)
            {
                return;
            }
            
            int startIndex = match.Index;
            string found = match.Value;
            
            // Replace the found target with -(target)
            inputText = inputText.Substring(0, startIndex) + "-(" + found + ")" + inputText.Substring(startIndex + found.Length);
            previewText = CalculateAndGetString(inputText);
            
            // Update caret position
            lastCaretPos += 3;
        }
        
        public void Clear()
        {
            inputText = string.Empty;
            previewText = string.Empty;
            
            SetCaretPosition(0);
        }
        
        public void Backspace()
        {
            if (string.IsNullOrEmpty(inputText))
                return;
            
            int caretPos = inputComponent.caretPosition;
            
            // Sort keywords by length descending to match longest first
            var sortedKeywords = keywords.OrderByDescending(k => k.Length);
            
            foreach (string word in sortedKeywords)
            {
                if (caretPos >= word.Length)
                {
                    int trimIndex = caretPos - word.Length;
                    string possibleWord = inputText.Substring(trimIndex, word.Length);
                    
                    if (string.Equals(possibleWord, word, IGNORE_CASE))
                    {
                        inputText = inputText.Remove(trimIndex, word.Length);
                        SetCaretPosition(trimIndex);
                        previewText = CalculateAndGetString(inputText);
                        
                        RefreshUI();
                        return;
                    }
                }
            }
            
            // Fallback: Delete single character
            inputText = DeleteAtCaret(inputText);
            previewText = CalculateAndGetString(inputText);
        }
        
        public void Equals()
        {
            inputText = CalculateAndGetString(inputText, false);
            previewText = string.Empty;
            
            SetCaretPosition(inputText?.Length ?? 0);
        }
        
        
        // PRIVATE METHODS
        private static string CalculateAndGetString(string expr, bool discardNumberOnly = true)
        {
            if (string.IsNullOrWhiteSpace(expr) || (discardNumberOnly && Regex.IsMatch(expr, @"^[0-9.]+$")))
                return string.Empty;
            
            try
            {
                return CommaFormat(EvaluateExpression(expr).ToString());
            }
            catch
            {
                return "Error";
            }
        }
        
        private static double EvaluateExpression(string expr)
        {
            expr = SanitizeExpression(expr);
            
            Debug.Log($"Sanitized Expression: {expr}");
            
            MathExpression expression = new(expr, new ScientificMathContext());
            return expression.Evaluate();
        }
        
        private static string SanitizeExpression(string expr)
        {
            if (string.IsNullOrWhiteSpace(expr))
                return string.Empty;
            
            // Remove spaces and commas
            expr = expr.Trim().Replace(" ", "").Replace(",", "");
            
            // ===== Auto-prepend "1" if starts with E notation =====
            // This covers: "E10", "e-3", etc.
            if (Regex.IsMatch(expr, @"^[eE][+-]?\d"))
                expr = "1" + expr;
            
            // Regex pattern that matches number, parentheses, constant, or function
            string constantPattern = string.Join("|", constants.Select(Regex.Escape).ToArray());
            string operandPattern = @"(?:\([^\)]+\)|\d+(?:\.\d+)?|" + constantPattern + @"|[a-zA-Z_]\w*\([^\)]*\))";
            
            // ===== Handle scientific notation E =====
            if (expr.IndexOf('E') >= 0 || expr.IndexOf('e') >= 0)
            {
                Regex sciRegex = new("(" + operandPattern + @")[eE]([+-]?\d+(?:\.\d+)?)");
                while (sciRegex.IsMatch(expr))
                {
                    expr = sciRegex.Replace(expr, "$1*10^($2)", 1);
                }
            }
            
            // ===== Handle % =====
            if (expr.Contains("%"))
            {
                Regex percentRegex = new("(" + operandPattern + @")(%+)");
                while (percentRegex.IsMatch(expr))
                {
                    expr = percentRegex.Replace(expr, match =>
                    {
                        string numberPart = match.Groups[1].Value;
                        string percentSymbols = match.Groups[2].Value;
                        int percentCount = percentSymbols.Length;
                        
                        string result = numberPart;
                        for (int i = 0; i < percentCount; i++)
                            result = "(" + result + "/100)";
                        
                        return result;
                    }, 1);
                }
            }
            
            // ===== Handle nth root (y√x) -> (x^(1/y)) =====
            if (expr.Contains("y√x"))
            {
                Regex rootRegex = new("(" + operandPattern + @")[ \t]*y√x[ \t]*(" + operandPattern + ")");
                while (rootRegex.IsMatch(expr))
                {
                    expr = rootRegex.Replace(expr, "($2^(1/$1))", 1);
                }
            }
            
            // Remove trailing operators
            while (expr.Length > 0 && "+-×÷*/%^".Contains(expr[expr.Length - 1]))
            {
                expr = expr.Substring(0, expr.Length - 1);
            }
            
            // Close unmatched parentheses
            // Balance parentheses
            int balance = 0;
            foreach (char c in expr)
            {
                if (c == '(') balance++;
                else if (c == ')') balance--;
            }
            if (balance > 0) expr += new string(')', balance);
            else if (balance < 0) expr = new string('(', -balance) + expr;
            
            // Replace empty functions
            expr = Regex.Replace(expr, @"([a-zA-Z_]\w*)\($", "$1(0)");
            
            // Dangling minus signs
            if (expr.EndsWith("-") && expr.Length > 1 && "+*/(^".Contains(expr[expr.Length - 2]))
                expr += "0";
            
            return expr;
        }
        
        private static string CommaFormat(string number)
        {
            string[] parts = number.Split('.');
            
            string integers = long.Parse(parts[0]).ToString("N0");
            string decimals = parts.Length > 1 ? "." + parts[1] : "";
            
            return integers + decimals;
        }
        
        
        private void RefreshUI(bool reapplyCaret = true)
        {
            if (inputComponent)
            {
                if (inputComponent.text != inputText)
                {
                    inputComponent.SetTextWithoutNotify(inputText);
                    inputComponent.ForceLabelUpdate();
                    
                    if (reapplyCaret)
                    {
                        int clamped = Mathf.Clamp(lastCaretPos, 0, inputText?.Length ?? 0);
                        SetCaretPosition(clamped);
                    }
                }
                
                if (!inputComponent.isFocused)
                {
                    inputComponent.ActivateInputField();
                }
            }
            
            if (previewComponent && previewComponent.text != previewText)
            {
                previewComponent.text = previewText;
            }
        }
        
        private string InsertAtCaret(string text, string value)
        {
            int caretPos = inputComponent.caretPosition;
            if (logKeyPress) Debug.Log("InsertAtCaret - caret: " + caretPos + " value: '" + value + "'");
            
            // If selection exists, replace it
            if (inputComponent.selectionStringAnchorPosition != inputComponent.selectionStringFocusPosition)
            {
                int startPos = Mathf.Min(inputComponent.selectionStringAnchorPosition, inputComponent.selectionStringFocusPosition);
                int endPos = Mathf.Max(inputComponent.selectionStringAnchorPosition, inputComponent.selectionStringFocusPosition);
                
                text = text.Substring(0, startPos) + value + text.Substring(endPos);
                caretPos = startPos + value.Length;
            }
            else
            {
                // Just insert at caret
                caretPos = Mathf.Clamp(caretPos, 0, text.Length);
                text = text.Substring(0, caretPos) + value + text.Substring(caretPos);
                caretPos += value.Length;
            }
            
            SetCaretPosition(caretPos);
            return text;
        }
        
        private string DeleteAtCaret(string text)
        {
            int caretPos = inputComponent.caretPosition;
            
            // If selection exists, remove it
            if (inputComponent.selectionStringAnchorPosition != inputComponent.selectionStringFocusPosition)
            {
                int startPos = Mathf.Min(inputComponent.selectionStringAnchorPosition, inputComponent.selectionStringFocusPosition);
                int endPos = Mathf.Max(inputComponent.selectionStringAnchorPosition, inputComponent.selectionStringFocusPosition);
                
                text = text.Substring(0, startPos) + text.Substring(endPos);
                caretPos = startPos;
            }
            else if (caretPos > 0)
            {
                caretPos = Mathf.Clamp(caretPos, 0, text.Length);
                text = text.Substring(0, caretPos - 1) + text.Substring(caretPos);
                caretPos--;
            }
            else
            {
                return text; // Nothing to delete
            }
            
            SetCaretPosition(caretPos);
            return text;
        }
        
        private void SetCaretPosition(int pos)
        {
            Debug.Log($"Caret position updated! {pos}  Current text: {inputText}    Text Length: {inputText?.Length ?? 0}");
            lastCaretPos = pos;
            
            // keep all of TMP's public caret/selection fields in sync
            inputComponent.caretPosition = pos;
            inputComponent.selectionAnchorPosition = pos;
            inputComponent.selectionFocusPosition = pos;
            
            inputComponent.selectionStringAnchorPosition = pos;
            inputComponent.selectionStringFocusPosition = pos;
            inputComponent.stringPosition = pos;
        }
        
        
        // EVENT LISTENER METHODS
        public void CalculatorKey_OnPressed(KeyInfo key)
        {
            string label = key.label;
            string value = key.value;
            bool isAction = key.isAction;
            
            if (logKeyPress) Debug.Log($"Key \"{label}\" pressed. Value: {value} Is Action: {isAction}");
            
            if (isAction)
            {
                if (value.Equals("+/-", IGNORE_CASE)) Flip();
                if (value.Equals("ac", IGNORE_CASE)) Clear();
                if (value.Equals("del", IGNORE_CASE)) Backspace();
                if (value.Equals("=", IGNORE_CASE)) Equals();
            }
            else Type(value);
            
            // One refresh after the action
            RefreshUI();
        }
        
        public void ShiftToggle_OnValueChanged(UIToggleButton toggleButton, bool isOn)
        {
            Shift(isOn);
        }
        
        public void AltToggle_OnValueChanged(UIToggleButton toggleButton, bool isOn)
        {
            Alt(isOn);
        }
        
        public void InputComponent_onValueChanged(string value)
        {
            // User typed directly — sync model & preview. Do NOT call RefreshUI() here.
            inputText = value;
            
            previewText = CalculateAndGetString(inputText);
            if (previewComponent) previewComponent.text = previewText;
            
            // Keep our last caret snapshot in sync with TMP's caret
            lastCaretPos = inputComponent.caretPosition;
        }
    }
}
