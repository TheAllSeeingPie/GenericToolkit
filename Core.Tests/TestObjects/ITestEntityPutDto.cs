using GenericToolkit.Core.EntityFramework;

namespace GenericToolkit.Core.Tests.TestObjects
{
    public interface ITestEntityPutDto : IEntity
    {
        int AProperty { get; set; }
    }
}