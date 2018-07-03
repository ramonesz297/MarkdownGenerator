using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MarkdownWikiGenerator
{
    public static class Beautifier
    {
        public static string BeautifyType(Type t, bool isFull = false)
        {
            if (t == null) return "";
            if (t == typeof(void)) return "void";
            if (!t.IsGenericType) return (isFull) ? t.FullName : t.Name;

            var innerFormat = string.Join(", ", t.GetGenericArguments().Select(x => BeautifyType(x)));
            return Regex.Replace(isFull ? t.GetGenericTypeDefinition().FullName : t.GetGenericTypeDefinition().Name, @"`.+$", "") + "<" + innerFormat + ">";
        }

        public static string ToMarkdownMethodInfo(MethodInfo methodInfo)
        {
            var isExtension = methodInfo.GetCustomAttributes<System.Runtime.CompilerServices.ExtensionAttribute>(false).Any();

            var seq = methodInfo.GetParameters().Select(x =>
            {
                string suffix = "";
                try
                {
                    //in some cases we can get error, for example new DateTime() as default value give error
                    suffix = x.HasDefaultValue ? (" = " + (x.DefaultValue ?? $"null")) : "";

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);

                    //if exception was received we can display default value for type
                    suffix = Activator.CreateInstance(x.ParameterType).ToString();
                }

                return "`" + BeautifyType(x.ParameterType) + "` " + x.Name + suffix;
            });

            return methodInfo.Name + "(" + (isExtension ? "this " : "") + string.Join(", ", seq) + ")";
        }
    }
}
