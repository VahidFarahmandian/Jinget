namespace Jinget.Core.Attributes.ValidationAttributes;

public class DateGreaterThanAttribute(string otherPropertyName) : ValidationAttribute
{
    /// <summary>
    /// check whether <paramref name="value"/> is greater than <paramref name="otherPropertyName"/>
    /// </summary>
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null) return ValidationResult.Success; // Allow nulls (optional, adjust as needed)

        DateTime startDate = (DateTime)value;
        DateTime? endDate = (DateTime?)validationContext.ObjectType.GetProperty(otherPropertyName)?.GetValue(validationContext.ObjectInstance);

        if (endDate.HasValue && startDate >= endDate)
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}