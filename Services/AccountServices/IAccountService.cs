using EmailScheduler.Models.Entities;
using Microsoft.AspNetCore.Http;

namespace EmailScheduler.Services.AccountServices
{
    public interface IAccountService
    {
        /// <summary>
        /// Signs up a user with Google authentication.
        /// </summary>
        /// <param name="httpContext">The current HTTP context.</param>
        /// <returns>The created user.</returns>
        Task<Users> SignUpWithGoogleAsync(HttpContext httpContext);
    }
}