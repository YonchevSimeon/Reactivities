namespace Application.Validators
{
    using FluentValidation;
    public static class ValidatorExtensions
    {
        public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            IRuleBuilderOptions<T, string> options = ruleBuilder
                .NotEmpty()
                .MinimumLength(6).WithMessage("Password must be at least 6 characters")
                .Matches("[A-Z]").WithMessage("Password must contains uppercase letter character")
                .Matches("[a-z]").WithMessage("Password must contains lowercase letter character")
                .Matches("[0-9]").WithMessage("Password must contains number")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contains non alphanumeric");

            return options;
        }
    }
}