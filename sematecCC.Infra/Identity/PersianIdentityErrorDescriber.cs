using Microsoft.AspNetCore.Identity;

public class PersianIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError PasswordTooShort(int length)
    {
        return new IdentityError
        {
            Code = nameof(PasswordTooShort),
            Description = $"رمز عبور باید حداقل {length} کاراکتر باشد."
        };
    }

    public override IdentityError PasswordRequiresDigit()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresDigit),
            Description = "رمز عبور باید حداقل شامل یک عدد باشد."
        };
    }

    //public override IdentityError PasswordRequiresNonAlphanumeric()
    //{
    //    return new IdentityError
    //    {
    //        Code = nameof(PasswordRequiresNonAlphanumeric),
    //        Description = "رمز عبور باید حداقل شامل یک کاراکتر غیر الفبا و عدد باشد."
    //    };
    //}

    //public override IdentityError PasswordRequiresUpper()
    //{
    //    return new IdentityError
    //    {
    //        Code = nameof(PasswordRequiresUpper),
    //        Description = "رمز عبور باید حداقل شامل یک حرف بزرگ باشد."
    //    };
    //}

    //public override IdentityError PasswordRequiresLower()
    //{
    //    return new IdentityError
    //    {
    //        Code = nameof(PasswordRequiresLower),
    //        Description = "رمز عبور باید حداقل شامل یک حرف کوچک باشد."
    //    };
    //}

    //public override IdentityError DuplicateUserName(string userName)
    //{
    //    return new IdentityError
    //    {
    //        Code = nameof(DuplicateUserName),
    //        Description = $"نام کاربری {userName} قبلاً ثبت شده است."
    //    };
    //}

    //public override IdentityError DuplicateEmail(string email)
    //{
    //    return new IdentityError
    //    {
    //        Code = nameof(DuplicateEmail),
    //        Description = $"ایمیل {email} قبلاً ثبت شده است."
    //    };
    //}

    //public override IdentityError InvalidEmail(string email)
    //{
    //    return new IdentityError
    //    {
    //        Code = nameof(InvalidEmail),
    //        Description = $"ایمیل {email} معتبر نیست."
    //    };
    //}

    // می‌تونی بقیه پیام‌ها رو هم به همین شکل فارسی بزاری
}
