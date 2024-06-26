﻿using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using MyBGList.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MyBGList.Swagger;

public class SortOrderFilter : IParameterFilter
{
    public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
    {
        var attributes = context.ParameterInfo?.GetCustomAttributes(true).Union(
            context.ParameterInfo.ParameterType.GetProperties().Where(property => property.Name == parameter.Name).SelectMany(property => property.GetCustomAttributes(true))
        ).OfType<SortOrderValidatorAttribute>();

        if (attributes != null)
        {
            foreach (var attribute in attributes)
            {
                parameter.Schema.Extensions.Add("pattern", new OpenApiString(string.Join("|", attribute.AllowedValues.Select(value => $"^{value}$"))));
            }
        }
    }
}