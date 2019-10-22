using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Common
{
    public partial class ContactInformationModel : BaseNopModel
    {
        public string Phone { get; set; }
        public string EmailAddress { get; set; }
    }
}