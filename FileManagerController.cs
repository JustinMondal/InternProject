using System;
using System.IO;
using System.Linq;
using System.Web.Http;
using System.Security;
using System.Configuration;
using Rock.Framework.Logging;
using System.Security.Principal;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;

namespace GURU.api
{
    [RoutePrefix("api/manager")]
    public class FileManagerController : BaseApiController
    {
        private static string _path;
        private static string _gfmusername;
        private static string _gfmpassword;

        public ILogger Logger = LoggerFactory.GetInstance();

        public FileManagerController()
        {
            var path = ConfigurationManager.AppSettings["GuruResourcesBasePath"] + "Guru_Resources/";
            switch (Rock.Framework.Environment.ApplicationId)
            {
                case 201217:
                    path += "Servicing";
                    break;
                default:
                    path += "Origination";
                    break;
            }
            _path = new Uri(path).LocalPath;
            _gfmusername = ConfigurationManager.AppSettings["GuruFileManagerUsername"];
            _gfmpassword = ConfigurationManager.AppSettings["GuruFileManagerPassword"];
        }


        [Route("getfiles"), HttpGet]
        public IHttpActionResult GetFiles()
        {
            try
            {
                var dirs = Directory.GetFiles(_path, "*.*", SearchOption.AllDirectories);
                return Ok(dirs.Select(x => x.Remove(0, _path.Length + 1)).ToArray());
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [Route("submit"), HttpPost]

        public IHttpActionResult SubmittedAction(List<String> x)
        {
            ImpersonateUser callUser = new ImpersonateUser();
            try
            {
                callUser.DeleteFiles(x);
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
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
            public static extern bool CloseHandle(IntPtr handle);

            public void DeleteFiles(List<String> x)
            {
                SafeTokenHandle safeTokenHandle;
                const int logon32ProviderDefault = 0;
                const int logon32LogonInteractive = 2;
                bool returnValue = ImpersonateUser.LogonUser(_gfmusername, "MI", _gfmpassword,
                    logon32LogonInteractive, logon32ProviderDefault,
                    out safeTokenHandle);

                if (false == returnValue)
                {
                    int ret = Marshal.GetLastWin32Error();
                    throw new System.ComponentModel.Win32Exception(ret);
                }
                using (safeTokenHandle)
                {
                    using (WindowsIdentity newId = new WindowsIdentity(safeTokenHandle.DangerousGetHandle()))
                    {
                        using (WindowsImpersonationContext impersonatedUser = newId.Impersonate())
                        {
                            foreach (var item in x)
                            {
                                if (File.Exists(Path.Combine(_path, item))) { File.Delete(Path.Combine(_path, item)); }
                                else { throw new FileNotFoundException("The file was not found"); }
                            }
                        }
                    }
                }
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
