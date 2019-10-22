using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Services.Authentication.External;

namespace FreemiumMedia.Nop.Plugin.ExternalAuth.LinkedIn.Infrastructure
{
    public class LinkedInAuthenticationRegistrar : IExternalAuthenticationRegistrar
    {
        public void Configure(AuthenticationBuilder builder)
        {
            builder.AddLinkedIn(options =>
            {
                var settings = EngineContext.Current.Resolve<LinkedInAuthenticationSettings>();
                options.ClientId = settings.ClientKeyIdentifier;
                options.ClientSecret = settings.ClientSecret;
                options.Scope.Add("r_basicprofile");
                options.Scope.Add("r_emailaddress");
                options.SaveTokens = true;
            });
        }
    }
}
