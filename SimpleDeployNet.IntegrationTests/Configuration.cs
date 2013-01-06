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
using System.Configuration;

namespace SimpleDeployNet.IntegrationTests
{
    public static class TestConfiguration
    {
        //<add key="msDeployEndPoint" value="https://mywebsite.com:8712/msdeploy.axd"/>
        //<add key="msDeployIISWebsite" value="website.com"/>
        //<add key="msDeployUsername" value="deploymentUser"/>
        //<add key="msDeployPassword" value="deploymentPassword"/>

        public static Uri MsDeployEndPoint
        {
            get
            {
                return new Uri(ConfigurationManager.AppSettings["msDeployEndPoint"]);
            }
        }

        public static string MsDeployIISWebsite
        {
            get
            {
                return ConfigurationManager.AppSettings["msDeployIISWebsite"];
            }
        }

        public static string MsDeployUsername
        {
            get
            {
                return ConfigurationManager.AppSettings["msDeployUsername"];
            }
        }

        public static string MsDeployPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["msDeployPassword"];
            }
        }

    }
}
