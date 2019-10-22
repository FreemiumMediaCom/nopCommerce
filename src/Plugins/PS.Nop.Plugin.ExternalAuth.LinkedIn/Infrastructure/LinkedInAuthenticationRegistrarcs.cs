using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Services.Authentication.External;

namespace PS.Nop.Plugin.ExternalAuth.LinkedIn.Infrastructure
{
    public class LinkedInAuthenticationRegistrarcs : IExternalAuthenticationRegistrar
    {
        public void Configure(AuthenticationBuilder builder)
        {

            //var options = new LinkedInAuthenticationOptions();
            //options.
            builder.AddLinkedIn(options =>
            {
                var settings = EngineContext.Current.Resolve<LinkedInExternalAuthSettings>();
                options.ClientId = settings.ClientKeyIdentifier;
                options.ClientSecret = settings.ClientSecret;
                options.Scope.Add("r_basicprofile");
                options.Scope.Add("r_emailaddress");
                //options.Scope.Add("r_formattedName");
                options.SaveTokens = true;
            });
        }
    }
}
