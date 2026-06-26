using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ProjectATLAS.Utility;

namespace ProjectATLAS.UI.Layout
{
    public class UIHorizontalLayout : MonoBehaviour
    {
        [Header("Horizontal Settings")]
        [SerializeField] private List<float> distributionList;
        [SerializeField] private float spacing;
        [SerializeField] private bool autoFillHeight;
        [SerializeField] [Range(0, 1)] float verticalAnchor;
        
        [Header("Debug Message")]
        [SerializeField] [TextArea(10, 20)] private string message;
        
        
        public void UpdateLayout()
        {
            // GET REFERENCE TO CHILD UI ELEMENTS
            RectTransform[] childUIElements = GetComponentsInChildren<RectTransform>()
                .Where(element => element.parent == transform)
                .ToArray();
            
            // GET INITIAL VALUES
            float parentWidth = (transform as RectTransform).rect.width;
            int numberOfElements = childUIElements.Length;
            float totalSpacing = spacing * (numberOfElements - 1);
            float sumOfElementWidths = parentWidth - totalSpacing;
            
            // CALCULATE DISTRIBUTION PERCENTAGES
            UpdateDistributionList(numberOfElements);
            float distributionSum = distributionList.Sum();
            float[] distributionPercentages = distributionList
                .Select(a => a / distributionSum).ToArray();
            
            // CALCULATE DISTRIBUTION POINTS
            List<float> distributionPoints = new() { 0.0f };
            for (int i = 0; i < numberOfElements; i++)
            {
                distributionPoints.Add(distributionPercentages[i] + distributionPoints.Last());
            }
            
            // CALCULATE ELEMENT WIDTHS
            float[] originalElementWidths = distributionPercentages
                .Select(a => a * parentWidth).ToArray();
            float[] distributedElementWidths = distributionPercentages
                .Select(a => a * sumOfElementWidths).ToArray();
            
            // CALCULATE ELEMENT OFFSETS
            float[] totalElementOffsets = new float[numberOfElements];
            for (int i = 0; i < numberOfElements; i++)
            {
                totalElementOffsets[i] = originalElementWidths[i] - distributedElementWidths[i];
            }
            
            // CALCULATE LEFT AND RIGHT OFFSETS
            (float left, float right)[] elementOffsets =
                Enumerable.Repeat((0.0f, 0.0f), numberOfElements).ToArray();
            
            for (int i = 0; i < numberOfElements; i++)
            {
                if (i == 0)
                {
                    elementOffsets[i].left = 0;
                    elementOffsets[i].right = totalElementOffsets[i];
                }
                else if (i == numberOfElements - 1)
                {
                    elementOffsets[i].left = totalElementOffsets[i];
                    elementOffsets[i].right = 0;
                }
                else
                {
                    elementOffsets[i].left = spacing - elementOffsets[i - 1].right;
                    elementOffsets[i].right = totalElementOffsets[i] - elementOffsets[i].left;
                }
            }
            
            // APPLY DISTRIBUTION AND SPACING
            for (int i = 0; i < numberOfElements; i++)
            {
                // RECORD PREVIOUS STATE FOR UNDO
                RectTransform rectTransform = childUIElements[i];
#if UNITY_EDITOR
                Undo.RecordObject(rectTransform, "Undo Update Horizontal Layout");
#endif
                
                // APPLY DISTRIBUTION TO ANCHOR POINTS
                rectTransform.SetAnchorLeft(distributionPoints[i]);
                rectTransform.SetAnchorRight(distributionPoints[i + 1]);
                rectTransform.SetAnchorBottom(autoFillHeight ? 0 : verticalAnchor);
                rectTransform.SetAnchorTop(autoFillHeight ? 1 : verticalAnchor);
                
                if (!autoFillHeight && rectTransform.sizeDelta.y <= 0)
                {
                    float parentHeight = (transform as RectTransform).sizeDelta.y;
                    rectTransform.sizeDelta = new(rectTransform.sizeDelta.x, parentHeight);
                }
                
                // APPLY DISTRIBUTION TO LEFT AND RIGHT OFFSETS
                rectTransform.SetAnchorOffsetLeft(elementOffsets[i].left);
                rectTransform.SetAnchorOffsetRight(elementOffsets[i].right);
                
                if (autoFillHeight)
                {
                    rectTransform.SetAnchorOffsetBottom(0);
                    rectTransform.SetAnchorOffsetTop(0);
                }
            }
            
            // PRINT DEBUG MESSAGE
            message = string.Empty;
            message += $"Parent Width: {parentWidth}\n";
            message += $"Number Of Elements: {numberOfElements}\n";
            message += $"Total Spacing: {totalSpacing}\n";
            message += $"Sum Of Element Widths: {sumOfElementWidths}\n";
            message += $"Distribution Sum:\n{string.Join(", ", distributionSum)}\n";
            message += $"Distribution Percentages:\n{string.Join(", ", distributionPercentages)}\n";
            message += $"Distribution Points:\n{string.Join(", ", distributionPoints)}\n";
            message += $"Original Element Widths:\n{string.Join(", ", originalElementWidths)}\n";
            message += $"Distributed Element Widths:\n{string.Join(", ", distributedElementWidths)}\n";
            message += $"Total Element Offsets:\n{string.Join(", ", totalElementOffsets)}\n";
            message += $"Element Offsets:\n{string.Join(", ", elementOffsets)}\n";
        }
        
        private void UpdateDistributionList(int numberOfElements)
        {
            if (distributionList == null || distributionList.Count == 0)
            {
                distributionList = Enumerable.Repeat(1.0f, numberOfElements).ToList();
            }
            else if (distributionList.Count < numberOfElements)
            {
                float averageDistribution = distributionList.Average();
                int numberOfElementsAdded = numberOfElements - distributionList.Count;
                distributionList.AddRange(Enumerable.Repeat(averageDistribution, numberOfElementsAdded));
            }
            else if (distributionList.Count > numberOfElements)
            {
                distributionList = distributionList.Take(numberOfElements).ToList();
            }
        }
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(UIHorizontalLayout))]
    public class CustomHorizontalLayoutEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Update Horizontal Layout"))
            {
                UIHorizontalLayout script = (UIHorizontalLayout)target;
                script.UpdateLayout();
            }
            DrawDefaultInspector();
        }
    }
    #endif
}
