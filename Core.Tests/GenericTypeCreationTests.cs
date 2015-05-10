using System;
using System.Data.Entity.Core.EntityClient;
using GenericToolkit.Core.EntityFramework;
using GenericToolkit.Core.WebApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenericToolkit.Core.Tests
{
    [TestClass]
    public class GenericTypeCreationTests
    {
        [TestMethod]
        public void SeeIfAGenericTypeCanBeNamed()
        {
            var inputType = typeof(IEntity);
            var type = typeof (GenericController<,,,>).MakeGenericType(inputType, inputType, inputType, inputType);
            var instance = Activator.CreateInstance(type, new GenericContext(new[] { typeof(IEntity) }, new EntityConnection())) as GenericController<IEntity, IEntity, IEntity, IEntity>;
            Assert.AreEqual("EntityController", instance.Name);
        }
    }
}
