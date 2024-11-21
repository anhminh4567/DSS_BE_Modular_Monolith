using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Options
{
    internal class ExternalAuthenticationOptions
    {
        public const string EXTERNAL_PROFILE_IMAGE_CLAIM_NAME = "external_img";
        public const string EXTERNAL_EMAIL_CLAIM_NAME = "external_mail";
        public const string EXTERNAL_USERNAME_CLAIM_NAME = "external_username";
        public const string EXTERNAL_FAILURE_CLAIM_NAME = "external_fail_to_fetch";
        public const string EXTERNAL_IDENTIFIER_CLAIM_NAME = "external_identifier";
        public const string Section = "ExternalAuthenticationSection";
        public GoogleAuthenticationOption GoogleAuthenticationOption { get; set; }
    }
}
