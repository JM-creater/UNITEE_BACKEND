namespace UNITEE_BACKEND.Models.StaticClass
{
    public static class StringExtensions
    {
        public static bool Has(this string str1, string str2)
            => str1.Contains(str2, StringComparison.OrdinalIgnoreCase);
    }
}
