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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Web.Deployment;

namespace SimpleDeployNet.Core
{
    public interface IMsDeployFileService
    {
        DeploymentObjectList FetchFileList(ConnectionProperties properties);
        bool DownloadFile(ConnectionProperties properties, string relativeRemoteFileName, string fullLocalFilePathToSave);
        bool UploadFile(ConnectionProperties properties, string relativeDestinationFileName, string fullLocalFilePath);
        bool DeleteFile(ConnectionProperties properties, string relativeRemoteFileName);
    }

    public class MsDeployFileService : IMsDeployFileService
    {
        /// <summary>
        /// Fetches the remote servers file list.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Connection properties cannot be null.;properties</exception>
        public DeploymentObjectList FetchFileList(ConnectionProperties properties)
        {
            if (properties == null) throw new ArgumentException("Connection properties cannot be null.", "properties");
            properties.Validate();

            var results = new DeploymentObjectList { Folders = new List<string>(), Files = new List<string>() };

            if (properties.AllowUntrustedCertificates)
            {
                ServicePointManager.ServerCertificateValidationCallback = (s, c, chain, err) => true;
            }

            var destOptions = new DeploymentBaseOptions
                                  {
                                      ComputerName = BuildMsDeployUri(properties).ToString(),
                                      UserName = properties.Username,
                                      Password = properties.Password,
                                      UseDelegation = true,
                                      AuthenticationType = "Basic"
                                  };

            foreach (var extension in destOptions.LinkExtensions.Where(extension => extension.Name == "ContentExtension"))
            {
                extension.Enabled = false;
                break;
            }

            using (var deploymentObject = DeploymentManager.CreateObject(DeploymentWellKnownProvider.ContentPath, properties.IISWebsiteName, destOptions))
            {
                var xmlResult = GetDeploymentObjectAsXmlDocument(deploymentObject);
                TextReader tr = new StringReader(xmlResult.InnerXml);
                var doc = XDocument.Load(tr);

                results.Files = (from f in doc.Element("MSDeploy.contentPath").Element("contentPath")
                                    .Element("dirPath")
                                    .Elements("filePath")
                                 select f.Attribute("path").Value).ToList();

                results.Folders = (from f in doc.Element("MSDeploy.contentPath").Element("contentPath")
                                    .Element("dirPath")
                                    .Elements("dirPath")
                                   select f.Attribute("path").Value).ToList();
            }

            return results;
        }

        /// <summary>
        /// Downloads a remote file.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="relativeRemoteFileName">Name of the relative remote file.</param>
        /// <param name="fullLocalFilePathToSave">The full local file path to save.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Connection properties cannot be null.;properties</exception>
        public bool DownloadFile(ConnectionProperties properties, string relativeRemoteFileName, string fullLocalFilePathToSave)
        {
            if (properties == null) throw new ArgumentException("Connection properties cannot be null.", "properties");
            properties.Validate();

            if (properties.AllowUntrustedCertificates)
            {
                ServicePointManager.ServerCertificateValidationCallback = (s, c, chain, err) => true;
            }

            var destinationOptions = new DeploymentBaseOptions
            {
                ComputerName = BuildMsDeployUri(properties).ToString(),
                UserName = properties.Username,
                Password = properties.Password,
                UseDelegation = true,
                AuthenticationType = "Basic"
            };
            var localOptions = new DeploymentBaseOptions();

            var syncOptions = new DeploymentSyncOptions { DoNotDelete = true };

            var remotePath = String.Format("{0}/{1}", properties.IISWebsiteName, relativeRemoteFileName.Trim(new[] { '/', '\\' }));

            using (var remoteFile = DeploymentManager.CreateObject(DeploymentWellKnownProvider.ContentPath, remotePath, destinationOptions))
            {
                var result = remoteFile.SyncTo(DeploymentWellKnownProvider.ContentPath, fullLocalFilePathToSave, localOptions, syncOptions);
                if (result.ObjectsAdded == 1 || result.ObjectsUpdated == 1) return true;
            }

            return false;
        }

