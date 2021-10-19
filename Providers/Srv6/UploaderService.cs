using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using VueExample.Entities;
using VueExample.Models.SRV6;
using VueExample.Models.SRV6.Uploader;
using VueExample.Providers.Abstract;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.Services.Abstract;

namespace VueExample.Providers.Srv6
{
    public class UploaderService : IUploaderService
    {
        private readonly IStandartWaferService _standartWaferService;
        private readonly IDieProvider _dieProvider;
        private readonly IStageProvider _stageProvider;
        private readonly ISRV6GraphicService _graphicService;
        private readonly IDieValueService _dieValueService;
        private readonly IShortLinkProvider _shortLinkProvider;
        private readonly IMeasurementRecordingService _measurementRecordingService;
        private readonly IFolderService _folderService;
        private readonly IElementService _elementService;
        private readonly IGraphic4Service _graphic4Service;
        public UploaderService (IGraphic4Service graphic4Service, IElementService elementService, IUploadingTypeService uploadingTypeService, IFolderService folderService, IStandartWaferService standartWaferService, ISRV6GraphicService graphicService, IMeasurementRecordingService measurementRecordingService, IDieProvider dieProvider, IDieValueService dieValueService, IShortLinkProvider shortLinkProvider, IStageProvider stageProvider)
        {
            _graphic4Service = graphic4Service;
            _elementService = elementService;
            _standartWaferService = standartWaferService;
            _stageProvider = stageProvider;
            _graphicService = graphicService;
            _measurementRecordingService = measurementRecordingService;
            _dieProvider = dieProvider;
            _shortLinkProvider = shortLinkProvider;
            _dieValueService = dieValueService;
            _folderService = folderService;
        }

        public async Task<IList<UploadingFileStatus>> CheckStatus(IList<UploadingFile> uploadingFiles)
        {
            var uploadingFileStatusList = new List<UploadingFileStatus>();
            foreach (var uploadingFile in uploadingFiles)
            {
                try
                {
                    var measurementRecording = await _measurementRecordingService.GetByNameAndWaferId("оп." + uploadingFile.OperationName, uploadingFile.WaferId);
                    var fkMrGraphicsList = new List<FkMrGraphic>();
                    foreach (var graphicName in uploadingFile.GraphicNames)
                    {
                        var graphic = await _graphicService.GetByCodeProductAndName(uploadingFile.CodeProductId, graphicName);
                        var fkMrGraphics = await _measurementRecordingService.GetFkMrGraphics(measurementRecording?.Id, graphic.Id);
                        fkMrGraphicsList.Add(fkMrGraphics);
                    }
                    if(fkMrGraphicsList.Count > 0)
                    {
                        var stage = await _stageProvider.GetByMeasurementRecordingId(measurementRecording.Id);
                        if(stage.IsNullObject)
                        {
                            uploadingFileStatusList.Add(new UploadingFileStatus{Guid = uploadingFile.Guid, AlreadyData = fkMrGraphicsList, Stage = new ViewModels.StageViewModel(), UploadStatus = "already"});
                        }
                        else
                        {
                            uploadingFileStatusList.Add(new UploadingFileStatus{Guid = uploadingFile.Guid, AlreadyData = fkMrGraphicsList, Stage = new ViewModels.StageViewModel {Id = stage.StageId, Name = stage.StageName}, UploadStatus = "already"});
                        }
                    }
                    else
                    {
                        uploadingFileStatusList.Add(new UploadingFileStatus{Guid = uploadingFile.Guid, Stage = new ViewModels.StageViewModel(), UploadStatus = "initial"});
                    }
                }
                catch(Exception)
                {
                    uploadingFileStatusList.Add(new UploadingFileStatus{Guid = uploadingFile.Guid, Stage = new ViewModels.StageViewModel(), UploadStatus = "initial"});
                }
            }
            return uploadingFileStatusList;
        }

        public async Task<string> UploadingGraphic4(UploadingFileGraphic4 uploadingFile)
        {
            var graphic4ParseList = await _folderService.GetGraphic4(uploadingFile);
            var bigMeasurementRecording = await _measurementRecordingService.GetOrAddBigMeasurement(uploadingFile.MeasurementRecordingName, uploadingFile.WaferId);
            var date = DateTime.Now.AddYears(-1);
            var measurementRecording = await _measurementRecordingService.GetOrCreate(uploadingFile.MeasurementRecordingName, 2, bigMeasurementRecording.Id, date, uploadingFile.StageId);
            var graphic4ViewModel = await _graphic4Service.CreateGraphic4(graphic4ParseList, measurementRecording.Id, uploadingFile.WaferId);
            var dieIdList = graphic4ParseList.FirstOrDefault().DieWithCodesList.Select(x => x.DieId).ToList();
            await _dieProvider.GetOrAddDieParameters(dieIdList, measurementRecording.Id);
            await _measurementRecordingService.GetOrCreateFkMrP(measurementRecording.Id, 247, uploadingFile.WaferId);
            var fkMrGraphicsList = (from graphic in uploadingFile.UploadingType.Graphics
                                    select new FkMrGraphic
                                    {
                                        GraphicId = graphic.GraphicSRV6Id,
                                        MeasurementRecordingId = measurementRecording.Id,
                                        DateFile = DateTime.Now,
                                        DateImport = DateTime.Now,
                                        Operator = uploadingFile.UserName,
                                        Comment = String.Empty
                                    }).ToList();
            await _measurementRecordingService.AddOrGetFkMrGraphics(fkMrGraphicsList);
            await _elementService.UpdateElementOnIdmr(measurementRecording.Id, uploadingFile.ElementId);
            return measurementRecording.Name;
        }

