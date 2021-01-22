using Newtonsoft.Json.Linq;
using System.Reflection;

namespace FS.TimeTracking.Application.ValidationConverters
{
    public abstract class ValidationDescriptionConverter
    {
        public static JProperty GetErrorArgument(CustomAttributeData attribute, string errorI18NPrefix, string defaultError)
        {
            //var errorArgument = attribute.NamedArguments!.FirstOrDefault(x => x.MemberName == nameof(ValidationAttribute.ErrorMessage)).TypedValue.Value;
            //var errorMessage = errorI18NPrefix + ((string)errorArgument ?? defaultError);
            //return new JProperty("message", errorMessage);
            return new("message", "");
        }
    }
}
