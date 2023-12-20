using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PluginsTreinamento
{
    public class ActionCEP : IPlugin
    {
        // metodo requierido para execução do Plugin recebendo como parametro os dados do provedor de serviços
        public void Execute(IServiceProvider serviceProvider)
        {
            // Variavel contendo o contexto da execução
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Variavel contendo o Service Factory da Organização
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            // Variavel contendo o Service Admin que estabelece os serviços de conexão com o dataverse
            IOrganizationService serviceAdmin = serviceFactory.CreateOrganizationService(null);

            // variavel do Traca que armazena informações de Log
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            var cep = context.InputParameters["CepInput"];
            trace.Trace("Cep informado: " + cep);

            var viaCEPurl = $"https://viacep.com.br/ws/{cep}/json/";
            string result = string.Empty;
            using(WebClient client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application.json";
                client.Encoding = Encoding.UTF8;
                result = client.DownloadString(viaCEPurl);
            }
            context.OutputParameters["ResultadoCEP"] = result;

            trace.Trace("Resultado: " + result);
        }
    }
}
