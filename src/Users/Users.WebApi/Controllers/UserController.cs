﻿
using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Claims;
using Users.Application.Contracts.DTOs;
using Users.Application.UseCases.Commands;
using Users.Application.UseCases.Queries;
using Users.Application.Validators;
using Users.Domain.Entities;
using static MassTransit.ValidationResultExtensions;

namespace Users.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IMediator mediator;

        public readonly Serilog.ILogger logger;
        public UserController(IMediator mediator, Serilog.ILogger logger)
        {
            this.mediator = mediator;
            this.logger = logger;
        }

        [HttpGet("GetUsersProfile")]
        public async Task<ActionResult<UserProfileDTO>> GetUser()
        {
            logger.Information("GetUser method called.");

            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                logger.Warning("GetUser called but userId is null or empty.");
                return NotFound("User ID not found.");
            }

            var result = await mediator.Send(new GetUserProfileQuery(userId));

            if (result == null)
            {
                logger.Warning("User with ID {UserId} not found.", userId);
                return NotFound("There is no information for the given user ID.");
            }

            logger.Information("User with ID {UserId} retrieved successfully.", userId);
            return Ok(result);


        }

        [HttpPost("ChangeUserSettings")]
        public async Task<ActionResult> ChangeUserSettings([FromBody] ChangeProfileInformationDTO model)
        {
            var validator = new ChangeProfileInformationDTOValidator();
            var validationResult = validator.Validate(model);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new { error = e.ErrorMessage }));
            }
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrWhiteSpace(userId))
                {
                    logger.Information("ChangeUserSettings was called but no user ID was found in the claims.");
                    return Unauthorized("User ID not found.");
                }

                logger.Information("Starting ChangeUserSettings for user {UserId}.", userId);

                model.Id = userId;
                await mediator.Send(new ChangeUserInformationCommand(model));

                logger.Information("Successfully changed settings for user {UserId}.", userId);
                return Ok();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error changing settings for user {UserId}.", model.Id);
                return StatusCode(500, "An error occurred while changing user settings.");
            }

        }

        [HttpPost("UploadProfilePhoto")]
        public async Task<ActionResult<UserProfileDTO>> UploadProfilePhoto([FromBody] ProfilePhotoDTO model)
        {
          
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                logger.Information("UploadProfilePhoto called but user ID is missing.");
                return Unauthorized("User ID is required.");
            }

            try
            {
                logger.Information("Attempting to upload profile photo for user {UserId}.", userId);

                model.Id = userId;
                await mediator.Send(new ChangeUserAvatarCommand(model));

                logger.Information("Profile photo updated successfully for user {UserId}. Fetching updated user profile.", userId);

                var result = await mediator.Send(new GetUserProfileQuery(model.Id));

                if (result == null)
                {
                    logger.Warning("Failed to fetch updated profile for user {UserId} after uploading photo.", userId);
                    return NotFound("User profile not found.");
                }

                logger.Information("Successfully retrieved updated profile for user {UserId} after photo upload.", userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while uploading profile photo for user {UserId}.", userId);
                return BadRequest("Something went wrong.");
            }
        }

        [HttpPost("GetSomeonesProfile")]
        public async Task<ActionResult<GiveSmbProfileDTO>> GetSomeonesProfile([FromBody] SomeonesProfileDTO request)
        {
            var validator = new SomeonesProfileDTOValidator();
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new { error = e.ErrorMessage }));
            }

            try
            {

                logger.Information("Fetching user data for ProfileId {ProfileId}.", request.ProfileId);


                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    logger.Information("UploadProfilePhoto called but user ID is missing.");
                    return Unauthorized("User ID is required.");
                }


                var result = await mediator.Send(new GetSmbProfileQuery(request.ProfileId, userId));

                if (result == null)
                {
                    logger.Warning("No information found for ProfileId {ProfileId}.", request.ProfileId);
                    return Ok("There is no information");
                }

                logger.Information("Successfully retrieved user data for ProfileId {ProfileId}.", request.ProfileId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while fetching user data for ProfileId {ProfileId}.", request.ProfileId);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost("Follow")]
        public async Task<ActionResult<UserProfileDTO>> Follow([FromBody] SomeonesProfileDTO request)
        {
            var validator = new SomeonesProfileDTOValidator();
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new { error = e.ErrorMessage }));
            }

            try
            {

                logger.Information("Follow/unfollow {ProfileId}.", request.ProfileId);

                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    logger.Information("Follow called but user ID is missing.");
                    return Unauthorized("User ID is required.");
                }

                var result = await mediator.Send(new CreateFollowingCommand(userId, request.ProfileId));

                if (result == null)
                {
                    logger.Warning("No information found for ProfileId {ProfileId}.", request.ProfileId);
                    return Ok("There is no information");
                }

                logger.Information("Successfully started following {ProfileId}.", request.ProfileId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while trying to follow {ProfileId}.", request.ProfileId);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost("GetSearchedUsers")]
        public async Task<ActionResult<UserProfileDTO>> GetSearchedUsers([FromBody] GetUsersByLettersDTO request)
        {
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    logger.Information("UploadProfilePhoto called but user ID is missing.");
                    return Unauthorized("User ID is required.");
                }

                var result = await mediator.Send(new GetUsersByLettersQuery(request.SearchingField, userId));

                if (result == null)
                {
                    logger.Warning("Nothing found while GetSearchedUsers.");
                    return Ok(null);
                }

                logger.Information("Successfully returned GetSearchedUsers.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while trying to GetSearchedUsers.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("GetUserFriends")]
        public async Task<ActionResult<UserProfileDTO>> GetUserFriends()
        {
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    logger.Information("GetUserFriends called but user ID is missing.");
                    return Unauthorized("User ID is required.");
                }

                var result = await mediator.Send(new GetUserFriendsQuery(userId));

                if (result == null)
                {
                    logger.Warning("Nothing found while GetUserFriends.");
                    return Ok(null);
                }

                logger.Information("Successfully returned GetUserFriends.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while trying to GetUserFriends.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }




        [HttpPost("CreateUserPoint")]
        public async Task<ActionResult> CreateUserPoint([FromBody] CreateMapPointDTO request)
        {
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    logger.Information("CreateUserPoint called but user ID is missing.");
                    return Unauthorized("User ID is required.");
                }

                await mediator.Send(new CreateMapPointCommand(request, userId));
                return Ok();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while trying to GetSearchedUsers.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("GetUserMapPoints")]
        public async Task<ActionResult<List<GiveUserMapPointsDTO>>> GetUserMapPoints()
        {
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    logger.Information("GetUserMapPoints called but user ID is missing.");
                    return Unauthorized("User ID is required.");
                }

                var result = await mediator.Send(new GetUserMapPointQuery(userId));

                if (result == null)
                {
                    logger.Warning("Nothing found while GetUserMapPoints.");
                    return Ok(null);
                }

                logger.Information("Successfully returned GetUserMapPoints.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while trying to GetUserMapPoints.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}