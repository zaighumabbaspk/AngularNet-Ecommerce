

using AutoMapper;
using eCommerce.Application.DTOs;
using eCommerce.Application.Services.Interfaces.Logging;
using eCommerce.Application.Validations.Authenticaton;
using eCommerce.Domain.Entities.Identity;
using eCommerce.Domain.Services.Interfaces.Authentication;
using eCommerceApp.Application.DTOs.Identity;
using eCommerceApp.Application.Validations;
using FluentValidation;


namespace eCommerce.Application.Services.implementation.Authentication
{


    public class AuthenticationService : IAuthenticationService
    {

        private readonly IUserManagement _userManagement;
        private readonly ITokenManagement _tokenManagement;
        private readonly IRoleManagement _roleManagement;
        private readonly IAppLogger<AuthenticationService> _logger;
        private readonly IValidator<CreateUser> _createUserValidator;
        private readonly IValidator<LoginUser> _loginUserValidator;
        private readonly IMapper _mapper;
        private readonly IValidationService _validationService; 


        public AuthenticationService(IUserManagement userManagement, ITokenManagement tokenManagement,
            IRoleManagement roleManagement , IAppLogger<AuthenticationService> logger, IValidator<CreateUser>
            createUserValiator , IValidationService validationService, IMapper mapper , IValidator<LoginUser> _loginUserValidator)
        {
            _userManagement = userManagement;
            _tokenManagement = tokenManagement;
            _roleManagement = roleManagement;
            _logger = logger;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _createUserValidator = createUserValiator;
            _loginUserValidator = _loginUserValidator;
            _validationService = validationService;


        }
        public async Task<ServiceResponse> CreateUser(CreateUser user)
        {
            var _Validator = await _validationService.ValidateAsync<CreateUser>(user, _createUserValidator);
            if (!_Validator.Success) return _Validator;

            var MappedModel = _mapper.Map<AppUser>(user);
            MappedModel.UserName = user.Email;
            MappedModel.PasswordHash = user.Password;

            var result = await _userManagement.CreateUser(MappedModel, user.Password);
            if (!result)
                return new ServiceResponse(false, "Email Already Exists or Unknown Error Occured");

            var _user = await _userManagement.GetUserByEmail(user.Email);
            var Users = await _userManagement.GetAllUsers();
            bool AssignedUser = await _roleManagement.AddUserToRole(_user, Users.Count() > 1 ? "User" : "Admin");

            if (!AssignedUser)
            {
                var RemovedUser = await _userManagement.RemoveUserByEmail(user.Email);
                if (RemovedUser <= 0)
                    _logger.LogError("User Could not be assigned role",
                        new Exception($"User with email {user.Email} unable to remove as a result of role assigning issue"));

                return new ServiceResponse(false, "Error occurred in Creating Account");
            }
            return new ServiceResponse(true, "User Created Successfully");
        }



        public async Task<LoginResponse> LoginUser(LoginUser user)
        {
            var _Validator = await _validationService.ValidateAsync(user, _loginUserValidator);
            if (!_Validator.Success)
                return new LoginResponse(Message: _Validator.Message);

            var MappedModel = _mapper.Map<AppUser>(user);
            MappedModel.PasswordHash = user.Password;
            bool LoginResult = await _userManagement.LoginUser(MappedModel , user.Password);
            if (!LoginResult)
                return new LoginResponse(Message: "Invalid Email or Password");

            var _user = await _userManagement.GetUserByEmail(user.Email);
            var Claims = await _userManagement.GetUserByClaims(_user.Email!);

            string JwtToken =  _tokenManagement.GenerateToken(Claims);
            string RefreshToken = _tokenManagement.GetRefreshToken();

            int saveToken = await _tokenManagement.AddRefreshToken(_user.Id, RefreshToken);
            return saveToken > 0 
                ? new LoginResponse(true, "Login Successful", JwtToken, RefreshToken) 
                : new LoginResponse( false, "Error Occurred while saving refresh token");

        }

        public async Task<LoginResponse> ReviveToken(string refreshToken)
        {
            bool ValidateToken = await _tokenManagement.ValidateRefreshToken(refreshToken);
            if (!ValidateToken)
                return new LoginResponse(Message: "Invalid Refresh Token");

            string UserId = await _tokenManagement.GetUserIdByRefreshToken(refreshToken);
            AppUser? user = await _userManagement.GetUserById(UserId);
            var claims = await _userManagement.GetUserByClaims(user!.Email!);
            string newJwtToken = _tokenManagement.GenerateToken(claims);
            string newRefreshToken = _tokenManagement.GetRefreshToken();
            await _tokenManagement.UpdateRefreshToken(UserId,newRefreshToken);
            return new LoginResponse(true, Token : newJwtToken, RefreshToken :newRefreshToken);

        }
    }
}
