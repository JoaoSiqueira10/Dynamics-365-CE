using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace MeuPrimeiroProjeto
{
    internal class DataModel
    {
        public void FetchXML(CrmServiceClient crmService) // recebe como parâmetro a conexão
        {
            string query = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                <entity name='account'>
                                    <attribute name='name' />
                                    <attribute name='telephone1' />
                                    <attribute name='accountid' />
                                    <attribute name='emailaddress1' />
                                    <order attribute='name' descending='false' />
                                </entity>
                            </fetch>"; // declara variável do tipo String contendo o FechXML da consulta

            // declara variável do tipo EntityCollection e executa o método RetrieveMultiple enviando como parâmetro a variável query 
            EntityCollection colecao = crmService.RetrieveMultiple(new FetchExpression(query));
            foreach (var item in colecao.Entities) // para cada entidade (registro) armazenará na variável item
            {
                Console.WriteLine(item["name"]); // exibe o atributo name
                if (item.Attributes.Contains("telephone1")) // verifica se retornou o atributo telephone1
                {
                    Console.WriteLine(item["telephone1"]); // exibe o atributo telephone
                }
            }
            Console.ReadKey(); // aguarda o usuário pressionar qualquer tecla para continuar
        }

        public void Create(CrmServiceClient crmService) // recebe como parâmetro a conexão
        {
            Guid newRecord = new Guid(); // declara variável do tipo Guid
            Entity newEntity = new Entity("account"); // declara variável do Entity a partir da entidade Account

            newEntity.Attributes.Add("name", "Conta Criada em Treinamento - " + DateTime.Now.ToString()); // nome
            newEntity.Attributes.Add("telephone1", "11985326471"); // telefone
            newEntity.Attributes.Add("emailaddress1", "contato@provedor.com"); // e-mail

            // executa o método Create e armazena na variável newRecord o Guid retornado do novo registro no Dataverse.
            newRecord = crmService.Create(newEntity);
            if (newRecord != Guid.Empty) // verificar se a variável newRecord contém um Guid
            {
                Console.WriteLine("Id do Registro criado : " + newRecord); // exibe informação no console do usuário
            }
            else // caso contrário
            {
                Console.WriteLine("newRecord não criado!"); // exibe informação que não foi criado o registro
            }
            Console.ReadKey(); // aguarda o usuário pressionar qualquer tecla para continuar
        }

        public void UpdateEntity(CrmServiceClient crmService, Guid guidAccount) // recebe como parâmetro a conexão e o Guid do registro a ser alterado
        {
            // declara variável do Entity a partir da entidade Account contendo o registro referente ao Guid informado
            Entity upEntity = new Entity("account", guidAccount);

            upEntity["name"] = "Luis Prado Update"; // atributo name com novo conteúdo
            upEntity["telephone1"] = "11978526941"; // atributo telephone1 com novo conteúdo
            upEntity["emailaddress1"] = "contato@meuprovedor.com"; // atributo emailaddress1 com novo conteúdo
            crmService.Update(upEntity); // executa o método Update a partir da conexão estabelecida
            Console.WriteLine("Update Finalizado");
            Console.ReadKey();
        }

        public void DeleteEntity(CrmServiceClient crmService, Guid guidAccount) // recebe como parâmetro a conexão e o Guid do registro a ser alterado*
        {
            // variável que recebe o conteúdo da entidade account referente ao Guid recebido como parâmetro
            Entity entityDelete = crmService.Retrieve("account", guidAccount, new ColumnSet("name"));

            Console.WriteLine("=====================================================================");
            Console.WriteLine("Confirma a exclusão da conta: " + entityDelete["name"] + "? (SIM/NAO)");
            var confirm = Console.ReadLine();

            if (confirm == "SIM" || confirm == "sim")
            {
                if (entityDelete.Attributes.Count > 0) // verifica se contém attributos
                {
                    crmService.Delete("account", guidAccount); // executa o método Delete a partir da conexão estabelecida
                    Console.WriteLine("Conta excluída!"); // exibe informação na console que a conta foi excluída
                    Console.WriteLine("=====================================================================");
                    Console.WriteLine("Pressione qualquer tecla para continuar");
                    Console.ReadKey(); // aguarda o usuário pressionar qualquer tecla para continuar

                }
            }
        }
    }
}
