using Backend_Development_LeMarconnes_Reserveringssysteem.Repositories;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests_BED_DAL.UnitTests
{
    public class UnitTestsGebruiker
    {
        private readonly string _testConnectionString = "Server=localhost;Database=TestDB;Trusted_Connection=true;";

        // ==================== USE CASE TESTING ====================

        // BK-UC5-CREATE
        // Test based on UC5: Aanmaken van een gast-account
        [Fact]
        public void Create_ValidGebruiker_ReturnsTrue()
        {
            // Arrange
            var repository = new GebruikerRepository(_testConnectionString);
            var gebruiker = new Gebruiker
            {
                Naam = "Jan Jansen",
                Emailadres = "jan.jansen@example.com",
                HashedWachtwoord = "hashed_password_123",
                Salt = "random_salt_456",
                Telefoon = "0612345678",
                Autokenteken = "AB-123-CD",
                Taal = "NL"
            };

            // Act
            var result = repository.Create(gebruiker);

            // Assert
            Assert.True(result);
        }

        // BK-UC11-READ
        // Test based on UC11: Inzien van Accountgegevens
        [Fact]
        public void GetGebruikers_ByID_ReturnsCorrectGebruiker()
        {
            // Arrange
            var repository = new GebruikerRepository(_testConnectionString);
            int testGebruikerID = 1;

            // Act
            var result = repository.GetGebruikers(testGebruikerID, "ALL", "ALL", 0, false);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(testGebruikerID, result[0].GebruikerID);
        }

        // ==================== BOUNDARY VALUE ANALYSIS ====================

        // BK-BVA-NAAM-MIN
        // Testing minimum length name (1 character)
        [Fact]
        public void Create_NaamOneCharacter_ReturnsTrue()
        {
            // Arrange
            var repository = new GebruikerRepository(_testConnectionString);
            var gebruiker = new Gebruiker
            {
                Naam = "A",
                Emailadres = "a@example.com",
                HashedWachtwoord = "hashed",
                Salt = "salt"
            };

            // Act
            var result = repository.Create(gebruiker);

            // Assert
            Assert.True(result);
        }

        // BK-BVA-TELEFOON-NULL
        // Testing boundary for optional Telefoon field (null is valid)
        [Fact]
        public void Create_TelefoonNull_ReturnsTrue()
        {
            // Arrange
            var repository = new GebruikerRepository(_testConnectionString);
            var gebruiker = new Gebruiker
            {
                Naam = "Test User",
                Emailadres = "test@example.com",
                HashedWachtwoord = "hashed",
                Salt = "salt",
                Telefoon = null,
                Autokenteken = null,
                Taal = null
            };

            // Act
            var result = repository.Create(gebruiker);

            // Assert
            Assert.True(result);
        }

        // ==================== ERROR GUESSING ====================

        // BK-ERR-DUPLICATE-EMAIL
        // Error: Creating user with duplicate email address
        [Fact]
        public void Create_DuplicateEmailadres_ThrowsException()
        {
            // Arrange
            var repository = new GebruikerRepository(_testConnectionString);
            var gebruiker = new Gebruiker
            {
                Naam = "Duplicate User",
                Emailadres = "existing@example.com",
                HashedWachtwoord = "hashed",
                Salt = "salt"
            };

            // Act & Assert
            Assert.Throws<SqlException>(() => repository.Create(gebruiker));
        }

        // BK-ERR-MISSING-NAAM
        // Error: Missing required field (Naam)
        [Fact]
        public void Create_MissingNaam_ThrowsException()
        {
            // Arrange
            var repository = new GebruikerRepository(_testConnectionString);
            var gebruiker = new Gebruiker
            {
                Naam = null,
                Emailadres = "test@example.com",
                HashedWachtwoord = "hashed",
                Salt = "salt"
            };

            // Act & Assert
            Assert.Throws<SqlException>(() => repository.Create(gebruiker));
        }

        // ==================== USE CASE TESTING ====================

        // BK-UC10-UPDATE
        // Test based on UC10: Updaten van Accountgegevens
        [Fact]
        public void Update_ExistingGebruiker_ReturnsTrue()
        {
            // Arrange
            var repository = new GebruikerRepository(_testConnectionString);
            var gebruiker = new Gebruiker
            {
                GebruikerID = 1,
                Naam = "Jan Pietersen",
                Emailadres = "jan.pietersen@example.com",
                HashedWachtwoord = "new_hashed_password",
                Salt = "new_salt",
                Telefoon = "0687654321",
                Autokenteken = "XY-987-ZW",
                Taal = "EN"
            };

            // Act
            var result = repository.Update(gebruiker);

            // Assert
            Assert.True(result);
        }

        // BK-UC17-LOGIN-FILTER
        // Test based on UC17: Inloggen - Finding user by name
        [Fact]
        public void GetGebruikers_ByNaam_ReturnsMatchingGebruikers()
        {
            // Arrange
            var repository = new GebruikerRepository(_testConnectionString);
            string testNaam = "Jan Jansen";

            // Act
            var result = repository.GetGebruikers(0, testNaam, "ALL", 0, false);

            // Assert
            Assert.NotNull(result);
            Assert.All(result, g => Assert.Equal(testNaam, g.Naam));
        }

        // ==================== BOUNDARY VALUE ANALYSIS ====================

        // BK-BVA-QUERY-ALL
        // Testing boundary when all filters are "ALL" (should return all users)
        [Fact]
        public void GetGebruikers_AllFiltersALL_ReturnsAllGebruikers()
        {
            // Arrange
            var repository = new GebruikerRepository(_testConnectionString);

            // Act
            var result = repository.GetGebruikers(0, "ALL", "ALL", 0, false);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Count > 0);
        }

        // BK-BVA-TAAL-SHORT
        // Testing boundary for Taal field (2 character language code)
        [Fact]
        public void Create_TaalTwoCharacters_ReturnsTrue()
        {
            // Arrange
            var repository = new GebruikerRepository(_testConnectionString);
            var gebruiker = new Gebruiker
            {
                Naam = "Test User",
                Emailadres = "test3@example.com",
                HashedWachtwoord = "hashed",
                Salt = "salt",
                Taal = "EN"
            };

            // Act
            var result = repository.Create(gebruiker);

            // Assert
            Assert.True(result);
        }

        // ==================== ERROR GUESSING ====================

        // BK-ERR-UPDATE-NONEXISTENT
        // Error: Updating non-existent user
        [Fact]
        public void Update_NonExistentGebruiker_ReturnsFalse()
        {
            // Arrange
            var repository = new GebruikerRepository(_testConnectionString);
            var gebruiker = new Gebruiker
            {
                GebruikerID = 999999,
                Naam = "Non-existent User",
                Emailadres = "nonexistent@example.com",
                HashedWachtwoord = "hashed",
                Salt = "salt"
            };

            // Act
            var result = repository.Update(gebruiker);

            // Assert
            Assert.False(result);
        }

        // BK-ERR-INVALID-EMAIL-FORMAT
        // Error: Invalid email format (constraint violation)
        [Fact]
        public void Create_InvalidEmailFormat_ThrowsException()
        {
            // Arrange
            var repository = new GebruikerRepository(_testConnectionString);
            var gebruiker = new Gebruiker
            {
                Naam = "Test User",
                Emailadres = "invalid-email-format",
                HashedWachtwoord = "hashed",
                Salt = "salt"
            };

            // Act & Assert
            Assert.Throws<SqlException>(() => repository.Create(gebruiker));
        }
    }
}
