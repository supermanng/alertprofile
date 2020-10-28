using System;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Applications.ExtentionMethods
{
    public static class StringExtensions
    {
        public static bool Equate(this string value, string comparer)
        {
            if ((string.IsNullOrEmpty(value)) || (string.IsNullOrEmpty(comparer)))
                throw new NullReferenceException();
            return value.Trim().ToLower().Equals(comparer.Trim().ToLower());
        }



    }
}
