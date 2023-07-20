using System;
using Jinget.Core.Enumerations;

namespace Jinget.Core.Exceptions
{
    /// <summary>
    /// Provide a mechanism for defining custom jinget exceptions
    /// Implements the <see cref="Exception" />
    /// </summary>
    /// <seealso cref="Exception" />
    public class JingetException : Exception
    {
        public int Code { get; set; }

        public ExceptionType Type { get; set; }

        /// <param name="message">Exception message to be thrown</param>
        /// <param name="code">Custom code of exception</param>
        /// <param name="type">Type of exception <see cref="ExceptionType"></param>
        public JingetException(string message, int code = -1, ExceptionType type = ExceptionType.JingetInternal)
            : this(message, null, code, type) { }

        /// <param name="message">Exception message to be thrown</param>
        /// <param name="ex">The inner exception to be thrown</param>
        /// <param name="code">Custom code of exception</param>
        /// <param name="type">Type of exception <see cref="ExceptionType"></param>
        public JingetException(string message, Exception ex, int code = -1, ExceptionType type = ExceptionType.JingetInternal)
            : base(message, ex)
        {
            Code = code;
            Type = type;
        }
    }
}
