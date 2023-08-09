using System.Xml.Linq;
using OptimaJet.Workflow.Core.Builder;
using OptimaJet.Workflow.Core.Parser;
using OptimaJet.Workflow.Core.Runtime;
using OptimaJet.Workflow.DbPersistence;

namespace Sample;

public static class Extension
{
    public static void AddWorkflowRuntime(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddSingleton<WorkflowRuntime>(_ =>
        {
            var runtime = new WorkflowRuntime();
            var provider = new MSSQLProvider(configuration.GetConnectionString("MSSQL"));
            var builder = new WorkflowBuilder<XElement>(provider, new XmlWorkflowParser(), provider).WithDefaultCache();
            
            runtime
                .WithBuilder(builder)
                .WithPersistenceProvider(provider)
                .AsSingleServer()
                .Start();

            runtime.AddLocalScheme("WorkflowSchema");
            return runtime;
        });
    }

    private static void AddLocalScheme(this WorkflowRuntime runtime, string schemeCode)
    {
        var schemeSource = File.ReadAllText($"{schemeCode}.xml");
        var pd = runtime.Builder.Parse(schemeSource);
        _ = runtime.Builder.SaveProcessSchemeAsync(schemeCode, pd).Result;
    }
}