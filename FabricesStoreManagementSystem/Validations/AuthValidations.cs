namespace FabricesStoreManagementSystem.Validations;

public class AuthValidations : AbstractValidator<LoginRequest>
{
    private const int MIN_PASSWORD_LENGTH = 8;
    private const int MAX_PASSWORD_LENGTH = 100;
    private const int MIN_USERNAME_LENGTH = 3;
    private const int MAX_USERNAME_LENGTH = 50;

    // Common weak passwords to reject (in production, use a comprehensive list)
    private static readonly HashSet<string> BannedPasswords = new(StringComparer.OrdinalIgnoreCase)
    {
        "password", "123456", "12345678", "123456789", "qwerty",
        "password1", "admin", "abc123", "letmein", "welcome",
        "كلمة السر", "1234567890", "111111", "123123"
    };

    public AuthValidations()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        // Username validation
        RuleFor(x => x.Username)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("اسم المستخدم مطلوب.")
            .Length(MIN_USERNAME_LENGTH, MAX_USERNAME_LENGTH)
            .WithMessage($"اسم المستخدم يجب أن يكون بين {MIN_USERNAME_LENGTH} و {MAX_USERNAME_LENGTH} حرفًا.")
            .Matches(@"^[a-zA-Z0-9_\.\-@]+$")
            .WithMessage("اسم المستخدم يمكن أن يحتوي على أحرف إنجليزية وأرقام ونقاط وشرطات وشرطات سفلية وعلامة @ فقط.")
            .Must(username => !username.Contains(" "))
            .WithMessage("اسم المستخدم لا يمكن أن يحتوي على مسافات.")
            .Must(username => !username.StartsWith(".") && !username.EndsWith("."))
            .WithMessage("اسم المستخدم لا يمكن أن يبدأ أو ينتهي بنقطة.")
            .Must(username => username.Count(c => c == '@') <= 1)
            .WithMessage("اسم المستخدم لا يمكن أن يحتوي على أكثر من علامة @ واحدة.");

        // Password validation (CRITICAL FOR SECURITY)
        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("كلمة المرور مطلوبة.");

        // Additional security: Check for suspicious patterns
        RuleFor(x => x)
            .Custom((request, context) =>
            {
                // Check for SQL injection patterns in username
                if (ContainsSqlInjectionPattern(request.Username))
                {
                    context.AddFailure("Username", "اسم المستخدم يحتوي على أنماط غير مسموح بها.");
                }

                // Check for SQL injection patterns in password
                if (ContainsSqlInjectionPattern(request.Password))
                {
                    context.AddFailure("Password", "كلمة المرور تحتوي على أنماط غير مسموح بها.");
                }
            });
    }

    private bool ContainsSqlInjectionPattern(string input)
    {
        if (string.IsNullOrEmpty(input))
            return false;

        var patterns = new[]
        {
            ";", "--", "/*", "*/", "@@", "char(", "nchar(",
            "varchar(", "nvarchar(", "alter ", "create ", "delete ",
            "drop ", "exec ", "execute ", "insert ", "select ", "update ", "union ",
            "'"
        };

        var upperInput = input.ToUpperInvariant();
        return patterns.Any(pattern => upperInput.Contains(pattern.ToUpperInvariant()));
    }
}