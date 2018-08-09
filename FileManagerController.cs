using System;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using System.Net;
using System.Management;
using System.Diagnostics;
using System.Data.SqlClient;
using GURU.Controllers;
using Dapper;
using System.Web.Http;
using System.ComponentModel;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Configuration;
using System.Linq;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using System.Data;
using Rock.Framework.Logging;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;

namespace GURU.api
{
    [RoutePrefix("api/manager")]

    public class ImpersonationDemo
    {
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
            int dwLogonType, int dwLogonProvider, out SafeTokenHandle phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);
        public static void Main(string[] args)
        {
            SafeTokenHandle safeTokenHandle;
            try
            {
                const int LOGON32_PROVIDER_DEFAULT = 0;
                const int LOGON32_LOGON_INTERACTIVE = 2;

                // Call LogonUser to obtain a handle to an access token.
                bool returnValue = LogonUser("JMondal-", "MI\\", "Meghna_55",
                    LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
                    out safeTokenHandle);

                Console.WriteLine("LogonUser called.");

                if (false == returnValue)
                {
                    int ret = Marshal.GetLastWin32Error();
                    Console.WriteLine("LogonUser failed with error code : {0}", ret);
                    throw new System.ComponentModel.Win32Exception(ret);
                }
                using (safeTokenHandle)
                {
                    Console.WriteLine("Did LogonUser Succeed? " + (returnValue ? "Yes" : "No"));
                    Console.WriteLine("Value of Windows NT token: " + safeTokenHandle);

                    // Check the identity.
                    Console.WriteLine("Before impersonation: "
                        + WindowsIdentity.GetCurrent().Name);
                    // Use the token handle returned by LogonUser.
                    using (WindowsIdentity newId = new WindowsIdentity(safeTokenHandle.DangerousGetHandle()))
                    {
                        using (WindowsImpersonationContext impersonatedUser = newId.Impersonate())
                        {
                            // Check the identity.
                            Console.WriteLine("After impersonation: "
                                + WindowsIdentity.GetCurrent().Name);
                        }
                    }
                    // Releasing the context object stops the impersonation
                    // Check the identity.
                    Console.WriteLine("After closing the context: " + WindowsIdentity.GetCurrent().Name);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occurred. " + ex.Message);
            }
        }

        public sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            private SafeTokenHandle()
                : base(true)
            {
            }

            [DllImport("kernel32.dll")]
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [SuppressUnmanagedCodeSecurity]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool CloseHandle(IntPtr handle);

            protected override bool ReleaseHandle()
            {
                return CloseHandle(handle);
            }
        }
    }

        public class FileManagerController : BaseApiController
        {
            String Path = ConfigurationManager.AppSettings["FilePath"];
            public ILogger logger = LoggerFactory.GetInstance();

            [Route("getfiles"), HttpGet]
            public IHttpActionResult GetFiles()
            {
                List<string> Folders = new List<string>();
                string[] dirs = System.IO.Directory.GetFiles(@Path);
                dirs = System.IO.Directory.GetFiles(@Path, "*.*", SearchOption.AllDirectories);
                return Ok(dirs);
            }
    

    [Route("submit"), HttpPost]
            public IHttpActionResult submittedAction(List<String> x)
            {
                try
                {

                    //string[] arr = { "10.10.10.53", "10.10.10.10", "10.10.10.34", "10.10.10.49" };
                    //try
                    //{
                    //    ConnectionOptions connOptions = new ConnectionOptions();
                    //    connOptions.Username = "USER1";
                    //    connOptions.Password = "PA$$W0RD";

                    //    ManagementScope scope = new ManagementScope("\\\\" + ipaddress + "\\root\\cimv2", connOptions);
                    //    scope.Connect();

                    //    //Query system for Operating System information
                    //    ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
                    //    ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                    //    ManagementObjectCollection queryCollection = searcher.Get();
                    //    foreach (ManagementObject m in queryCollection)
                    //    {
                    //        // Display the remote computer information
                    //        Console.WriteLine("Computer Name : {0}", m["csname"]);
                    //        Console.WriteLine("Windows Directory : {0}", m["WindowsDirectory"]);
                    //    }
                    //}

                    //catch (Exception ex)
                    //{
                    //    Console.WriteLine("Exception Message: " + Environment.NewLine + ex.Message);
                    //    Console.WriteLine("Inner Exception: " + Environment.NewLine + ex.InnerException);
                    //}

                    NetworkCredential theNetworkCredential = new NetworkCredential("JMondal-", "!!!!!", "MI\\");
                    //NetworkCredential theNetworkCredential = new NetworkCredential("GuruFileManager", "!!!!!", "MI");
                    CredentialCache theNetCache = new CredentialCache();
                    theNetCache.Add(new Uri(@"\\utilitybox"), "Basic", theNetworkCredential);
                    string userID = WindowsIdentity.GetCurrent().Name;
                    string[] theFolders = Directory.GetDirectories(@"\\test.rockfin.com\data\Guru_Resources");

                    for (int i = 0; i < x.Count; i++)
                    {
                        string FullPath = Path + x[i];
                        if (File.Exists(FullPath)) { File.Delete(FullPath); }
                        else { throw new System.InvalidOperationException("FilePath is invalid, or FileNotFound"); }                   
                    }
                    return Ok(x);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    return InternalServerError(e);
                }
         
            }
        }
}
