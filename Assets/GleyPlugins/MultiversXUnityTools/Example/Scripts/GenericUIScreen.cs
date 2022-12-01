using UnityEngine;

namespace MultiversXUnityExamples
{
    /// <summary>
    /// Generic UI template class, every UI Screen will inherit from this
    /// Useful when you have more advanced UI screens
    /// </summary>
    public abstract class GenericUIScreen : MonoBehaviour
    {
        public virtual void Init(params object[] args)
        {

        }
    }
}
