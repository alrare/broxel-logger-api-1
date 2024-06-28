using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.GoogleCloudLogging;


namespace Broxel.Logger.Api.Infraestructure
{

    public class DataAccessGCP
    {
        private readonly IConfiguration _iconfig;

        public DataAccessGCP(IConfiguration iconfig)
        {
            _iconfig = iconfig;
        }
       
       
        public bool ConfigurationLogGCP(string typeLog, string messageLog, string serviceAccount)
        {
            //Configuracion Log GCP     
            var config = new GoogleCloudLoggingSinkOptions();
            var keyFile = string.Empty;

            switch (typeLog)
            {
                case "Information":
                    serviceAccount = "GoogleCloudLoggingAudit";
                    keyFile = _iconfig.GetValue<string>(serviceAccount+":KeyFile");
                    break;

                case "Warning":
                    serviceAccount = "GoogleCloudLoggingApplication";
                    keyFile = _iconfig.GetValue<string>(serviceAccount+":KeyFile");
                    break;
            }

            _iconfig.GetSection(serviceAccount.ToString()).Bind(config);
            var resource = Environment.CurrentDirectory + "\\" + keyFile;

            if (resource != null)
            {
                using var sr = new StreamReader(resource);
                config.GoogleCredentialJson = sr.ReadToEnd();
            }



            //Configuracion Serilog
            Func<LoggerConfiguration, LoggerConfiguration>? configure = null;
            var log = new LoggerConfiguration();
            if (configure != null)
            {
                log = configure(log);
            }

            Log.Logger = log
                .Enrich.FromLogContext()
                .Enrich.WithCorrelationId()
                .Enrich.WithExceptionDetails()
                .WriteTo.GoogleCloudLogging(config)
                .CreateLogger();


            //Registrar log en GCP
            switch (typeLog)
            {
                case "Information":
                    Log.Logger.Information(messageLog);
                    break;
                case "Warning":
                    Log.Logger.Warning(messageLog);
                    break;
                case "Error":
                    Log.Logger.Error(messageLog);
                    break;
            }
            return true;
        }

    }
}