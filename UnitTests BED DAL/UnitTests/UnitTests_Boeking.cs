using Backend_Development_LeMarconnes_Reserveringssysteem.Repositories;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests_BED_DAL.UnitTests
{
    public class UnitTestsBoeking
    {
        readonly string _testConnectionString = "Server=localhost;Database=TestDB;Trusted_Connection=true;";

        // ==================== USE CASE TESTING ====================

        // BK-UC1-CREATE
        // Test based on UC1: Boeken van een faciliteit - Creating a new booking
        [Fact]
        public void Create_ValidBoeking_ReturnsTrue()
        {
            // Arrange
            var repository = new BoekingRepository(_testConnectionString);
            var boeking = new Boeking
            {
                GebruikerID = 1,
                AccommodatieID = 1,
                Datum = DateTime.Now,
                CheckInDatum = DateTime.Now.AddDays(7),
                CheckOutDatum = DateTime.Now.AddDays(10),
                AantalVolwassenen = 2,
                AantalJongeKinderen = 1,
                AantalOudereKinderen = 0,
                Opmerking = "Graag een kamer op de begane grond",
                Cancelled = false
            };

            // Act
            var result = repository.Create(boeking);

            // Assert
            Assert.True(result);
        }

        // BK-UC2-READ
        // Test based on UC2: Inzien van boeking - Retrieving booking by ID
        [Fact]
        public void GetBoekingen_ByBoekingID_ReturnsCorrectBoeking()
        {
            // Arrange
            var repository = new BoekingRepository(_testConnectionString);
            int testBoekingID = 1;

            // Act
            var result = repository.GetBoekingen(testBoekingID, 0, 0, 0, false, false, false);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(testBoekingID, result[0].BoekingID);
        }

        // ==================== BOUNDARY VALUE ANALYSIS ====================

        // BK-BVA-ADULTS-MIN
        // Testing minimum boundary for AantalVolwassenen (1 adult minimum)
        [Fact]
        public void Create_AantalVolwassenenOne_ReturnsTrue()
        {
            // Arrange
            var repository = new BoekingRepository(_testConnectionString);
            var boeking = new Boeking
            {
                GebruikerID = 1,
                AccommodatieID = 1,
                Datum = DateTime.Now,
                CheckInDatum = DateTime.Now.AddDays(1),
                CheckOutDatum = DateTime.Now.AddDays(2),
                AantalVolwassenen = 1,
                AantalJongeKinderen = 0,
                AantalOudereKinderen = 0,
                Cancelled = false
            };

            // Act
            var result = repository.Create(boeking);

            // Assert
            Assert.True(result);
        }

        // BK-BVA-ADULTS-MAX
        // Testing maximum boundary for AantalVolwassenen (byte max = 255)
        [Fact]
        public void Create_AantalVolwassenenMax_ReturnsTrue()
        {
            // Arrange
            var repository = new BoekingRepository(_testConnectionString);
            var boeking = new Boeking
            {
                GebruikerID = 1,
                AccommodatieID = 1,
                Datum = DateTime.Now,
                CheckInDatum = DateTime.Now.AddDays(1),
                CheckOutDatum = DateTime.Now.AddDays(2),
                AantalVolwassenen = 255,
                Cancelled = false
            };

            // Act
            var result = repository.Create(boeking);

            // Assert
            Assert.True(result);
        }

        // ==================== ERROR GUESSING ====================

        // BK-ERR-CHECKOUT-BEFORE-CHECKIN
        // Error: CheckOutDatum before CheckInDatum
        [Fact]
        public void Create_CheckOutBeforeCheckIn_ThrowsException()
        {
            // Arrange
            var repository = new BoekingRepository(_testConnectionString);
            var boeking = new Boeking
            {
                GebruikerID = 1,
                AccommodatieID = 1,
                Datum = DateTime.Now,
                CheckInDatum = DateTime.Now.AddDays(10),
                CheckOutDatum = DateTime.Now.AddDays(5),
                AantalVolwassenen = 2,
                Cancelled = false
            };

            // Act & Assert
            Assert.Throws<SqlException>(() => repository.Create(boeking));
        }

        // BK-ERR-INVALID-GEBRUIKER
        // Error: Non-existent GebruikerID (FK constraint violation)
        [Fact]
        public void Create_InvalidGebruikerID_ThrowsException()
        {
            // Arrange
            var repository = new BoekingRepository(_testConnectionString);
            var boeking = new Boeking
            {
                GebruikerID = 999999,
                AccommodatieID = 1,
                Datum = DateTime.Now,
                CheckInDatum = DateTime.Now.AddDays(1),
                CheckOutDatum = DateTime.Now.AddDays(2),
                AantalVolwassenen = 2,
                Cancelled = false
            };

            // Act & Assert
            Assert.Throws<SqlException>(() => repository.Create(boeking));
        }

        // ==================== USE CASE TESTING ====================

        // BK-UC3-UPDATE
        // Test based on UC3: Updaten van een Boeking - Updating booking details
        [Fact]
        public void Update_ExistingBoeking_ReturnsTrue()
        {
            // Arrange
            var repository = new BoekingRepository(_testConnectionString);
            var boeking = new Boeking
            {
                BoekingID = 1,
                GebruikerID = 1,
                AccommodatieID = 1,
                Datum = DateTime.Now,
                CheckInDatum = DateTime.Now.AddDays(14),
                CheckOutDatum = DateTime.Now.AddDays(17),
                AantalVolwassenen = 3,
                AantalJongeKinderen = 0,
                AantalOudereKinderen = 1,
                Opmerking = "Bijgewerkte boeking",
                Cancelled = false
            };

            // Act
            var result = repository.Update(boeking);

            // Assert
            Assert.True(result);
        }

        // BK-UC4-CANCEL
        // Test based on UC4: Annuleren van een boeking - Cancelling a booking
        [Fact]
        public void Update_CancelBoeking_SetsCancelledToTrue()
        {
            // Arrange
            var repository = new BoekingRepository(_testConnectionString);
            var boeking = new Boeking
            {
                BoekingID = 1,
                GebruikerID = 1,
                AccommodatieID = 1,
                Datum = DateTime.Now,
                CheckInDatum = DateTime.Now.AddDays(7),
                CheckOutDatum = DateTime.Now.AddDays(10),
                AantalVolwassenen = 2,
                Cancelled = true
            };

            // Act
            var result = repository.Update(boeking);

            // Assert
            Assert.True(result);
        }

        // ==================== BOUNDARY VALUE ANALYSIS ====================

        // BK-BVA-QUERY-ID-ZERO
        // Testing boundary when ID = 0 (should return all bookings)
        [Fact]
        public void GetBoekingen_IDZero_ReturnsAllBoekingen()
        {
            // Arrange
            var repository = new BoekingRepository(_testConnectionString);

            // Act
            var result = repository.GetBoekingen(0, 0, 0, 0, false, false, false);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Count > 0);
        }

        // BK-BVA-OPMERKING-NULL
        // Testing boundary for Opmerking (null is valid)
        [Fact]
        public void Create_NullOpmerking_ReturnsTrue()
        {
            // Arrange
            var repository = new BoekingRepository(_testConnectionString);
            var boeking = new Boeking
            {
                GebruikerID = 1,
                AccommodatieID = 1,
                Datum = DateTime.Now,
                CheckInDatum = DateTime.Now.AddDays(1),
                CheckOutDatum = DateTime.Now.AddDays(2),
                AantalVolwassenen = 2,
                Opmerking = null,
                Cancelled = false
            };

            // Act
            var result = repository.Create(boeking);

            // Assert
            Assert.True(result);
        }

        // ==================== ERROR GUESSING ====================

        // BK-ERR-INVALID-ACCOMMODATIE
        // Error: Non-existent AccommodatieID (FK constraint violation)
        [Fact]
        public void Create_InvalidAccommodatieID_ThrowsException()
        {
            // Arrange
            var repository = new BoekingRepository(_testConnectionString);
            var boeking = new Boeking
            {
                GebruikerID = 1,
                AccommodatieID = 999999,
                Datum = DateTime.Now,
                CheckInDatum = DateTime.Now.AddDays(1),
                CheckOutDatum = DateTime.Now.AddDays(2),
                AantalVolwassenen = 2,
                Cancelled = false
            };

            // Act & Assert
            Assert.Throws<SqlException>(() => repository.Create(boeking));
        }

        // BK-ERR-UPDATE-NONEXISTENT
        // Error: Updating non-existent booking
        [Fact]
        public void Update_NonExistentBoeking_ReturnsFalse()
        {
            // Arrange
            var repository = new BoekingRepository(_testConnectionString);
            var boeking = new Boeking
            {
                BoekingID = 999999,
                GebruikerID = 1,
                AccommodatieID = 1,
                Datum = DateTime.Now,
                CheckInDatum = DateTime.Now.AddDays(1),
                CheckOutDatum = DateTime.Now.AddDays(2),
                AantalVolwassenen = 2,
                Cancelled = false
            };

            // Act
            var result = repository.Update(boeking);

            // Assert
            Assert.False(result);
        }
    }
}
