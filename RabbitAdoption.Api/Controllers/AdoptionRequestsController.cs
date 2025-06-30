using MediatR;
using Microsoft.AspNetCore.Mvc;
using RabbitAdoption.Api.Models;
using RabbitAdoption.Application.AdoptionRequests.Commands;
using RabbitAdoption.Application.AdoptionRequests.Queries;
using RabbitAdoption.Infrastructure.Messaging; // pentru RabbitMqPublishException

namespace RabbitAdoption.Api.Controllers
{
    [ApiController]
    public class AdoptionRequestsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdoptionRequestsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("api/adoptions")]
        public async Task<IActionResult> SubmitAdoptionRequest([FromBody] SubmitAdoptionRequestDto dto)
        {
            var command = new SubmitAdoptionRequestCommand(
                dto.AdopterName,
                dto.Contact,
                dto.Size,
                dto.Color,
                dto.Age,
                dto.Priority,
                dto.YearsRabbitExperience
            );
            try
            {
                var id = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetAdoptionRequestStatus), new { id }, null);
            }
            catch (RabbitMqPublishException ex)
            {
                return Accepted(new { message = "Din pacate cererea dumneavoastra nu poate fi momentan procesata. Va rugam reveniti mai tarziu." });
            }
        }

        [HttpGet]
        [Route("api/adoptions/{id}")]
        public async Task<IActionResult> GetAdoptionRequestStatus([FromRoute] Guid id)
        {
            var status = await _mediator.Send(new GetAdoptionRequestStatusQuery(id));
            if (status == null)
                return NotFound();
            return Ok(status);
        }
    }
}
