using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectDataAccess.DbModel;
using RiskAPI.Helpers;
using RiskAPI.Models;
using RiskAPI.Repository;

namespace RiskAPI.Controllers
{
    [Route("api/[controller]/v1")]
    public class DataContainerRiskController : ControllerBase
    {
        private readonly IDataContainerRiskService _dataContainerRiskService;

        public DataContainerRiskController(IDataContainerRiskService dataContainerRiskService) { _dataContainerRiskService = dataContainerRiskService; }

        [HttpGet]
        [Route("getDataContainerList")]
        public async Task<ActionResult> getDataContainerList(string containerReference)
        {
            HttpResult<List<DataContainerList>> getDataContainerList = new();
            try
            {
                getDataContainerList = await _dataContainerRiskService.getDataContainerList(containerReference);
                if (getDataContainerList.Root != null) return Ok(getDataContainerList);
                else return BadRequest(getDataContainerList);
            }
            catch
            {
                getDataContainerList.Root = null;
                getDataContainerList.Code = System.Net.HttpStatusCode.InternalServerError;
                getDataContainerList.Message = Messages.DataContainerRiskControllerMessgaes.getErrorMessage;
                return StatusCode(StatusCodes.Status500InternalServerError, getDataContainerList);
            }
        }

        [HttpGet]
        [Route("getDataRiskList")]
        public async Task<ActionResult> getDataRiskList(int dataContainerId)
        {
            //HttpResult<List<DataRiskListbyDCID>> getDataRiskListbyDataContainerId = new();
            HttpResult<DataRiskListbyDCID> getDataRiskListbyDataContainerId = new();
            try
            {
                getDataRiskListbyDataContainerId = await _dataContainerRiskService.getDataRiskListbyDataContainerId(dataContainerId);
                if (getDataRiskListbyDataContainerId.Root != null) return Ok(getDataRiskListbyDataContainerId);
                else return BadRequest(getDataRiskListbyDataContainerId);
            }
            catch
            {
                getDataRiskListbyDataContainerId.Root = null;
                getDataRiskListbyDataContainerId.Code = System.Net.HttpStatusCode.InternalServerError;
                getDataRiskListbyDataContainerId.Message = Messages.DataContainerRiskControllerMessgaes.getErrorMessage;
                return StatusCode(StatusCodes.Status500InternalServerError, getDataRiskListbyDataContainerId);
            }
        }

        [HttpPost]
        [Route("modifyDataContainer")]
        public async Task<ActionResult> operationonDataContainer([FromBody] DataContainerRequest dataContainerRequest)
        {
            HttpResult<int> dataContainerResponse = new();
            try
            {
                if (!ModelState.IsValid)
                {
                    dataContainerResponse.Root = -1;
                    dataContainerResponse.Code = System.Net.HttpStatusCode.BadRequest;
                    dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.BadRequestMessage;
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                dataContainerResponse = await _dataContainerRiskService.operationonDataContainer(dataContainerRequest);
                if (dataContainerResponse.Root > 0) return Ok(dataContainerResponse);
                else return BadRequest(dataContainerResponse);
            }
            catch
            {
                dataContainerResponse.Root = -1;
                dataContainerResponse.Code = System.Net.HttpStatusCode.InternalServerError;
                dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.addErrorMessage;
                return StatusCode(StatusCodes.Status500InternalServerError, dataContainerResponse);
            }
        }

        [HttpPost]
        [Route("modifyDataRisk")]
        public async Task<ActionResult> operationonDataRisk([FromBody] DataRiskRequest dataRiskRequest)
        {
            HttpResult<int> dataRiskResponse = new();
            try
            {
                if (!ModelState.IsValid)
                {
                    dataRiskResponse.Root = -1;
                    dataRiskResponse.Code = System.Net.HttpStatusCode.BadRequest;
                    dataRiskResponse.Message = Messages.DataContainerRiskControllerMessgaes.BadRequestMessage;
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                dataRiskResponse = await _dataContainerRiskService.operationonDataRisk(dataRiskRequest);
                if (dataRiskResponse.Root > 0) return Ok(dataRiskResponse);
                else return BadRequest(dataRiskResponse);
            }
            catch
            {
                dataRiskResponse.Root = -1;
                dataRiskResponse.Code = System.Net.HttpStatusCode.InternalServerError;
                dataRiskResponse.Message = Messages.DataContainerRiskControllerMessgaes.addErrorMessage;
                return StatusCode(StatusCodes.Status500InternalServerError, dataRiskResponse);
            }
        }
    }
}
