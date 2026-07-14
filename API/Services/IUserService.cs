public interface IUserService
{
    Task<ServiceResult<UserResponse>> CreateUserAsync(CreateUserRequest userReq);
}