using System;

namespace Server.Common.Extensions;

public static class RuleBuilderExtensions
{
      public static IRuleBuilderOptions<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder)
      {
          return ruleBuilder
              .NotEmpty().WithMessage("Password is required")
              .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
              .MaximumLength(32).WithMessage("Password must be at most 32 characters long")
              .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
              .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
              .Matches("[0-9]").WithMessage("Password must contain at least one digit")
              .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
      }

      public static IRuleBuilderOptions<T, string> ElfakEmail<T>(this IRuleBuilder<T, string> ruleBuilder)
      {
          return ruleBuilder
              .NotEmpty().WithMessage("Email is required")
              .EmailAddress().WithMessage("Email must be a valid email address")
              .Matches(@"^\S+$")
              .WithMessage("Email must be a valid email address")
              .Must(e => e.EndsWith("@elfak.rs", StringComparison.OrdinalIgnoreCase))
              .WithMessage("Email must be an elfak.rs address.");
      }

      public static IRuleBuilderOptions<T, string> Url<T>(this IRuleBuilder<T, string> ruleBuilder)
      {
          return ruleBuilder
              .NotEmpty()
              .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
              .WithMessage("The URL must be a well-formed absolute URI.");
      }
}
