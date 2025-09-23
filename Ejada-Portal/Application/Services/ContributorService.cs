using Application.DTOs;
using Application.Services.IServices;
using Domain.Entities;
using Infrastructure.Repository.IRepository;

namespace Application.Services
{
    public class ContributorService : IContributorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContributorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<ContributorDTO> GetAllContributors()
        {
            var contributors = _unitOfWork.Contributor.GetAll();
            return contributors.Select(MapToDTO).ToList();
        }

        public ContributorDTO GetContributorById(int id)
        {
            var contributor = _unitOfWork.Contributor.Get(c => c.Id == id);
            return contributor != null ? MapToDTO(contributor) : null;
        }

        public ContributorDTO MapToDTO(Contributor contributor)
        {
            return new ContributorDTO
            {
                Id = contributor.Id,
                Name = contributor.Name,
                Description = contributor.Description,
                Photo = contributor.Photo,
                FullDescription = contributor.FullDescription,
                Role = contributor.Role,
                Email = contributor.Email,
                LinkedInUrl = contributor.LinkedInUrl,
                GitHubUrl = contributor.GitHubUrl,
                TwitterUrl = contributor.TwitterUrl
            };
        }

        public Contributor MapToContributor(ContributorDTO contributorDTO)
        {
            return new Contributor
            {
                Id = contributorDTO.Id,
                Name = contributorDTO.Name,
                Description = contributorDTO.Description,
                Photo = contributorDTO.Photo,
                FullDescription = contributorDTO.FullDescription,
                Role = contributorDTO.Role,
                Email = contributorDTO.Email,
                LinkedInUrl = contributorDTO.LinkedInUrl,
                GitHubUrl = contributorDTO.GitHubUrl,
                TwitterUrl = contributorDTO.TwitterUrl
            };
        }

        public void CreateContributor(ContributorDTO contributorDTO)
        {
            var contributor = MapToContributor(contributorDTO);
            _unitOfWork.Contributor.Create(contributor);
            _unitOfWork.Save();
        }

        public void UpdateContributor(ContributorDTO contributorDTO)
        {
            var contributor = MapToContributor(contributorDTO);
            contributor.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Contributor.Update(contributor);
            _unitOfWork.Save();
        }

        public void DeleteContributor(int id)
        {
            var contributor = _unitOfWork.Contributor.Get(c => c.Id == id);
            if (contributor != null)
            {
                _unitOfWork.Contributor.Delete(contributor);
                _unitOfWork.Save();
            }
        }
    }
}