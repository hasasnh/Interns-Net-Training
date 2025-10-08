using Application.DTOs;
using Application.Services.IServices;
using Domain.Entities;
using Infrastructure.Repository.IRepository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SessionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SessionDto>> GetAllSessionsAsync()
        {
            var sessions = await _unitOfWork.Session.GetAllAsync();
            return sessions.Select(s => new SessionDto
            {
                Id = s.Id,
                PresenterName = s.PresenterName,
                SessionName = s.SessionName
            });
        }

        public async Task<SessionDto> GetSessionByIdAsync(int id)
        {
            var s = await _unitOfWork.Session.GetByIdAsync(id);
            if (s == null) return null;

            return new SessionDto
            {
                Id = s.Id,
                PresenterName = s.PresenterName,
                SessionName = s.SessionName
            };
        }

        public async Task<SessionDto> CreateSessionAsync(SessionDto dto)
        {
            var s = new Session
            {
                PresenterName = dto.PresenterName,
                SessionName = dto.SessionName
            };
            await _unitOfWork.Session.AddAsync(s);
            await _unitOfWork.SaveAsync();
            dto.Id = s.Id;
            return dto;
        }

        public async Task<SessionDto> UpdateSessionAsync(SessionDto dto)
        {
            var s = await _unitOfWork.Session.GetByIdAsync(dto.Id);
            if (s == null) return null;

            s.PresenterName = dto.PresenterName;
            s.SessionName = dto.SessionName;

            await _unitOfWork.Session.UpdateAsync(s);
            await _unitOfWork.SaveAsync();
            return dto;
        }

        public async Task<bool> DeleteSessionAsync(int id)
        {
            var s = await _unitOfWork.Session.GetByIdAsync(id);
            if (s == null) return false;

            await _unitOfWork.Session.DeleteAsync(id);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
