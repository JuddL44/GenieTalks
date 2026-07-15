public interface IUserService
{
    Task<ServiceResult<UserResponse>> CreateUserAsync(CreateUserRequest userReq);
    Task<ServiceResult<UserResponse>> GetUserByIdAsync(Guid id);
    Task<ServiceResult<UserResponse>> UpdateUserByIdAsync(UpdateUserRequest update, Guid id);
    Task<ServiceResult<UserResponse>> DeleteUserByIdAsync(Guid id);
}