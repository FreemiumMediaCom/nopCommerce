using System;
using System.Threading.Tasks;


namespace Nop.Services.Plugins
{
    /// <summary>
    /// Base plugin
    /// </summary>
    public abstract class BasePlugin : IPlugin
    {
        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public async virtual Task<string> GetConfigurationPageUrl()
        {
            return null;
        }

        /// <summary>
        /// Gets or sets the plugin descriptor
        /// </summary>
        public virtual PluginDescriptor PluginDescriptor { get; set; }

        /// <summary>
        /// Install plugin
        /// </summary>
        public async virtual Task Install() 
        {
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public async virtual Task Uninstall() 
        {
        }

        /// <summary>
        /// Prepare plugin to the uninstallation
        /// </summary>
        public async virtual Task PreparePluginToUninstall()
        {
            //any can put any custom validation logic here
            //throw an exception if this plugin cannot be uninstalled
            //for example, requires some other certain plugins to be uninstalled first
        }
    }
}
