using Application.DTOs;
using Application.Services.IServices;
using Domain.Entities;
using Infrastructure.Repository;
using Infrastructure.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class SessionRatingService : ISessionRatingService
    {
        private readonly ISessionRatingRepository _ratingRepo;
        private readonly ISessionRepository _sessionRepo;

        public SessionRatingService(ISessionRatingRepository ratingRepo, ISessionRepository sessionRepo)
        {
            _ratingRepo = ratingRepo ?? throw new ArgumentNullException(nameof(ratingRepo));
            _sessionRepo = sessionRepo ?? throw new ArgumentNullException(nameof(sessionRepo));
        }

        public async Task<SessionRatingDto> AddRatingAsync(SessionRatingDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var rating = new SessionRating
            {
                SessionId = dto.SessionId,
                RateSession = dto.RateSession,
                RatePresenter = dto.RatePresenter,
                Comments = dto.Comments,
                UserId = dto.UserId
            };

            await _ratingRepo.AddAsync(rating);

            dto.Id = rating.Id;

            var session = await _sessionRepo.GetByIdAsync(dto.SessionId);
            if (session != null)
            {
                dto.SessionName = session.SessionName;
                dto.PresenterName = session.PresenterName;
            }
            else
            {
                dto.SessionName = "Unknown Session";
                dto.PresenterName = "Unknown Presenter";
            }

            return dto;
        }
        public async Task<IEnumerable<SessionRatingDto>> GetRatingsBySessionAsync(int sessionId)
        {
            if (sessionId <= 0)
                throw new ArgumentException("Invalid session ID", nameof(sessionId));

            var ratings = await _ratingRepo.GetBySessionIdAsync(sessionId);
            return ratings.Select(r => new SessionRatingDto
            {
                Id = r.Id,
                SessionId = r.SessionId,
                SessionName = r.Session?.SessionName,
                PresenterName = r.Session?.PresenterName,
                RateSession = r.RateSession,
                RatePresenter = r.RatePresenter,
                Comments = r.Comments,
                UserId = r.UserId
            }).ToList();
        }


        public async Task<IEnumerable<SessionRatingDto>> GetRatingsByUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            var ratings = await _ratingRepo.GetByUserIdAsync(userId);
            return ratings.Select(r => new SessionRatingDto
            {
                Id = r.Id,
                SessionId = r.SessionId,
                SessionName = r.Session?.SessionName,
                PresenterName = r.Session?.PresenterName,
                RateSession = r.RateSession,
                RatePresenter = r.RatePresenter,
                Comments = r.Comments,
                UserId = r.UserId
            }).ToList();
        }

        public async Task<IEnumerable<SessionRatingSummaryDto>> GetSessionRatingsSummaryAsync()
        {
            var sessions = await _sessionRepo.GetAllAsync();
            var result = new List<SessionRatingSummaryDto>();

            foreach (var s in sessions)
            {
                var ratings = await _ratingRepo.GetBySessionIdAsync(s.Id);
                if (!ratings.Any()) continue;

                result.Add(new SessionRatingSummaryDto
                {
                    SessionId = s.Id,
                    SessionName = s.SessionName,
                    PresenterName = s.PresenterName,
                    AveragePresenterRate = ratings.Average(r => r.RatePresenter),
                    AverageSessionRate = ratings.Average(r => r.RateSession),
                    TotalRatings = ratings.Count()
                });
            }

            return result;
        }

    }
}
