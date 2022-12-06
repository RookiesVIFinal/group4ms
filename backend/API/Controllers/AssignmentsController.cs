using API.Attributes;
using Application.Common.Models;
using Application.DTOs.Assignments.GetAssignment;
using Application.DTOs.Assignments.GetListAssignments;
using Application.Queries;
using Application.Queries.Assignments;
using Application.Services;
using Application.Services.Interfaces;
using Domain.Shared.Constants;
using Domain.Shared.Enums;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AssignmentsController : BaseController
{
    private readonly IAssignmentService _assignmentService;

    public AssignmentsController(IAssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
    }

    [Authorize(UserRole.Admin)]
    [HttpGet("{id}")]
    public async Task<ActionResult<Response<GetAssignmentResponse>>> GetById(Guid id)
    {
        if (CurrentUser == null)
        {
            return BadRequest(new Response(false, ErrorMessages.BadRequest));
        }

        var request = new GetAssignmentRequest
        {
            Id = id,
            Location = CurrentUser.Location
        };

        try
        {
            var response = await _assignmentService.GetAsync(request);

            if (!response.IsSuccess)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
        catch (Exception exception)
        {
            return HandleException(exception);
        }
    }

    [Authorize(UserRole.Admin)]
    [HttpGet]
    public async Task<ActionResult<Response>> GetList(
        [FromQuery] PagingQuery pagingQuery,
        [FromQuery] SortQuery sortQuery,
        [FromQuery] AssignmentFilter assignmentFilter,
        [FromQuery] SearchQuery searchQuery)
    {
        if (CurrentUser == null)
        {
            return BadRequest(new Response(false, ErrorMessages.BadRequest));
        }

        if (sortQuery.SortField == ModelField.None)
        {
            sortQuery.SortField = ModelField.AssetName;
        }

        var request = new GetListAssignmentsRequest(pagingQuery, sortQuery, searchQuery, assignmentFilter, CurrentUser.Location);

        try
        {
            var response = await _assignmentService.GetListAsync(request);

            if (!response.IsSuccess)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
        catch (Exception exception)
        {
            return HandleException(exception);
        }
    }
}