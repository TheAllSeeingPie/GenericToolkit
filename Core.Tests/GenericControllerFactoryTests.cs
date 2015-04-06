using GenericToolkit.Core.WebApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenericToolkit.Core.Tests
{
    [TestClass]
    public class GenericControllerFactoryTests
    {
        [TestMethod]
        public void GenericControllerFactory_Generate_should_create_a_controller_with_a_typename_of_TestEntityController()
        {
            var controller = GenericControllerFactory.Generate<ITestEntity, ITestEntityGetDto, ITestEntityPostDto, ITestEntityPutDto>(null);
            Assert.AreEqual("TestEntityController", controller.GetType().Name);
        }
    }
}
