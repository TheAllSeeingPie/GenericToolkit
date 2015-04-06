using GenericToolkit.Core.EntityFramework;

namespace GenericToolkit.Core.Tests
{
    public interface ITestEntityPutDto : IEntity
    {
        int AProperty { get; set; }
    }
}