        /// <summary>
        /// Uploads a file to the remote host.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="relativeDestinationFileName">Name of the relative destination file.</param>
        /// <param name="fullLocalFilePath">The full local file path.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Connection properties cannot be null.;properties</exception>
        public bool UploadFile(ConnectionProperties properties, string relativeDestinationFileName, string fullLocalFilePath)
        {
            if (properties==null) throw new ArgumentException("Connection properties cannot be null.", "properties");
            properties.Validate();

            if (properties.AllowUntrustedCertificates)
            {
                ServicePointManager.ServerCertificateValidationCallback = (s, c, chain, err) => true;
            }

            var destinationOptions = new DeploymentBaseOptions
            {
                ComputerName = BuildMsDeployUri(properties).ToString(),
                UserName = properties.Username,
                Password = properties.Password,
                UseDelegation = true,
                AuthenticationType = "Basic"
            };

            var syncOptions = new DeploymentSyncOptions { DoNotDelete = true };

            var remotePath = String.Format("{0}/{1}", properties.IISWebsiteName, relativeDestinationFileName.Trim(new []{'/','\\'}));

            using (var localFile = DeploymentManager.CreateObject(DeploymentWellKnownProvider.ContentPath, fullLocalFilePath))
            {
                var result = localFile.SyncTo(DeploymentWellKnownProvider.ContentPath, remotePath, destinationOptions, syncOptions);
                if (result.ObjectsAdded == 1 || result.ObjectsUpdated == 1) return true;
            }

            return true;
        }

        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="relativeRemoteFileName">Name of the relative remote file.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Connection properties cannot be null.;properties</exception>
        public bool DeleteFile(ConnectionProperties properties, string relativeRemoteFileName)
        {
            if (properties == null) throw new ArgumentException("Connection properties cannot be null.", "properties");
            properties.Validate();

            ServicePointManager.ServerCertificateValidationCallback = (s, c, chain, err) => true;

            var destionationOptions = new DeploymentBaseOptions
            {
                ComputerName = BuildMsDeployUri(properties).ToString(),
                UserName = properties.Username,
                Password = properties.Password,
                UseDelegation = true,
                AuthenticationType = "Basic"
            };

            var syncOptions = new DeploymentSyncOptions
            {
                DeleteDestination = true
            };

            var remotePath = String.Format("{0}/{1}", properties.IISWebsiteName, relativeRemoteFileName.Trim(new[] { '/', '\\' }));

            using (var deploymentObject = DeploymentManager.CreateObject(DeploymentWellKnownProvider.ContentPath, remotePath, destionationOptions))
            {
                var results = deploymentObject.SyncTo(destionationOptions, syncOptions);
                if (results.ObjectsDeleted == 1) return true;
            }

            return true;
        }
        
        private static Uri BuildMsDeployUri(ConnectionProperties properties)
        {
            properties.Validate();

            var builder = new UriBuilder(properties.MsDeployEndpointUri) { Query = String.Format("site={0}", properties.IISWebsiteName) };

            return builder.Uri;
        }

        private static XmlDocument GetDeploymentObjectAsXmlDocument(DeploymentObject obj)
        {
            using (var sw = new StringWriter())
            {
                using (var tw = new XmlTextWriter(sw))
                {
                    WriteXmlObject(obj, tw);
                }

                var doc = new XmlDocument();
                doc.LoadXml(sw.ToString());
                return doc;
            }
        }

        private static void WriteXmlObject(DeploymentObject obj, XmlTextWriter writer)
        {
            writer.WriteStartElement(obj.Name);
            foreach (var attr in obj.Attributes)
            {
                writer.WriteAttributeString(attr.Name, attr.Value.Value as string);
            }
            foreach (var childObj in obj.GetChildren())
            {
                WriteXmlObject(childObj, writer);
            }
            writer.WriteEndElement();
        }
    }
}
