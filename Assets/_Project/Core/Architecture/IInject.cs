using UnityEngine;

namespace ProjectATLAS.Architecture
{
    /// <summary>
    /// Defines a contract for injecting a single dependency of type <see cref="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the dependency to inject.</typeparam>
    public interface IInject<T>
    {
        /// <summary>
        /// Injects the specified dependency of type <see cref="T"/>.
        /// </summary>
        /// <param name="dependency">The dependency to inject.</param>
        public void Inject(T dependency);
    }
}
