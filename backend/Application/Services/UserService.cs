using Application.Common.Models;
using Application.DTOs.Users;
using Application.DTOs.Users.Authentication;
using Application.DTOs.Users.ChangePassword;
using Application.DTOs.Users.GetListUsers;
using Application.DTOs.Users.GetUser;
using Application.Helpers;
using Application.DTOs.Users.CreateUser;
using Application.Services.Interfaces;
using Domain.Entities.Users;
using Domain.Shared.Constants;
using Domain.Shared.Enums;
using Domain.Shared.Helpers;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text.RegularExpressions;
using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace Application.Services;

public class UserService : BaseService, IUserService
{

    public UserService(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }

    public async Task<Response<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest requestModel)
    {
        var userRepository = UnitOfWork.AsyncRepository<User>();

        var user = await userRepository.GetAsync(u => !u.IsDeleted && u.Username == requestModel.Username);

        if (user == null ||
            !HashStringHelper.IsValid(requestModel.Password, user.HashedPassword))
        {
            return new Response<AuthenticationResponse>(false, ErrorMessages.LoginFailed);
        }

        var token = JwtHelper.GenerateJwtToken(user);
        var authenticationResponse = new AuthenticationResponse(user, token);

        return new Response<AuthenticationResponse>(true, authenticationResponse);
    }

    public async Task<Response> ChangePasswordAsync(ChangePasswordRequest requestModel)
    {
        if (requestModel.Id == null)
        {
            return new Response(false, ErrorMessages.BadRequest);
        }

        var userRepository = UnitOfWork.AsyncRepository<User>();

        var user = await userRepository.GetAsync(u => !u.IsDeleted && u.Id == requestModel.Id);

        if (user == null)
        {
            return new Response(false, ErrorMessages.BadRequest);
        }

        if (!user.IsFirstTimeLogIn &&
            !HashStringHelper.IsValid(requestModel.OldPassword, user.HashedPassword))
        {
            return new Response(false, ErrorMessages.WrongOldPassword);
        }

        if (HashStringHelper.IsValid(requestModel.NewPassword, user.HashedPassword))
        {
            return new Response(false, ErrorMessages.MatchingOldAndNewPassword);
        }

        user.HashedPassword = HashStringHelper.HashString(requestModel.NewPassword);

        if (user.IsFirstTimeLogIn)
        {
            user.IsFirstTimeLogIn = false;
        }

        await userRepository.UpdateAsync(user);
        await UnitOfWork.SaveChangesAsync();

        return new Response(true, "Success");
    }

    public async Task<Response<CreateUserResponse>> CreateUserAsync(CreateUserRequest requestModel)
    {
        var userRepository = UnitOfWork.AsyncRepository<User>();

        var user = new User();

        var responseModel = new CreateUserResponse(user);

        if (GetAge(requestModel.DateOfBirth) < 18)
        {
            return new Response<CreateUserResponse>(false, ErrorMessages.InvalidAge, responseModel);
        }

        if (DateTime.Compare(requestModel.DateOfBirth, requestModel.JoinedDate) != -1
            || requestModel.JoinedDate.DayOfWeek == DayOfWeek.Saturday || requestModel.JoinedDate.DayOfWeek == DayOfWeek.Sunday)
        {
            return new Response<CreateUserResponse>(false, ErrorMessages.InvalidJoinedDate, responseModel);
        }

        var latestStaffCode = userRepository.ListAsync(user => user.IsDeleted == false).Result.OrderByDescending(user => user.StaffCode).First().StaffCode;

        if (latestStaffCode == null)
        {
            return new Response<CreateUserResponse>(false, ErrorMessages.InternalServerError, responseModel);
        }


        var latestUserName = userRepository.ListAsync(user => user.IsDeleted == false).Result.OrderByDescending(user => user.Username)
            .Where(user => user.Username.Contains(GetNewUserNameWithoutNumber(requestModel.FirstName,requestModel.LastName))).First().Username;

        if (latestUserName == null)
        {
            return new Response<CreateUserResponse>(false, ErrorMessages.InternalServerError, responseModel);
        }

        var newStaffCode = GetNewStaffCode(latestStaffCode);
        var newUserName = GetNewUserName(requestModel.FirstName, requestModel.LastName, latestUserName);
        var newPassword = HashStringHelper.HashString(GetNewPassword(requestModel.FirstName, requestModel.LastName, requestModel.DateOfBirth)); 

        user = new User
        {
            Id = Guid.NewGuid(),
            StaffCode = newStaffCode,
            FirstName = requestModel.FirstName,
            LastName = requestModel.LastName,
            Username = newUserName,
            HashedPassword = newPassword,
            DateOfBirth = requestModel.DateOfBirth,
            Gender = requestModel.Gender,
            JoinedDate = requestModel.JoinedDate,
            Role = requestModel.Role,
            Location = requestModel.Location,
            IsFirstTimeLogIn = true,
        };

        responseModel = new CreateUserResponse(user);

        await userRepository.AddAsync(user);
        await UnitOfWork.SaveChangesAsync();

        return new Response<CreateUserResponse>(true, "Success", responseModel);
    }

    public async Task<UserInternalModel?> GetInternalModelByIdAsync(Guid id)
    {
        var userRepository = UnitOfWork.AsyncRepository<User>();

        var user = await userRepository.GetAsync(u => !u.IsDeleted && u.Id == id);

        if (user == null)
        {
            return null;
        }

        return new UserInternalModel(user);
    }

    public async Task<Response<GetUserResponse>> GetAsync(GetUserRequest request)
    {
        var userRepository = UnitOfWork.AsyncRepository<User>();

        var user = await userRepository.GetAsync(u => !u.IsDeleted &&
                                                        u.Location == request.Location &&
                                                        u.Id == request.Id);

        if (user == null)
        {
            return new Response<GetUserResponse>(false, ErrorMessages.NotFound);
        }

        var getUserDto = new GetUserResponse(user);

        return new Response<GetUserResponse>(true, getUserDto);
    }

    public async Task<Response<GetListUsersResponse>> GetListAsync(GetListUsersRequest request)
    {
        var userRepository = UnitOfWork.AsyncRepository<User>();

        var users = (await userRepository.ListAsync(u => !u.IsDeleted &&
                                                            u.Location == request.Location))
                                .Select(u => new GetUserResponse(u))
                                .AsQueryable();

        var validSortFields = new[]
        {
            ModelFields.StaffCode,
            ModelFields.FullName,
            ModelFields.Username,
            ModelFields.JoinedDate,
            ModelFields.Role
        };

        var validFilterFields = new[]
        {
            ModelFields.Role
        };

        var searchFields = new[]
        {
            ModelFields.FullName,
            ModelFields.StaffCode
        };

        var processedList = users.FilterByField(validFilterFields,
                                                request.FilterQuery.FilterField,
                                                request.FilterQuery.FilterValue)
                                    .SearchByField(searchFields,
                                                request.SearchQuery.SearchValue)
                                    .SortByField(validSortFields,
                                                request.SortQuery.SortField,
                                                request.SortQuery.SortDirection);

        var paginatedList = new PagedList<GetUserResponse>(processedList,
                                                            request.PagingQuery.PageIndex,
                                                            request.PagingQuery.PageSize);

        var response = new GetListUsersResponse(paginatedList);

        return new Response<GetListUsersResponse>(true, response);
    }

    public static int GetAge(DateTime birthDate)
    {
        var today = DateTime.Now;

        var age = today.Year - birthDate.Year;

        if (today.Month < birthDate.Month || (today.Month == birthDate.Month && today.Day < birthDate.Day)) { age--; }

        return age;
    }

    public static string GetNewStaffCode(string previousStaffCode)
    {
        var prefix = "SD";

        var number = Regex.Match(previousStaffCode, @"\d+").Value;

        var nextStaffCodeNumber = (number == "" || number == null) ? 1 : Convert.ToInt32(number) + 1;

        return prefix + nextStaffCodeNumber.ToString().PadLeft(4, '0');
    }

    public static string GetNewUserName(string firstName, string lastName, string previousUserName)
    {
        var fullname = firstName + " " + lastName;

        var nameWordArray = fullname.Split(" ");

        var userName = nameWordArray[nameWordArray.Length - 1];

        for (int i = 0; i < nameWordArray.Length - 1; i++)
        {
            userName += nameWordArray[i].Substring(0,1);
        }

        var previousUserNameWithoutNumber = Regex.Match(previousUserName, @"[a-zA-Z]+").Value;

        if (previousUserNameWithoutNumber == null)
        {
            return userName.ToLower() + "1";
        }

        var previousNumber = Regex.Match(previousUserName, @"\d+").Value;

        var number = Convert.ToInt32(previousNumber) + 1;

        return userName.ToLower() + number; 
    }

    public static string GetNewUserNameWithoutNumber(string firstName, string lastName)
    {
        var fullname = firstName + " " + lastName;

        var nameWordArray = fullname.Split(" ");

        var userName = nameWordArray[nameWordArray.Length - 1];

        for (int i = 0; i < nameWordArray.Length - 1; i++)
        {
            userName += nameWordArray[i].Substring(0, 1);
        }

        return userName.ToLower();
    }

    public static string GetNewPassword(string firstName, string lastName, DateTime dateOfBirth)
    {
        var fullname = firstName + " " + lastName;

        var nameWordArray = fullname.Split(" ");

        var userName = nameWordArray[nameWordArray.Length - 1];

        for (int i = 0; i < nameWordArray.Length - 1; i++)
        {
            userName += nameWordArray[i].Substring(0, 1);
        }

        return userName.ToLower() + "@" + dateOfBirth.ToString("ddMMyyyy");
    }
}