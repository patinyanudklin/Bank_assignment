using BrandNewDay_Assignment.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandNewDay_Assignment.UnitTests
{
    [TestFixture]
    class ProgramTest
    {
        // We can mock all the functions which require user input by
        // create an interface and a new class which call a Console.ReadLine

        // We can test the correctness by measure if the function being called properly regarding each input

        [Test]
        public void CreateNewAcct()
        {
            // Arrange
            var mockDB = new Mock<IDataProcessor>();
            var mockMyConsole = new Mock<IConsoleReadLine>();

            mockDB.Setup(x => x.IsNewCustomer(It.IsAny<string>())).Returns(false);
            mockDB.Setup(x => x.CreateNewAcct(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>()));
            mockMyConsole.Setup(x => x.Input()).Returns("1111");

            Program p = new Program(mockDB.Object, mockMyConsole.Object);

            // Act
            p.CreateNewAcct();

            // Assert
            mockDB.Verify(m => m.IsNewCustomer(It.IsAny<string>()), Times.Once());
            mockDB.Verify(m => m.CreateNewAcct(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>()), Times.Once());
        }
        
    }
}
