using Application.DTOs;
using Application.Services.IServices;
using Infrastructure.Repository.IRepository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _uow;

        public SessionService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public SessionDto MapToDto(Domain.Entities.Session e)
        {
            var dto = new SessionDto();
            dto.Id = e.Id;
            dto.OwnerName = e.OwnerName;
            dto.Name = e.Name;
            return dto;
        }

        public Domain.Entities.Session MapToEntity(SessionDto d)
        {
            var entity = new Domain.Entities.Session();
            entity.Id = d.Id;
            entity.OwnerName = d.OwnerName;
            entity.Name = d.Name;
            return entity;
        }

        public Task AddAsync(SessionDto dto)
        {
            var entity = MapToEntity(dto);
            _uow.Session.Create(entity);
            _uow.Save();
            return Task.CompletedTask;
        }

        public Task<IEnumerable<SessionDto>> GetAllAsync()
        {
            var list = _uow.Session .GetAll().Select(MapToDto).ToList();

            return Task.FromResult<IEnumerable<SessionDto>>(list);
        }
    }
}
