using AutoMapper;
using Intranet.Core.Constants;
using Intranet.Core.DTOs.Activities;
using Intranet.Core.Interfaces.Services;
using Intranet.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intranet.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ActivitiesController : ControllerBase
    {
        private readonly IActivityService _activityService;
        private readonly IMapper _mapper;
        private readonly IAuthUser _authUser;

        public ActivitiesController(
            IActivityService activityService,
            IMapper mapper,
            IAuthUser authUser)
        {
            _activityService = activityService;
            _mapper = mapper;
            _authUser = authUser;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActivityForListing>>> GetActivitiesAsync(
            [FromQuery] ActivitiesRequestFilter filter)
        {
            var activities = await _activityService.GetActivitiesAsync(filter);

            Response.AddPaginationHeader(activities);

            var listing = _mapper.Map<List<ActivityForListing>>(activities);

            return listing;
        }
    }
}

