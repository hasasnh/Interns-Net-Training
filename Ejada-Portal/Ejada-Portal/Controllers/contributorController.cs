using Application.DTOs;  
using Application.ServiceManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;  
  
namespace Ejada_Portal.Controllers  
{
    [Authorize]
    public class ContributorController : Controller  
    {  
        private readonly IServiceManager _serviceManager;  
  
        public ContributorController(IServiceManager serviceManager)  
        {  
            _serviceManager = serviceManager;  
        }  
  
        public IActionResult Index()  
        {  
            var contributors = _serviceManager.ContributorService.GetAllContributors();  
            return View(contributors);  
        }  
  
        public IActionResult Details(int id)  
        {  
            var contributor = _serviceManager.ContributorService.GetContributorById(id);  
            if (contributor == null)  
            {  
                return NotFound();  
            }  
            return View(contributor);  
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContributorDTO contributorDTO, IFormFile photoFile)
        {
            if (ModelState.IsValid)
            {
                // Handle file upload
                if (photoFile != null && photoFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "contributors");
                    
                    // Create directory if it doesn't exist
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Generate unique filename
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photoFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    // Save file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await photoFile.CopyToAsync(stream);
                    }

                    // Update photo path
                    contributorDTO.Photo = $"/uploads/contributors/{fileName}";
                }

                _serviceManager.ContributorService.CreateContributor(contributorDTO);
                TempData["Success"] = "Contributor created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(contributorDTO);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var contributor = _serviceManager.ContributorService.GetContributorById(id);
            if (contributor == null)
            {
                return NotFound();
            }
            return View(contributor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ContributorDTO contributorDTO, IFormFile photoFile)
        {
            if (ModelState.IsValid)
            {
                // Handle file upload
                if (photoFile != null && photoFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "contributors");
                    
                    // Create directory if it doesn't exist
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Generate unique filename
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photoFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    // Save file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await photoFile.CopyToAsync(stream);
                    }

                    // Update photo path
                    contributorDTO.Photo = $"/uploads/contributors/{fileName}";
                }

                _serviceManager.ContributorService.UpdateContributor(contributorDTO);
                TempData["Success"] = "Contributor updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(contributorDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                _serviceManager.ContributorService.DeleteContributor(id);
                TempData["Success"] = "Contributor deleted successfully!";
                return Json(new { success = true, message = "Contributor deleted successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting contributor: " + ex.Message });
            }
        }
    }  
} 
