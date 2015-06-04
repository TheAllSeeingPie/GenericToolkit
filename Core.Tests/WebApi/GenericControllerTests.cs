using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Effort;
using GenericToolkit.Core.EntityFramework;
using GenericToolkit.Core.Tests.TestObjects;
using GenericToolkit.Core.WebApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenericToolkit.Core.Tests.WebApi
{
    [TestClass]
    public class GenericControllerTests
    {
        private DbConnection connection;
        private GenericContext context;
        private ITestEntity testEntity;
        private DbSet dbSet;
        private GenericController<ITestEntity, ITestEntityGetDto, ITestEntityPostDto, ITestEntityPutDto> controller;

        [TestInitialize]
        public void Initialise()
        {
            connection = DbConnectionFactory.CreateTransient();
            var entityType = typeof(ITestEntity);
            context = new GenericContext(new[] { entityType }, connection);
            testEntity = TypeGenerator.Generate<ITestEntity>();
            dbSet = context.Set(entityType);
            controller = new GenericController<ITestEntity, ITestEntityGetDto, ITestEntityPostDto, ITestEntityPutDto>(context);
            Mapping.Register(new[] { typeof(ITestEntity) }, new[] { typeof(ITestEntityGetDto), typeof(ITestEntityPostDto), typeof(ITestEntityPutDto) });
        }

        [TestCleanup]
        public void Cleanup()
        {
            connection.Dispose();
            context.Dispose();
            controller.Dispose();
        }

        [TestMethod]
        public async Task Get_without_id_returns_all_entities()
        {
            dbSet.Add(testEntity);
            await context.SaveChangesAsync();

            var getDtos = await controller.Get() as OkNegotiatedContentResult<IEnumerable<int>>;

            Assert.AreEqual(1, getDtos.Content.Count());
        }

        [TestMethod]
        public async Task Get_with_id_returns_an_entity()
        {
            dbSet.Add(testEntity);
            await context.SaveChangesAsync();

            var getDto = await controller.Get(1) as OkNegotiatedContentResult<ITestEntityGetDto>;
            
            Assert.AreEqual("1", getDto.Content.Id);
        }

        [TestMethod]
        public async Task Post_entity_gets_saved_correctly()
        {
            var postDto = TypeGenerator.Generate<ITestEntityPostDto>();
            postDto.AProperty = 123.0;
            var result = await controller.Post(postDto) as OkNegotiatedContentResult<int>;

            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<int>));
            Assert.AreEqual(1, result.Content);

            var entity = await dbSet.FindAsync(1);

            Assert.AreEqual("123", ((ITestEntity)entity).AProperty);
        }

        [TestMethod]
        public async Task Put_entity_gets_saved_correctly()
        {
            var testEntity1 = TypeGenerator.Generate<ITestEntity>();
            dbSet.Add(testEntity);
            dbSet.Add(testEntity1);

            await context.SaveChangesAsync();

            var putDto = TypeGenerator.Generate<ITestEntityPutDto>();
            putDto.Id = 2;
            putDto.AProperty = 123;
            var result = await controller.Put(putDto);

            Assert.IsInstanceOfType(result, typeof(OkResult));

            dbSet = context.Set(typeof(ITestEntity));
            var entity = await dbSet.FindAsync(putDto.Id);

            Assert.AreEqual("123", ((ITestEntity)entity).AProperty);
        }

        [TestMethod]
        public async Task Delete_entity_gets_removed()
        {
            dbSet.Add(testEntity);

            await context.SaveChangesAsync();
            
            Assert.AreEqual(1, (await dbSet.ToListAsync()).Count);

            await controller.Delete(1);

            Assert.AreEqual(0, (await dbSet.ToListAsync()).Count);
        }
    }
}
