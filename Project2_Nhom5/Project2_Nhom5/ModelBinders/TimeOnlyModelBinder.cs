using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace Project2_Nhom5.ModelBinders
{
    public class TimeOnlyModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
                return Task.CompletedTask;

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
            var value = valueProviderResult.FirstValue;

            if (string.IsNullOrEmpty(value))
                return Task.CompletedTask;

            // Try multiple time formats
            if (TimeOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var timeOnly))
            {
                bindingContext.Result = ModelBindingResult.Success(timeOnly);
            }
            else if (TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out var timeSpan))
            {
                bindingContext.Result = ModelBindingResult.Success(TimeOnly.FromTimeSpan(timeSpan));
            }
            else
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid time format");
            }

            return Task.CompletedTask;
        }
    }
} 