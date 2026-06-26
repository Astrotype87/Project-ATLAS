using System;
using UnityEngine;

using AstrotypeTools.InspectorAttributes;
using UnityEngine.Serialization;

namespace ProjectATLAS.Library.Glossary
{
    [CreateAssetMenu(fileName = "GlossaryData", menuName = "Scriptable Objects/GlossaryData")]
    public class GlossaryData : ScriptableObject
    {
        public int chapter;
        public LevelTerms[] levelTerms;
    }
    
    [Serializable]
    public class LevelTerms
    {
        [Indent(-1)] public int level;
        [Indent(-1, applyToCollection:true)] public GlossaryTerm[] terms;
    }
    
    [Serializable]
    public struct GlossaryTerm
    {
        [Indent(-1)] public string term;
        
        [FormerlySerializedAs("unit")]
        [Indent(-1)] public string symbol;
        
        [FormerlySerializedAs("type")]
        [Indent(-1)] public TermType type;
        
        [FormerlySerializedAs("description")]
        [Indent(-1), TextArea(2, 4)] public string definition;
        
        [FormerlySerializedAs("details")]
        [Indent(-1), TextArea(4, 20)] public string explanation;
        
        [Indent(-1), TextArea(4, 20)] public string formula;
    }
    
    public enum TermType
    {
        /// <summary> Simple definition of a word or phrase. </summary>
        Term,
        /// <summary> Describes the standard measure of a quantity. </summary>
        Unit,
        /// <summary> A fundamental physical quantity. </summary>
        BaseQuantity,
        /// <summary> A quantity derived from combinations of other base or derived quantities, represented by a formula or equation. </summary>
        DerivedQuantity,
        /// <summary> A mathematical theorem, formula, or expression outside the context of physics. </summary>
        Expression
    }
}
