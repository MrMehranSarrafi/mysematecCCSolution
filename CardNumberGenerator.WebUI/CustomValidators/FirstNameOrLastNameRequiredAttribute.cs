using System.ComponentModel.DataAnnotations;
using System.Reflection;
namespace CardNoGenerator.WebUI.CustomValidators
{
    public class FirstNameOrLastNameRequiredAttribute : ValidationAttribute
    {
        public string OtherPropertyName { get; set; }
        public FirstNameOrLastNameRequiredAttribute(string otherPropertyName, string defaultErrorMessage = null)
        {
            OtherPropertyName = otherPropertyName;
            ErrorMessage = defaultErrorMessage ??  " ورود {0} یا {1} اجباری می باشد.";
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Get the current property value (e.g., FirstName)
            string? currentValue = value?.ToString();
            // Get the other property (e.g., LastName)
            PropertyInfo? otherProperty = validationContext.ObjectType.GetProperty(OtherPropertyName);
            if (otherProperty == null)
            {
                return new ValidationResult($"Property '{OtherPropertyName}' not found on type '{validationContext.ObjectType.Name}'.");
            }
            object? otherValue = otherProperty.GetValue(validationContext.ObjectInstance);
            string? otherCurrentValue = otherValue?.ToString();
            // Validation: At least one must be non-empty
            if (string.IsNullOrWhiteSpace(currentValue) && string.IsNullOrWhiteSpace(otherCurrentValue))
            {
                // Get display names for better error message
                string currentDisplayName = GetDisplayName(validationContext.MemberName, validationContext.ObjectType);
                string otherDisplayName = GetDisplayName(OtherPropertyName, validationContext.ObjectType);
                string message = string.Format(ErrorMessage, currentDisplayName, otherDisplayName);
                //return new ValidationResult(message, new[] { validationContext.MemberName, OtherPropertyName });
                return new ValidationResult(message, new[] { validationContext.MemberName});
            }
            return ValidationResult.Success;
        }
        private string GetDisplayName(string propertyName, Type type)
        {
            var property = type.GetProperty(propertyName);
            if (property == null) return propertyName;
            var displayAttr = property.GetCustomAttribute<DisplayAttribute>();
            return displayAttr?.Name ?? propertyName;
        }
    }
}
