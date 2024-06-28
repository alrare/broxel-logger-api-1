using Serilog;
using System.Security.AccessControl;
using Broxel.Logger.Api.Application;
using Microsoft.AspNetCore.Mvc;
using Google.Apis.Auth.OAuth2;

namespace Broxel.Logger.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BroxelLoggingController : ControllerBase
    {

        private readonly IConfiguration _iconfig;

        public BroxelLoggingController(IConfiguration iconfig)
        {
            _iconfig = iconfig;
        }

        [HttpPost]
        [Route("PostLogAudit")]
        public bool PostLogAudit(string messageLog)
        {
            var saveLogGCP = new SaveLogGCP(_iconfig);
            var audit = saveLogGCP.AuditLogGCP("Information", messageLog, "GoogleCloudLoggingAudit");        
            return true;
        }

        [HttpPost]
        [Route("PostLogOperational")]
        public bool PostLogOperational()
        {
            //var saveLogGCP = new SaveLogGCP(_iconfig);
            //var audit = saveLogGCP.AuditLogGCP("Warning", "GoogleCloudLoggingOperational");        
            return true;
        }


        [HttpPost]
        [Route("PostLogApplication")]
        public bool PostLogApplication()
        {
            //var saveLogGCP = new SaveLogGCP(_iconfig);
            //var audit = saveLogGCP.AuditLogGCP("Error", "GoogleCloudLoggingApplication");        
            return true;
        }
    }
}