using RiskAPI.Models;
using ProjectDataAccess.DbModel;
using Microsoft.EntityFrameworkCore;
using System.Net;
using RiskAPI.Helpers;
using System.Collections;
using System.Collections.Immutable;
using Microsoft.OpenApi.Models;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace RiskAPI.Repository
{
    public interface IDataContainerRiskService
    {
        Task<HttpResult<List<DataContainerList>>> getDataContainerList(string containerReference);
        //Task<HttpResult<List<DataRiskListbyDCID>>> getDataRiskListbyDataContainerId(int ContainerId);
        Task<HttpResult<DataRiskListbyDCID>> getDataRiskListbyDataContainerId(int ContainerId);
        Task<HttpResult<int>> operationonDataContainer(DataContainerRequest dataContainerRequest);
        Task<HttpResult<int>> operationonDataRisk(DataRiskRequest dataRiskRequest);
    }
    public class DataContainerRiskService : IDataContainerRiskService
    {
        public readonly Risk_DbContext _risk_DbContext;
        public DataContainerRiskService(Risk_DbContext risk_DbContext) { _risk_DbContext = risk_DbContext; }
        public async Task<HttpResult<List<DataContainerList>>> getDataContainerList(string containerReference)
        {
            HttpResult<List<DataContainerList>> dataContainerResponse = new();
            try
            {
                //var getData = await (from d in _risk_DbContext.v_ContainerID
                //                     join e in _risk_DbContext.v_DataContainerID on d.ContainerId equals e.ContainerID
                //                     join a in _risk_DbContext.v_DataContainer_CFG on e.dataContainerID equals a.DataContainerId
                //                     join b in _risk_DbContext.v_DataContainer_VER on new { a = a.DataContainerId, b = a.id_version }
                //                     equals new { a = b.DataContainerId, b = b.id_version }
                //                     where d.ContainerReference == containerReference 
                //                     && (b.updated_by != "DELETED" || b.updated_on==null) 
                //                     &&( b.submit_by != "DELETED" || b.submit_by==null)
                //                     select new DataContainerList
                //                     {
                //                         DataContainerID = a.DataContainerId,
                //                         id_version = a.id_version,
                //                         Title = a.Title,
                //                         Description = a.Description,
                //                         approved = a.approved,
                //                         ContainerReference = d.ContainerReference,
                //                         data_Version = b
                //                     }).ToListAsync();
                if (containerReference != null)
                {
                    var getData = await (from d in _risk_DbContext.v_ContainerID
                                         join e in _risk_DbContext.v_DataContainerID on d.ContainerId equals e.ContainerID
                                         join a in _risk_DbContext.v_DataContainer_CFG on e.dataContainerID equals a.DataContainerId
                                         join b in _risk_DbContext.v_DataContainer_VER on new { a = a.DataContainerId, b = a.id_version }
                                         equals new { a = b.DataContainerId, b = b.id_version }
                                         where d.ContainerReference == containerReference
                                            && (b.updated_by != "DELETED" || b.updated_on == null)
                                          && (b.submit_by != "DELETED" || b.submit_by == null)
                                         select new DataContainerList
                                         {
                                             DataContainerID = a.DataContainerId,
                                             id_version = a.id_version,
                                             Title = a.Title,
                                             Description = a.Description,
                                             approved = a.approved,
                                             ContainerReference = d.ContainerReference
                                         }).ToListAsync();

                    foreach (var dd in getData)
                    {
                        var sss = _risk_DbContext.v_DataContainer_VER.Where(x => x.DataContainerId == dd.DataContainerID).OrderByDescending(x => x.id_version).FirstOrDefaultAsync();
                        if (sss.Result != null)
                        {
                            var sssq = await _risk_DbContext.v_DataContainer_VER.Where(x => (x.DataContainerId == dd.DataContainerID) &&
                          (x.id_version == sss.Result.id_version) ).FirstOrDefaultAsync();

                            //&& (x.updated_by != "DELETED" || x.updated_on == null)
                            //                   && (x.submit_by != "DELETED" || x.submit_by == null)
                            dd.data_Version = sssq;
                        }
                        else dd.data_Version = null;
                    }

                    dataContainerResponse.Root = getData;
                    if (getData != null && getData.Count() > 0)
                    {
                        dataContainerResponse.Code = HttpStatusCode.OK;
                        dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.getDataContainerRiskMessage;
                    }
                    else
                    {
                        dataContainerResponse.Code = HttpStatusCode.NoContent;
                        dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.NoDataContainerRiskMessage;
                    }
                }
                else
                {
                    dataContainerResponse.Root = null;
                    dataContainerResponse.Code = HttpStatusCode.BadRequest;
                    dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.contRefInputMessage;
                }
            }
            catch
            {
                dataContainerResponse.Root = null;
                dataContainerResponse.Code = HttpStatusCode.BadRequest;
                dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.BadRequestMessage;
            }
            return dataContainerResponse;
        }
        //public async Task<HttpResult<List<DataRiskListbyDCID>>> getDataRiskListbyDataContainerId(int dataContainerId)
        public async Task<HttpResult<DataRiskListbyDCID>> getDataRiskListbyDataContainerId(int dataContainerId)
        {
            //HttpResult<List<DataRiskListbyDCID>> dataRiskResponse = new();
            HttpResult<DataRiskListbyDCID> dataRiskResponse = new();
            try
            {
                if (dataContainerId > 0)
                {
                    var dataList = await (from a in _risk_DbContext.v_DataContainer_CFG
                                          join b in _risk_DbContext.v_DataContainer_VER on new { a = a.DataContainerId, b = a.id_version }
                                          equals new { a = b.DataContainerId, b = b.id_version }
                                          where a.DataContainerId == dataContainerId
                                           && (b.updated_by != "DELETED" || b.updated_on == null)
                                           && (b.submit_by != "DELETED" || b.submit_by == null)
                                          select new DataRiskListbyDCID
                                          {
                                              DataContainerID = a.DataContainerId,
                                              id_version = a.id_version,
                                              Title = a.Title,
                                              Description = a.Description,
                                              approved = a.approved
                                          }).OrderByDescending(x => x.id_version).FirstOrDefaultAsync();

                    //var dataList = await (from a in _risk_DbContext.v_DataContainer_CFG
                    //                      where a.DataContainerId == dataContainerId
                    //                      select new DataRiskListbyDCID
                    //                      {
                    //                          DataContainerID = a.DataContainerId,
                    //                          id_version = a.id_version,
                    //                          Title = a.Title,
                    //                          Description = a.Description,
                    //                          approved = a.approved
                    //                      }).OrderByDescending(x => x.id_version).FirstOrDefaultAsync();

                    //foreach (var lsdj in dataList)
                    //{
                    //    lsdj.dataRiskList = new List<v_DataRisk_CFG>();
                    //    List<v_DataRisk_CFG> datariskList = await _risk_DbContext.v_DataRisk_CFG.Where(x => (x.DataContainerID == lsdj.DataContainerID) &&
                    //    (x.id_version == lsdj.id_version)).ToListAsync();
                    //    foreach (v_DataRisk_CFG asas in datariskList)
                    //        lsdj.dataRiskList.Add(asas);

                    //}

                    if (dataList != null && dataList.DataContainerID > 0)
                    {
                        dataList.data_Version = null;
                        var sss = _risk_DbContext.v_DataContainer_VER.Where(x => x.DataContainerId == dataList.DataContainerID).OrderByDescending(x => x.id_version).FirstOrDefaultAsync();
                        if (sss.Result != null)
                        {
                            var sssq = await _risk_DbContext.v_DataContainer_VER.Where(x => (x.DataContainerId == dataList.DataContainerID) &&
                          (x.id_version == sss.Result.id_version)).FirstOrDefaultAsync();
                            if(sssq!=null)
                            dataList.data_Version = sssq;
                        }

                        List<v_DataRisk_CFG> dataRiskList = new List<v_DataRisk_CFG>();
                        List<v_DataRisk_CFG> datariskList = await _risk_DbContext.v_DataRisk_CFG.Where(x => (x.DataContainerID == dataList.DataContainerID) &&
                        (x.id_version == dataList.id_version)).ToListAsync();
                        dataList.dataRiskList = new List<v_DataRisk_CFG>();
                        foreach (v_DataRisk_CFG asas in datariskList)
                            dataList.dataRiskList.Add(asas);
                    }


                    dataRiskResponse.Root = dataList;
                    if (dataList != null)
                    {
                        dataRiskResponse.Code = HttpStatusCode.OK;
                        dataRiskResponse.Message = Messages.DataContainerRiskControllerMessgaes.getDataContainerRiskMessage;
                    }
                    else
                    {
                        dataRiskResponse.Code = HttpStatusCode.NoContent;
                        dataRiskResponse.Message = Messages.DataContainerRiskControllerMessgaes.NoDataContainerRiskMessage;
                    }
                }
                else
                {
                    dataRiskResponse.Root = null;
                    dataRiskResponse.Code = HttpStatusCode.BadRequest;
                    dataRiskResponse.Message = Messages.DataContainerRiskControllerMessgaes.datacontidInputMessage;
                }
            }
            catch
            {
                dataRiskResponse.Root = null;
                dataRiskResponse.Code = HttpStatusCode.BadRequest;
                dataRiskResponse.Message = Messages.DataContainerRiskControllerMessgaes.BadRequestMessage;
            }
            return dataRiskResponse;
        }
        public async Task<HttpResult<int>> NewDraftPublish(DataContainerRequest dataContainerRequest)
        {
            int dtcntrID = 0, verID = 0, containerID = 0;
            HttpResult<int> dataContainerResponse = new();


            if (dataContainerRequest.DataContainerID == 0 &&
                             dataContainerRequest.Title != string.Empty &&
                             dataContainerRequest.Description != string.Empty)
            {
                try
                {
                    var getData = await (from d in _risk_DbContext.v_ContainerID
                                         join e in _risk_DbContext.v_DataContainerID on d.ContainerId equals e.ContainerID
                                         join a in _risk_DbContext.v_DataContainer_CFG on e.dataContainerID equals a.DataContainerId
                                         where a.Title == dataContainerRequest.Title && d.ContainerReference == dataContainerRequest.ContainerReference
                                         select (a)).FirstOrDefaultAsync();
                    if (getData == null)
                    {
                        _risk_DbContext.Database.BeginTransaction();

                        if (_risk_DbContext.ContainerID.Count() > 0)
                        {
                            var dtRec = _risk_DbContext.ContainerID.FirstOrDefaultAsync(x => x.ContainerReference == dataContainerRequest.ContainerReference);
                            if (dtRec.Result != null)
                            {
                                var res = await _risk_DbContext.ContainerID.OrderByDescending(x => x.ContainerId).LastOrDefaultAsync();
                                if (res != null && res.ContainerId > 0) containerID = res.ContainerId;
                            }
                            else
                            {
                                containerID = _risk_DbContext.ContainerID.ToList().Max(x => x.ContainerId) + 1;
                                _risk_DbContext.ContainerID.Add(new ContainerID
                                {
                                    ContainerId = containerID,
                                    Source = "",
                                    ContainerReference = dataContainerRequest.ContainerReference
                                });
                                await _risk_DbContext.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            _risk_DbContext.ContainerID.Add(new ContainerID
                            {
                                ContainerId = 1,
                                Source = "",
                                ContainerReference = dataContainerRequest.ContainerReference
                            });
                            await _risk_DbContext.SaveChangesAsync();
                            containerID = _risk_DbContext.ContainerID.ToList().Max(x => x.ContainerId);
                        }

                        //add v1 in Datatable DataContainer
                        if (_risk_DbContext.v_DataContainer_CFG.Count() > 0)
                        {
                            dtcntrID = await _risk_DbContext.v_DataContainer_CFG.MaxAsync(x => x.DataContainerId) + 1;
                            if (dtcntrID > 0)
                            {
                                var dtRec = _risk_DbContext.v_DataContainer_CFG.FirstOrDefaultAsync(x => x.DataContainerId == dtcntrID);
                                if (dtRec.Result == null) verID = 1;
                            }
                        }
                        else
                        {
                            dtcntrID = 1; verID = 1;
                        }

                        _risk_DbContext.DataContainer.Add(new DataContainer
                        {
                            DataContainerID = dtcntrID,
                            id_version = verID,
                            Title = dataContainerRequest.Title,
                            Description = dataContainerRequest.Description
                        });

                        await _risk_DbContext.SaveChangesAsync();

                        //add v1 in Control DataContainer
                        _risk_DbContext.Data_Control.Add(new Data_Control
                        {
                            DataContainerId = dtcntrID,
                            id_version = verID,
                            approved = dataContainerRequest.approved
                        });

                        await _risk_DbContext.SaveChangesAsync();

                        //add v1 in Version DataContainer
                        if (dataContainerRequest.approved == false)
                        {
                            _risk_DbContext.Data_Version.Add(new Data_Version
                            {
                                DataContainerId = dtcntrID,
                                id_version = verID,
                                created_by = "1",
                                created_on = DateTime.Now.ToUniversalTime()
                            });
                        }
                        else
                        {
                            _risk_DbContext.Data_Version.Add(new Data_Version
                            {
                                DataContainerId = dtcntrID,
                                id_version = verID,
                                created_by = "1",
                                created_on = DateTime.Now.ToUniversalTime(),
                                submit_by = "1",
                                submit_on = DateTime.Now.ToUniversalTime()
                            });
                        }

                        await _risk_DbContext.SaveChangesAsync();

                        //add v1 in Datatable DataContainerID
                        _risk_DbContext.DataContainerID.Add(new DataContainerID
                        {
                            dataContainerID = dtcntrID,
                            ContainerID = containerID,
                            DataContainerReference = dataContainerRequest.DataContainerReference
                        });
                        await _risk_DbContext.SaveChangesAsync();


                        _risk_DbContext.Database.CommitTransaction();
                        dataContainerResponse.Code = System.Net.HttpStatusCode.OK;
                        dataContainerResponse.Root = dtcntrID;
                        dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.addDataContainerRiskMessage;
                    }
                    else {
                        dataContainerResponse.Code = System.Net.HttpStatusCode.BadRequest;
                        dataContainerResponse.Root = -1;
                        dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.datacontainervalMessage;
                    }
                }
                catch
                {
                    _risk_DbContext.Database.RollbackTransaction();
                    dataContainerResponse.Code = System.Net.HttpStatusCode.Conflict;
                    dataContainerResponse.Root = -1;
                    dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.addErrorMessage;
                }
            }
            else
            {
                dataContainerResponse.Code = System.Net.HttpStatusCode.NotFound;
                dataContainerResponse.Root = -1;
                dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.addErrorMessage;
            }
            return dataContainerResponse;
        }
        public async Task<HttpResult<int>> operationonDataContainer(DataContainerRequest dataContainerRequest)
        {
            int dtcntrID = 0, verID = 0, containerID = 0;
            HttpResult<int> dataContainerResponse = new();
            bool proceed = false;

            //var verdata = await _risk_DbContext.v_DataContainer_VER.Where(x => x.DataContainerId == dataContainerRequest.DataContainerID 
            //&&            (x.updated_by != "DELETED" || x.updated_on == null) && (x.submit_by != "DELETED" || x.submit_by == null))
            //    .OrderByDescending(x => x.id_version).FirstOrDefaultAsync();
            var verdata = await _risk_DbContext.v_DataContainer_VER.Where(x => x.DataContainerId == dataContainerRequest.DataContainerID)
             .OrderByDescending(x => x.id_version).FirstOrDefaultAsync();
            if (verdata != null && dataContainerRequest.data_Version!=null)
            {
                if (dataContainerRequest.data_Version.DataContainerId == verdata.DataContainerId &&
                        dataContainerRequest.data_Version.id_version == verdata.id_version &&
                        dataContainerRequest.data_Version.created_by == verdata.created_by &&
                        dataContainerRequest.data_Version.created_on == verdata.created_on &&
                        dataContainerRequest.data_Version.updated_by == verdata.updated_by &&
                        dataContainerRequest.data_Version.updated_on == verdata.updated_on &&
                        dataContainerRequest.data_Version.submit_by == verdata.submit_by &&
                        dataContainerRequest.data_Version.submit_on == verdata.submit_on) proceed = true;
                else proceed = false;
            }
            else
            {
                if (dataContainerRequest.data_Version == verdata) proceed = true;
                else proceed = false;
            }
            if (proceed)
            {
                try
                {
                    switch (dataContainerRequest.operationType)
                    {
                        case 1://New->Draft,Publish                     
                            dataContainerResponse = await NewDraftPublish(dataContainerRequest);
                            break;
                        case 2:
                            if (dataContainerRequest.DataContainerID > 0 &&
                               dataContainerRequest.Title != string.Empty &&
                               dataContainerRequest.Description != string.Empty)
                            {
                                if (dataContainerRequest.approved == false)//Draft
                                {
                                    //var dtDraft = await _risk_DbContext.v_DataContainer_CFG.
                                    //    FirstOrDefaultAsync(x => x.DataContainerId == dataContainerRequest.DataContainerID
                                    //    && x.id_version == dataContainerRequest.data_Version.id_version && x.approved == false);

                                    var dtDraft = await (from a in _risk_DbContext.v_DataContainer_CFG
                                                         join b in _risk_DbContext.v_DataContainer_VER on new { a = a.DataContainerId, b = a.id_version }
                                                     equals new { a = b.DataContainerId, b = b.id_version }
                                                         where (a.DataContainerId == dataContainerRequest.DataContainerID
                                                         && a.approved == false
                                                         && (b.updated_by != "DELETED" || b.updated_on == null)
                                                         && (b.submit_by != "DELETED" || b.submit_by == null))
                                                         select (a)).FirstOrDefaultAsync();

                                    dataContainerResponse.Root = dataContainerRequest.DataContainerID;

                                    if (dtDraft != null)//Draft->Draft
                                    {
                                        #region Draft->Draft

                                        try
                                        {
                                            _risk_DbContext.Database.BeginTransaction();
                                            //Delete v1 in Datatable
                                            var delData = await _risk_DbContext.DataContainer.
                                                FirstOrDefaultAsync(x => x.DataContainerID == dtDraft.DataContainerId &&
                                            x.id_version == dtDraft.id_version);
                                            if (delData != null) _risk_DbContext.DataContainer.Remove(delData);
                                            await _risk_DbContext.SaveChangesAsync();

                                            //add v1 in Datatable
                                            _risk_DbContext.DataContainer.Add(new DataContainer
                                            {
                                                DataContainerID = dtDraft.DataContainerId,
                                                id_version = dtDraft.id_version,
                                                Title = dataContainerRequest.Title,
                                                Description = dataContainerRequest.Description
                                            });
                                            await _risk_DbContext.SaveChangesAsync();

                                            //update v1 in Version
                                            var updVer = await _risk_DbContext.Data_Version.
                                                FirstOrDefaultAsync(x => x.DataContainerId == dtDraft.DataContainerId &&
                                            x.id_version == dtDraft.id_version);

                                            if (updVer != null)
                                            {
                                                updVer.updated_by = "1";
                                                updVer.updated_on = DateTime.Now.ToUniversalTime();
                                            }
                                            await _risk_DbContext.SaveChangesAsync();

                                            //Delete v1 in Datatable DataContainerID
                                            var delDataCnt = await _risk_DbContext.DataContainerID.
                                              FirstOrDefaultAsync(x => x.dataContainerID == dtDraft.DataContainerId);
                                            if (delDataCnt != null) _risk_DbContext.DataContainerID.Remove(delDataCnt);
                                            await _risk_DbContext.SaveChangesAsync();

                                            //add v1 in Datatable DataContainerID
                                            if (_risk_DbContext.ContainerID.Count() > 0)
                                            {
                                                var res = await _risk_DbContext.ContainerID.OrderByDescending(x => x.ContainerId).LastOrDefaultAsync();
                                                if (res != null && res.ContainerId > 0)
                                                {
                                                    containerID = res.ContainerId;
                                                    _risk_DbContext.DataContainerID.Add(new DataContainerID
                                                    {
                                                        dataContainerID = dtDraft.DataContainerId,
                                                        ContainerID = containerID,
                                                        DataContainerReference = dataContainerRequest.DataContainerReference
                                                    });
                                                    await _risk_DbContext.SaveChangesAsync();
                                                }
                                            }
                                            _risk_DbContext.Database.CommitTransaction();
                                            dataContainerResponse.Code = System.Net.HttpStatusCode.OK;
                                            dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.updateDataContainerRiskMessage;
                                        }
                                        catch
                                        {
                                            _risk_DbContext.Database.RollbackTransaction();
                                            dataContainerResponse.Code = System.Net.HttpStatusCode.Conflict;
                                            dataContainerResponse.Root = -1;
                                            dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.updateErrorMessage;
                                        }
                                        #endregion
                                    }
                                    else if (dtDraft == null)//Publish->Draft
                                    {
                                        #region Publish->Draft

                                        //var dtPublish = await _risk_DbContext.v_DataContainer_CFG.
                                        //    FirstOrDefaultAsync(x => x.DataContainerId == dataContainerRequest.DataContainerID
                                        //    && x.id_version == dataContainerRequest.id_version && x.approved == true);

                                        var dtPublish = await (from a in _risk_DbContext.v_DataContainer_CFG
                                                               join b in _risk_DbContext.v_DataContainer_VER on new { a = a.DataContainerId, b = a.id_version }
                                                           equals new { a = b.DataContainerId, b = b.id_version }
                                                               where (a.DataContainerId == dataContainerRequest.DataContainerID
                                                               && a.approved == true
                                                               && (b.updated_by != "DELETED" || b.updated_on == null)
                                                               && (b.submit_by != "DELETED" || b.submit_by == null))
                                                               select (a)).FirstOrDefaultAsync();

                                        dataContainerResponse.Root = dataContainerRequest.DataContainerID;

                                        if (dtPublish != null)
                                        {
                                            try
                                            {
                                                _risk_DbContext.Database.BeginTransaction();

                                                 verID = 1;

                                                //check publish record v1 and increment version v2
                                                if (_risk_DbContext.v_DataContainer_CFG.Count() > 0)
                                                {
                                                    var lstData = await _risk_DbContext.v_DataContainer_CFG.
                                                        Where(x => x.DataContainerId == dataContainerRequest.DataContainerID).
                                                        OrderByDescending(x => x.id_version).FirstOrDefaultAsync(x => x.approved == true);
                                                    if (lstData != null) verID = lstData.id_version + 1;
                                                }

                                                //find the records matching that version v2 and delete it
                                                var deldtData = _risk_DbContext.DataContainer.Where(x => x.DataContainerID == dataContainerRequest.DataContainerID
                                                && x.id_version == verID);
                                                if (deldtData != null) _risk_DbContext.DataContainer.RemoveRange(deldtData);
                                                await _risk_DbContext.SaveChangesAsync();

                                                //if (_risk_DbContext.v_DataContainer_CFG.Count() > 0)
                                                //{
                                                //    var lstData = await _risk_DbContext.v_DataContainer_CFG.
                                                //        Where(x => x.DataContainerId == dataContainerRequest.DataContainerID).
                                                //        OrderByDescending(x => x.id_version).FirstOrDefaultAsync();
                                                //    if (lstData != null) verID = lstData.id_version + 1;
                                                //}

                                                //add v2 Datatable DataContainer
                                                _risk_DbContext.DataContainer.Add(new DataContainer
                                                {
                                                    DataContainerID = dataContainerRequest.DataContainerID,
                                                    id_version = verID,
                                                    Title = dataContainerRequest.Title,
                                                    Description = dataContainerRequest.Description
                                                });

                                                await _risk_DbContext.SaveChangesAsync();


                                                //Delete all records for the particular DataContainer ID and version //check
                                                var delcntrData = _risk_DbContext.Data_Control.
                                                    Where(x => x.DataContainerId == dataContainerRequest.DataContainerID && x.id_version == verID);
                                                if (delcntrData != null) _risk_DbContext.Data_Control.RemoveRange(delcntrData);
                                                await _risk_DbContext.SaveChangesAsync();

                                                //add v2 with approved 0 in Control DataContainer
                                                _risk_DbContext.Data_Control.Add(new Data_Control
                                                {
                                                    DataContainerId = dataContainerRequest.DataContainerID,
                                                    id_version = verID,
                                                    approved = dataContainerRequest.approved
                                                });

                                                await _risk_DbContext.SaveChangesAsync();

                                                //delete version for the v2 record
                                                var delverData = _risk_DbContext.Data_Version.
                                            Where(x => x.DataContainerId == dataContainerRequest.DataContainerID && x.id_version == verID);
                                                if (delverData != null) _risk_DbContext.Data_Version.RemoveRange(delverData);
                                                await _risk_DbContext.SaveChangesAsync();

                                                //add v2 in Version DataContainer
                                                _risk_DbContext.Data_Version.Add(new Data_Version
                                                {
                                                    DataContainerId = dataContainerRequest.DataContainerID,
                                                    id_version = verID,
                                                    created_by = "1",
                                                    created_on = DateTime.Now.ToUniversalTime()
                                                });
                                                await _risk_DbContext.SaveChangesAsync();

                                                //No Change in Datatable DataContainerID

                                                _risk_DbContext.Database.CommitTransaction();
                                                dataContainerResponse.Code = System.Net.HttpStatusCode.OK;
                                                dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.updateDataContainerRiskMessage;
                                            }
                                            catch
                                            {
                                                _risk_DbContext.Database.RollbackTransaction();
                                                dataContainerResponse.Code = System.Net.HttpStatusCode.Conflict;
                                                dataContainerResponse.Root = -1;
                                                dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.updateErrorMessage;
                                            }
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        dataContainerResponse.Code = System.Net.HttpStatusCode.NoContent;
                                        dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.NoDataContainerRiskMessage;
                                    }
                                }
                                else//Publish
                                {
                                    //var dtPublish = await _risk_DbContext.v_DataContainer_CFG.
                                    //    FirstOrDefaultAsync(x => x.DataContainerId == dataContainerRequest.DataContainerID
                                    //&& x.id_version == dataContainerRequest.data_Version.id_version && x.approved == true);

                                    var dtPublish = await (from a in _risk_DbContext.v_DataContainer_CFG
                                                           join b in _risk_DbContext.v_DataContainer_VER on new { a = a.DataContainerId, b = a.id_version }
                                                       equals new { a = b.DataContainerId, b = b.id_version }
                                                           where (a.DataContainerId == dataContainerRequest.DataContainerID
                                                           && a.approved == true
                                                           && (b.updated_by != "DELETED" || b.updated_on == null)
                                                           && (b.submit_by != "DELETED" || b.submit_by == null))
                                                           select (a)).FirstOrDefaultAsync();

                                    dataContainerResponse.Root = dataContainerRequest.DataContainerID;
                                    if (dtPublish != null)//Publish->Publish
                                    {
                                        #region Publish->Publish
                                        try
                                        {
                                            _risk_DbContext.Database.BeginTransaction();
                                            verID = 1;

                                            if (_risk_DbContext.v_DataContainer_CFG.Count() > 0)
                                            {
                                                var lstData = await _risk_DbContext.v_DataContainer_CFG.
                                                    Where(x => x.DataContainerId == dataContainerRequest.DataContainerID).
                                                    OrderByDescending(x => x.id_version).FirstOrDefaultAsync(x => x.approved == true);
                                                if (lstData != null) verID = lstData.id_version + 1;
                                            }

                                            var deldtData = _risk_DbContext.DataContainer.Where(x => x.DataContainerID == dataContainerRequest.DataContainerID
                                            && x.id_version == verID);
                                            if (deldtData != null) _risk_DbContext.DataContainer.RemoveRange(deldtData);
                                            await _risk_DbContext.SaveChangesAsync();

                                            //add v1 in Datatable DataContainer
                                            _risk_DbContext.DataContainer.Add(new DataContainer
                                            {
                                                DataContainerID = dataContainerRequest.DataContainerID,
                                                id_version = verID,
                                                Title = dataContainerRequest.Title,
                                                Description = dataContainerRequest.Description
                                            });
                                            await _risk_DbContext.SaveChangesAsync();

                                            //if (_risk_DbContext.v_DataContainer_CFG.Count() > 0)
                                            //{
                                            //    var lstData = await _risk_DbContext.v_DataContainer_CFG.
                                            //        Where(x => x.DataContainerId == dataContainerRequest.DataContainerID).
                                            //        OrderByDescending(x => x.id_version).FirstOrDefaultAsync();
                                            //    if (lstData != null) verID = lstData.id_version + 1;

                                            //} 

                                            await _risk_DbContext.SaveChangesAsync();

                                            //Delete all records for the particular DataContainer ID
                                            var delcntrData = _risk_DbContext.Data_Control.
                                                Where(x => x.DataContainerId == dataContainerRequest.DataContainerID);
                                            if (delcntrData != null) _risk_DbContext.Data_Control.RemoveRange(delcntrData);
                                            await _risk_DbContext.SaveChangesAsync();

                                            //add v2 with approved 1 in Control DataContainer
                                            _risk_DbContext.Data_Control.Add(new Data_Control
                                            {
                                                DataContainerId = dataContainerRequest.DataContainerID,
                                                id_version = verID,
                                                approved = dataContainerRequest.approved
                                            });
                                            await _risk_DbContext.SaveChangesAsync();

                                            var delverData = _risk_DbContext.Data_Version.
                                             Where(x => x.DataContainerId == dataContainerRequest.DataContainerID && x.id_version == verID);
                                            if (delverData != null) _risk_DbContext.Data_Version.RemoveRange(delverData);
                                            await _risk_DbContext.SaveChangesAsync();

                                            //add v2 in Version DataContainer
                                            _risk_DbContext.Data_Version.Add(new Data_Version
                                            {
                                                DataContainerId = dataContainerRequest.DataContainerID,
                                                id_version = verID,
                                                created_by = "1",
                                                created_on = DateTime.Now.ToUniversalTime(),
                                                submit_by = "1",
                                                submit_on = DateTime.Now.ToUniversalTime()
                                            });

                                            await _risk_DbContext.SaveChangesAsync();

                                            //No Change in Datatable DataContainerID

                                            _risk_DbContext.Database.CommitTransaction();
                                            dataContainerResponse.Code = System.Net.HttpStatusCode.OK;
                                            dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.updateDataContainerRiskMessage;
                                        }
                                        catch
                                        {
                                            _risk_DbContext.Database.RollbackTransaction();
                                            dataContainerResponse.Code = System.Net.HttpStatusCode.Conflict;
                                            dataContainerResponse.Root = -1;
                                            dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.updateErrorMessage;
                                        }
                                        #endregion
                                    }
                                    else if (dtPublish == null)//Draft->Publish
                                    {
                                        #region Draft->Publish

                                        //var dtDraft = await _risk_DbContext.v_DataContainer_CFG.
                                        //    FirstOrDefaultAsync(x => x.DataContainerId == dataContainerRequest.DataContainerID
                                        //    && x.id_version == dataContainerRequest.data_Version.id_version && x.approved == false);

                                        var dtDraft = await (from a in _risk_DbContext.v_DataContainer_CFG
                                                             join b in _risk_DbContext.v_DataContainer_VER on new { a = a.DataContainerId, b = a.id_version }
                                                         equals new { a = b.DataContainerId, b = b.id_version }
                                                             where (a.DataContainerId == dataContainerRequest.DataContainerID
                                                             && a.approved == false
                                                             && (b.updated_by != "DELETED" || b.updated_on == null)
                                                             && (b.submit_by != "DELETED" || b.submit_by == null))
                                                             select (a)).FirstOrDefaultAsync();

                                        dataContainerResponse.Root = dataContainerRequest.DataContainerID;

                                        if (dtDraft != null)
                                        {
                                            try
                                            {
                                                _risk_DbContext.Database.BeginTransaction();

                                                //Delete v1 in Datatable DataContainer
                                                var delData = await _risk_DbContext.DataContainer.
                                                    FirstOrDefaultAsync(x => x.DataContainerID == dtDraft.DataContainerId &&
                                                x.id_version == dtDraft.id_version);
                                                if (delData != null) _risk_DbContext.DataContainer.Remove(delData);
                                                await _risk_DbContext.SaveChangesAsync();

                                                //add v1 in Datatable DataContainer
                                                _risk_DbContext.DataContainer.Add(new DataContainer
                                                {
                                                    DataContainerID = dtDraft.DataContainerId,
                                                    id_version = dtDraft.id_version,
                                                    Title = dataContainerRequest.Title,
                                                    Description = dataContainerRequest.Description
                                                });
                                                await _risk_DbContext.SaveChangesAsync();

                                                //Delete all records for the particular DataContainer ID
                                                var delcntrData = _risk_DbContext.Data_Control.
                                                    Where(x => x.DataContainerId == dataContainerRequest.DataContainerID);
                                                if (delcntrData != null) _risk_DbContext.Data_Control.RemoveRange(delcntrData);
                                                await _risk_DbContext.SaveChangesAsync();

                                                //add v1 with approved 1 in Control
                                                _risk_DbContext.Data_Control.Add(new Data_Control
                                                {
                                                    DataContainerId = dtDraft.DataContainerId,
                                                    id_version = dtDraft.id_version,
                                                    approved = dataContainerRequest.approved
                                                });
                                                await _risk_DbContext.SaveChangesAsync();

                                                //update v1 in version
                                                var updVer = await _risk_DbContext.Data_Version.
                                                    FirstOrDefaultAsync(x => x.DataContainerId == dtDraft.DataContainerId &&
                                           x.id_version == dtDraft.id_version);

                                                if (updVer != null)
                                                {
                                                    updVer.submit_by = "1";
                                                    updVer.submit_on = DateTime.Now.ToUniversalTime();
                                                }
                                                await _risk_DbContext.SaveChangesAsync();

                                                //Delete v1 in Datatable DataContainerID
                                                var delDataCnt = await _risk_DbContext.DataContainerID.
                                                  FirstOrDefaultAsync(x => x.dataContainerID == dtDraft.DataContainerId);
                                                if (delDataCnt != null) _risk_DbContext.DataContainerID.Remove(delDataCnt);
                                                await _risk_DbContext.SaveChangesAsync();

                                                //add v1 in Datatable DataContainerID
                                                if (_risk_DbContext.ContainerID.Count() > 0)
                                                {
                                                    var res = await _risk_DbContext.ContainerID.OrderByDescending(x => x.ContainerId).LastOrDefaultAsync();
                                                    if (res != null && res.ContainerId > 0)
                                                    {
                                                        containerID = res.ContainerId;
                                                        _risk_DbContext.DataContainerID.Add(new DataContainerID
                                                        {
                                                            dataContainerID = dtDraft.DataContainerId,
                                                            ContainerID = containerID,
                                                            DataContainerReference = dataContainerRequest.DataContainerReference
                                                        });
                                                        await _risk_DbContext.SaveChangesAsync();
                                                    }
                                                }
                                                _risk_DbContext.Database.CommitTransaction();
                                                dataContainerResponse.Code = System.Net.HttpStatusCode.OK;
                                                dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.updateDataContainerRiskMessage;
                                            }
                                            catch
                                            {
                                                _risk_DbContext.Database.RollbackTransaction();
                                                dataContainerResponse.Code = System.Net.HttpStatusCode.Conflict;
                                                dataContainerResponse.Root = -1;
                                                dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.updateErrorMessage;
                                            }
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        dataContainerResponse.Code = System.Net.HttpStatusCode.NoContent;
                                        dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.NoDataContainerRiskMessage;
                                    }
                                }
                            }
                            else
                            {
                                dataContainerResponse.Code = System.Net.HttpStatusCode.NotFound;
                                dataContainerResponse.Root = -1;
                                dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.updateErrorMessage;
                            }
                            break;
                        case 3:
                            if (dataContainerRequest.DataContainerID > 0)
                            {
                                var deldtDraftPub = await _risk_DbContext.v_DataContainer_VER.
                                      FirstOrDefaultAsync(x => x.DataContainerId == dataContainerRequest.DataContainerID
                                      && x.id_version == dataContainerRequest.data_Version.id_version && x.approved == false && x.updated_by == "DELETED");
                                if (deldtDraftPub != null)
                                {
                                    dataContainerResponse.Root = dataContainerRequest.DataContainerID;
                                    if (dataContainerRequest.approved == false)
                                    {
                                        #region Delete->Draft

                                        try
                                        {
                                            _risk_DbContext.Database.BeginTransaction();
                                            //Delete v1 in Datatable
                                            var delData = await _risk_DbContext.DataContainer.
                                                FirstOrDefaultAsync(x => x.DataContainerID == deldtDraftPub.DataContainerId &&
                                            x.id_version == deldtDraftPub.id_version);
                                            if (delData != null) _risk_DbContext.DataContainer.Remove(delData);
                                            await _risk_DbContext.SaveChangesAsync();

                                            //add v1 in Datatable
                                            _risk_DbContext.DataContainer.Add(new DataContainer
                                            {
                                                DataContainerID = deldtDraftPub.DataContainerId,
                                                id_version = deldtDraftPub.id_version,
                                                Title = dataContainerRequest.Title,
                                                Description = dataContainerRequest.Description
                                            });
                                            await _risk_DbContext.SaveChangesAsync();

                                            //update v1 in Version
                                            var updVer = await _risk_DbContext.Data_Version.
                                                FirstOrDefaultAsync(x => x.DataContainerId == deldtDraftPub.DataContainerId &&
                                            x.id_version == deldtDraftPub.id_version);

                                            if (updVer != null)
                                            {
                                                updVer.updated_by = "1";
                                                updVer.updated_on = DateTime.Now.ToUniversalTime();
                                            }
                                            await _risk_DbContext.SaveChangesAsync();

                                            _risk_DbContext.Database.CommitTransaction();

                                            dataContainerResponse.Code = System.Net.HttpStatusCode.OK;
                                            dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.updateDataContainerRiskMessage;
                                        }
                                        catch
                                        {
                                            _risk_DbContext.Database.RollbackTransaction();
                                            dataContainerResponse.Code = System.Net.HttpStatusCode.Conflict;
                                            dataContainerResponse.Root = -1;
                                            dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.updateErrorMessage;
                                        }
                                        #endregion
                                    }
                                    else if (dataContainerRequest.approved == true)
                                    {
                                        #region Delete->Publish

                                        try
                                        {
                                            _risk_DbContext.Database.BeginTransaction();

                                            //Delete v1 in Datatable DataContainer
                                            var delData = await _risk_DbContext.DataContainer.
                                                FirstOrDefaultAsync(x => x.DataContainerID == deldtDraftPub.DataContainerId &&
                                            x.id_version == deldtDraftPub.id_version);
                                            if (delData != null) _risk_DbContext.DataContainer.Remove(delData);
                                            await _risk_DbContext.SaveChangesAsync();

                                            //add v1 in Datatable DataContainer
                                            _risk_DbContext.DataContainer.Add(new DataContainer
                                            {
                                                DataContainerID = deldtDraftPub.DataContainerId,
                                                id_version = deldtDraftPub.id_version,
                                                Title = dataContainerRequest.Title,
                                                Description = dataContainerRequest.Description
                                            });
                                            await _risk_DbContext.SaveChangesAsync();

                                            //Delete all records for the particular DataContainer ID
                                            var delcntrData = _risk_DbContext.Data_Control.
                                                Where(x => x.DataContainerId == dataContainerRequest.DataContainerID);
                                            if (delcntrData != null) _risk_DbContext.Data_Control.RemoveRange(delcntrData);
                                            await _risk_DbContext.SaveChangesAsync();

                                            //add v1 with approved 1 in Control
                                            _risk_DbContext.Data_Control.Add(new Data_Control
                                            {
                                                DataContainerId = deldtDraftPub.DataContainerId,
                                                id_version = deldtDraftPub.id_version,
                                                approved = dataContainerRequest.approved
                                            });
                                            await _risk_DbContext.SaveChangesAsync();

                                            //update v1 in version
                                            var updVer = await _risk_DbContext.Data_Version.
                                                FirstOrDefaultAsync(x => x.DataContainerId == deldtDraftPub.DataContainerId &&
                                       x.id_version == deldtDraftPub.id_version);

                                            if (updVer != null)
                                            {
                                                updVer.updated_by = "1";
                                                updVer.updated_on = DateTime.Now.ToUniversalTime();
                                                updVer.submit_by = "1";
                                                updVer.submit_on = DateTime.Now.ToUniversalTime();
                                            }
                                            await _risk_DbContext.SaveChangesAsync();

                                            //Delete v1 in Datatable DataContainerID
                                            var delDataCnt = await _risk_DbContext.DataContainerID.
                                              FirstOrDefaultAsync(x => x.dataContainerID == deldtDraftPub.DataContainerId);
                                            if (delDataCnt != null) _risk_DbContext.DataContainerID.Remove(delDataCnt);
                                            await _risk_DbContext.SaveChangesAsync();

                                            //add v1 in Datatable DataContainerID
                                            if (_risk_DbContext.ContainerID.Count() > 0)
                                            {
                                                var res = await _risk_DbContext.ContainerID.OrderByDescending(x => x.ContainerId).LastOrDefaultAsync();
                                                if (res != null && res.ContainerId > 0)
                                                {
                                                    containerID = res.ContainerId;
                                                    _risk_DbContext.DataContainerID.Add(new DataContainerID
                                                    {
                                                        dataContainerID = deldtDraftPub.DataContainerId,
                                                        ContainerID = containerID,
                                                        DataContainerReference = dataContainerRequest.DataContainerReference
                                                    });
                                                    await _risk_DbContext.SaveChangesAsync();
                                                }
                                            }
                                            _risk_DbContext.Database.CommitTransaction();
                                            dataContainerResponse.Code = System.Net.HttpStatusCode.OK;
                                            dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.updateDataContainerRiskMessage;
                                        }
                                        catch
                                        {
                                            _risk_DbContext.Database.RollbackTransaction();
                                            dataContainerResponse.Code = System.Net.HttpStatusCode.Conflict;
                                            dataContainerResponse.Root = -1;
                                            dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.updateErrorMessage;
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        dataContainerResponse.Code = System.Net.HttpStatusCode.NoContent;
                                        dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.NoDataContainerRiskMessage;
                                    }
                                }
                                else
                                {
                                    var deldtDraft = await _risk_DbContext.v_DataContainer_VER.
                                          FirstOrDefaultAsync(x => x.DataContainerId == dataContainerRequest.DataContainerID
                                          && x.id_version == dataContainerRequest.data_Version.id_version && x.updated_by != "DELETED");

                                    if (deldtDraft != null)
                                    {
                                        dataContainerResponse.Root = dataContainerRequest.DataContainerID;
                                        if (dataContainerRequest.approved == false)
                                        {
                                            #region Draft->Delete
                                            try
                                            {
                                                //No Change in Datatable DataContainer
                                                //No Change in Control DataContainer

                                                //update v1 in version DataContainer
                                                var updVer = await _risk_DbContext.Data_Version.
                                                    FirstOrDefaultAsync(x => x.DataContainerId == dataContainerRequest.DataContainerID &&
                                           x.id_version == dataContainerRequest.data_Version.id_version);

                                                if (updVer != null)
                                                {
                                                    updVer.updated_by = "DELETED";
                                                    updVer.updated_on = DateTime.Now.ToUniversalTime();
                                                }
                                                await _risk_DbContext.SaveChangesAsync();

                                                dataContainerResponse.Code = System.Net.HttpStatusCode.OK;
                                                dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.deleteDataContainerRiskMessage;
                                            }
                                            catch
                                            {
                                                dataContainerResponse.Code = System.Net.HttpStatusCode.Conflict;
                                                dataContainerResponse.Root = -1;
                                                dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.deleteErrorMessage;
                                            }
                                            #endregion
                                        }
                                        else if (dataContainerRequest.approved == true)
                                        {
                                            #region Publish->Delete
                                            try
                                            {
                                               
                                                verID = 1; 
                                                if (_risk_DbContext.v_DataContainer_CFG.Count() > 0)
                                                {
                                                    var lstData = await _risk_DbContext.v_DataContainer_CFG.
                                                        Where(x => x.DataContainerId == dataContainerRequest.DataContainerID).
                                                        OrderByDescending(x => x.id_version).FirstOrDefaultAsync(x => x.approved == true);
                                                    if (lstData != null) verID = lstData.id_version + 1;
                                                }

                                                var deldtData = _risk_DbContext.DataContainer.Where(x => x.DataContainerID == dataContainerRequest.DataContainerID
                                                && x.id_version == verID);
                                                if (deldtData != null) _risk_DbContext.DataContainer.RemoveRange(deldtData);
                                                await _risk_DbContext.SaveChangesAsync();

                                                ////add v1 in Datatable DataContainer
                                                //_risk_DbContext.DataContainer.Add(new DataContainer
                                                //{
                                                //    DataContainerID = dataContainerRequest.DataContainerID,
                                                //    id_version = verID,
                                                //    Title = dataContainerRequest.Title,
                                                //    Description = dataContainerRequest.Description
                                                //});
                                                //await _risk_DbContext.SaveChangesAsync();

                                                //if (_risk_DbContext.v_DataContainer_CFG.Count() > 0)
                                                //{
                                                //    var lstData = await _risk_DbContext.v_DataContainer_CFG.
                                                //        Where(x => x.DataContainerId == dataContainerRequest.DataContainerID).
                                                //        OrderByDescending(x => x.id_version).FirstOrDefaultAsync();
                                                //    if (lstData != null) verID = lstData.id_version + 1;

                                                //}
                                                _risk_DbContext.Database.BeginTransaction();

                                                //Delete all records for the particular DataContainer ID //check
                                                var delcntrData = _risk_DbContext.Data_Control.
                                                    Where(x => x.DataContainerId == dataContainerRequest.DataContainerID);
                                                if (delcntrData != null) _risk_DbContext.Data_Control.RemoveRange(delcntrData);
                                                await _risk_DbContext.SaveChangesAsync();

                                                //add v2 with approved 0 in Control DataContainer

                                                _risk_DbContext.Data_Control.Add(new Data_Control
                                                {
                                                    DataContainerId = dataContainerRequest.DataContainerID,
                                                    id_version = verID,
                                                    approved = false
                                                });

                                                await _risk_DbContext.SaveChangesAsync();

                                                var delverData = _risk_DbContext.Data_Version.
                                             Where(x => x.DataContainerId == dataContainerRequest.DataContainerID && x.id_version == verID);
                                                if (delverData != null) _risk_DbContext.Data_Version.RemoveRange(delverData);
                                                await _risk_DbContext.SaveChangesAsync();

                                                //add v2 in Version DataContainer 
                                                _risk_DbContext.Data_Version.Add(new Data_Version
                                                {
                                                    DataContainerId = dataContainerRequest.DataContainerID,
                                                    id_version = verID,
                                                    created_by = "1",
                                                    created_on = DateTime.Now.ToUniversalTime(),
                                                    updated_by = "DELETED",
                                                    updated_on = DateTime.Now.ToUniversalTime()
                                                });

                                                await _risk_DbContext.SaveChangesAsync();
                                                _risk_DbContext.Database.CommitTransaction();

                                                dataContainerResponse.Code = System.Net.HttpStatusCode.OK;
                                                dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.deleteDataContainerRiskMessage;
                                            }
                                            catch
                                            {
                                                _risk_DbContext.Database.RollbackTransaction();
                                                dataContainerResponse.Code = System.Net.HttpStatusCode.Conflict;
                                                dataContainerResponse.Root = -1;
                                                dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.deleteErrorMessage;
                                            }
                                            #endregion
                                        }
                                        else
                                        {
                                            dataContainerResponse.Code = System.Net.HttpStatusCode.NoContent;
                                            dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.NoDataContainerRiskMessage;
                                        }
                                    }
                                }
                                //No Change in Datatable DataContainerID
                            }
                            else
                            {
                                dataContainerResponse.Code = System.Net.HttpStatusCode.NotFound;
                                dataContainerResponse.Root = -1;
                                dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.updateErrorMessage;
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _risk_DbContext.Database.RollbackTransaction();
                    dataContainerResponse.Root = -1;
                    dataContainerResponse.Code = System.Net.HttpStatusCode.BadRequest;
                    if (ex.InnerException != null) dataContainerResponse.Message = ex.InnerException.Message;
                    else dataContainerResponse.Message = ex.Message;
                }
            }
            else
            {
                dataContainerResponse.Code = System.Net.HttpStatusCode.BadRequest;
                dataContainerResponse.Root = -1;
                dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.versionmismatchMessage;
            }
            return dataContainerResponse;
        }
        public async Task<HttpResult<int>> operationonDataRisk(DataRiskRequest dataRiskRequest)
        {
            int dtcntrID = 0, verID = 0, containerID = 0, riskID = 0;
            HttpResult<int> dataContainerResponse = new();
            bool proceed = false;

            //var verdata = await _risk_DbContext.v_DataContainer_VER.Where(x => x.DataContainerId == dataRiskRequest.DataContainerID 
            //&& x.approved== dataRiskRequest.data_Version.approved).OrderByDescending(x => x.id_version).FirstOrDefaultAsync();
            //var verdata = await _risk_DbContext.v_DataContainer_VER.Where(x => x.DataContainerId == dataRiskRequest.DataContainerID &&
            //(x.updated_by != "DELETED" || x.updated_on == null) && (x.submit_by != "DELETED" || x.submit_by == null)
            //).OrderByDescending(x => x.id_version)
            //    .FirstOrDefaultAsync();
            var verdata = await _risk_DbContext.v_DataContainer_VER.Where(x => x.DataContainerId == dataRiskRequest.DataContainerID)
                .OrderByDescending(x => x.id_version)
             .FirstOrDefaultAsync();
            if (verdata != null && dataRiskRequest.data_Version!=null)
            {
                if (dataRiskRequest.data_Version.DataContainerId == verdata.DataContainerId &&
                        dataRiskRequest.data_Version.id_version == verdata.id_version &&
                        dataRiskRequest.data_Version.created_by == verdata.created_by &&
                        dataRiskRequest.data_Version.created_on == verdata.created_on &&
                        dataRiskRequest.data_Version.updated_by == verdata.updated_by &&
                        dataRiskRequest.data_Version.updated_on == verdata.updated_on &&
                        dataRiskRequest.data_Version.submit_by == verdata.submit_by &&
                        dataRiskRequest.data_Version.submit_on == verdata.submit_on) proceed = true;
                else proceed = false;
            }
            else
            {
                if (dataRiskRequest.data_Version == verdata) proceed = true;
                else proceed = false;
            }
            if (proceed)
            {
                try
                {
                    switch (dataRiskRequest.operationType)
                    {
                        case 1://New->Draft,Publish                     
                            if (dataRiskRequest.DataContainerID == 0 &&
                                dataRiskRequest.Title != string.Empty &&
                                dataRiskRequest.Description != string.Empty)
                            {
                                try
                                {
                                    var getData = await (from d in _risk_DbContext.v_ContainerID
                                                         join e in _risk_DbContext.v_DataContainerID on d.ContainerId equals e.ContainerID
                                                         join a in _risk_DbContext.v_DataContainer_CFG on e.dataContainerID equals a.DataContainerId
                                                         where a.Title == dataRiskRequest.Title && d.ContainerReference == dataRiskRequest.ContainerReference
                                                         select (a)).FirstOrDefaultAsync();
                                    if (getData == null)
                                    {
                                        _risk_DbContext.Database.BeginTransaction();

                                        if (_risk_DbContext.ContainerID.Count() > 0)
                                        {
                                            var dtRec = _risk_DbContext.ContainerID.FirstOrDefaultAsync(x => x.ContainerReference == dataRiskRequest.ContainerReference);
                                            if (dtRec.Result != null)
                                            {
                                                var res = await _risk_DbContext.ContainerID.OrderByDescending(x => x.ContainerId).LastOrDefaultAsync();
                                                if (res != null && res.ContainerId > 0) containerID = res.ContainerId;
                                            }
                                            else
                                            {
                                                containerID = _risk_DbContext.ContainerID.ToList().Max(x => x.ContainerId) + 1;
                                                _risk_DbContext.ContainerID.Add(new ContainerID
                                                {
                                                    ContainerId = containerID,
                                                    Source = "",
                                                    ContainerReference = dataRiskRequest.ContainerReference
                                                });
                                                await _risk_DbContext.SaveChangesAsync();
                                            }
                                        }
                                        else
                                        {
                                            _risk_DbContext.ContainerID.Add(new ContainerID
                                            {
                                                ContainerId = 1,
                                                Source = "",
                                                ContainerReference = dataRiskRequest.ContainerReference
                                            });
                                            await _risk_DbContext.SaveChangesAsync();
                                            containerID = _risk_DbContext.ContainerID.ToList().Max(x => x.ContainerId);
                                        }

                                        //add v1 in Datatable DataContainer
                                        if (_risk_DbContext.v_DataContainer_CFG.Count() > 0)
                                        {
                                            dtcntrID = await _risk_DbContext.v_DataContainer_CFG.MaxAsync(x => x.DataContainerId) + 1;
                                            if (dtcntrID > 0)
                                            {
                                                var dtRec = _risk_DbContext.v_DataContainer_CFG.FirstOrDefaultAsync(x => x.DataContainerId == dtcntrID);
                                                if (dtRec.Result == null) verID = 1;
                                            }
                                        }
                                        else
                                        {
                                            dtcntrID = 1; verID = 1;
                                        }

                                        //if (_risk_DbContext.ContainerID.Count() > 0)
                                        //{
                                        //    var res = await _risk_DbContext.ContainerID.OrderByDescending(x => x.ContainerId).LastOrDefaultAsync();
                                        //    if (res != null && res.ContainerId > 0) containerID = res.ContainerId;
                                        //}                                       

                                        _risk_DbContext.DataContainer.Add(new DataContainer
                                        {
                                            DataContainerID = dtcntrID,
                                            id_version = verID,
                                            Title = dataRiskRequest.Title,
                                            Description = dataRiskRequest.Description
                                        });

                                        await _risk_DbContext.SaveChangesAsync();

                                        //add v1 in Control DataContainer
                                        _risk_DbContext.Data_Control.Add(new Data_Control
                                        {
                                            DataContainerId = dtcntrID,
                                            id_version = verID,
                                            approved = dataRiskRequest.approved
                                        });

                                        await _risk_DbContext.SaveChangesAsync();

                                        //add v1 in Version DataContainer
                                        if (dataRiskRequest.approved == false)
                                        {
                                            _risk_DbContext.Data_Version.Add(new Data_Version
                                            {
                                                DataContainerId = dtcntrID,
                                                id_version = verID,
                                                created_by = "1",
                                                created_on = DateTime.Now.ToUniversalTime()
                                            });
                                        }
                                        else
                                        {
                                            _risk_DbContext.Data_Version.Add(new Data_Version
                                            {
                                                DataContainerId = dtcntrID,
                                                id_version = verID,
                                                created_by = "1",
                                                created_on = DateTime.Now.ToUniversalTime(),
                                                submit_by = "1",
                                                submit_on = DateTime.Now.ToUniversalTime()
                                            });
                                        }

                                        await _risk_DbContext.SaveChangesAsync();

                                        //add v1 in Datatable DataContainerID
                                        _risk_DbContext.DataContainerID.Add(new DataContainerID
                                        {
                                            dataContainerID = dtcntrID,
                                            ContainerID = containerID,
                                            DataContainerReference = dataRiskRequest.DataContainerReference
                                        });
                                        await _risk_DbContext.SaveChangesAsync();

                                        //add v1 in Datatable DataRisk
                                        riskID = 1;

                                        if (dataRiskRequest.dataRisk.Count() > 0)
                                        {
                                            foreach (DataRiskDet dtRisk in dataRiskRequest.dataRisk)
                                            {

                                                if (_risk_DbContext.DataRisk.ToList().Count() > 0)
                                                { riskID = _risk_DbContext.DataRisk.ToList().Max(x => x.RiskID) + 1; }
                                                else { riskID = 1; }

                                                _risk_DbContext.DataRisk.Add(new DataRisk
                                                {
                                                    DataContainerID = dtcntrID,
                                                    id_version = verID,
                                                    RiskID = riskID,
                                                    CQARiskID = dtRisk.CQARiskID,
                                                    Category = dtRisk.Category,
                                                    Description = dtRisk.Description,
                                                    Probability = dtRisk.Probability,
                                                    ImpactDescription = dtRisk.ImpactDescription,
                                                    GrossRiskValue = dtRisk.GrossRiskValue,
                                                    MitigationType = dtRisk.MitigationType,
                                                    MitigationActionDescription = dtRisk.MitigationActionDescription,
                                                    MitigatedProbability = dtRisk.MitigatedProbability,
                                                    MitigationCost = dtRisk.MitigationCost,
                                                    MitigatedImpactValue = dtRisk.MitigatedImpactValue,
                                                    NetRiskCost = dtRisk.NetRiskCost,
                                                    Currency_Code = dtRisk.Currency_Code,
                                                    Comment = dtRisk.Comment,
                                                    isChecked = dtRisk.isChecked,
                                                    isManual = dtRisk.isManual,
                                                    RiskOwner = dtRisk.RiskOwner
                                                });
                                                await _risk_DbContext.SaveChangesAsync();
                                                riskID = _risk_DbContext.DataRisk.ToList().Max(x => x.RiskID) + 1;
                                            }
                                        }

                                        _risk_DbContext.Database.CommitTransaction();
                                        dataContainerResponse.Code = System.Net.HttpStatusCode.OK;
                                        dataContainerResponse.Root = dtcntrID;
                                        dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.addDataContainerRiskMessage;
                                    }
                                    else
                                    {
                                        dataContainerResponse.Code = System.Net.HttpStatusCode.BadRequest;
                                        dataContainerResponse.Root = -1;
                                        dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.datacontainervalMessage;
                                    }
                                }
                                catch
                                {
                                    _risk_DbContext.Database.RollbackTransaction();
                                    dataContainerResponse.Code = System.Net.HttpStatusCode.Conflict;
                                    dataContainerResponse.Root = -1;
                                    dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.addErrorMessage;
                                }
                            }
                            else
                            {
                                dataContainerResponse.Code = System.Net.HttpStatusCode.NotFound;
                                dataContainerResponse.Root = -1;
                                dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.addErrorMessage;
                            }
                            break;
                        case 2:
                            if (dataRiskRequest.DataContainerID > 0 &&
                               dataRiskRequest.Title != string.Empty &&
                               dataRiskRequest.Description != string.Empty)
                            {
                                if (dataRiskRequest.approved == false)//Draft
                                {
                                    //var dtDraft = await _risk_DbContext.v_DataContainer_CFG.
                                    //    FirstOrDefaultAsync(x => x.DataContainerId == dataRiskRequest.DataContainerID
                                    //    && x.id_version == dataRiskRequest.data_Version.id_version && x.approved == false);

                                    var dtDraft = await (from a in _risk_DbContext.v_DataContainer_CFG
                                                     join b in _risk_DbContext.v_DataContainer_VER on new { a = a.DataContainerId, b = a.id_version }
                                                 equals new { a = b.DataContainerId, b = b.id_version }
                                                     where (a.DataContainerId == dataRiskRequest.DataContainerID
                                                     && a.approved == false
                                                     && (b.updated_by != "DELETED" || b.updated_on == null)
                                                     && (b.submit_by != "DELETED" || b.submit_by == null))
                                                     select (a)).FirstOrDefaultAsync();
                                    
                                    dataContainerResponse.Root = dataRiskRequest.DataContainerID;

                                    if (dtDraft != null)//Draft->Draft
                                    {
                                        #region Draft->Draft

                                        try
                                        {
                                            _risk_DbContext.Database.BeginTransaction();

                                            //Delete v1 in Datatable DataContainerID
                                            var delData = await _risk_DbContext.DataContainer.
                                                FirstOrDefaultAsync(x => x.DataContainerID == dtDraft.DataContainerId &&
                                            x.id_version == dtDraft.id_version);
                                            if (delData != null) _risk_DbContext.DataContainer.Remove(delData);
                                            await _risk_DbContext.SaveChangesAsync();

                                            //add v1 in Datatable DataContainerID
                                            _risk_DbContext.DataContainer.Add(new DataContainer
                                            {
                                                DataContainerID = dtDraft.DataContainerId,
                                                id_version = dtDraft.id_version,
                                                Title = dataRiskRequest.Title,
                                                Description = dataRiskRequest.Description
                                            });
                                            await _risk_DbContext.SaveChangesAsync();

                                            //update v1 in Version DataContainerID
                                            var updVer = await _risk_DbContext.Data_Version.
                                                FirstOrDefaultAsync(x => x.DataContainerId == dtDraft.DataContainerId &&
                                            x.id_version == dtDraft.id_version);

                                            if (updVer != null)
                                            {
                                                updVer.updated_by = "1";
                                                updVer.updated_on = DateTime.Now.ToUniversalTime();
                                            }
                                            await _risk_DbContext.SaveChangesAsync();

                                            //Delete v1 in Datatable DataContainerID
                                            var delDataCnt = await _risk_DbContext.DataContainerID.
                                              FirstOrDefaultAsync(x => x.dataContainerID == dtDraft.DataContainerId);
                                            if (delDataCnt != null) _risk_DbContext.DataContainerID.Remove(delDataCnt);
                                            await _risk_DbContext.SaveChangesAsync();

                                            //add v1 in Datatable DataContainerID
                                            if (_risk_DbContext.ContainerID.Count() > 0)
                                            {
                                                var res = await _risk_DbContext.ContainerID.OrderByDescending(x => x.ContainerId).LastOrDefaultAsync();
                                                if (res != null && res.ContainerId > 0)
                                                {
                                                    containerID = res.ContainerId;
                                                    _risk_DbContext.DataContainerID.Add(new DataContainerID
                                                    {
                                                        dataContainerID = dtDraft.DataContainerId,
                                                        ContainerID = containerID,
                                                        DataContainerReference = dataRiskRequest.DataContainerReference
                                                    });
                                                    await _risk_DbContext.SaveChangesAsync();
                                                }
                                            }

                                            //Delete v1 in Datatable DataRisk
                                            var delRisk = _risk_DbContext.DataRisk.Where(x => x.DataContainerID == dtDraft.DataContainerId &&
                                            x.id_version == dtDraft.id_version);
                                            if (delRisk != null) _risk_DbContext.DataRisk.RemoveRange(delRisk);
                                            await _risk_DbContext.SaveChangesAsync();

                                            //add v1 in Datatable DataRisk
                                            riskID = 1;

                                            if (dataRiskRequest.dataRisk.Count() > 0)
                                            {
                                                if (_risk_DbContext.DataRisk.ToList().Count() > 0)
                                                { riskID = _risk_DbContext.DataRisk.ToList().Max(x => x.RiskID) + 1; }
                                                else { riskID = 1; }
                                                foreach (DataRiskDet dtRisk in dataRiskRequest.dataRisk)
                                                {

                                                    _risk_DbContext.DataRisk.Add(new DataRisk
                                                    {
                                                        DataContainerID = dtDraft.DataContainerId,
                                                        id_version = dtDraft.id_version,
                                                        RiskID = riskID,
                                                        CQARiskID = dtRisk.CQARiskID,
                                                        Category = dtRisk.Category,
                                                        Description = dtRisk.Description,
                                                        Probability = dtRisk.Probability,
                                                        ImpactDescription = dtRisk.ImpactDescription,
                                                        GrossRiskValue = dtRisk.GrossRiskValue,
                                                        MitigationType = dtRisk.MitigationType,
                                                        MitigationActionDescription = dtRisk.MitigationActionDescription,
                                                        MitigatedProbability = dtRisk.MitigatedProbability,
                                                        MitigationCost = dtRisk.MitigationCost,
                                                        MitigatedImpactValue = dtRisk.MitigatedImpactValue,
                                                        NetRiskCost = dtRisk.NetRiskCost,
                                                        Currency_Code = dtRisk.Currency_Code,
                                                        Comment = dtRisk.Comment,
                                                        isChecked = dtRisk.isChecked,
                                                        isManual = dtRisk.isManual,
                                                        RiskOwner = dtRisk.RiskOwner
                                                    });
                                                    await _risk_DbContext.SaveChangesAsync();
                                                    riskID = _risk_DbContext.DataRisk.ToList().Max(x => x.RiskID) + 1;
                                                }
                                            }

                                            _risk_DbContext.Database.CommitTransaction();
                                            dataContainerResponse.Code = System.Net.HttpStatusCode.OK;
                                            dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.updateDataContainerRiskMessage;
                                        }
                                        catch
                                        {
                                            _risk_DbContext.Database.RollbackTransaction();
                                            dataContainerResponse.Code = System.Net.HttpStatusCode.Conflict;
                                            dataContainerResponse.Root = -1;
                                            dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.updateErrorMessage;
                                        }
                                        #endregion
                                    }
                                    else if (dtDraft == null)//Publish->Draft
                                    {
                                        #region Publish->Draft

                                        //var dtPublish = await _risk_DbContext.v_DataContainer_CFG.
                                        //    FirstOrDefaultAsync(x => x.DataContainerId == dataRiskRequest.DataContainerID
                                        //    && x.id_version == dataRiskRequest.data_Version.id_version && x.approved == true);
                                        var dtPublish = await (from a in _risk_DbContext.v_DataContainer_CFG
                                                               join b in _risk_DbContext.v_DataContainer_VER on new { a = a.DataContainerId, b = a.id_version }
                                                           equals new { a = b.DataContainerId, b = b.id_version }
                                                               where (a.DataContainerId == dataRiskRequest.DataContainerID
                                                               && a.approved == true
                                                               && (b.updated_by != "DELETED" || b.updated_on == null)
                                                               && (b.submit_by != "DELETED" || b.submit_by == null))
                                                               select (a)).FirstOrDefaultAsync();



                                        dataContainerResponse.Root = dataRiskRequest.DataContainerID;

                                        if (dtPublish != null)
                                        {
                                            try
                                            {
                                                _risk_DbContext.Database.BeginTransaction();

                                               
                                                verID = 1;
                                                //check publish record v1 and increment version v2
                                                if (_risk_DbContext.v_DataContainer_CFG.Count() > 0)
                                                {
                                                    var lstData = await _risk_DbContext.v_DataContainer_CFG.
                                                        Where(x => x.DataContainerId == dataRiskRequest.DataContainerID).
                                                        OrderByDescending(x => x.id_version).FirstOrDefaultAsync(x => x.approved == true);
                                                    if (lstData != null) verID = lstData.id_version + 1;
                                                }

                                                //find the records matching that version v2 and delete it
                                                var deldtData = _risk_DbContext.DataContainer.Where(x => x.DataContainerID == dataRiskRequest.DataContainerID
                                                && x.id_version == verID);
                                                if (deldtData != null) _risk_DbContext.DataContainer.RemoveRange(deldtData);
                                                await _risk_DbContext.SaveChangesAsync();

                                                //if (_risk_DbContext.v_DataContainer_CFG.Count() > 0)
                                                //{
                                                //    var lstData = await _risk_DbContext.v_DataContainer_CFG.
                                                //        Where(x => x.DataContainerId == dataRiskRequest.DataContainerID).
                                                //        OrderByDescending(x => x.id_version).FirstOrDefaultAsync();
                                                //    if (lstData != null) verID = lstData.id_version + 1;
                                                //}

                                                //add v2 Datatable DataContainer
                                                _risk_DbContext.DataContainer.Add(new DataContainer
                                                {
                                                    DataContainerID = dataRiskRequest.DataContainerID,
                                                    id_version = verID,
                                                    Title = dataRiskRequest.Title,
                                                    Description = dataRiskRequest.Description
                                                });

                                                await _risk_DbContext.SaveChangesAsync();


                                                //Delete all records for the particular DataContainer ID and version//check
                                                var delcntrData = _risk_DbContext.Data_Control.
                                                    Where(x => x.DataContainerId == dataRiskRequest.DataContainerID && x.id_version==verID);
                                                if (delcntrData != null) _risk_DbContext.Data_Control.RemoveRange(delcntrData);
                                                await _risk_DbContext.SaveChangesAsync();

                                                //add v2 with approved 0 in Control DataContainer
                                                _risk_DbContext.Data_Control.Add(new Data_Control
                                                {
                                                    DataContainerId = dataRiskRequest.DataContainerID,
                                                    id_version = verID,
                                                    approved = dataRiskRequest.approved
                                                });

                                                await _risk_DbContext.SaveChangesAsync();

                                                //delete version for the v2 record
                                                var delverData = _risk_DbContext.Data_Version.
                                            Where(x => x.DataContainerId == dataRiskRequest.DataContainerID && x.id_version == verID);
                                                if (delverData != null) _risk_DbContext.Data_Version.RemoveRange(delverData);
                                                await _risk_DbContext.SaveChangesAsync();

                                                //add v2 in Version DataContainer
                                                _risk_DbContext.Data_Version.Add(new Data_Version
                                                {
                                                    DataContainerId = dataRiskRequest.DataContainerID,
                                                    id_version = verID,
                                                    created_by = "1",
                                                    created_on = DateTime.Now.ToUniversalTime()
                                                });

                                                await _risk_DbContext.SaveChangesAsync();

                                                //No Change in Datatable DataContainerID

                                                //add v1 in Datatable DataRisk
                                                riskID = 1;

                                                var delriskData = _risk_DbContext.DataRisk.
                                         Where(x => x.DataContainerID == dataRiskRequest.DataContainerID && x.id_version == verID);
                                                if (delriskData != null) _risk_DbContext.DataRisk.RemoveRange(delriskData);
                                                await _risk_DbContext.SaveChangesAsync();

                                                if (dataRiskRequest.dataRisk.Count() > 0)
                                                {
                                                    if (_risk_DbContext.DataRisk.ToList().Count() > 0)
                                                    { riskID = _risk_DbContext.DataRisk.ToList().Max(x => x.RiskID) + 1; }
                                                    else { riskID = 1; }
                                                    foreach (DataRiskDet dtRisk in dataRiskRequest.dataRisk)
                                                    {
                                                        _risk_DbContext.DataRisk.Add(new DataRisk
                                                        {
                                                            DataContainerID = dataRiskRequest.DataContainerID,
                                                            id_version = verID,
                                                            RiskID = riskID,
                                                            CQARiskID = dtRisk.CQARiskID,
                                                            Category = dtRisk.Category,
                                                            Description = dtRisk.Description,
                                                            Probability = dtRisk.Probability,
                                                            ImpactDescription = dtRisk.ImpactDescription,
                                                            GrossRiskValue = dtRisk.GrossRiskValue,
                                                            MitigationType = dtRisk.MitigationType,
                                                            MitigationActionDescription = dtRisk.MitigationActionDescription,
                                                            MitigatedProbability = dtRisk.MitigatedProbability,
                                                            MitigationCost = dtRisk.MitigationCost,
                                                            MitigatedImpactValue = dtRisk.MitigatedImpactValue,
                                                            NetRiskCost = dtRisk.NetRiskCost,
                                                            Currency_Code = dtRisk.Currency_Code,
                                                            Comment = dtRisk.Comment,
                                                            isChecked = dtRisk.isChecked,
                                                            isManual = dtRisk.isManual,
                                                            RiskOwner = dtRisk.RiskOwner
                                                        });
                                                        await _risk_DbContext.SaveChangesAsync();
                                                        riskID = _risk_DbContext.DataRisk.ToList().Max(x => x.RiskID) + 1;
                                                    }
                                                }

                                                _risk_DbContext.Database.CommitTransaction();
                                                dataContainerResponse.Code = System.Net.HttpStatusCode.OK;
                                                dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.updateDataContainerRiskMessage;
                                            }
                                            catch
                                            {
                                                _risk_DbContext.Database.RollbackTransaction();
                                                dataContainerResponse.Code = System.Net.HttpStatusCode.Conflict;
                                                dataContainerResponse.Root = -1;
                                                dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.updateErrorMessage;
                                            }
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        dataContainerResponse.Code = System.Net.HttpStatusCode.NoContent;
                                        dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.NoDataContainerRiskMessage;
                                    }
                                }
                                else//Publish
                                {
                                    //var dtPublish = await _risk_DbContext.v_DataContainer_CFG.
                                    //    FirstOrDefaultAsync(x => x.DataContainerId == dataRiskRequest.DataContainerID
                                    //&& x.id_version == dataRiskRequest.data_Version.id_version && x.approved == true);
                                    var dtPublish = await (from a in _risk_DbContext.v_DataContainer_CFG
                                                           join b in _risk_DbContext.v_DataContainer_VER on new { a = a.DataContainerId, b = a.id_version }
                                                       equals new { a = b.DataContainerId, b = b.id_version }
                                                           where (a.DataContainerId == dataRiskRequest.DataContainerID
                                                           && a.approved == true
                                                           && (b.updated_by != "DELETED" || b.updated_on == null)
                                                           && (b.submit_by != "DELETED" || b.submit_by == null))
                                                           select (a)).FirstOrDefaultAsync();

                                    dataContainerResponse.Root = dataRiskRequest.DataContainerID;
                                    if (dtPublish != null)//Publish->Publish
                                    {
                                        #region Publish->Publish

                                        try
                                        {
                                            //add v2 Datatable DataContainer
                                            _risk_DbContext.Database.BeginTransaction();
                                            verID = 1;

                                            if (_risk_DbContext.v_DataContainer_CFG.Count() > 0)
                                            {
                                                var lstData = await _risk_DbContext.v_DataContainer_CFG.
                                                    Where(x => x.DataContainerId == dataRiskRequest.DataContainerID).
                                                    OrderByDescending(x => x.id_version).FirstOrDefaultAsync(x => x.approved == true);
                                                if (lstData != null) verID = lstData.id_version + 1;
                                            }

                                            var deldtData = _risk_DbContext.DataContainer.Where(x => x.DataContainerID == dataRiskRequest.DataContainerID
                                            && x.id_version == verID);
                                            if (deldtData != null) _risk_DbContext.DataContainer.RemoveRange(deldtData);
                                            await _risk_DbContext.SaveChangesAsync();

                                            //add v1 in Datatable DataContainer
                                            _risk_DbContext.DataContainer.Add(new DataContainer
                                            {
                                                DataContainerID = dataRiskRequest.DataContainerID,
                                                id_version = verID,
                                                Title = dataRiskRequest.Title,
                                                Description = dataRiskRequest.Description
                                            });
                                            await _risk_DbContext.SaveChangesAsync();


                                            //if (_risk_DbContext.v_DataContainer_CFG.Count() > 0)
                                            //{
                                            //    var lstData = await _risk_DbContext.v_DataContainer_CFG.
                                            //        Where(x => x.DataContainerId == dataRiskRequest.DataContainerID).
                                            //        OrderByDescending(x => x.id_version).FirstOrDefaultAsync();
                                            //    if (lstData != null) verID = lstData.id_version + 1;

                                            //}

                                            //Delete all records for the particular DataContainer ID
                                            var delcntrData = _risk_DbContext.Data_Control.
                                                Where(x => x.DataContainerId == dataRiskRequest.DataContainerID);
                                            if (delcntrData != null) _risk_DbContext.Data_Control.RemoveRange(delcntrData);
                                            await _risk_DbContext.SaveChangesAsync();

                                            //add v2 with approved 1 in Control DataContainer
                                            _risk_DbContext.Data_Control.Add(new Data_Control
                                            {
                                                DataContainerId = dataRiskRequest.DataContainerID,
                                                id_version = verID,
                                                approved = dataRiskRequest.approved
                                            });
                                            await _risk_DbContext.SaveChangesAsync();

                                            var delverData = _risk_DbContext.Data_Version.
                                               Where(x => x.DataContainerId == dataRiskRequest.DataContainerID && x.id_version == verID);
                                            if (delverData != null) _risk_DbContext.Data_Version.RemoveRange(delverData);
                                            await _risk_DbContext.SaveChangesAsync();

                                            //add v2 in Version DataContainer
                                            _risk_DbContext.Data_Version.Add(new Data_Version
                                            {
                                                DataContainerId = dataRiskRequest.DataContainerID,
                                                id_version = verID,
                                                created_by = "1",
                                                created_on = DateTime.Now.ToUniversalTime(),
                                                submit_by = "1",
                                                submit_on = DateTime.Now.ToUniversalTime()
                                            });

                                            await _risk_DbContext.SaveChangesAsync();

                                            //No Change in Datatable DataContainerID

                                            //add v1 in Datatable DataRisk
                                            riskID = 1;

                                            var delriskData = _risk_DbContext.DataRisk.
                                           Where(x => x.DataContainerID == dataRiskRequest.DataContainerID && x.id_version == verID);
                                            if (delriskData != null) _risk_DbContext.DataRisk.RemoveRange(delriskData);
                                            await _risk_DbContext.SaveChangesAsync();

                                            if (dataRiskRequest.dataRisk.Count() > 0)
                                            {
                                                if (_risk_DbContext.DataRisk.ToList().Count() > 0)
                                                { riskID = _risk_DbContext.DataRisk.ToList().Max(x => x.RiskID) + 1; }
                                                else { riskID = 1; }
                                                foreach (DataRiskDet dtRisk in dataRiskRequest.dataRisk)
                                                {
                                                    _risk_DbContext.DataRisk.Add(new DataRisk
                                                    {
                                                        DataContainerID = dataRiskRequest.DataContainerID,
                                                        id_version = verID,
                                                        RiskID = riskID,
                                                        CQARiskID = dtRisk.CQARiskID,
                                                        Category = dtRisk.Category,
                                                        Description = dtRisk.Description,
                                                        Probability = dtRisk.Probability,
                                                        ImpactDescription = dtRisk.ImpactDescription,
                                                        GrossRiskValue = dtRisk.GrossRiskValue,
                                                        MitigationType = dtRisk.MitigationType,
                                                        MitigationActionDescription = dtRisk.MitigationActionDescription,
                                                        MitigatedProbability = dtRisk.MitigatedProbability,
                                                        MitigationCost = dtRisk.MitigationCost,
                                                        MitigatedImpactValue = dtRisk.MitigatedImpactValue,
                                                        NetRiskCost = dtRisk.NetRiskCost,
                                                        Currency_Code = dtRisk.Currency_Code,
                                                        Comment = dtRisk.Comment,
                                                        isChecked = dtRisk.isChecked,
                                                        isManual = dtRisk.isManual,
                                                        RiskOwner = dtRisk.RiskOwner
                                                    });
                                                    await _risk_DbContext.SaveChangesAsync();
                                                    riskID = _risk_DbContext.DataRisk.ToList().Max(x => x.RiskID) + 1;
                                                }
                                            }

                                            _risk_DbContext.Database.CommitTransaction();
                                            dataContainerResponse.Code = System.Net.HttpStatusCode.OK;
                                            dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.updateDataContainerRiskMessage;
                                        }
                                        catch
                                        {
                                            _risk_DbContext.Database.RollbackTransaction();
                                            dataContainerResponse.Code = System.Net.HttpStatusCode.Conflict;
                                            dataContainerResponse.Root = -1;
                                            dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.updateErrorMessage;
                                        }

                                        #endregion
                                    }
                                    else if (dtPublish == null)//Draft->Publish
                                    {
                                        #region Draft->Publish

                                        //var dtDraft = await _risk_DbContext.v_DataContainer_CFG.
                                        //    FirstOrDefaultAsync(x => x.DataContainerId == dataRiskRequest.DataContainerID
                                        //    && x.id_version == dataRiskRequest.data_Version.id_version && x.approved == false);

                                        var dtDraft = await (from a in _risk_DbContext.v_DataContainer_CFG
                                                             join b in _risk_DbContext.v_DataContainer_VER on new { a = a.DataContainerId, b = a.id_version }
                                                         equals new { a = b.DataContainerId, b = b.id_version }
                                                             where (a.DataContainerId == dataRiskRequest.DataContainerID
                                                             && a.approved == false
                                                             && (b.updated_by != "DELETED" || b.updated_on == null)
                                                             && (b.submit_by != "DELETED" || b.submit_by == null))
                                                             select (a)).FirstOrDefaultAsync();

                                        dataContainerResponse.Root = dataRiskRequest.DataContainerID;

                                        if (dtDraft != null)
                                        {
                                            try
                                            {
                                                _risk_DbContext.Database.BeginTransaction();

                                                //Delete v1 in Datatable DataContainer
                                                var delData = await _risk_DbContext.DataContainer.
                                                    FirstOrDefaultAsync(x => x.DataContainerID == dataRiskRequest.DataContainerID &&
                                                x.id_version == dataRiskRequest.data_Version.id_version);
                                                if (delData != null) _risk_DbContext.DataContainer.Remove(delData);
                                                await _risk_DbContext.SaveChangesAsync();

                                                //add v1 in Datatable DataContainer
                                                _risk_DbContext.DataContainer.Add(new DataContainer
                                                {
                                                    DataContainerID = dataRiskRequest.DataContainerID,
                                                    id_version = dataRiskRequest.data_Version.id_version,
                                                    Title = dataRiskRequest.Title,
                                                    Description = dataRiskRequest.Description
                                                });
                                                await _risk_DbContext.SaveChangesAsync();

                                                //Delete all records for the particular DataContainer ID
                                                var delcntrData = _risk_DbContext.Data_Control.
                                                    Where(x => x.DataContainerId == dataRiskRequest.DataContainerID);
                                                if (delcntrData != null) _risk_DbContext.Data_Control.RemoveRange(delcntrData);
                                                await _risk_DbContext.SaveChangesAsync();

                                                //add v1 with approved 1 in Control
                                                _risk_DbContext.Data_Control.Add(new Data_Control
                                                {
                                                    DataContainerId = dtDraft.DataContainerId,
                                                    id_version = dtDraft.id_version,
                                                    approved = dataRiskRequest.approved
                                                });
                                                await _risk_DbContext.SaveChangesAsync();

                                                //update v1 in version
                                                var updVer = await _risk_DbContext.Data_Version.
                                                    FirstOrDefaultAsync(x => x.DataContainerId == dtDraft.DataContainerId &&
                                           x.id_version == dtDraft.id_version);

                                                if (updVer != null)
                                                {
                                                    updVer.submit_by = "1";
                                                    updVer.submit_on = DateTime.Now.ToUniversalTime();
                                                }
                                                await _risk_DbContext.SaveChangesAsync();

                                                //Delete v1 in Datatable DataContainerID
                                                var delDataCnt = await _risk_DbContext.DataContainerID.
                                                  FirstOrDefaultAsync(x => x.dataContainerID == dtDraft.DataContainerId);
                                                if (delDataCnt != null) _risk_DbContext.DataContainerID.Remove(delDataCnt);
                                                await _risk_DbContext.SaveChangesAsync();

                                                //add v1 in Datatable DataContainerID
                                                if (_risk_DbContext.ContainerID.Count() > 0)
                                                {
                                                    var res = await _risk_DbContext.ContainerID.OrderByDescending(x => x.ContainerId).LastOrDefaultAsync();
                                                    if (res != null && res.ContainerId > 0)
                                                    {
                                                        containerID = res.ContainerId;
                                                        _risk_DbContext.DataContainerID.Add(new DataContainerID
                                                        {
                                                            dataContainerID = dtDraft.DataContainerId,
                                                            ContainerID = containerID,
                                                            DataContainerReference = dataRiskRequest.DataContainerReference
                                                        });
                                                        await _risk_DbContext.SaveChangesAsync();
                                                    }
                                                }

                                                //Delete v1 in Datatable DataRisk
                                                var delRisk = _risk_DbContext.DataRisk.Where(x => x.DataContainerID == dtDraft.DataContainerId &&
                                                x.id_version == dtDraft.id_version);
                                                if (delRisk != null) _risk_DbContext.DataRisk.RemoveRange(delRisk);
                                                await _risk_DbContext.SaveChangesAsync();

                                                //add v1 in Datatable DataRisk
                                                riskID = 1;

                                                if (dataRiskRequest.dataRisk.Count() > 0)
                                                {
                                                    if (_risk_DbContext.DataRisk.ToList().Count() > 0)
                                                    { riskID = _risk_DbContext.DataRisk.ToList().Max(x => x.RiskID) + 1; }
                                                    else { riskID = 1; }
                                                    foreach (DataRiskDet dtRisk in dataRiskRequest.dataRisk)
                                                    {



                                                        _risk_DbContext.DataRisk.Add(new DataRisk
                                                        {
                                                            DataContainerID = dtDraft.DataContainerId,
                                                            id_version = dtDraft.id_version,
                                                            RiskID = riskID,
                                                            CQARiskID = dtRisk.CQARiskID,
                                                            Category = dtRisk.Category,
                                                            Description = dtRisk.Description,
                                                            Probability = dtRisk.Probability,
                                                            ImpactDescription = dtRisk.ImpactDescription,
                                                            GrossRiskValue = dtRisk.GrossRiskValue,
                                                            MitigationType = dtRisk.MitigationType,
                                                            MitigationActionDescription = dtRisk.MitigationActionDescription,
                                                            MitigatedProbability = dtRisk.MitigatedProbability,
                                                            MitigationCost = dtRisk.MitigationCost,
                                                            MitigatedImpactValue = dtRisk.MitigatedImpactValue,
                                                            NetRiskCost = dtRisk.NetRiskCost,
                                                            Currency_Code = dtRisk.Currency_Code,
                                                            Comment = dtRisk.Comment,
                                                            isChecked = dtRisk.isChecked,
                                                            isManual = dtRisk.isManual,
                                                            RiskOwner = dtRisk.RiskOwner
                                                        });
                                                        await _risk_DbContext.SaveChangesAsync();
                                                        riskID = _risk_DbContext.DataRisk.ToList().Max(x => x.RiskID) + 1;
                                                    }
                                                }

                                                _risk_DbContext.Database.CommitTransaction();
                                                dataContainerResponse.Code = System.Net.HttpStatusCode.OK;
                                                dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.updateDataContainerRiskMessage;
                                            }
                                            catch
                                            {
                                                _risk_DbContext.Database.RollbackTransaction();
                                                dataContainerResponse.Code = System.Net.HttpStatusCode.Conflict;
                                                dataContainerResponse.Root = -1;
                                                dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.updateErrorMessage;
                                            }
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        dataContainerResponse.Code = System.Net.HttpStatusCode.NoContent;
                                        dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.NoDataContainerRiskMessage;
                                    }
                                }
                            }
                            else
                            {
                                dataContainerResponse.Code = System.Net.HttpStatusCode.NotFound;
                                dataContainerResponse.Root = -1;
                                dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.updateErrorMessage;
                            }
                            break;
                        case 3:
                            if (dataRiskRequest.DataContainerID > 0)
                            {
                                var deldtDraftPub = await _risk_DbContext.v_DataContainer_VER.
                                      FirstOrDefaultAsync(x => x.DataContainerId == dataRiskRequest.DataContainerID
                                      && x.id_version == dataRiskRequest.data_Version.id_version && x.approved == false && x.updated_by == "DELETED");
                                if (deldtDraftPub != null)
                                {
                                    dataContainerResponse.Root = dataRiskRequest.DataContainerID;
                                    if (dataRiskRequest.approved == false)
                                    {
                                        #region Delete->Draft

                                        try
                                        {
                                            _risk_DbContext.Database.BeginTransaction();
                                            //Delete v1 in Datatable DataContainer
                                            var delData = await _risk_DbContext.DataContainer.
                                                FirstOrDefaultAsync(x => x.DataContainerID == deldtDraftPub.DataContainerId &&
                                            x.id_version == deldtDraftPub.id_version);
                                            if (delData != null) _risk_DbContext.DataContainer.Remove(delData);
                                            await _risk_DbContext.SaveChangesAsync();

                                            //add v1 in Datatable DataContainer
                                            _risk_DbContext.DataContainer.Add(new DataContainer
                                            {
                                                DataContainerID = deldtDraftPub.DataContainerId,
                                                id_version = deldtDraftPub.id_version,
                                                Title = dataRiskRequest.Title,
                                                Description = dataRiskRequest.Description
                                            });
                                            await _risk_DbContext.SaveChangesAsync();

                                            //update v1 in Version DataContainer
                                            var updVer = await _risk_DbContext.Data_Version.
                                                FirstOrDefaultAsync(x => x.DataContainerId == deldtDraftPub.DataContainerId &&
                                            x.id_version == deldtDraftPub.id_version);

                                            if (updVer != null)
                                            {
                                                updVer.updated_by = "1";
                                                updVer.updated_on = DateTime.Now.ToUniversalTime();
                                            }
                                            await _risk_DbContext.SaveChangesAsync();

                                            //Delete v1 in Datatable DataRisk
                                            var delRisk = _risk_DbContext.DataRisk.Where(x => x.DataContainerID == deldtDraftPub.DataContainerId &&
                                            x.id_version == deldtDraftPub.id_version);
                                            if (delRisk != null) _risk_DbContext.DataRisk.RemoveRange(delRisk);
                                            await _risk_DbContext.SaveChangesAsync();

                                            //add v1 in Datatable DataRisk
                                            riskID = 1;

                                            if (dataRiskRequest.dataRisk.Count() > 0)
                                            {
                                                foreach (DataRiskDet dtRisk in dataRiskRequest.dataRisk)
                                                {
                                                    if (_risk_DbContext.DataRisk.ToList().Count() > 0)
                                                    { riskID = _risk_DbContext.DataRisk.ToList().Max(x => x.RiskID) + 1; }
                                                    else { riskID = 1; }

                                                    _risk_DbContext.DataRisk.Add(new DataRisk
                                                    {
                                                        DataContainerID = deldtDraftPub.DataContainerId,
                                                        id_version = deldtDraftPub.id_version,
                                                        RiskID = riskID,
                                                        CQARiskID = dtRisk.CQARiskID,
                                                        Category = dtRisk.Category,
                                                        Description = dtRisk.Description,
                                                        Probability = dtRisk.Probability,
                                                        ImpactDescription = dtRisk.ImpactDescription,
                                                        GrossRiskValue = dtRisk.GrossRiskValue,
                                                        MitigationType = dtRisk.MitigationType,
                                                        MitigationActionDescription = dtRisk.MitigationActionDescription,
                                                        MitigatedProbability = dtRisk.MitigatedProbability,
                                                        MitigationCost = dtRisk.MitigationCost,
                                                        MitigatedImpactValue = dtRisk.MitigatedImpactValue,
                                                        NetRiskCost = dtRisk.NetRiskCost,
                                                        Currency_Code = dtRisk.Currency_Code,
                                                        Comment = dtRisk.Comment,
                                                        isChecked = dtRisk.isChecked,
                                                        isManual = dtRisk.isManual,
                                                        RiskOwner = dtRisk.RiskOwner
                                                    });
                                                    await _risk_DbContext.SaveChangesAsync();
                                                    riskID = _risk_DbContext.DataRisk.ToList().Max(x => x.RiskID) + 1;
                                                }
                                            }

                                            _risk_DbContext.Database.CommitTransaction();

                                            dataContainerResponse.Code = System.Net.HttpStatusCode.OK;
                                            dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.updateDataContainerRiskMessage;
                                        }
                                        catch
                                        {
                                            _risk_DbContext.Database.RollbackTransaction();
                                            dataContainerResponse.Code = System.Net.HttpStatusCode.Conflict;
                                            dataContainerResponse.Root = -1;
                                            dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.updateErrorMessage;
                                        }
                                        #endregion
                                    }
                                    else if (dataRiskRequest.approved == true)
                                    {
                                        #region Delete->Publish

                                        try
                                        {
                                            _risk_DbContext.Database.BeginTransaction();

                                            //Delete v1 in Datatable DataContainer
                                            var delData = await _risk_DbContext.DataContainer.
                                                FirstOrDefaultAsync(x => x.DataContainerID == deldtDraftPub.DataContainerId &&
                                            x.id_version == deldtDraftPub.id_version);
                                            if (delData != null) _risk_DbContext.DataContainer.Remove(delData);
                                            await _risk_DbContext.SaveChangesAsync();

                                            //add v1 in Datatable DataContainer
                                            _risk_DbContext.DataContainer.Add(new DataContainer
                                            {
                                                DataContainerID = deldtDraftPub.DataContainerId,
                                                id_version = deldtDraftPub.id_version,
                                                Title = dataRiskRequest.Title,
                                                Description = dataRiskRequest.Description
                                            });
                                            await _risk_DbContext.SaveChangesAsync();

                                            //Delete all records for the particular DataContainer ID
                                            var delcntrData = _risk_DbContext.Data_Control.
                                                Where(x => x.DataContainerId == dataRiskRequest.DataContainerID);
                                            if (delcntrData != null) _risk_DbContext.Data_Control.RemoveRange(delcntrData);
                                            await _risk_DbContext.SaveChangesAsync();

                                            //add v1 with approved 1 in Control
                                            _risk_DbContext.Data_Control.Add(new Data_Control
                                            {
                                                DataContainerId = deldtDraftPub.DataContainerId,
                                                id_version = deldtDraftPub.id_version,
                                                approved = dataRiskRequest.approved
                                            });
                                            await _risk_DbContext.SaveChangesAsync();

                                            //update v1 in version
                                            var updVer = await _risk_DbContext.Data_Version.
                                                FirstOrDefaultAsync(x => x.DataContainerId == deldtDraftPub.DataContainerId &&
                                       x.id_version == deldtDraftPub.id_version);

                                            if (updVer != null)
                                            {
                                                updVer.updated_by = "1";
                                                updVer.updated_on = DateTime.Now.ToUniversalTime();
                                                updVer.submit_by = "1";
                                                updVer.submit_on = DateTime.Now.ToUniversalTime();
                                            }
                                            await _risk_DbContext.SaveChangesAsync();

                                            //Delete v1 in Datatable DataContainerID
                                            var delDataCnt = await _risk_DbContext.DataContainerID.
                                              FirstOrDefaultAsync(x => x.dataContainerID == deldtDraftPub.DataContainerId);
                                            if (delDataCnt != null) _risk_DbContext.DataContainerID.Remove(delDataCnt);
                                            await _risk_DbContext.SaveChangesAsync();

                                            //add v1 in Datatable DataContainerID
                                            if (_risk_DbContext.ContainerID.Count() > 0)
                                            {
                                                var res = await _risk_DbContext.ContainerID.OrderByDescending(x => x.ContainerId).LastOrDefaultAsync();
                                                if (res != null && res.ContainerId > 0)
                                                {
                                                    containerID = res.ContainerId;
                                                    _risk_DbContext.DataContainerID.Add(new DataContainerID
                                                    {
                                                        dataContainerID = deldtDraftPub.DataContainerId,
                                                        ContainerID = containerID,
                                                        DataContainerReference = dataRiskRequest.DataContainerReference
                                                    });
                                                    await _risk_DbContext.SaveChangesAsync();
                                                }
                                            }

                                            //Delete v1 in Datatable DataRisk
                                            var delRisk = _risk_DbContext.DataRisk.Where(x => x.DataContainerID == deldtDraftPub.DataContainerId &&
                                            x.id_version == deldtDraftPub.id_version);
                                            if (delRisk != null) _risk_DbContext.DataRisk.RemoveRange(delRisk);
                                            await _risk_DbContext.SaveChangesAsync();

                                            //add v1 in Datatable DataRisk
                                            riskID = 1;

                                            if (dataRiskRequest.dataRisk.Count() > 0)
                                            {
                                                foreach (DataRiskDet dtRisk in dataRiskRequest.dataRisk)
                                                {
                                                    if (_risk_DbContext.DataRisk.ToList().Count() > 0)
                                                    { riskID = _risk_DbContext.DataRisk.ToList().Max(x => x.RiskID) + 1; }
                                                    else { riskID = 1; }
                                                    

                                                    _risk_DbContext.DataRisk.Add(new DataRisk
                                                    {
                                                        DataContainerID = deldtDraftPub.DataContainerId,
                                                        id_version = deldtDraftPub.id_version,
                                                        RiskID = riskID,
                                                        CQARiskID = dtRisk.CQARiskID,
                                                        Category = dtRisk.Category,
                                                        Description = dtRisk.Description,
                                                        Probability = dtRisk.Probability,
                                                        ImpactDescription = dtRisk.ImpactDescription,
                                                        GrossRiskValue = dtRisk.GrossRiskValue,
                                                        MitigationType = dtRisk.MitigationType,
                                                        MitigationActionDescription = dtRisk.MitigationActionDescription,
                                                        MitigatedProbability = dtRisk.MitigatedProbability,
                                                        MitigationCost = dtRisk.MitigationCost,
                                                        MitigatedImpactValue = dtRisk.MitigatedImpactValue,
                                                        NetRiskCost = dtRisk.NetRiskCost,
                                                        Currency_Code = dtRisk.Currency_Code,
                                                        Comment = dtRisk.Comment,
                                                        isChecked = dtRisk.isChecked,
                                                        isManual = dtRisk.isManual,
                                                        RiskOwner = dtRisk.RiskOwner
                                                    });
                                                    await _risk_DbContext.SaveChangesAsync();
                                                    riskID = _risk_DbContext.DataRisk.ToList().Max(x => x.RiskID) + 1;
                                                }
                                            }

                                            _risk_DbContext.Database.CommitTransaction();
                                            dataContainerResponse.Code = System.Net.HttpStatusCode.OK;
                                            dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.updateDataContainerRiskMessage;
                                        }
                                        catch
                                        {
                                            _risk_DbContext.Database.RollbackTransaction();
                                            dataContainerResponse.Code = System.Net.HttpStatusCode.Conflict;
                                            dataContainerResponse.Root = -1;
                                            dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.updateErrorMessage;
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        dataContainerResponse.Code = System.Net.HttpStatusCode.NoContent;
                                        dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.NoDataContainerRiskMessage;
                                    }
                                }
                                else
                                {
                                    var deldtDraft = await _risk_DbContext.v_DataContainer_VER.
                                          FirstOrDefaultAsync(x => x.DataContainerId == dataRiskRequest.DataContainerID
                                          && x.id_version == dataRiskRequest.data_Version.id_version && x.updated_by != "DELETED");

                                    if (deldtDraft != null)
                                    {
                                        dataContainerResponse.Root = dataRiskRequest.DataContainerID;
                                        if (dataRiskRequest.approved == false)
                                        {
                                            #region Draft->Delete
                                            try
                                            {
                                                //No Change in Datatable DataContainer
                                                //No Change in Control DataContainer

                                                //update v1 in version DataContainer
                                                var updVer = await _risk_DbContext.Data_Version.
                                                    FirstOrDefaultAsync(x => x.DataContainerId == dataRiskRequest.DataContainerID &&
                                           x.id_version == dataRiskRequest.data_Version.id_version);

                                                if (updVer != null)
                                                {
                                                    updVer.updated_by = "DELETED";
                                                    updVer.updated_on = DateTime.Now.ToUniversalTime();
                                                }
                                                await _risk_DbContext.SaveChangesAsync();

                                                dataContainerResponse.Code = System.Net.HttpStatusCode.OK;
                                                dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.deleteDataContainerRiskMessage;
                                            }
                                            catch
                                            {
                                                dataContainerResponse.Code = System.Net.HttpStatusCode.Conflict;
                                                dataContainerResponse.Root = -1;
                                                dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.deleteErrorMessage;
                                            }
                                            #endregion
                                        }
                                        else if (dataRiskRequest.approved == true)
                                        {
                                            #region Publish->Delete
                                            try
                                            {
                                                _risk_DbContext.Database.BeginTransaction();

                                                verID = 1;
                                                if (_risk_DbContext.v_DataContainer_CFG.Count() > 0)
                                                {
                                                    var lstData = await _risk_DbContext.v_DataContainer_CFG.
                                                        Where(x => x.DataContainerId == dataRiskRequest.DataContainerID).
                                                        OrderByDescending(x => x.id_version).FirstOrDefaultAsync(x=>x.approved==true);
                                                    if (lstData != null) verID = lstData.id_version + 1;
                                                }

                                                var deldtData =_risk_DbContext.DataContainer.Where(x => x.DataContainerID == dataRiskRequest.DataContainerID
                                                && x.id_version == verID);
                                                if (deldtData != null) _risk_DbContext.DataContainer.RemoveRange(deldtData);
                                                await _risk_DbContext.SaveChangesAsync();

                                                ////add v1 in Datatable DataContainer
                                                //_risk_DbContext.DataContainer.Add(new DataContainer
                                                //{
                                                //    DataContainerID = dataRiskRequest.DataContainerID,
                                                //    id_version = verID,
                                                //    Title = dataRiskRequest.Title,
                                                //    Description = dataRiskRequest.Description
                                                //});
                                                //await _risk_DbContext.SaveChangesAsync();

                                                //if (_risk_DbContext.v_DataContainer_CFG.Count() > 0)
                                                //{
                                                //    var lstData = await _risk_DbContext.v_DataContainer_CFG.
                                                //        Where(x => x.DataContainerId == dataRiskRequest.DataContainerID).
                                                //        OrderByDescending(x => x.id_version).FirstOrDefaultAsync();
                                                //    if (lstData != null) verID = lstData.id_version + 1;

                                                //}

                                                //Delete all records for the particular DataContainer ID //check
                                                var delcntrData = _risk_DbContext.Data_Control.
                                                    Where(x => x.DataContainerId == dataRiskRequest.DataContainerID);
                                                if (delcntrData != null) _risk_DbContext.Data_Control.RemoveRange(delcntrData);
                                                await _risk_DbContext.SaveChangesAsync();
                                                                              

                                                //add v2 with approved 0 in Control DataContainer
                                               
                                                _risk_DbContext.Data_Control.Add(new Data_Control
                                                {
                                                    DataContainerId = dataRiskRequest.DataContainerID,
                                                    id_version = verID,
                                                    approved = false
                                                });
                                                await _risk_DbContext.SaveChangesAsync();


                                                var delverData = _risk_DbContext.Data_Version.
                                                 Where(x => x.DataContainerId == dataRiskRequest.DataContainerID && x.id_version == verID);
                                                if (delverData != null) _risk_DbContext.Data_Version.RemoveRange(delverData);
                                                await _risk_DbContext.SaveChangesAsync();

                                                //add v2 in Version DataContainer 
                                                _risk_DbContext.Data_Version.Add(new Data_Version
                                                {
                                                    DataContainerId = dataRiskRequest.DataContainerID,
                                                    id_version = verID,
                                                    created_by = "1",
                                                    created_on = DateTime.Now.ToUniversalTime(),
                                                    updated_by = "DELETED",
                                                    updated_on = DateTime.Now.ToUniversalTime()
                                                });

                                                await _risk_DbContext.SaveChangesAsync();

                                                //risk
                                                riskID = 1;
                                                var delriskData = _risk_DbContext.DataRisk.
                                         Where(x => x.DataContainerID == dataRiskRequest.DataContainerID && x.id_version == verID);
                                                if (delriskData != null) _risk_DbContext.DataRisk.RemoveRange(delriskData);
                                                await _risk_DbContext.SaveChangesAsync();

                                                //if (dataRiskRequest.dataRisk.Count() > 0)
                                                //{
                                                //    if (_risk_DbContext.DataRisk.ToList().Count() > 0)
                                                //    { riskID = _risk_DbContext.DataRisk.ToList().Max(x => x.RiskID) + 1; }
                                                //    else { riskID = 1; }
                                                //    foreach (DataRiskDet dtRisk in dataRiskRequest.dataRisk)
                                                //    {
                                                //        _risk_DbContext.DataRisk.Add(new DataRisk
                                                //        {
                                                //            DataContainerID = dataRiskRequest.DataContainerID,
                                                //            id_version = verID,
                                                //            RiskID = riskID,
                                                //            CQARiskID = dtRisk.CQARiskID,
                                                //            Category = dtRisk.Category,
                                                //            Description = dtRisk.Description,
                                                //            Probability = dtRisk.Probability,
                                                //            ImpactDescription = dtRisk.ImpactDescription,
                                                //            GrossRiskValue = dtRisk.GrossRiskValue,
                                                //            MitigationType = dtRisk.MitigationType,
                                                //            MitigationActionDescription = dtRisk.MitigationActionDescription,
                                                //            MitigatedProbability = dtRisk.MitigatedProbability,
                                                //            MitigationCost = dtRisk.MitigationCost,
                                                //            MitigatedImpactValue = dtRisk.MitigatedImpactValue,
                                                //            NetRiskCost = dtRisk.NetRiskCost,
                                                //            Currency_Code = dtRisk.Currency_Code,
                                                //            Comment = dtRisk.Comment,
                                                //            isChecked = dtRisk.isChecked,
                                                //            isManual = dtRisk.isManual,
                                                //            RiskOwner = dtRisk.RiskOwner
                                                //        });
                                                //        await _risk_DbContext.SaveChangesAsync();
                                                //        riskID = _risk_DbContext.DataRisk.ToList().Max(x => x.RiskID) + 1;
                                                //    }
                                                //}

                                                _risk_DbContext.Database.CommitTransaction();

                                                dataContainerResponse.Code = System.Net.HttpStatusCode.OK;
                                                dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.deleteDataContainerRiskMessage;
                                            }
                                            catch
                                            {
                                                _risk_DbContext.Database.RollbackTransaction();
                                                dataContainerResponse.Code = System.Net.HttpStatusCode.Conflict;
                                                dataContainerResponse.Root = -1;
                                                dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.deleteErrorMessage;
                                            }
                                            #endregion
                                        }
                                        else
                                        {
                                            dataContainerResponse.Code = HttpStatusCode.NoContent;
                                            dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.NoDataContainerRiskMessage;
                                        }
                                    }
                                }
                                //No Change in Datatable DataContainerID
                            }
                            else
                            {
                                dataContainerResponse.Code = System.Net.HttpStatusCode.NotFound;
                                dataContainerResponse.Root = -1;
                                dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.updateErrorMessage;
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _risk_DbContext.Database.RollbackTransaction();
                    dataContainerResponse.Root = -1;
                    dataContainerResponse.Code = System.Net.HttpStatusCode.BadRequest;
                    if (ex.InnerException != null) dataContainerResponse.Message = ex.InnerException.Message;
                    else dataContainerResponse.Message = ex.Message;
                }
            }
            else
            {
                dataContainerResponse.Code = System.Net.HttpStatusCode.BadRequest;
                dataContainerResponse.Root = -1;
                dataContainerResponse.Message = Messages.DataContainerRiskControllerMessgaes.versionmismatchMessage;
            }
            return dataContainerResponse;
        }
    }
}
