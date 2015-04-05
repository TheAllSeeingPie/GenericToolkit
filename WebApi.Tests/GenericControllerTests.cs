using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using GenericToolkit.Core;
using GenericToolkit.Core.EntityFramework;
using GenericToolkit.Core.WebApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenericToolkit.WebApi.Tests
{
    [TestClass]
    public class GenericControllerTests
    {
        public interface ITestEntity : IEntity
        {
            string AProperty { get; set; }
        }

        public interface ITestEntityGetDto
        {
            string Id { get; set; }
        }

        public interface ITestEntityPostDto
        {
            double AProperty { get; set; }
        }

        public interface ITestEntityPutDto : IEntity
        {
            int AProperty { get; set; }
        }

        [TestMethod]
        public void Get_without_id_returns_all_entities()
        {
            var connection = Effort.DbConnectionFactory.CreateTransient();
            var context = new GenericContext(new[] { typeof(ITestEntity) }, connection);
            var testEntity = TypeGenerator.Generate<ITestEntity>();
            var dbSet = context.Set(typeof(ITestEntity));
            dbSet.Add(testEntity);
            context.SaveChanges();
            var controller = new GenericController<ITestEntity, ITestEntityGetDto, ITestEntityPostDto, ITestEntityPutDto>(context);

            var getDtos = controller.Get();

            Assert.AreEqual(1, getDtos.Count());
        }

        [TestMethod]
        public async Task Get_with_id_returns_an_entity()
        {
            var connection = Effort.DbConnectionFactory.CreateTransient();
            var context = new GenericContext(new[] { typeof(ITestEntity) }, connection);
            var testEntity = TypeGenerator.Generate<ITestEntity>();
            var dbSet = context.Set(typeof(ITestEntity));
            dbSet.Add(testEntity);
            context.SaveChanges();
            var controller = new GenericController<ITestEntity, ITestEntityGetDto, ITestEntityPostDto, ITestEntityPutDto>(context);

            var getDto = await controller.Get(1);

            Assert.AreEqual("1", getDto.Id);
        }

        [TestMethod]
        public async Task Post_entity_gets_saved_correctly()
        {
            var connection = Effort.DbConnectionFactory.CreateTransient();
            var context = new GenericContext(new[] { typeof(ITestEntity) }, connection);

            var controller = new GenericController<ITestEntity, ITestEntityGetDto, ITestEntityPostDto, ITestEntityPutDto>(context);

            var postDto = TypeGenerator.Generate<ITestEntityPostDto>();
            postDto.AProperty = 123.0;
            var result = await controller.Post(postDto);

            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<int>));
            Assert.AreEqual(1, ((OkNegotiatedContentResult<int>)result).Content);

            var dbSet = context.Set(typeof(ITestEntity));
            var entity = await dbSet.FindAsync(1);

            Assert.AreEqual("123", ((ITestEntity)entity).AProperty);
        }

        [TestMethod]
        public async Task Put_entity_gets_saved_correctly()
        {
            var connection = Effort.DbConnectionFactory.CreateTransient();
            var context = new GenericContext(new[] { typeof(ITestEntity) }, connection);
            var testEntity = TypeGenerator.Generate<ITestEntity>();
            var testEntity1 = TypeGenerator.Generate<ITestEntity>();
            var dbSet = context.Set(typeof(ITestEntity));
            dbSet.Add(testEntity);
            dbSet.Add(testEntity1);

            await context.SaveChangesAsync();

            var controller = new GenericController<ITestEntity, ITestEntityGetDto, ITestEntityPostDto, ITestEntityPutDto>(context);

            var putDto = TypeGenerator.Generate<ITestEntityPutDto>();
            putDto.Id = 2;
            putDto.AProperty = 123;
            var result = await controller.Put(putDto);

            Assert.IsInstanceOfType(result, typeof(OkResult));

            dbSet = context.Set(typeof(ITestEntity));
            var entity = await dbSet.FindAsync(putDto.Id);

            Assert.AreEqual("123", ((ITestEntity)entity).AProperty);
        }
    }
}
