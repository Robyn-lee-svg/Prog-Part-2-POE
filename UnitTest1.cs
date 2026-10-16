using System.ComponentModel.DataAnnotations;
using Xunit;
using System.Collections.Generic;
using RaceDay.DTOs;


namespace RaceDay.Tests
    {
        public class UnitTest1
        {
            [Fact]
            public void LoginDto_ValidEmailAndPassword_PassesValidation()
            {
                
                var login = new LoginDto
                {
                    Email = "runner@example.com",
                    Password = "TestPassword123!"
                };

                
                var context = new ValidationContext(login);
                var results = new List<ValidationResult>();

                bool isValid = Validator.TryValidateObject(
                    login, context, results, validateAllProperties: true);

                
                Assert.True(isValid);
            }

            [Fact]
            public void LoginDto_InvalidEmail_FailsValidation()
            {
                
                var login = new LoginDto
                {
                    Email = "not-an-email",
                    Password = "TestPassword123!"
                };

                
                var context = new ValidationContext(login);
                var results = new List<ValidationResult>();

                bool isValid = Validator.TryValidateObject(
                    login, context, results, validateAllProperties: true);

                
                Assert.False(isValid);
            }
        }
    }


