namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class CallerArgumentExpressionAttribute : Attribute
    {
        public CallerArgumentExpressionAttribute(string parameterName) => ParameterName = parameterName;
        
        // ReSharper disable once UnusedMember.Global
        public string ParameterName { get; private set; }
    }
}