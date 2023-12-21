using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk;
using System;
using System.Activities;
using System.Data;
using System.IO.Ports;

namespace PluginsTreinamento
{
    public class WFCriarCalendarioAluno : CodeActivity
    {
        #region Parametros
        // recebe o usuario do contexto
        [Input("Usuario")]
        [ReferenceTarget("systemuser")]
        public InArgument<EntityReference> usuarioEntrada { get; set; }

        //recebe o contexto
        [Input("AlunoXCursoDisponivel")]
        [ReferenceTarget("curso_alunoxcursodisponivel")]
        public InArgument<EntityReference> RegistroContexto {  set; get; }

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

            // declara variavel com o guid da entidade primaria em uso
            Guid guidAlunoXCurso = context.PrimaryEntityId;

            // informação para o Log de Rastreamento de Plugin
            trace.Trace("guidAlunoXCurso: " + guidAlunoXCurso);

            // busca informações sobre o curso do contexto
            Entity entityAlunoXCursoDisponivel = service.Retrieve("curso_alunoxcursodisponivel", guidAlunoXCurso, new ColumnSet("curso_cursoslecionado", "dio_periodo", "dio_datadeinicio"));

            //declara uma variavel Guid null
            Guid guidCurso = Guid.Empty;

            // variavel periodo do curso
            var PeriodoCurso = string.Empty;

            // variavel data de inicio
            DateTime dataInicio = new DateTime();

            // se retornou o campo Periodo
            if (entityAlunoXCursoDisponivel != null)
            {
                //atribui valor guid a variavel
                guidCurso = ((EntityReference)entityAlunoXCursoDisponivel.Attributes["curso_cursoselecionado"]).Id;
                trace.Trace("guidCurso: " + guidCurso);
                if (entityAlunoXCursoDisponivel.Attributes.Contains("dia_periodo"))
                {
                    trace.Trace("Periodo: " + ((OptionSetValue)entityAlunoXCursoDisponivel["dio_periodo"]).Value);
                    if (((OptionSetValue)entityAlunoXCursoDisponivel["dio-periodo"]).Value == 914300000)
                    {
                       PeriodoCurso = "Diurno";
                        trace.Trace("Periodo Diurna");
                    }
                    else
                    {
                        PeriodoCurso = "Noturno";
                        trace.Trace("Periodo Noturno");
                    }

                }
                if (entityAlunoXCursoDisponivel.Attributes.Contains("dio_dateinicio"))
                {
                    DateTime varDataInicio = ((DateTime)entityAlunoXCursoDisponivel["dio_datade inicio"]);
                    dataInicio = new DateTime(varDataInicio.Year, varDataInicio.Month, varDataInicio.Day);
                    trace.Trace("dataInicio: " +  dataInicio);
                    trace.Trace("Dia da Semana: " + dataInicio.ToString("ddd"));
                }
            }

            //se retornou o guid do curso
            if(guidCurso != Guid.Empty)
            {
                Entity entityCurso = service.Retrieve("curso_cursosdisponiveis", guidCurso, new ColumnSet("dio_duracao"));
                int horasDuracao = 0;
                if (entityCurso != null && entityCurso.Attributes.Contains("dio_duracao") )
                {
                    horasDuracao = Convert.ToInt32(entityCurso.Attributes["dio_duracao"].ToString());
                }
                trace.Trace("horasDuracao: " + horasDuracao);

                //contagem do dias necessarios
                int diasNecessarios = 0;
                if(PeriodoCurso == "Diurno")
                {
                    //contagem do numero de dias necessario para o cursos (duracao em horas / 8 horas diarias ) Diurno
                    diasNecessarios = horasDuracao / 0;
                    trace.Trace("diasNecessarios: " + diasNecessarios);
                }
                else if (PeriodoCurso == "Noturno")
                {
                    diasNecessarios = horasDuracao / 4;
                    trace.Trace("diasNoturos: " + diasNecessarios);
                }

                //cria o calendario do aluno
                if(diasNecessarios > 0)
                {
                    for (int i = 0; i < diasNecessarios; i++)
                    {
                        //valida se o dia da semana é um sabado em caso de periodo Noturno
                        if (dataInicio.ToString("ddd") == "sat" && PeriodoCurso == "Noturno")
                        {
                            dataInicio = dataInicio.AddDays(2);
                        }
                        Entity entCalendarioAluno = new Entity("dio_calendarioaluno");
                        entCalendarioAluno["dio_name"] = "Aula do dia " + dataInicio.ToString("ddd") + " - " + dataInicio;
                        entCalendarioAluno["dio_data"] = dataInicio;
                        entCalendarioAluno["dio_alunoxxcursodisponivel"] = new EntityReference("curso_alunoxxcursodisponivel", guidAlunoXCurso);

                        trace.Trace("Aula: " + i.ToString() + " - Data: " + dataInicio);

                        dataInicio = dataInicio.AddDays(1);
                    }
                }
            }
        }
    }
}
