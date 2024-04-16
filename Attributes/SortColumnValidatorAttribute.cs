using System.ComponentModel.DataAnnotations;

namespace MyBGList.Attributes
{
    public class SortColumnValidatorAttribute : ValidationAttribute
    {
        public Type EntityType { get; set; }

        public SortColumnValidatorAttribute(Type entityType) : base("Value must be one of the following: {0}.")
        {
            EntityType = entityType;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (EntityType != null)
            {
                var strValue = value as string;

                if (!string.IsNullOrEmpty(strValue) && EntityType.GetProperties().Any(property => property.Name.Equals(strValue, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult(FormatErrorMessage(string.Join(", ", EntityType!.GetProperties().Select(property => property.Name).ToArray())));
        }
    }
}