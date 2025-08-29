using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Project2_Nhom5.ModelBinders
{
    public class DateOnlyTimeOnlyModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context.Metadata.ModelType == typeof(DateOnly))
            {
                return new DateOnlyModelBinder();
            }

            if (context.Metadata.ModelType == typeof(TimeOnly))
            {
                return new TimeOnlyModelBinder();
            }

            return null;
        }
    }
} 