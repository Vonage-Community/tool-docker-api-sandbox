using NJsonSchema.Validation;

namespace DockerApiSandbox.Api.InputValidation;

public static class ValidationErrorExtensions
{
    public static string Format(this ValidationError error) =>
        $"Property: {error.Property}, Kind: {error.Kind.ToString()}, Path: {error.Path}";
}