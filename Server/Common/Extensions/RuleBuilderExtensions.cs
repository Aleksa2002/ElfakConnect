using System;

namespace Server.Common.Extensions;

public static class RuleBuilderExtensions
{
      public static IRuleBuilderOptions<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder)
      {
          return ruleBuilder
              .NotEmpty()
              .MinimumLength(8)
              .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
              .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
              .Matches("[0-9]").WithMessage("Password must contain at least one digit")
              .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
      }

      public static IRuleBuilderOptions<T, string> ElfakEmail<T>(this IRuleBuilder<T, string> ruleBuilder)
      {
          return ruleBuilder
              .NotEmpty()
              .EmailAddress()
              .Matches(@"^\S+$")
              .WithMessage("Email must not contain whitespace characters.")
              .Must(e => e.EndsWith("@elfak.rs", StringComparison.OrdinalIgnoreCase))
              .WithMessage("Email must be an elfak.rs address.");
      }
}
