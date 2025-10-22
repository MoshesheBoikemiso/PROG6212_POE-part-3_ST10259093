using Xunit;
using CMCS_Prototype.Models;

namespace CMCS_Prototype.Tests
{
    public class BasicTests
    {
        [Fact]
        public void Claim_CalculateAmount_ShouldCalculateCorrectly()
        {
            // Arrange
            var claim = new Claim { TotalHours = 40, TotalAmount = 0 };

            // Act - Simulate the calculation from your controller
            decimal hourlyRate = 180;
            claim.TotalAmount = claim.TotalHours * hourlyRate;

            // Assert
            Assert.Equal(7200, claim.TotalAmount);
        }

        [Fact]
        public void Claim_DefaultStatus_ShouldBeSubmitted()
        {
            // Arrange & Act
            var claim = new Claim();

            // Assert
            Assert.Equal("Submitted", claim.Status);
        }

        [Fact]
        public void User_Role_ShouldBeStored()
        {
            // Arrange & Act
            var user = new User { UserRole = "Lecturer" };

            // Assert
            Assert.Equal("Lecturer", user.UserRole);
        }

        [Fact]
        public void Document_FileName_ShouldBeSet()
        {
            // Arrange & Act
            var document = new Document { FileName = "timesheet.pdf" };

            // Assert
            Assert.Equal("timesheet.pdf", document.FileName);
        }
    }

    public class BusinessLogicTests
    {
        [Theory]
        [InlineData(10, 1800)]
        [InlineData(20, 3600)]
        [InlineData(40, 7200)]
        public void CalculateClaimAmount_ShouldReturnCorrectValue(decimal hours, decimal expectedAmount)
        {
            // Arrange
            decimal hourlyRate = 180;

            // Act
            decimal calculatedAmount = hours * hourlyRate;

            // Assert
            Assert.Equal(expectedAmount, calculatedAmount);
        }

        [Fact]
        public void Claim_WithNotes_ShouldStoreNotes()
        {
            // Arrange & Act
            var claim = new Claim { Notes = "This is a test note" };

            // Assert
            Assert.NotNull(claim.Notes);
            Assert.Equal("This is a test note", claim.Notes);
        }
    }
}