using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Serialization;
using TMPro;
using KBCore.Refs;

using AstrotypeTools.InspectorAttributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

using MathEvaluation;
using MathEvaluation.Context;
using MathEvaluation.Parameters;

namespace ProjectATLAS.Lesson
{
    /// <summary> User interface for exploring formula computations with variable sliders. </summary>
    public class FormulaExplorer : MonoBehaviour
    {
        [Header("Display")]
        [SerializeField] private string title;
        [SerializeField, TextArea(2, 4)] private string formula;
        [SerializeField, TextArea(2, 10)] private string breakdown;
        
        [Header("Solution")]
        [SerializeField, TextArea(2, 10)] private string solution;
        [SerializeField] private Computation[] computations;
        [SerializeField, Child(Flag.Editable), InlineEditor] private VariableSlider[] variableSliders;
        
        [Header("Components")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text formulaText;
        [SerializeField] private TMP_Text breakdownText;
        [Space]
        [SerializeField] private TMP_Text formulaText2;
        [SerializeField] private TMP_Text solutionText;
        [SerializeField] private TMP_Text answerText;
        
        // PROPERTIES
        public double ResultValue { get; private set; }
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            foreach (var variableSlider in variableSliders)
            {
                variableSlider.OnSliderUpdated += VariableSliders_OnSliderUpdated;
            }
            this.ValidateRefs();
        }
        
        private void OnValidate()
        {
            RefreshUI();
        }
        
        
        // PUBLIC METHODS
        public void RefreshUI()
        {
            RefreshDetails();
            RefreshSolution();
        }
        
        
        // PRIVATE METHODS
        private void RefreshDetails()
        {
            string formulaString = formula;
            string breakdownString = breakdown;
            
            foreach (var variableSlider in variableSliders)
            {
                string varDisplay = variableSlider.Display;
                string colorHex = ColorUtility.ToHtmlStringRGB(variableSlider.Color);
                string coloredVar = $"<color=#{colorHex}>{varDisplay}</color>";
                
                // Formula string color coding
                formulaString = Regex.Replace(formulaString, varDisplay, coloredVar);
                
                // Breakdown string color coding
                breakdownString = Regex.Replace(breakdownString, $@"(?<!\w){Regex.Escape(varDisplay)}(?!\w)", coloredVar);
            }
            
            // Update title, formula, and breakdown text
            if (titleText) titleText.text = title;
            if (formulaText) formulaText.text = formulaString;
            if (breakdownText) breakdownText.text = breakdownString;
        }
        
        private void RefreshSolution()
        {
            // Skip if no reference to solutionText component
            if (solutionText == null) return;
            
            // Updated solution and formula text and math parameters for pre-calculations
            string formulaString = formula;
            string solutionString = solution;
            string answerString = "";
            MathParameters parameters = new();
            
            // For each variable sliders
            foreach (var variableSlider in variableSliders)
            {
                string varDisplay = variableSlider.Display;
                string varName = variableSlider.Variable;
                double varValue = variableSlider.Value;
                string colorHex = ColorUtility.ToHtmlStringRGB(variableSlider.Color);
                
                // Register parameter for evaluation
                parameters.BindVariable(varValue, varName);
                
                // Replace variable name with colored value in solution string
                string coloredValue = $"<color=#{colorHex}>{FormatFullDouble(varValue)}</color>";
                solutionString = Regex.Replace(
                    solutionString,
                    $@"\b{Regex.Escape(varName)}\b",
                    coloredValue
                );
                
                // Replace variable name with colored label in formula string
                string coloredVar = $"<color=#{colorHex}>{varDisplay}</color>";
                formulaString = Regex.Replace(
                    formulaString,
                    varDisplay,
                    coloredVar
                );
            }
            
            // For each computations
            for (int i = 0; i < computations.Length; i++)
            {
                try
                {
                    // Create math expression object and evaluate each computations
                    MathExpression expression = new(computations[i].expression.Trim(), new ScientificMathContext());
                    double result = expression.Evaluate(parameters);
                    
                    if (i == computations.Length - 1)
                    {
                        // Save result if last computation (complete computation of answer)
                        ResultValue = result;
                    }
                    
                    // Replace computations with their computed results
                    solutionString = Regex.Replace(
                        solutionString,
                        $@"\b{Regex.Escape(computations[i].alias)}\b",
                        FormatFullDouble(result, true)
                    );
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to compute alias '{computations[i].alias}': {ex.Message}");
                }
            }
            
            // Transfer last line of solutionString to the answerString
            answerString = solutionString.Split('\n', StringSplitOptions.RemoveEmptyEntries).Last();
            int lastNewLineIndex = solutionString.LastIndexOf('\n');
            solutionString = solutionString[..lastNewLineIndex];
            
            // Update solution text component
            formulaText2.text = formulaString;
            solutionText.text = solutionString;
            answerText.text = answerString;
        }
        
        
        private static string FormatFullDouble(double value, bool useComma = false)
        {
            if (double.IsNaN(value)) return "NaN";
            if (double.IsPositiveInfinity(value)) return "∞";
            if (double.IsNegativeInfinity(value)) return "-∞";
            
            double abs = Math.Abs(value);
            string format;
            
            // choose base numeric format
            if ((abs != 0 && abs < 1e-6) || abs >= 1e21)
            {
                format = useComma ? "N99" : "F99"; // N adds comma grouping
                string result = value.ToString(format, CultureInfo.InvariantCulture)
                    .TrimEnd('0')
                    .TrimEnd('.');
                
                // Remove grouping if InfinityCulture was forced to use comma with small exponents
                if (useComma)
                    result = AddThousandsSeparator(result);
                
                return result;
            }
            else
            {
                format = useComma
                    ? "#,0.#############################"
                    : "0.#############################";
                
                return value.ToString(format, CultureInfo.InvariantCulture);
            }
        }
        
        private static string AddThousandsSeparator(string number)
        {
            if (number.Contains("E") || number.Contains("e"))
                return number; // skip scientific notation
                
            if (!number.Contains('.'))
                return string.Format(CultureInfo.InvariantCulture, "{0:N0}", double.Parse(number, CultureInfo.InvariantCulture));
            
            string[] parts = number.Split('.');
            string integerPart = string.Format(CultureInfo.InvariantCulture, "{0:N0}", double.Parse(parts[0], CultureInfo.InvariantCulture));
            return integerPart + "." + parts[1];
        }
        
        
        
        // EVENT LISTENER METHODS
        private void VariableSliders_OnSliderUpdated(string variable, double value)
        {
            RefreshSolution();
        }
        
        
        // STRUCTS
        [Serializable]
        public struct Computation
        {
            public string alias;
            
            [FormerlySerializedAs("computation")]
            [TextArea(2, 4)] public string expression;
        }
    }
    
    
#if UNITY_EDITOR
    [CustomEditor(typeof(FormulaExplorer))]
    public class FormulaExplorerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Refresh"))
            {
                FormulaExplorer script = target as FormulaExplorer;
                if (script) script.RefreshUI();
            }
            
            base.OnInspectorGUI();
        }
    }
#endif
    
}
