using JobApplicationLibrary.Models;
using System.Security.Cryptography;
using Moq;
using JobApplicationLibrary.Services;
using FluentAssertions;

namespace JobApplicationLibrary.UnitTest
{
    public class ApplicationEvaluateUnitTest
    {

        //UunitOfWork_Contdition_ExpectedResult
        //Condition_Result

        [Test]
        public void Application_WithUnderAge_TransferredToAutoRejected()
        {
            //Arrange
            var evaluator = new ApplicationEvaluator(null);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 17
                }
            };

            //Action
            var appResult = evaluator.Evaluate(form);

            //Assert
            Assert.AreEqual(appResult, ApplicationResult.AutoRejected );

        }

        [Test]
        public void Application_WithNoTechStack_TransferredToAutoRejected()
        {
            //Arrange

            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;

            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");
            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(true); //Return true for any input


            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 19,
                    IdentityNumber = ""
                },
                
                
                TechStackList = new System.Collections.Generic.List<string>() {""} //empty techstack
            };

            //Action
            var appResult = evaluator.Evaluate(form);

            //Assert
            Assert.AreEqual(ApplicationResult.AutoRejected, appResult);

        }

        [Test]
        public void Application_WithTechStackOver75PandExperianceOver15Years_TransferredToAutoAccepted()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;

            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");
            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(true); //Return true for any input



            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 19
                },
                TechStackList = new System.Collections.Generic.List<string>() { "C#", "RabbitMQ", "Microservice", "VisualStudio" },
                YearsOfExperience = 16
                
            };

            //Action
            var appResult = evaluator.Evaluate(form);

            //Assert
            //Assert.AreEqual(ApplicationResult.AutoAccepted, appResult);
            appResult.Should().Be(ApplicationResult.AutoAccepted); //fluent assertion

        }

        [Test]
        public void Application_WithInvalidIdentityNumber_TransferredToHR()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>(MockBehavior.Strict); //Strinct All invocation must have a corresponding setup.

            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY"); //nested interfaces
            mockValidator.SetupProperty(i => i.ValidationMode);
            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(false); //Return true for any input



            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 19
                }
            };

            //Action
            var appResult = evaluator.Evaluate(form);

            //Assert
            Assert.AreEqual(ApplicationResult.TransferredToHR, appResult);

        }

        [Test]
        public void Application_WithOfficeLocation_TransferredToCTO()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>(MockBehavior.Strict); //Strinct All invocation must have a corresponding setup.
            
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("SPAIN"); //nested interfaces
            mockValidator.SetupProperty(i => i.ValidationMode);

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 19
                }
               
            };

            //Action
            var appResult = evaluator.Evaluate(form);

            //Assert
            Assert.AreEqual(appResult, ApplicationResult.TransferredToCTO);

        }

        [Test]
        public void Application_WithOver50_ValidationModeToDetailed()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("SPAIN"); //nested interfaces

            mockValidator.SetupProperty(i => i.ValidationMode);  
            //mockValidator.SetupAllProperties();

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 51
                }

            };

            //Action
            var appResult = evaluator.Evaluate(form);

            //Assert
            Assert.AreEqual(ValidationMode.Detailed, mockValidator.Object.ValidationMode);

        }
        [Test]
        public void Application_WithNullApplicant_ThrowsArguemntNullException() {


            //Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication(); //applicant is being sent null

            //Action
            Action appResultAction = () => evaluator.Evaluate(form);

            //Assert
            appResultAction.Should().Throw<ArgumentNullException>();
            

        }

        [Test]
        public void Application_WithDefaultValue_IsValidCalled()
        {
            //Arrange

            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;

            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");
            

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 19,
                    IdentityNumber = "123"
                },

            };

            //Action
            var appResult = evaluator.Evaluate(form);

            //Assert

            mockValidator.Verify(i => i.IsValid(It.IsAny<string>()), Times.Once);
           
        }

        [Test]
        public void Application_WithYoungAge_IsValidNeverCalled()
        {
            //Arrange

            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;

            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");


            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 15
                },

            };

            //Action
            var appResult = evaluator.Evaluate(form);

            //Assert

            mockValidator.Verify(i => i.IsValid(It.IsAny<string>()),Times.Never);

        }


    }
}