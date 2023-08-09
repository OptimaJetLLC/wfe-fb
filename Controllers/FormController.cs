using Microsoft.AspNetCore.Mvc;
using OptimaJet.Workflow.Core.Model;
using OptimaJet.Workflow.Core.Persistence;
using OptimaJet.Workflow.Core.Runtime;
using Sample.Models;

namespace Sample.Controllers
{
    [ApiController]
    [Route("API")]
    public class FormController : ControllerBase
    {
        private readonly WorkflowRuntime _runtime;

        public FormController(WorkflowRuntime runtime)
        {
            _runtime = runtime;
        }

        [HttpGet("processes")]
        public async Task<FormData> GetProcesses()
        {
            var grid = await _runtime.PersistenceProvider.GetProcessInstancesAsync();
            return new(grid);
        }
        
        [HttpPost("process")]
        public async Task<Guid> CreateProcess()
        {
            var processId = Guid.NewGuid();
            await _runtime.CreateInstanceAsync("WorkflowSchema", processId);
            return processId;
        }

        [HttpDelete("process")]
        public async Task<int> DeleteProcess([FromBody] Guid[] ids)
        {
            await Task.WhenAll(ids.Select(id => _runtime.DeleteInstanceAsync(id)));
            return ids.Length;
        }

        [HttpGet("process/{id:guid}")]
        public async Task<ProcessParams> GetProcessParams(Guid id)
        {
            var process = await _runtime.GetProcessInstanceAndFillProcessParametersAsync(id);
            return new(
                process.GetParameter<string>(nameof(ProcessParams.Title)),
                process.GetParameter<string>(nameof(ProcessParams.FirstName)),
                process.GetParameter<string>(nameof(ProcessParams.LastName)),
                process.GetParameter<int?>(nameof(ProcessParams.Type)),
                process.GetParameter<string>(nameof(ProcessParams.Comment))
            );
        }

        [HttpPut("process/{id:guid}")]
        public async Task<bool> UpdateProcessParams(Guid id, [FromBody] ProcessParams? processParams)
        {
            if (processParams is null) return false;
            var process = await _runtime.GetProcessInstanceAndFillProcessParametersAsync(id);

            process.SetParameter(nameof(ProcessParams.Title), processParams.Title, ParameterPurpose.Persistence);
            process.SetParameter(nameof(ProcessParams.FirstName), processParams.FirstName, ParameterPurpose.Persistence);
            process.SetParameter(nameof(ProcessParams.LastName), processParams.LastName, ParameterPurpose.Persistence);
            process.SetParameter(nameof(ProcessParams.Type), processParams.Type, ParameterPurpose.Persistence);
            process.SetParameter(nameof(ProcessParams.Comment), processParams.Comment, ParameterPurpose.Persistence);

            await process.SaveAsync(_runtime);
            return true;
        }
    }
}