using Effort;
using GenericToolkit.Core;
using GenericToolkit.Core.EntityFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenericToolkit.EntityFramework.Tests
{
    [TestClass]
    public class GenericContextTests
    {
        public interface ITestEntity : IEntity
        {
            string AProperty { get; set; }
        }

        [TestMethod]
        public void GenericContext_should_create_a_model_containing_a_ITestEntity()
        {
            var connection = DbConnectionFactory.CreateTransient();
            var context = new GenericContext(new[] {typeof (ITestEntity)}, connection);

            var testEntity = TypeGenerator.Generate<ITestEntity>();

            var dbSet = context.Set(typeof(ITestEntity));
            testEntity.AProperty = "Hello world!";
            dbSet.Add(testEntity);
            context.SaveChanges();
            var returned = dbSet.Find(1);

            Assert.IsNotNull(returned);
            Assert.AreEqual("Hello world!", ((ITestEntity)returned).AProperty);
        }
    }
}