        public async Task<MeasurementRecordingStatus> CheckStatusGraphic4(string waferId, string measurementRecordingName)
        {
            var directoryPath = $"{ExtraConfiguration.UploadingGraphic4Path}\\{waferId}";
            var isExist = _folderService.IsWaferExistsInFolder(directoryPath,measurementRecordingName);
            if(!isExist)
                return new MeasurementRecordingStatus{Status = "notExists", MeasurementRecordingId = 0};
            var measurementRecording = await _measurementRecordingService.GetByNameAndWaferId($"оп.{measurementRecordingName}", waferId);
            if(measurementRecording != null)
            {
                return new MeasurementRecordingStatus{Status = "alreadyUploaded", MeasurementRecordingId = measurementRecording.Id};
            }
            return new MeasurementRecordingStatus{Status = "ready", MeasurementRecordingId = 0};
        }

        public async Task<string> Uploading (UploadingFile uploadingFile, int type)
        {
            DateTime modifieddate = DateTime.Now;
            var diegraphicList = new List<DieGraphics>();
            var dieList = new List<string>();

            foreach (var data in uploadingFile.Data)
            {
                string code = data.Key;
                if (!String.IsNullOrEmpty(uploadingFile.Map))
                {
                    code = await _standartWaferService.GetCodeFromStandartWafer(code, uploadingFile.Map);
                }
                var die = await _dieProvider.GetByWaferIdAndCode(uploadingFile.WaferId, code);
                if (die != null)
                {
                    dieList.Add(Convert.ToString(die.DieId));
                    if (uploadingFile.IsNewMeasurement)
                    {
                        await _dieProvider.GetOrAddDieParameter(die.DieId, (int) uploadingFile.MeasurementRecordingId);
                    }

                    if (type == 1)
                    {
                        for (var i = 0; i < uploadingFile.Graphics.Count; i++)
                        {
                            var ordinateList = new List<string>();
                            var abcissList = new List<string>();
                            abcissList = data.Value.AbscissList.GetRange(0, data.Value.AbscissList.Count);

                            for (int j = 0; j < data.Value.AbscissList.Count; j++)
                            {
                                ordinateList.Add(data.Value.ValueLists.ElementAt(i).Value[j]);
                            }

                            if (double.Parse(abcissList.FirstOrDefault(), CultureInfo.InvariantCulture) > double.Parse(abcissList.Last(), CultureInfo.InvariantCulture))
                            {
                                ordinateList.Reverse();
                                abcissList.Reverse();
                            }

                            var graphicstring = abcissList.Aggregate(String.Empty, (current, abc) => current + "*" + abc);
                            graphicstring = graphicstring + "X";
                            graphicstring = ordinateList.Aggregate(graphicstring, (current, ord) => current + "*" + ord);
                            diegraphicList.Add(new DieGraphics
                            {
                                DieId = die.DieId,
                                GraphicId = uploadingFile.Graphics[i].Id,
                                MeasurementRecordingId = uploadingFile.MeasurementRecordingId,
                                ValueString = graphicstring
                            });
                        }
                    }

                    if (type == 2)
                    {
                        for (int i = 0; i < uploadingFile.Graphics.Count; i++)
                        {
                            diegraphicList.Add(new DieGraphics
                            {
                                    DieId = die.DieId,
                                    GraphicId = uploadingFile.Graphics[i].Id,
                                    MeasurementRecordingId = uploadingFile.MeasurementRecordingId,
                                    ValueString = code + "X" + Convert.ToString(data.Value.ValueLists.ElementAt(i).Value.FirstOrDefault())
                            });
                        }
                    }
                }
            }

            await _dieValueService.CreateDieGraphics(diegraphicList);
            var fkMrGraphicList = (from graphic in uploadingFile.Graphics
                                   select new FkMrGraphic
                                   {
                                       GraphicId = graphic.Id,
                                       MeasurementRecordingId = uploadingFile.MeasurementRecordingId,
                                       DateFile = modifieddate,
                                       DateImport = DateTime.Now,
                                       Operator = uploadingFile.UserName,
                                       Comment = uploadingFile.Comment
                                   }).ToList();
            await _measurementRecordingService.AddOrGetFkMrGraphics(fkMrGraphicList);
            var dieidlink = dieList.Aggregate(String.Empty, (current, l) => current + "x" + l);
            var shorturl = _shortLinkProvider.Obfuscate(dieidlink.Remove(0,1));
            var fullLink = "srv6.svr.lan/Ns/WaferMapFaster.aspx?wid=" + uploadingFile.WaferId + "&idmrpcm=" + uploadingFile.MeasurementRecordingId + "&islink=t&dies=" +
                shorturl + "&square=" + "0" + "&rddl=" + "0" + "&lg=" + "false";
            var link = await _shortLinkProvider.CreateSRV6(fullLink);
            return link.ShortLink;
        }
    }
}