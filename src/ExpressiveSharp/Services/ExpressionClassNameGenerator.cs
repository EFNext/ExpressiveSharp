using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressiveSharp.Services
{
    public static class ExpressionClassNameGenerator
    {
        public const string Namespace = "ExpressiveSharp.Generated";

        public static string GenerateName(string? namespaceName, IEnumerable<string>? nestedInClassNames, string memberName)
        {
            return GenerateName(namespaceName, nestedInClassNames, memberName, null);
        }

        public static string GenerateName(string? namespaceName, IEnumerable<string>? nestedInClassNames, string memberName, IEnumerable<string>? parameterTypeNames)
        {
            var stringBuilder = new StringBuilder();

            return GenerateNameImpl(stringBuilder, namespaceName, nestedInClassNames, memberName, parameterTypeNames);
        }

        public static string GenerateFullName(string? namespaceName, IEnumerable<string>? nestedInClassNames, string memberName)
        {
            return GenerateFullName(namespaceName, nestedInClassNames, memberName, null);
        }

        public static string GenerateFullName(string? namespaceName, IEnumerable<string>? nestedInClassNames, string memberName, IEnumerable<string>? parameterTypeNames)
        {
            var stringBuilder = new StringBuilder(Namespace);
            stringBuilder.Append('.');

            return GenerateNameImpl(stringBuilder, namespaceName, nestedInClassNames, memberName, parameterTypeNames);
        }

        /// <summary>
        /// Class-level name (no member/parameter suffix) for the consolidated partial class.
        /// </summary>
        public static string GenerateClassName(string? namespaceName, IEnumerable<string>? nestedInClassNames)
        {
            var sb = new StringBuilder();
            return GenerateClassNameImpl(sb, namespaceName, nestedInClassNames);
        }

        /// <summary>
        /// Same as <see cref="GenerateClassName"/> but prefixed with <see cref="Namespace"/>.
        /// </summary>
        public static string GenerateClassFullName(string? namespaceName, IEnumerable<string>? nestedInClassNames)
        {
            var sb = new StringBuilder(Namespace);
            sb.Append('.');
            return GenerateClassNameImpl(sb, namespaceName, nestedInClassNames);
        }

        /// <summary>
        /// Method-name suffix encoding a member and its parameter types (e.g. "Add_P0_int").
        /// </summary>
        public static string GenerateMethodSuffix(string memberName, IEnumerable<string>? parameterTypeNames)
        {
            var sb = new StringBuilder();

            if (memberName.IndexOf('.') >= 0)
            {
                sb.Append(memberName.Replace(".", "__"));
            }
            else
            {
                sb.Append(memberName);
            }

            if (parameterTypeNames is not null)
            {
                var parameterIndex = 0;
                foreach (var parameterTypeName in parameterTypeNames)
                {
                    sb.Append("_P");
                    sb.Append(parameterIndex);
                    sb.Append('_');
                    AppendSanitizedTypeName(sb, parameterTypeName);
                    parameterIndex++;
                }
            }

            return sb.ToString();
        }

        static string GenerateClassNameImpl(StringBuilder stringBuilder, string? namespaceName, IEnumerable<string>? nestedInClassNames)
        {
            if (namespaceName is not null)
            {
                foreach (var c in namespaceName)
                {
                    stringBuilder.Append(c == '.' ? '_' : c);
                }
            }

            stringBuilder.Append('_');
            var arity = 0;

            if (nestedInClassNames is not null)
            {
                foreach (var className in nestedInClassNames)
                {
                    var arityCharacterIndex = className.IndexOf('`');
                    if (arityCharacterIndex is -1)
                    {
                        stringBuilder.Append(className);
                    }
                    else
                    {
#if NETSTANDARD2_0
                        arity += int.Parse(className.Substring(arityCharacterIndex + 1));
#else
                        arity += int.Parse(className.AsSpan().Slice(arityCharacterIndex + 1));
#endif
                        stringBuilder.Append(className, 0, arityCharacterIndex);
                    }

                    stringBuilder.Append('_');
                }
            }

            // Remove trailing '_' from class names (the member name used to follow)
            if (stringBuilder.Length > 0 && stringBuilder[stringBuilder.Length - 1] == '_')
            {
                stringBuilder.Length--;
            }

            if (arity > 0)
            {
                stringBuilder.Append('`');
                stringBuilder.Append(arity);
            }

            return stringBuilder.ToString();
        }

        static string GenerateNameImpl(StringBuilder stringBuilder, string? namespaceName, IEnumerable<string>? nestedInClassNames, string memberName, IEnumerable<string>? parameterTypeNames)
        {
            if (namespaceName is not null)
            {
                foreach (var c in namespaceName)
                {
                    stringBuilder.Append(c == '.' ? '_' : c);
                }
            }

            stringBuilder.Append('_');
            var arity = 0;

            if (nestedInClassNames is not null)
            {

                foreach (var className in nestedInClassNames)
                {
                    var arityCharacterIndex = className.IndexOf('`');
                    if (arityCharacterIndex is -1)
                    {
                        stringBuilder.Append(className);
                    }
                    else
                    {
#if NETSTANDARD2_0
                        arity += int.Parse(className.Substring(arityCharacterIndex + 1));
#else
                        arity += int.Parse(className.AsSpan().Slice(arityCharacterIndex + 1));
#endif
                        stringBuilder.Append(className, 0, arityCharacterIndex);
                    }

                    stringBuilder.Append('_');
                }

            }

            // Explicit interface members contain '.', which is invalid in identifiers.
            if (memberName.IndexOf('.') >= 0)
            {
                stringBuilder.Append(memberName.Replace(".", "__"));
            }
            else
            {
                stringBuilder.Append(memberName);
            }

            // Encode parameter types so overloaded methods produce distinct names.
            if (parameterTypeNames is not null)
            {
                var parameterIndex = 0;
                foreach (var parameterTypeName in parameterTypeNames)
                {
                    stringBuilder.Append("_P");
                    stringBuilder.Append(parameterIndex);
                    stringBuilder.Append('_');
                    AppendSanitizedTypeName(stringBuilder, parameterTypeName);
                    parameterIndex++;
                }
            }

            // Generic arity goes at the very end to match CLR generic naming.
            if (arity > 0)
            {
                stringBuilder.Append('`');
                stringBuilder.Append(arity);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Strips the <c>global::</c> prefix and replaces every character invalid in a C#
        /// identifier with <c>'_'</c>.
        /// </summary>
        private static void AppendSanitizedTypeName(StringBuilder sb, string typeName)
        {
            const string GlobalPrefix = "global::";
            var start = typeName.StartsWith(GlobalPrefix, StringComparison.Ordinal) ? GlobalPrefix.Length : 0;

            for (var i = start; i < typeName.Length; i++)
            {
                var c = typeName[i];
                sb.Append(IsInvalidIdentifierChar(c) ? '_' : c);
            }
        }

        private static bool IsInvalidIdentifierChar(char c) =>
            !char.IsLetterOrDigit(c) && c != '_';
    }
}
