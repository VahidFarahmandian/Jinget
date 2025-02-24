namespace Jinget.Core.Attributes.ValidationAttributes;

public class TimeGreaterThanAttribute(string otherPropertyName) : ValidationAttribute
{
    /// <summary>
    /// check whether <paramref name="value"/> is greater than <paramref name="otherPropertyName"/>
    /// </summary>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success; // Allow nulls (optional, adjust as needed)

        TimeSpan startTime = (TimeSpan)value;
        TimeSpan? endTime = (TimeSpan?)validationContext.ObjectType.GetProperty(otherPropertyName)?.GetValue(validationContext.ObjectInstance);

        if (endTime.HasValue && startTime >= endTime)
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}