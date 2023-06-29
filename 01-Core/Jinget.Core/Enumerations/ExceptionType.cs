namespace Jinget.Core.Enumerations
{
    /// <summary>
    /// Define different exception types
    /// </summary>
    public enum ExceptionType
    {
        /// <summary>
        /// Exceptions raised from the jinget internal
        /// </summary>
        JingetInternal,
        
        /// <summary>
        /// Exceptions raised from the .Net framework internal
        /// </summary>
        DotNetInternal,

        /// <summary>
        /// Custom exceptoins defined by user itself in his/her code.
        /// </summary>
        Custom
    }
}
