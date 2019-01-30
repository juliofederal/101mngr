﻿using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Orleans;
using _101mngr.AuthorizationServer.Models;
using _101mngr.Contracts;
using _101mngr.Contracts.Models;
using _101mngr.WebApp.Models.Requests;
using _101mngr.WebApp.Services;

namespace _101mngr.WebApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly AuthorizationService _authorizationService;
        private readonly IClusterClient _clusterClient;
        private readonly ILogger<AccountController> _logger;

        public AccountController(AuthorizationService authorizationService, IClusterClient clusterClient, ILogger<AccountController> logger)
        {
            _authorizationService = authorizationService;
            _clusterClient = clusterClient;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetId(long accountId)
        {
            var playerGrain = _clusterClient.GetGrain<IPlayerGrain>(accountId);
            var result = await playerGrain.GetPlayer();
            return Ok(result);
        }

        [HttpGet("match-history")]
        public async Task<IActionResult> GetMatchHistory()
        {
            var accountId = long.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "sub")?.Value);
            var playerGrain = _clusterClient.GetGrain<IPlayerGrain>(accountId);
            var matchHistory = await playerGrain.GetMatchHistory();
            return Ok(matchHistory);
        }

        /// <summary>
        /// Register new user
        /// </summary>
        [ProducesResponseType(typeof(long), (int)HttpStatusCode.OK)]
        [AllowAnonymous]
        [HttpPost("register")] // todo: add email confirmation
        public async Task<IActionResult> Register([FromBody] RegisterInputModel inputModel)
        {
            try
            {
                var accountId =
                    await _authorizationService.Register(inputModel.UserName, inputModel.Email, inputModel.Password);
                var playerGrain = _clusterClient.GetGrain<IPlayerGrain>(accountId);
                await playerGrain.Create(new CreatePlayerDto
                {
                    UserName = inputModel.UserName,
                    Email = inputModel.Email
                });
                return Ok(new { Id = accountId });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Register error");
                throw;
            }
        }

        [HttpPut("profile")]
        public async Task<IActionResult> ProfileInfo([FromBody]ProfileInfoInputModel inputModel)
        {
            try
            {
                var accountId = long.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "sub")?.Value);
                var playerGrain = _clusterClient.GetGrain<IPlayerGrain>(accountId);
                await playerGrain.ProfileInfo(new ProfileInfoDto
                {
                    FirstName = inputModel.FirstName,
                    LastName = inputModel.LastName,
                    DateOfBirth = inputModel.DateOfBirth,
                    CountryCode = inputModel.CountryCode,
                    Weight = inputModel.Weight,
                    Height = inputModel.Height,
                    PlayerType = inputModel.PlayerType
                });
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "ProfileInfo error");
                throw;
            }

            // todo: enable country check
            bool IsCountryCodeValid(string countryCode) => CultureInfo
                .GetCultures(CultureTypes.SpecificCultures)
                .Select(culture => new RegionInfo(culture.Name))
                .Any(region => region.TwoLetterISORegionName == countryCode);
        }

        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginInputModel model)
        {
            var token = await _authorizationService.Login(model.Email, model.Password);
            return Ok(new { Token = $"Bearer {token}" });
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var accountId = long.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "sub")?.Value);
            var playerGrain = _clusterClient.GetGrain<IPlayerGrain>(accountId);
            var playerData = await playerGrain.GetPlayerInfo();
            return Ok(playerData);
            //var userInfo = await _authorizationService.GetUserInfo(accountId);
            //return Content(userInfo, "application/json");
        }

        [AllowAnonymous]
        [HttpPost("username/available")]
        public async Task<IActionResult> CheckUserName()
        {
            return Ok(new { IsAvailable = true});
        }

        /// <summary>
        /// Logouts user
        /// </summary>
        [HttpPost("logout")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var token = await HttpContext.GetTokenAsync("access_token");
                await _authorizationService.Logout(token);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}