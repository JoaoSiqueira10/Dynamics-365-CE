using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MeuPrimeiroProjeto
{
    internal class Conexao
    {
        private static CrmServiceClient crmServiceClientTrinamento; // variável do tipo CrmServiceCliente
        
        public CrmServiceClient ObterConexao() // método que devolve uma CrmServiceCliente
        {
            // variável contendo a ConnectionString
            var connectionStringCRM = @"AuthType=OAuth;
            Username = userdyn365@treindio.onmicrosoft.com;
            Password = Pass@word99; SkipDiscovery = True;
            AppId = 51f81489-12ee-4a9e-aaae-a2591f45987d;
            RedirectUri = app://58145B91-0C36-4500-8554-080854F2AC97;
            Url = https://org07de2ee3.crm2.dynamics.com/main.aspx;";
            if (crmServiceClientTrinamento == null) // verifica se a variável crmServiceClientTrinamento está vazia
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // define o protocolo segurança
                crmServiceClientTrinamento = new CrmServiceClient(connectionStringCRM); // executa a conexão
            }
            return crmServiceClientTrinamento; // retorna a conexão para o método chamador
        }
    }
}
