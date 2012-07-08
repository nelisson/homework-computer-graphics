namespace Utilities.Validators
{
    using System;
    using System.Globalization;
    using System.Windows.Controls;

    public class FloatRule : ValidationRule
    {
        #region Overrides of ValidationRule

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                var v = (string) value;

                if (!string.IsNullOrEmpty(v))
                {
                    float p = float.Parse(v);
                }
            }
            catch (Exception e)
            {
                return new ValidationResult(false, "Illegal characters or "
                                                   + e.Message);
            }

            return new ValidationResult(true, null);
        }

        #endregion
    }
}