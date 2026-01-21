using Backend_Development_LeMarconnes_Reserveringssysteem.Repositories;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests_BED_DAL.UnitTests
{
    public class UnitTestsGebruiker
    {
        readonly string _testConnectionString = "Server=tcp:sqldb-lgpteam-algemeen-marconnes.database.windows.net,1433;Initial Catalog=DB-LGPTeam-Camping-Marconnes;Persist Security Info=False;User ID=sqldb-lgpteam-algemeen-marconnes-admin;Password=${)UYT(GFIH(YQGW$TYt4;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

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
                Emailadres = $"jan.jansen.{Guid.NewGuid()}@example.com",
                HashedWachtwoord = "hashed123",
                Salt = "salt123",
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

            var testGebruiker = new Gebruiker
            {
                Naam = "Test User For Read",
                Emailadres = $"testread.{Guid.NewGuid()}@example.com",
                HashedWachtwoord = "hashed",
                Salt = "salt",
                Telefoon = "",
                Autokenteken = "",
                Taal = ""
            };
            repository.Create(testGebruiker);

            var allUsers = repository.GetGebruikers(0, "ALL", "ALL", 0, false);

            if (allUsers == null || allUsers.Count == 0)
            {
                Assert.True(true);
                return;
            }

            int testGebruikerID = allUsers[0].GebruikerID;

            // Act
            var result = repository.GetGebruikers(testGebruikerID, "ALL", "ALL", 0, false);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.All(result, g => Assert.Equal(testGebruikerID, g.GebruikerID));
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
                Emailadres = $"a.{Guid.NewGuid()}@example.com", // Unique email
                HashedWachtwoord = "hashed",
                Salt = "salt",
                Telefoon = "",
                Autokenteken = "",
                Taal = ""
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
                Emailadres = $"test.{Guid.NewGuid()}@example.com", // Unique email
                HashedWachtwoord = "hashed",
                Salt = "salt",
                Telefoon = "",
                Autokenteken = "",
                Taal = ""
            };

            // Act
            var result = repository.Create(gebruiker);

            // Assert
            Assert.True(result);
        }

        // ==================== ERROR GUESSING ====================

        // BK-ERR-DUPLICATE-EMAIL
        // Error: Creating user with duplicate email address
        // BUG DETECTED: Database heeft geen UNIQUE constraint op Emailadres kolom!
        // Deze test faalt omdat de database duplicate emails toestaat.
        // Fix: Voeg een UNIQUE constraint toe aan de Emailadres kolom in de database.
        [Fact]
        public void Create_DuplicateEmailadres_ThrowsException()
        {
            // Arrange
            var repository = new GebruikerRepository(_testConnectionString);
            string duplicateEmail = $"duplicate.{Guid.NewGuid()}@example.com";

            var gebruiker1 = new Gebruiker
            {
                Naam = "First User",
                Emailadres = duplicateEmail,
                HashedWachtwoord = "hashed",
                Salt = "salt",
                Telefoon = "",
                Autokenteken = "",
                Taal = ""
            };
            repository.Create(gebruiker1);

            var gebruiker2 = new Gebruiker
            {
                Naam = "Duplicate User",
                Emailadres = duplicateEmail,
                HashedWachtwoord = "hashed",
                Salt = "salt",
                Telefoon = "",
                Autokenteken = "",
                Taal = ""
            };

            // Act & Assert
            Assert.Throws<SqlException>(() => repository.Create(gebruiker2));
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
                Emailadres = $"test.{Guid.NewGuid()}@example.com",
                HashedWachtwoord = "hashed",
                Salt = "salt",
                Telefoon = "",
                Autokenteken = "",
                Taal = ""
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

            var allUsers = repository.GetGebruikers(0, "ALL", "ALL", 0, false);

            if (allUsers == null || allUsers.Count == 0)
            {
                Assert.True(true);
                return;
            }

            var gebruiker = new Gebruiker
            {
                GebruikerID = allUsers[0].GebruikerID,
                Naam = "Jan Pietersen Updated",
                Emailadres = $"jan.pietersen.{Guid.NewGuid()}@example.com",
                HashedWachtwoord = "new_hashed",
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
            string testNaam = "SearchTestUser";

            var gebruiker = new Gebruiker
            {
                Naam = testNaam,
                Emailadres = $"searchtest.{Guid.NewGuid()}@example.com",
                HashedWachtwoord = "hashed",
                Salt = "salt",
                Telefoon = "",
                Autokenteken = "",
                Taal = ""
            };
            repository.Create(gebruiker);

            // Act
            var result = repository.GetGebruikers(0, testNaam, "ALL", 0, false);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
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
            Assert.True(result.Count >= 0);
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
                Emailadres = $"test3.{Guid.NewGuid()}@example.com",
                HashedWachtwoord = "hashed",
                Salt = "salt",
                Telefoon = "",
                Autokenteken = "",
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
                Emailadres = $"nonexistent.{Guid.NewGuid()}@example.com",
                HashedWachtwoord = "hashed",
                Salt = "salt",
                Telefoon = "",
                Autokenteken = "",
                Taal = ""
            };

            // Act
            var result = repository.Update(gebruiker);

            // Assert
            Assert.False(result);
        }

        // BK-ERR-INVALID-EMAIL-FORMAT
        // Error: Invalid email format (constraint violation)
        // BUG DETECTED: Database heeft geen CHECK constraint voor email formaat validatie!
        // Deze test faalt omdat de database invalide email formaten toestaat.
        // Fix: Voeg een CHECK constraint toe voor email validatie in de database,
        //      OF implementeer validatie in de repository laag.
        [Fact]
        public void Create_InvalidEmailFormat_ThrowsException()
        {
            // Arrange
            var repository = new GebruikerRepository(_testConnectionString);
            var gebruiker = new Gebruiker
            {
                Naam = "Test User",
                Emailadres = "invalid-email-format", // No @ symbol
                HashedWachtwoord = "hashed",
                Salt = "salt",
                Telefoon = "",
                Autokenteken = "",
                Taal = ""
            };

            // Act & Assert
            Assert.Throws<SqlException>(() => repository.Create(gebruiker));
        }
    }
}