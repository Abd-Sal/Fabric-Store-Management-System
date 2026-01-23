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
            .WithMessage("كلمة المرور مطلوبة.")
            .MinimumLength(MIN_PASSWORD_LENGTH)
            .WithMessage($"كلمة المرور يجب أن تحتوي على {MIN_PASSWORD_LENGTH} أحرف على الأقل.")
            .MaximumLength(MAX_PASSWORD_LENGTH)
            .WithMessage($"كلمة المرور لا يمكن أن تتجاوز {MAX_PASSWORD_LENGTH} حرفًا.")
            .Must(ContainUpperCase)
            .WithMessage("كلمة المرور يجب أن تحتوي على حرف كبير واحد على الأقل.")
            .Must(ContainLowerCase)
            .WithMessage("كلمة المرور يجب أن تحتوي على حرف صغير واحد على الأقل.")
            .Must(ContainDigit)
            .WithMessage("كلمة المرور يجب أن تحتوي على رقم واحد على الأقل.")
            .Must(ContainSpecialCharacter)
            .WithMessage("كلمة المرور يجب أن تحتوي على رمز خاص واحد على الأقل (!@#$%^&*).")
            .Must(NotBeCommonPassword)
            .WithMessage("كلمة المرور ضعيفة جدًا. يرجى اختيار كلمة مرور أقوى.")
            .Must(NotContainUsername)
            .WithMessage("كلمة المرور لا يمكن أن تحتوي على اسم المستخدم.");

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

    // Password complexity validation methods
    private bool ContainUpperCase(string password)
    {
        return !string.IsNullOrEmpty(password) && password.Any(char.IsUpper);
    }

    private bool ContainLowerCase(string password)
    {
        return !string.IsNullOrEmpty(password) && password.Any(char.IsLower);
    }

    private bool ContainDigit(string password)
    {
        return !string.IsNullOrEmpty(password) && password.Any(char.IsDigit);
    }

    private bool ContainSpecialCharacter(string password)
    {
        if (string.IsNullOrEmpty(password)) return false;

        var specialCharacters = "!@#$%^&*()_+-=[]{}|;:,.<>?";
        return password.Any(c => specialCharacters.Contains(c));
    }

    private bool NotBeCommonPassword(string password)
    {
        if (string.IsNullOrEmpty(password)) return false;

        // Check against banned passwords
        if (BannedPasswords.Contains(password))
            return false;

        // Check for simple patterns
        if (IsSimplePattern(password))
            return false;

        return true;
    }

    private bool NotContainUsername(LoginRequest request, string password)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(request.Username))
            return true;

        return !password.Contains(request.Username, StringComparison.OrdinalIgnoreCase);
    }

    private bool IsSimplePattern(string password)
    {
        // Check for repeating characters
        if (password.Distinct().Count() <= 2)
            return true;

        // Check for keyboard patterns
        var keyboardPatterns = new[] { "qwerty", "asdfgh", "zxcvbn", "йцукен", "фывапр" };
        foreach (var pattern in keyboardPatterns)
        {
            if (password.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    private bool ContainsSqlInjectionPattern(string input)
    {
        if (string.IsNullOrEmpty(input))
            return false;

        var patterns = new[]
        {
            ";", "--", "/*", "*/", "@@", "char(", "nchar(",
            "varchar(", "nvarchar(", "alter ", "create ", "delete ",
            "drop ", "exec ", "execute ", "insert ", "select ", "update ", "union "
        };

        var upperInput = input.ToUpperInvariant();
        return patterns.Any(pattern => upperInput.Contains(pattern.ToUpperInvariant()));
    }
}