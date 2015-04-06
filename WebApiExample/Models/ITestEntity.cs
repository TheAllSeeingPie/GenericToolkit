using GenericToolkit.Core.EntityFramework;

namespace WebApiExample.Models
{
    public interface ITestEntity : IEntity
    {
        string AProperty { get; set; }
    }
}