using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace EjadaPortal.Tests.Helpers
{
    public static class UserManagerMockHelper
    {
        public static Mock<UserManager<User>> Create()
        {
            var store = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
                store.Object, null, null, null, null, null, null, null, null
            );
        }
    }
}
