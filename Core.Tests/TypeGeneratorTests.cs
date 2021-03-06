﻿using System;
using GenericToolkit.Core.WebApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenericToolkit.Core.Tests
{
    [TestClass]
    public class TypeGeneratorTests
    {
        public interface IMyTestInterface
        {
             
        }

        public class MyTestClass
        {
            
        }

        [TestMethod]
        public void TypeGenerator_returns_instance_of_interface_IMyTestInterface()
        {
            var instance = TypeGenerator.Generate<IMyTestInterface>();

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(IMyTestInterface));
        }

        [TestMethod]
        public void TypeGenerator_can_instantiate_classes()
        {
            var instance = TypeGenerator.Generate<MyTestClass>();
            Assert.IsInstanceOfType(instance, typeof(MyTestClass));
        }

        [TestMethod]
        public void IMyTestInterface_should_have_Type_equivalent_proxy()
        {
            var instance = TypeGenerator.Generate<IMyTestInterface>();
            Assert.IsInstanceOfType(instance, typeof(IMyTestInterface));
        }

        public interface IUseOnce { }

        [TestMethod]
        public void TypeGenerator_generates_type_if_not_in_dictionary()
        {
            TypeGenerator.GetGeneratedType<IUseOnce>();
        }
    }
}
