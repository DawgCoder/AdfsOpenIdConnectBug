using Microsoft.AspNetCore.Components;

namespace BlazorOidc
{
    public class LoginService
    {
        private readonly NavigationManager _navigationManager;

        public LoginService(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        public void Login()
        {
            _navigationManager.NavigateTo("account/login", true);
        }
    }
}