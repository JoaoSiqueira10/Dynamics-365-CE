using Microsoft.SqlServer.Server;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PluginsTreinamento
{
    public class PluginAccountPreOperation : IPlugin
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

            // verifica se contem dados para o destino e se corresponde a uma Entity
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                // variavel para herdar o conteudo do atributo telephone1 do contesto
                Entity entidadeContexto = (Entity)context.InputParameters["Target"];

                if (entidadeContexto.LogicalName == "account") // verifica se a entidade do contexto é account
                {
                    if (entidadeContexto.Attributes.Contains("telephone1")) // verifica se contem o atributo telephone1
                    {
                        // variavel para herdar o conteudo do atributo telephone1 do contexto
                        var phone1 = entidadeContexto["telephone1"].ToString();

                        // variavel string contendo FetchXML para consulta de contato
                        string FetchContact = @"?xml version='1.0'?>" + 
                            "< fetch distinct='false' mapping='logical'  output-format='xml-platform' version='1.0'>" +
                                "< entity name = 'account' >" +
                                    "< attribute name = 'fullname' />" +
                                    "< attribute name = 'telephone1' />" +
                                    "< attribute name = 'accountid' />" +
                                    "< order descending='false' attribute='fullname'/>" +
                                    "< filter type = 'and'/>" +
                                        "<condition attribute='telephone1' value='" + phone1 + "' operator='eq'/>" +
                                    "</filter>" +
                                "</entity >" +
                            "</fetch > ";

                        trace.Trace("FetchContact: " + FetchContact); // armazena informacoes de LOG

                        // variavel contendo o retorno da consulta FetchXML
                        var primarycontact = serviceAdmin.RetrieveMultiple(new FetchExpression(FetchContact));

                        if(primarycontact.Entities.Count > 0) // verifica se contem entidade
                        {
                            // para cada entidade retornada atribui a variavel entityContact
                            foreach (var entityContact in primarycontact.Entities)
                            {
                                // atribui referencia de entidade para o atributo primarycontactid (contado primario)
                                entidadeContexto["primarycontact"] = new EntityReference("contact", entityContact.Id);
                            }
                        }
                    }
                }                
            }
        }
    }
}
