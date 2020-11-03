using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EVEMon.Common;
using Tests.Helpers;
using Xunit;

namespace Tests.EVEMon.Common
{
    public static class AsyncVoidMethodsTests
    {
        /// <summary>
        /// Ensures that no method is marked asynchronous void.
        /// </summary>
        [Fact]
        public static void EnsureNoAsyncVoidMethods()
        {
            IEnumerable<MethodInfo> asyncVoidMethods = typeof(EveMonClient).Assembly.GetAsyncVoidMethods();

            List<string> messages = asyncVoidMethods.Select(method =>
                $"'{method.DeclaringType?.Name}.{method.Name}' is an async void method.")
                .ToList();

            Assert.False(messages.Any(),
                $"Async void methods found!{Environment.NewLine}{string.Join(Environment.NewLine, messages)}");
        }
    }
}
