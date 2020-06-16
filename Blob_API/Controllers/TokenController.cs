using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using Blob_API.AuthModel;
using Blob_API.RessourceModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;

namespace Blob_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IOptions<IdentityOptions> _identityOptions;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<UserRole> _roleManager; // opt.

        public TokenController(
            IOptions<IdentityOptions> identityOptions,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            RoleManager<UserRole> roleManager)
        {
            _identityOptions = identityOptions;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("~/api/token"), Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> TokenExchange()
        {
            var req = HttpContext.GetOpenIdConnectRequest();

            if (req == null)
            {
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.InvalidRequest,
                    ErrorDescription = "No payload."
                });
            }

            // Check for correct GrantType = Password flow.
            if (!req.IsPasswordGrantType())
            {
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.UnsupportedGrantType,
                    ErrorDescription = "The specified grant type is not supported."
                });
            }

            // Check if the user exists by its username.
            var user = await _userManager.FindByNameAsync(req.Username);
            if (user == null)
            {
                // TODO: Should it only return error without specifing a type for more security???
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.InvalidGrant,
                    ErrorDescription = "The username or password is invalid."
                });
            }

            // Ensure the user is allowed to sign in.
            if (!await _signInManager.CanSignInAsync(user))
            {
                // TODO: Should it only return error without specifing a type for more security???
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.InvalidGrant,
                    ErrorDescription = "The specified user is not allowed to sign in."
                });
            }

            // Ensure the user is not already locked out.
            if (_userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(user))
            {
                // TODO: Should it only return error without specifing a type for more security???
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.InvalidGrant,
                    ErrorDescription = "The username or password is invalid."
                });
            }

            // Ensure the password is valid.
            if (!await _userManager.CheckPasswordAsync(user, req.Password))
            {
                if (_userManager.SupportsUserLockout)
                {
                    await _userManager.AccessFailedAsync(user);
                }

                // TODO: Should it only return error without specifing a type for more security???
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.InvalidGrant,
                    ErrorDescription = "The username or password is invalid."
                });
            }

            // Reset the lockout count.
            if (_userManager.SupportsUserLockout)
            {
                await _userManager.ResetAccessFailedCountAsync(user);
            }

            // Lock up the user´s roles (if any).
            var roles = new string[0];
            if (_userManager.SupportsUserRole)
            {
                roles = (await _userManager.GetRolesAsync(user)).ToArray();
            }

            // Create a new authentication ticket with the user identity.
            var ticket = await CreateTicketAsync(user, roles);
            

            return SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);
        }

        private async Task<AuthenticationTicket> CreateTicketAsync(User user, string[] roles)
        {
            var req = HttpContext.GetOpenIdConnectRequest();

            // principal = authenticated user.
            var principal = await _signInManager.CreateUserPrincipalAsync(user);

            AddRolesToPrincipal(principal, roles);

            var ticket = new AuthenticationTicket(principal, new AuthenticationProperties(), OpenIdConnectServerDefaults.AuthenticationScheme);

            // Token lifetime to 12 hours
            ticket.SetAccessTokenLifetime(TimeSpan.FromHours(12));
            ticket.SetAuthorizationCodeLifetime(TimeSpan.FromHours(12));
            ticket.SetIdentityTokenLifetime(TimeSpan.FromHours(12));

            ticket.SetScopes(OpenIddictConstants.Scopes.Roles);

            // Explicitly specify which claims should be included in the access token
            foreach (var claim in ticket.Principal.Claims)
            {
                // Never include the security stamp (it's a secret value)
                if (claim.Type == _identityOptions.Value.ClaimsIdentity.SecurityStampClaimType) continue;

                // TODO: If there are any other private/secret claims on the user that should
                // not be exposed publicly, handle them here!
                // The token is encoded but not encrypted, so it is effectively plaintext.

                claim.SetDestinations(OpenIdConnectConstants.Destinations.AccessToken);
            }

            return ticket;
        }

        private static void AddRolesToPrincipal(ClaimsPrincipal principal, string[] roles)
        {
            var identity = principal.Identity as ClaimsIdentity;

            var alreadyHasRolesClaim = identity.Claims.Any(c => c.Type == "role");
            if (!alreadyHasRolesClaim && roles.Any())
            {
                identity.AddClaims(roles.Select(r => new Claim("role", r)));
            }

            var newPrincipal = new ClaimsPrincipal(identity);
        }
    }
}