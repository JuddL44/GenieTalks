public interface IUserService
{
    Task<ServiceResult<UserResponse>> CreateUserAsync(CreateUserRequest userReq);
    Task<ServiceResult<UserResponse>> GetUserByIdAsync(Guid id);
}