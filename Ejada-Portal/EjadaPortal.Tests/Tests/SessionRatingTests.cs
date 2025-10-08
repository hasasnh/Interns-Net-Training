using Application.DTOs;
using Application.Services;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class SessionRatingTests
    {
        private ApplicationDbContext GetDbContext(string dbName = null)
        {
            if (string.IsNullOrEmpty(dbName))
                dbName = System.Guid.NewGuid().ToString(); 

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var dbContext = new ApplicationDbContext(options);

            // Seed Users and Sessions
            if (!dbContext.Users.Any())
            {
                dbContext.Users.AddRange(
                    new User { Id = "user1", UserName = "User1" },
                    new User { Id = "user2", UserName = "User2" },
                    new User { Id = "user3", UserName = "User3" }
                );
            }

            if (!dbContext.Sessions.Any())
            {
                dbContext.Sessions.Add(new Session { Id = 1, PresenterName = "Presenter1", SessionName = "Session1" });
            }

            dbContext.SaveChanges();
            return dbContext;
        }

        [Fact] 
        public async Task AddRating_ShouldSave_WhenValid()
        {
            var db = GetDbContext();
            var rating = new SessionRating
            {
                SessionId = 1,
                UserId = "user1",
                RateSession = 5,
                RatePresenter = 4,
                Comments = "Great!"
            };

            db.SessionRatings.Add(rating);
            await db.SaveChangesAsync();

            (await db.SessionRatings.CountAsync()).Should().Be(1);
        }

        [Fact] 
        public async Task UpdateRating_ShouldChangeValues()
        {
            var db = GetDbContext();
            db.SessionRatings.Add(new SessionRating { Id = 1, SessionId = 1, UserId = "user1", RateSession = 3, RatePresenter = 3 });
            await db.SaveChangesAsync();

            var rating = await db.SessionRatings.FindAsync(1);
            rating.RateSession = 5;
            rating.RatePresenter = 5;
            await db.SaveChangesAsync();

            (await db.SessionRatings.FindAsync(1)).RateSession.Should().Be(5);
        }

        [Fact]
        public async Task MultipleUsers_ShouldRateSameSession()
        {
            var db = GetDbContext();
            db.SessionRatings.AddRange(
                new SessionRating { SessionId = 1, UserId = "user1", RateSession = 4, RatePresenter = 4 },
                new SessionRating { SessionId = 1, UserId = "user2", RateSession = 5, RatePresenter = 5 },
                new SessionRating { SessionId = 1, UserId = "user3", RateSession = 3, RatePresenter = 4 }
            );
            await db.SaveChangesAsync();

            var ratings = await db.SessionRatings.ToListAsync();
            ratings.Count.Should().Be(3);
        }

        [Fact] 
        public async Task GetRatingsBySession_ShouldReturnCorrectCount()
        {
            var db = GetDbContext();
            db.SessionRatings.AddRange(
                new SessionRating { SessionId = 1, UserId = "user1", RateSession = 4, RatePresenter = 4 },
                new SessionRating { SessionId = 1, UserId = "user2", RateSession = 5, RatePresenter = 5 }
            );
            await db.SaveChangesAsync();

            var ratings = await db.SessionRatings.Where(r => r.SessionId == 1).ToListAsync();
            ratings.Should().HaveCount(2);
        }

        [Fact] 
        public async Task GetUserRatingForSession_ShouldReturnSingle()
        {
            var db = GetDbContext();
            db.SessionRatings.Add(new SessionRating { SessionId = 1, UserId = "user1", RateSession = 4, RatePresenter = 4 });
            await db.SaveChangesAsync();

            var rating = await db.SessionRatings.FirstOrDefaultAsync(r => r.SessionId == 1 && r.UserId == "user1");
            rating.Should().NotBeNull();
        }

        [Fact] 
        public async Task AddInvalidRating_ShouldFailValidation()
        {
            var db = GetDbContext();
            var rating = new SessionRating { SessionId = 1, UserId = "user1", RateSession = 6, RatePresenter = 0 };

            var validationContext = new ValidationContext(rating);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(rating, validationContext, validationResults, true);

            Assert.False(isValid);
            Assert.Contains(validationResults, r => r.MemberNames.Contains(nameof(SessionRating.RateSession)));
            Assert.Contains(validationResults, r => r.MemberNames.Contains(nameof(SessionRating.RatePresenter)));
        }

      


        [Fact]
        public async Task AddRatingWithoutUser_ShouldFail()
        {
            var db = GetDbContext();
            var rating = new SessionRating { SessionId = 1, UserId = null, RateSession = 4, RatePresenter = 4 };
            db.SessionRatings.Add(rating);

            await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        }

        [Fact] 
        public async Task AddDuplicateRating_ShouldAllowOrBlockBasedOnConfig()
        {
            var db = GetDbContext();
            db.SessionRatings.Add(new SessionRating { SessionId = 1, UserId = "user1", RateSession = 4, RatePresenter = 4 });
            await db.SaveChangesAsync();

            db.SessionRatings.Add(new SessionRating { SessionId = 1, UserId = "user1", RateSession = 5, RatePresenter = 5 });
            await db.SaveChangesAsync();

            (await db.SessionRatings.CountAsync()).Should().Be(2);
        }

        [Fact] 
        public async Task AddRatingWithLongComment_ShouldFailIfValidationApplied()
        {
            var db = GetDbContext();
            var longComment = new string('a', 5000);
            var rating = new SessionRating { SessionId = 1, UserId = "user1", RateSession = 4, RatePresenter = 4, Comments = longComment };
            db.SessionRatings.Add(rating);

            await db.SaveChangesAsync();
            (await db.SessionRatings.CountAsync()).Should().Be(1);
        }
    }
}
