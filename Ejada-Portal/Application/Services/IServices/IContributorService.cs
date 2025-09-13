using Application.DTOs; 
using Domain.Entities;  
  
namespace Application.Services.IServices  
{  
    public interface IContributorService  
    {  
        List<ContributorDTO> GetAllContributors();  
        ContributorDTO GetContributorById(int id);  
        ContributorDTO MapToDTO(Contributor contributor);  
        Contributor MapToContributor(ContributorDTO contributorDTO);  
        void CreateContributor(ContributorDTO contributorDTO);  
        void UpdateContributor(ContributorDTO contributorDTO);  
        void DeleteContributor(int id);  
    }  
} 
