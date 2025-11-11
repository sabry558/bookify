using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Bookify.DTOs.Reservations;
using Bookify.Services.Reservations;
using Bookify.Models;

namespace Bookify.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _service;

        public ReservationController(IReservationService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize] // user must be logged in
        public async Task<IActionResult> Create([FromBody] ReservationCreateDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _service.CreateAsync(userId, dto);
            if (result == null) return BadRequest("Invalid reservation data.");
            return Ok(result);
        }

        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> GetMyReservations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var reservations = await _service.GetUserReservationsAsync(userId);
            return Ok(reservations);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] ReservationStatus status)
        {
            var success = await _service.UpdateStatusAsync(id, status);
            if (!success) return NotFound();
            return Ok();
        }
    }
}