using GenericToolkit.Core.EntityFramework;

namespace WebApiExample.Models
{
    public interface IPerson : IEntity
    {
        string Name { get; set; }
        int Age { get; set; }
    }
}