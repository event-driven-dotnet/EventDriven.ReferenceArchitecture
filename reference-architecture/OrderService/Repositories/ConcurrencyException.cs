using System;
using System.Diagnostics.CodeAnalysis;

namespace OrderService.Repositories
{
    [ExcludeFromCodeCoverage]
    public class ConcurrencyException : Exception { }
}