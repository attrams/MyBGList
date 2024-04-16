﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MyBGList.Attributes;

namespace MyBGList.DTO;

public class RequestDTO<T> : IValidatableObject
{
    [DefaultValue(value: 0)]
    public int PageIndex { get; set; } = 0;

    [DefaultValue(value: 10)]
    [Range(1, 100)]
    public int PageSize { get; set; } = 10;

    [DefaultValue(value: "Name")]
    public string? SortColumn { get; set; } = "Name";

    [DefaultValue(value: "ASC")]
    [SortOrderValidator]
    public string? SortOrder { get; set; } = "ASC";

    [DefaultValue(value: null)]
    public string? FilterQuery { get; set; } = null;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var validator = new SortColumnValidatorAttribute(typeof(T));
        var result = validator.GetValidationResult(SortColumn, validationContext);

        return (result != null) ? new[] { result } : new ValidationResult[0];
    }
}
