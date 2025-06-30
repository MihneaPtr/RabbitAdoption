using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RabbitAdoption.Api.Models;
using RabbitAdoption.Application.Rabbits.Commands;
using RabbitAdoption.Application.Rabbits.Queries;
using RabbitAdoption.Application.Common.Interfaces;

namespace RabbitAdoption.Api.Controllers
{
    [ApiController]
    [Route("api/rabbits")]
    public class RabbitsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRabbitRepository _rabbitRepository;

        public RabbitsController(IMediator mediator, IRabbitRepository rabbitRepository)
        {
            _mediator = mediator;
            _rabbitRepository = rabbitRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddRabbit([FromBody] RabbitDto dto)
        {
            var id = await _mediator.Send(new AddRabbitCommand(dto.Size, dto.Color, dto.Age));
            return CreatedAtAction(nameof(GetRabbit), new { id }, null);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRabbit([FromRoute] Guid id)
        {
            var rabbit = await _mediator.Send(new GetRabbitQuery(id));
            if (rabbit == null)
                return NotFound();
            return Ok(rabbit);
        }

        [HttpGet("adopted-per-day")]
        public async Task<IActionResult> GetRabbitsAdoptedPerDay([FromQuery] int days = 7)
        {
            var stats = await _rabbitRepository.GetRabbitsAdoptedPerDayAsync(days);
            return Ok(stats);
        }

        [HttpGet("adopted-vs-unadopted")]
        public async Task<IActionResult> GetAdoptedVsUnadopted()
        {
            var totalAdopted = await _rabbitRepository.CountAsync(r => r.IsAdopted);
            var totalUnadopted = await _rabbitRepository.CountAsync(r => !r.IsAdopted);
            return Ok(new { Adopted = totalAdopted, Unadopted = totalUnadopted });
        }
    }
}
