using System.Data.Common;
using System.Data.Entity;
using Effort;
using GenericToolkit.Core.EntityFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenericToolkit.Core.Tests
{
    [TestClass]
    public class GenericContextTests
    {
        private DbConnection connection;
        private GenericContext context;
        private ITestEntity testEntity;
        private DbSet dbSet;

        [TestInitialize]
        public void Initialise()
        {
            connection = DbConnectionFactory.CreateTransient();
            context = new GenericContext(new[] { typeof(ITestEntity) }, connection);
            testEntity = TypeGenerator.Generate<ITestEntity>();
            dbSet = context.Set(typeof(ITestEntity));
        }

        [TestCleanup]
        public void Cleanup()
        {
            connection.Dispose();
            context.Dispose();
        }

        [TestMethod]
        public void GenericContext_should_create_a_model_containing_a_ITestEntity()
        {
            testEntity.AProperty = "Hello world!";
            dbSet.Add(testEntity);
            context.SaveChanges();
            var returned = dbSet.Find(1);

            Assert.IsNotNull(returned);
            Assert.AreEqual("Hello world!", ((ITestEntity)returned).AProperty);
        }
    }
}