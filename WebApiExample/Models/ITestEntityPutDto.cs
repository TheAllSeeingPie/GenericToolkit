using GenericToolkit.Core.EntityFramework;

namespace WebApiExample.Models
{
    public interface ITestEntityPutDto : IEntity
    {
        int AProperty { get; set; }
    }
}