using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Bookify.DTOs.Reservations;
using Bookify.Models;

namespace Bookify.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReservationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [Authorize] // user must be logged in
        public async Task<IActionResult> CreateReservation([FromBody] ReservationCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var isAvailable = await _unitOfWork.Reservations.IsRoomAvailableAsync(dto.RoomId, dto.CheckIn, dto.CheckOut);
            if (!isAvailable)
                return BadRequest("The selected room is not available for the chosen dates.");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get current user id
            var reservation = new Reservation
            {
                RoomId = dto.RoomId,
                CheckIn = dto.CheckIn,
                CheckOut = dto.CheckOut,
                Status = ReservationStatus.Confirmed,
                UserId = userId 
            };
            await _unitOfWork.Reservations.AddAsync(reservation);
            await _unitOfWork.SaveAsync();
            return Ok(new { message = "Reservation created successfully", reservation });
        }

        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> GetMyReservations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var reservations = await _unitOfWork.Reservations.GetReservationsByUserAsync(userId);
            return Ok(reservations);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _unitOfWork.Reservations.GetAllAsync());
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableRooms([FromQuery] DateTime checkin, [FromQuery] DateTime checkout)
        {

            var availableRooms = await _unitOfWork.Reservations.GetAvailableRoomsAsync(checkin, checkout);

            if (!availableRooms.Any())
                return NotFound("No rooms available for the selected dates.");

            return Ok(availableRooms);
        }
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateReservationStatusDTO dto)
        {
            var success = await _unitOfWork.Reservations.UpdateStatusAsync(id, dto.Status);
            if (!success) return NotFound();
            return Ok();
        }
    }
}