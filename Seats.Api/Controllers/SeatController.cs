using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Seats.Application.Commands;
using Seats.Application.Queries;
using Seats.Application.Dtos;

namespace Seats.Api.Controllers
{
    [ApiController]
    [Route("seats")]
    public class SeatController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SeatController> _logger;

        public SeatController(IMediator mediator, ILogger<SeatController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("CreateSeat")]
        [Authorize]
        public async Task<IActionResult> CreateSeat([FromBody] CreateSeatDto createSeatDto)
        {
            try
            {
                var command = new CreateSeatCommand(createSeatDto);
                var seatId = await _mediator.Send(command);
                _logger.LogInformation("Asiento creado exitosamente con ID: {SeatId}", seatId);
                return Ok(new { SeatId = seatId, Message = "Asiento creado exitosamente." });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al crear asiento: {ErrorMessage}", e.Message);
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpPost("CreateSeats")]
        [Authorize]
        public async Task<IActionResult> CreateSeats([FromBody] CreateSeatsDto createSeatsDto)
        {
            try
            {
                var command = new CreateSeatsCommand(createSeatsDto);
                await _mediator.Send(command);
                _logger.LogInformation("Lote de asientos creado exitosamente.");
                return Ok(new { Message = "Lote de asientos creado exitosamente." });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al crear lote de asientos: {ErrorMessage}", e.Message);
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpDelete("DeleteSeat")]
        [Authorize]
        public async Task<IActionResult> DeleteSeat([FromBody] DeleteSeatDto deleteSeatDto)
        {
            try
            {
                var command = new DeleteSeatCommand(deleteSeatDto);
                await _mediator.Send(command);
                _logger.LogInformation("Asiento eliminado exitosamente.");
                return Ok(new { Message = "Asiento eliminado exitosamente." });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al eliminar asiento: {ErrorMessage}", e.Message);
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpDelete("DeleteSeats")]
        [Authorize]
        public async Task<IActionResult> DeleteSeats([FromBody] DeleteSeatsDto deleteSeatsDto)
        {
            try
            {
                var command = new DeleteSeatsCommand(deleteSeatsDto);
                await _mediator.Send(command);
                _logger.LogInformation("Lote de asientos eliminado exitosamente.");
                return Ok(new { Message = "Lote de asientos eliminado exitosamente." });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al eliminar lote de asientos: {ErrorMessage}", e.Message);
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpPut("UpdateSeatStatus")]
        [Authorize]
        public async Task<IActionResult> UpdateSeatStatus([FromBody] UpdateSeatStatusDto updateSeatStatusDto)
        {
            try
            {
                var command = new UpdateSeatStatusCommand(updateSeatStatusDto);
                await _mediator.Send(command);
                _logger.LogInformation("Estado del asiento actualizado exitosamente. ID: {SeatId}", updateSeatStatusDto.SeatId);
                return Ok(new { Message = "Estado del asiento actualizado exitosamente.", SeatId = updateSeatStatusDto.SeatId });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al actualizar estado del asiento: {ErrorMessage}", e.Message);
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpGet("GetSeatById/{id}/{eventId}/{functionId}/{zoneId}/{venueId}")]
        [Authorize]
        public async Task<IActionResult> GetSeatById(Guid id, Guid eventId, Guid functionId, Guid zoneId, Guid venueId)
        {
            try
            {
                var query = new GetSeatByIdQuery(id, eventId, functionId, zoneId, venueId);
                var seat = await _mediator.Send(query);
                if (seat == null)
                {
                    _logger.LogWarning(" Asiento con ID {SeatId} no encontrado.", id);
                    return NotFound(new { Message = $"Asiento con ID {id} no encontrado." });
                }
                return Ok(seat);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al obtener asiento por ID: {ErrorMessage}", e.Message);
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpGet("GetSeatsByZone/{zoneId}/{eventId}/{functionId}/{venueId}")]
        [Authorize]
        public async Task<IActionResult> GetSeatsByZone(Guid zoneId, Guid eventId, Guid functionId, Guid venueId)
        {
            try
            {
                var query = new GetSeatsByZoneQuery(zoneId, eventId, functionId, venueId);
                var seats = await _mediator.Send(query);
                if (seats == null || !seats.Any())
                {
                    _logger.LogWarning(" No se encontraron asientos para la zona {ZoneId} en el evento {EventId}.", zoneId, eventId);
                    return NotFound(new { Message = $"No se encontraron asientos para la zona {zoneId} en el evento {eventId}." });
                }
                return Ok(seats);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al obtener asientos por zona: {ErrorMessage}", e.Message);
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpGet("GetAllSeats")]
        [Authorize]
        public async Task<IActionResult> GetAllSeats()
        {
            try
            {
                var query = new GetAllSeatsQuery();
                var seats = await _mediator.Send(query);
                _logger.LogInformation("Numero total de asientos: {Count}", seats?.Count ?? 0);
                return Ok(seats);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al obtener todos los asientos: {ErrorMessage}", e.Message);
                return BadRequest(new { Message = e.Message });
            }
        }
    }
}
