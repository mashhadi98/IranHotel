namespace VisitorApp.Domain.Common.ResponseMessages;
public partial class ErrorMessages
{
    public static class Users
    {
        public static string NotFound = "کاربر مورد نظر یافت نشد";
        public static string EmailRequired = "ایمیل اجباری است";
        public static string FirstNameRequired = "نام اجباری است";
        public static string LastNameRequired = "نام خانوادگی اجباری است";
        public static string EmailAlreadyExists = "این ایمیل قبلاً استفاده شده است";
        public static string InvalidEmailFormat = "فرمت ایمیل صحیح نیست";
        public static string CannotDeleteAdmin = "حذف کاربر ادمین مجاز نیست";
        public static string UserAlreadyActive = "کاربر در حال حاضر فعال است";
        public static string UserAlreadyInactive = "کاربر در حال حاضر غیرفعال است";
        public static string PhoneNumberInvalid = "شماره تلفن صحیح نیست";
    }

    public static class Roles
    {
        public static string NotFound = "نقش مورد نظر یافت نشد";
        public static string NameRequired = "نام نقش اجباری است";
        public static string DescriptionRequired = "توضیحات نقش اجباری است";
        public static string NameAlreadyExists = "این نام نقش قبلاً استفاده شده است";
        public static string CannotDeleteSystemRole = "حذف نقش‌های سیستمی مجاز نیست";
        public static string RoleAlreadyActive = "نقش در حال حاضر فعال است";
        public static string RoleAlreadyInactive = "نقش در حال حاضر غیرفعال است";
        public static string RoleHasUsers = "این نقش دارای کاربران مرتبط می‌باشد و قابل حذف نیست";
    }
} 