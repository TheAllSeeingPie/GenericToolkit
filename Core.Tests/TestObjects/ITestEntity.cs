using GenericToolkit.Core.EntityFramework;

namespace GenericToolkit.Core.Tests.TestObjects
{
    public interface ITestEntity : IEntity
    {
        string AProperty { get; set; }
    }
}