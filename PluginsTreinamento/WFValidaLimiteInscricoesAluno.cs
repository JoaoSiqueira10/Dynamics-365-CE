using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;

namespace PluginsTreinamento
{
    public class WFValidaLimiteInscricoesAluno : CodeActivity
    {
        #region Parametros
        // recebe o usuario do contexto
        [Input("Usuario")]
        [ReferenceTarget("systemuser")]
        public InArgument<EntityReference> usuarioEntrada {  get; set; }

        [Output("Saida")]
        public OutArgument<string> saida { get; set; }
        #endregion
        protected override void Execute(CodeActivityContext executionContext)
        {
            // create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService trace = executionContext.GetExtension<ITracingService>();

            // informação para o Log de Rastreamento de Plugin
            trace.Trace("curso_alunoxcursodisponivel: " + context.PrimaryEntityId);
            
            // declara variavel com o guid da entidade primaria em uso
            Guid guidAlunoXCurso = context.PrimaryEntityId;

            // informação para o Log de Rastreamento de Plugin
            trace.Trace("guidAlunoXCurso: " + guidAlunoXCurso);

            String fetchAlunoXCursos = "< fetch distinct='false' mapping='logical'  output-format='xml-platform' version='1.0'>";
            fetchAlunoXCursos += "<entity name='curso_alunoxcursodisponivel'>";
            fetchAlunoXCursos += "<attibute name='curso_alunoxcursodisponivel'/>";
            fetchAlunoXCursos += "<attibute name='curso_name' />";
            fetchAlunoXCursos += "<attibute name='curso_emcurso' />";
            fetchAlunoXCursos += "<attibute name='createdon' />";
            fetchAlunoXCursos += "<attibute name='curso_aluno' />";
            fetchAlunoXCursos += "<order descending= 'false' attribute= 'curso_nome' />";
            fetchAlunoXCursos += "<filter type= 'and'>";
            fetchAlunoXCursos += "<condition attribute = 'curso_alunoxcursodisponivel' value = '" + guidAlunoXCurso + "'uitype = 'curso_alunoxcursodisponivel'";
            fetchAlunoXCursos += "</filter>";
            fetchAlunoXCursos += "</entity>";
            fetchAlunoXCursos += "</fetch>";
            trace.Trace("fetchAlunoXCurso: " + fetchAlunoXCursos);

            var entityAlunoXCursos = service.RetrieveMultiple(new FetchExpression(fetchAlunoXCursos));
            trace.Trace("entityAlunoXCursos: " + fetchAlunoXCursos);

            Guid guidAluno = Guid.Empty;
            foreach (var item in entityAlunoXCursos.Entities)
            {
                string nomeCurso = item.Attributes["curso_nome"].ToString();
                trace.Trace("nomeCurso: " + nomeCurso);

                var entityAluno = ((EntityReference)item.Attributes["curso_aluno"]).Id;
                guidAluno = ((EntityReference)item.Attributes["curso_aluno"]).Id;
                trace.Trace("entityAluno: " + entityAluno);
            }

            String fetchAlunoXCursosQtde = "< fetch distinct='false' mapping='logical'  output-format='xml-platform' version='1.0'>";
            fetchAlunoXCursosQtde += "<entity name='curso_alunoxcursodisponivel'>";
            fetchAlunoXCursosQtde += "<attibute name='curso_alunoxcursodisponivel'/>";
            fetchAlunoXCursosQtde += "<attibute name='curso_name' />";
            fetchAlunoXCursosQtde += "<attibute name='curso_emcurso' />";
            fetchAlunoXCursosQtde += "<attibute name='createdon' />";
            fetchAlunoXCursosQtde += "<attibute name='curso_aluno' />";
            fetchAlunoXCursosQtde += "<order descending= 'false' attribute= 'curso_nome' />";
            fetchAlunoXCursosQtde += "<filter type= 'and'>";
            fetchAlunoXCursosQtde += "<condition attribute = 'curso_alunoxcursodisponivel' value = '" + guidAlunoXCurso + "'uitype = 'curso_alunoxcursodisponivel'";
            fetchAlunoXCursosQtde += "</filter>";
            fetchAlunoXCursosQtde += "</entity>";
            fetchAlunoXCursosQtde += "</fetch>";
            trace.Trace("fetchAlunoXCursoQtde: " + fetchAlunoXCursosQtde);

            var entityAlunoXCursosQtde = service.RetrieveMultiple(new FetchExpression(fetchAlunoXCursosQtde));
            trace.Trace("entityAlunoXCursosQtde: " + entityAlunoXCursosQtde.Entities.Count);
            if (entityAlunoXCursosQtde.Entities.Count > 2)
            {
                saida.Set(executionContext, "Aluno excedeu limite de cursos ativos!");
                trace.Trace("Aluno excedeu limite de cursos ativos!");
                throw new InvalidPluginExecutionException("Aluno excedeu o limite de cursos!");
            }
        }
    }
}
