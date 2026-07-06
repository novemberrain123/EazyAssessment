using EazyAssessment.Server.Data;
using EazyAssessment.Server.DTOs;
using EazyAssessment.Server.Models;
using EazyAssessment.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EazyAssessment.Server.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BlobStorageService _blobStorageService;

        public UsersController(ApplicationDbContext context, BlobStorageService blobStorageService)
        {
            _context = context;
            _blobStorageService = blobStorageService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized();

            var user = await _context.Users.FindAsync(userId.Value);

            if (user == null)
                return NotFound("User not found.");

            return Ok(new UserProfile
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ProfilePictureUrl = user.ProfilePictureUrl
            });
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized();

            var user = await _context.Users.FindAsync(userId.Value);

            if (user == null)
                return NotFound("User not found.");

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new UserProfile
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ProfilePictureUrl = user.ProfilePictureUrl
            });
        }

        [HttpPost("profile-picture")]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file)
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized();

            var user = await _context.Users.FindAsync(userId.Value);

            if (user == null)
                return NotFound("User not found.");

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (file.Length > 2 * 1024 * 1024)
                return BadRequest("File size cannot exceed 2 MB.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return BadRequest("Only JPG and PNG files are allowed.");

            var newImageUrl = await _blobStorageService.UploadAsync(file);

            var oldImageUrl = user.ProfilePictureUrl;

            user.ProfilePictureUrl = newImageUrl;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Delete old image after DB update succeeds
            if (!string.IsNullOrWhiteSpace(oldImageUrl))
            {
                await _blobStorageService.DeleteAsync(oldImageUrl);
            }

            return Ok(new
            {
                profilePictureUrl = newImageUrl
            });
        }

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userIdClaim, out var userId))
                return userId;

            return null;
        }
    }
}
