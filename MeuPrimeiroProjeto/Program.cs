using System;

namespace MeuPrimeiroProjeto
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var crmService = new Conexao().ObterConexao();

            DataModel model = new DataModel();

            model.FetchXML(crmService);
            model.Create(crmService);
            model.UpdateEntity(crmService, new Guid("string copiada da URL"));
            model.DeleteEntity(crmService, new Guid("string copiada da URL"));
        }
    }
}
