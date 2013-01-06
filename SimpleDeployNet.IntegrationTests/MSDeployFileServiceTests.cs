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

using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleDeployNet.Core;

namespace SimpleDeployNet.IntegrationTests
{
    [TestClass]
    public class MSDeployFileServiceTests
    {
        private ConnectionProperties _connectionProperties;
        private string _tempFilePath;

        [TestMethod]
        public void MSDeployFileService_UploadTestFile_ReturnsTrue()
        {
            var uploadFile = new MsDeployFileService()
                .UploadFile(_connectionProperties, 
                "/remoteFilePath.txt", 
                _tempFilePath);

            Assert.IsTrue(uploadFile, "Upload failed");
        } 
        
        [TestMethod]
        public void MSDeployFileService_DownloadTestFile_ReturnsTrue()
        {
            //delete setup'd temp file
            File.Delete(_tempFilePath);

            var downloadFile = new MsDeployFileService()
                .DownloadFile(_connectionProperties,
                "/remoteFilePath.txt",
                _tempFilePath);

            Assert.IsTrue(File.Exists(_tempFilePath), "Download failed");
        }   
     
        [TestMethod]
        public void MSDeployFileService_DeleteTestFile_ReturnsTrue()
        {
            var deleteFile = new MsDeployFileService()
                .DeleteFile(_connectionProperties,
                "/remoteFilePath.txt");

            Assert.IsTrue(deleteFile, "Delete failed");
        }     

        [TestMethod]
        public void MSDeployFileService_ListFiles_ReturnsAList()
        {
            var fileList = new MsDeployFileService()
                .FetchFileList(_connectionProperties);

            Assert.IsNotNull(fileList.Files, "File list failed");
        }


        [TestInitialize]
        public void Initialise()
        {
            _connectionProperties = new ConnectionProperties
            {
                MsDeployEndpointUri = TestConfiguration.MsDeployEndPoint,
                AllowUntrustedCertificates = true,
                IISWebsiteName = TestConfiguration.MsDeployIISWebsite,
                Username = TestConfiguration.MsDeployUsername,
                Password = TestConfiguration.MsDeployPassword
            };

            _tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
            File.WriteAllText(_tempFilePath,"test upload content");
        }
        [TestCleanup]
        public void TearDown()
        {
            File.Delete(_tempFilePath);
        }
    }
}
