using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginsTreinamento
{
    public class PluginsAsyncPostOperation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try //tentativa de execucao
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

                    for (int i = 0; i < 10; i++)
                    {
                        // varivel para nova entidade Contato vazia
                        var Contact = new Entity("contact");

                        // atribuição dos atributos para novo registro da entidade Contato
                        Contact.Attributes["firstname"] = "Contato Assinc vinculado a Conta";
                        Contact.Attributes["lastname"] = entidadeContexto["name"];
                        Contact.Attributes["parentcustomerid"] = new EntityReference("account", context.PrimaryEntityId);
                        Contact.Attributes["ownerid"] = new EntityReference("systemuser", context.UserId);

                        trace.Trace("firstname: " + Contact.Attributes["firstname"]);

                        serviceAdmin.Create(Contact); // executa metodo Create para entidade Contato
                    }
                }
            }catch(InvalidPluginExecutionException ex) //em caso de falha
            {
                throw new InvalidPluginExecutionException("Erro ocorrido: " + ex.Message); // exibe a Exception
            }
            
        }
    }
}
