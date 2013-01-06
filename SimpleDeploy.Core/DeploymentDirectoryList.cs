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

using System.Collections.Generic;

namespace SimpleDeployNet.Core
{
    public class DeploymentObjectList
    {
        public List<string> Files { get; set; }
        public List<string> Folders { get; set; }
    }
}
