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

    public class FileManagerController : BaseApiController
    {
        static string path = ConfigurationManager.AppSettings["FilePath"];

        public ILogger logger = LoggerFactory.GetInstance();
        [Route("getfiles"), HttpGet]
        public IHttpActionResult GetFiles()
        {
            List<string> Folders = new List<string>();
            string[] dirs = System.IO.Directory.GetFiles(path);
            dirs = System.IO.Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            return Ok(dirs);
        }

        [Route("submit"), HttpPost]

        public IHttpActionResult submittedAction(List<String> x)
        {

            ImpersonateUser callUser = new ImpersonateUser();

            try
            {
                callUser.Main(x);
            }           
            catch (Exception e)
            {
                logger.Error(e.Message);
                return InternalServerError(e);
            }
            return Ok(x);
        }

        public class ImpersonateUser
        {
            [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
            int dwLogonType, int dwLogonProvider, out SafeTokenHandle phToken);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            public extern static bool CloseHandle(IntPtr handle);
            public void Main(List<String> x)
            {
                SafeTokenHandle safeTokenHandle;
                try
                {
                    const int LOGON32_PROVIDER_DEFAULT = 0;
                    const int LOGON32_LOGON_INTERACTIVE = 2;
                    bool returnValue = LogonUser("GuruFileManager", "MI", "",
                        LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
                        out safeTokenHandle);

                    Debug.WriteLine("LogonUser called.");

                    if (false == returnValue)
                    {
                        int ret = Marshal.GetLastWin32Error();
                        Debug.WriteLine("LogonUser failed with error code : {0}", ret);
                        throw new System.ComponentModel.Win32Exception(ret);
                    }
                    using (safeTokenHandle)
                    {
                        Debug.WriteLine("Did LogonUser Succeed? " + (returnValue ? "Yes" : "No"));
                        Debug.WriteLine("Value of Windows NT token: " + safeTokenHandle);

                        Debug.WriteLine("Before impersonation: "
                            + WindowsIdentity.GetCurrent().Name);
                        using (WindowsIdentity newId = new WindowsIdentity(safeTokenHandle.DangerousGetHandle()))
                        {
                            using (WindowsImpersonationContext impersonatedUser = newId.Impersonate())
                            {
                                Debug.WriteLine("After impersonation: "
                                    + WindowsIdentity.GetCurrent().Name);

                                Debug.WriteLine(x);

                                string[] theFiles = Directory.GetFiles(@"C:\Users\JMondal");
                            }

                        }
                        Debug.WriteLine("After closing the context: " + WindowsIdentity.GetCurrent().Name);
                    }
                } 
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception occurred. " + ex.Message);
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
    }
}
