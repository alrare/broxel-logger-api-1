using Broxel.Logger.Api.Infraestructure;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Broxel.Logger.Api.Application
{
    public interface ISaveLogGCP
    {
        public bool AuditLogGCP(string typeLog, string messageLog,string serviceAccount);
        public bool OperationalLogGCP(string typeLog, string serviceAccount);
        public bool ApplicationLogGCP(string typeLog, string serviceAccount);
    }

    public class SaveLogGCP : ISaveLogGCP
    {
        private readonly IConfiguration _iconfig;

        public SaveLogGCP(IConfiguration iconfig)
        {
            _iconfig = iconfig;
        }

        public bool AuditLogGCP(string typeLog, string messageLog, string serviceAccount)
        {
            var daGCP = new DataAccessGCP(_iconfig);
            var auditConfig = daGCP.ConfigurationLogGCP(typeLog, messageLog, serviceAccount);
            return true;
        }

        public bool OperationalLogGCP(string typeLog, string serviceAccount)
        {
            throw new NotImplementedException();
        }

        public bool ApplicationLogGCP(string typeLog, string serviceAccount)
        {
            //var daGCP = new DataAccessGCP(_iconfig);
            //var auditConfig = daGCP.ConfigurationLogGCP(typeLog, serviceAccount);
            return true;
        }
    }
}