using UnityEngine;

namespace ProjectATLAS.Library.Glossary
{
    public static class GlossaryUtil
    {
        public static string ToShortString(this TermType termType)
        {
            return termType switch
            {
                TermType.Term => "Term",
                TermType.Unit => "Unit",
                TermType.BaseQuantity => "Base",
                TermType.DerivedQuantity => "Derived",
                TermType.Expression => "Expression",
                _ => ""
            };
        }
        
        public static string ToFullString(this TermType termType)
        {
            return termType switch
            {
                TermType.Term => "Term",
                TermType.Unit => "Unit",
                TermType.BaseQuantity => "Base Quantity",
                TermType.DerivedQuantity => "Derived Quantity",
                TermType.Expression => "Expression",
                _ => ""
            };
        }
    }
}
