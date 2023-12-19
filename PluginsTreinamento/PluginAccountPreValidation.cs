using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginsTreinamento
{
    public class PluginAccountPreValidation : IPlugin
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

            // varivel do tipo Entity vazia
            Entity entidadeContexto = null;

            if (context.InputParameters.Contains("Target")) // varifica se contem dados para o destino
            {
                entidadeContexto = (Entity)context.InputParameters["Target"]; // atribui o contexto da entidade para variavel

                trace.Trace("Entidade do Contexto: " + entidadeContexto.Attributes.Count); // armazena informações de LOG

                if(entidadeContexto == null) // verifica se a entidade do contexto esta vazia
                {
                    return; // caso verdadeira retorna sem nada para executar
                }

                if (!entidadeContexto.Contains("telephone1")) // Verifica se o atributo telephone não esta presente no contexto
                {
                    throw new InvalidPluginExecutionException("Campo Telefone principal é obrigatório!"); // exibe Exception de Erro
                }
            }
        }
    }
}
