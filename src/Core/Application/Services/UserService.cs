namespace Identity.Auth.Core.Application.Services;

using Identity.Auth.Core.Domain.Application;
using Identity.Auth.Core.Domain.Dtos;
using Identity.Auth.Core.Domain.Entities;
using Identity.Auth.Core.Domain.Enums;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Role = Identity.Auth.Core.Domain.Constants.IdentityConstants.Role;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityUserDto> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var identityUser = await _userManager.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Include(x => x.RefreshTokens)
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

        return identityUser.Adapt<IdentityUserDto>();
    }

    public async Task<IdentityUserDto> GetUserByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (Guid.TryParse(id, out var userId))
        {
            var identityUser = await _userManager.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Include(x => x.AccessTokens)
            .Include(x => x.RefreshTokens)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

            return identityUser.Adapt<IdentityUserDto>();
        }

        return null;
    }

    public async Task<IdentityUserDto> RegisterUserAsync(RegisterUserDto userDto, CancellationToken cancellationToken = default)
    {
        var applicationUser = new ApplicationUser
        {
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            UserName = userDto.UserName,
            Email = userDto.Email,
            UserState = UserState.Active,
            CreatedAt = DateTime.Now,
        };

        var identityResult = await _userManager.CreateAsync(applicationUser, userDto.Password);
        //if (!identityResult.Succeeded)
        //    throw new RegisterIdentityUserException(string.Join(',', identityResult.Errors.Select(e => e.Description)));

        var roleResult = await _userManager.AddToRolesAsync(
            applicationUser,
            userDto.Roles ?? new List<string> { Role.User });

        //if (!roleResult.Succeeded)
        //    throw new RegisterIdentityUserException(string.Join(',', roleResult.Errors.Select(e => e.Description)));

        return new IdentityUserDto
        {
            Id = applicationUser.Id,
            Email = applicationUser.Email,
            UserName = applicationUser.UserName,
            FirstName = applicationUser.FirstName,
            LastName = applicationUser.LastName,
            Roles = userDto.Roles ?? new List<string> { Role.User },
            RefreshTokens = applicationUser?.RefreshTokens?.Select(x => x.Token),
            CreatedAt = DateTime.Now,
            UserState = UserState.Active
        };
    }

    public async Task UpdateUserStateAsync(string userId, UserState userState)
    {
        var user = await _userManager.FindByIdAsync(userId);

        var previousState = user.UserState;
        if (previousState == userState)
        {
            return;
        }

        //if (await _userManager.IsInRoleAsync(user, Data.IdentityConstants.Role.Admin))
        //{
        //    throw new UserStateCannotBeChangedException(request.State, request.UserId);
        //}

        user.UserState = userState;

        await _userManager.UpdateAsync(user);
    }
}
