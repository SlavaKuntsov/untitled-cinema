using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MovieService.API.Controllers.Http;

[ApiController]
[Route("[controller]")]
public class MovieController : ControllerBase
{
	private readonly IMediator _mediator;
	private readonly IMapper _mapper;

	public MovieController(IMediator mediator, IMapper mapper)
	{
		_mediator = mediator;
		_mapper = mapper;
	}
}