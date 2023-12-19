using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PluginsTreinamento
{
    public class PluginAccountPostOperation : IPlugin
    {
        // metodo requierido para execução do Plugin recebendo como parametro os dados do provedor de serviços
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

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    // variavel para herdar o conteudo do atributo telephone1 do contesto
                    Entity entidadeContexto = (Entity)context.InputParameters["Target"];

                    if (!entidadeContexto.Contains("telephone1")) //verifica se o atributo telephone1não esta presente no contexto
                    {
                        throw new InvalidPluginExecutionException("Campo Telefone principal é obrigatorio!"); //exibe Exception de Erro
                    }

                    // variavel para nova entidade TASK vazia
                    var Task = new Entity("task");

                    // atribuição dos atributos para novo registo da entidade TASK
                    Task.Attributes["ownerid"] = new EntityReference("systemuser", context.UserId);
                    Task.Attributes["regardingobjectid"] = new EntityReference("account", context.PrimaryEntityId);
                    Task.Attributes["subject"] = "Visite nosso site: " + entidadeContexto["websiteurl"];
                    Task.Attributes["description"] = "TASK criada via Plugin Post Operation";

                    serviceAdmin.Create(Task);
                }
            }catch(InvalidPluginExecutionException ex) //em caso de falha
            {
                throw new InvalidPluginExecutionException("Erro ocorrido: " + ex.Message); // exibe Exception
            }
        }
        
    }

}
