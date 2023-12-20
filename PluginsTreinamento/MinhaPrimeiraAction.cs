using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginsTreinamento
{
    public class MinhaPrimeiraAction : IPlugin
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

            trace.Trace("Minha Primeira Action executada com sucesso e criando Lead no Dataverse!");

            Entity entLead = new Entity("Lead");
            entLead["subject"] = "Lead criado via action";
            entLead["firstname"] = "Primeiro Nome";
            entLead["lastname"] = "Lastname Lead";
            entLead["mobiletelephone"] = "920220720";
            entLead["ownerid"] = new EntityReference("systemuser", context.UserId);
            Guid guidLead = serviceAdmin.Create(entLead);
            trace.Trace("Lead criado: " + guidLead);


        }
    }
}
