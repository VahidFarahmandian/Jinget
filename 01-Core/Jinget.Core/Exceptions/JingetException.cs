﻿namespace Jinget.Core.Exceptions;

/// <summary>
/// Provide a mechanism for defining custom jinget exceptions
/// Implements the <see cref="Exception" />
/// </summary>
/// <seealso cref="Exception" />
/// <param name="message">Exception message to be thrown</param>
/// <param name="ex">The inner exception to be thrown</param>
/// <param name="code">Custom code of exception</param>
/// <param name="type">Type of exception <see cref="ExceptionType"></see></param>
public class JingetException(string message, Exception ex, int code = -1, ExceptionType type = ExceptionType.JingetInternal) : Exception(message, ex)
{
    public int Code { get; set; } = code;

    public ExceptionType Type { get; set; } = type;

    /// <param name="message">Exception message to be thrown</param>
    /// <param name="code">Custom code of exception</param>
    /// <param name="type">Type of exception <see cref="ExceptionType"></see></param>
    public JingetException(string message, int code = -1, ExceptionType type = ExceptionType.JingetInternal)
        : this(message, new Exception(), code, type) { }
}
