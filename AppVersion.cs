using System;

namespace api.Core.Utils
{
    public class AppVersion
    {
        /// <summary>
        /// Indicates the minimum version required for mobile application to access new features.
        /// </summary>
        private string baseAppVersion;

        public AppVersion()
        {
            baseAppVersion = "1.14.0";
        }

        /// <summary>
        /// It indicates whether the current version of the mobile application is greater than the base version.
        /// </summary>
        /// <param name="currentAppVersion">Current mobile aplication version.</param>
        /// <returns>True when the current version is greater than or equal to the version base.</returns>
        public bool IsAppNewVersion(string currentAppVersion)
        {
            var vb = new Version(baseAppVersion);
            var vc = new Version(currentAppVersion);
            return vc.CompareTo(vb) >= 0;
        }
    }
}