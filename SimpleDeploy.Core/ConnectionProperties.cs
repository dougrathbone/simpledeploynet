/*
 * SimpleDeploy.Net
 * Copyright Doug Rathbone
 * http://www.diaryofaninja.com
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the Microsoft Public License as published by
 * Microsoft.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the Microsoft Public License
 * along with this program.  If not, see <http://opensource.org/licenses/MS-PL>.
*/

using System;

namespace SimpleDeployNet.Core
{
    public class ConnectionProperties
    {
        public Uri MsDeployEndpointUri { get; set; }

        public string IISWebsiteName { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public bool AllowUntrustedCertificates{ get; set; }

        public void Validate()
        {
            if (MsDeployEndpointUri == null) throw new ArgumentNullException("MSDeployEndpointUri");
            if (String.IsNullOrEmpty(Username)) throw new ArgumentNullException("Username");
            if (String.IsNullOrEmpty(Password)) throw new ArgumentNullException("Password");
            if (String.IsNullOrEmpty(IISWebsiteName)) throw new ArgumentNullException("IISWebsiteName");

            var deployUrl = new UriBuilder(MsDeployEndpointUri);
            if (deployUrl.Scheme != Uri.UriSchemeHttp && deployUrl.Scheme != Uri.UriSchemeHttps)
            {
                throw new UriFormatException(
                    "The Uri provided is of the wrong format, please use an url that is 'http' or 'https' scheme.");
            }
        }

    }
}
