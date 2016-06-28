using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Greet.ConvenienceLib
{
    /// <summary>
    /// Various methods to create folders and test permissions on a file
    /// </summary>
    public static class FilesFolders
    {

        /// <summary>
        /// Tries to create a folder from the full path given
        /// </summary>
        /// <param name="folder_string"></param>
        /// <returns>Returns true is succeed, false if failed for permissions reasons</returns>
        public static bool CreateFolder(String folder_string)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(folder_string));
            return Directory.Exists(folder_string);
        }

        public static bool TestWritePermissions(string path)
        {
            string NtAccountName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            DirectoryInfo di = new DirectoryInfo(path);
            DirectorySecurity acl = di.GetAccessControl(AccessControlSections.Access);
            AuthorizationRuleCollection rules = acl.GetAccessRules(true, true, typeof(NTAccount));

            //Go through the rules returned from the DirectorySecurity
            foreach (AuthorizationRule rule in rules)
            {
                //If we find one that matches the identity we are looking for
                if (rule.IdentityReference.Value.Equals(NtAccountName, StringComparison.CurrentCultureIgnoreCase))
                {
                    //Cast to a FileSystemAccessRule to check for access rights
                    if ((((FileSystemAccessRule)rule).FileSystemRights & FileSystemRights.WriteData) > 0)
                        return true;
                }
            }
            return false;
        }
    }
}
