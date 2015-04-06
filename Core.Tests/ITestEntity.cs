using GenericToolkit.Core.EntityFramework;

namespace GenericToolkit.Core.Tests
{
    public interface ITestEntity : IEntity
    {
        string AProperty { get; set; }
    }
}