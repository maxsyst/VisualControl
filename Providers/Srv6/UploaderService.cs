using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using VueExample.Entities;
using VueExample.Models.SRV6.Uploader;
using VueExample.Providers.Abstract;
using VueExample.Providers.Srv6.Interfaces;

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
        public UploaderService (IStandartWaferService standartWaferService, ISRV6GraphicService graphicService, IMeasurementRecordingService measurementRecordingService, IDieProvider dieProvider, IDieValueService dieValueService, IShortLinkProvider shortLinkProvider, IStageProvider stageProvider) 
        {
            _standartWaferService = standartWaferService;
            _stageProvider = stageProvider;
            _graphicService = graphicService;
            _measurementRecordingService = measurementRecordingService;
            _dieProvider = dieProvider;
            _shortLinkProvider = shortLinkProvider;
            _dieValueService = dieValueService;
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
            foreach (var graphic in uploadingFile.Graphics)
            {
                await _measurementRecordingService.AddOrGetFkMrGraphics (new FkMrGraphic 
                {
                        GraphicId = graphic.Id,
                        MeasurementRecordingId = uploadingFile.MeasurementRecordingId,
                        DateFile = modifieddate,
                        DateImport = DateTime.Now,
                        Operator = uploadingFile.UserName,
                        Comment = uploadingFile.Comment
                });
            }

            var dieidlink = dieList.Aggregate(String.Empty, (current, l) => current + "x" + l);
            var shorturl = _shortLinkProvider.Obfuscate(dieidlink.Remove(0,1));
            var fullLink = "srv6.svr.lan/Ns/WaferMapFaster.aspx?wid=" + uploadingFile.WaferId + "&idmrpcm=" + uploadingFile.MeasurementRecordingId + "&islink=t&dies=" +
                shorturl + "&square=" + "0" + "&rddl=" + "0" + "&lg=" + "false";
            var link = await _shortLinkProvider.CreateSRV6(fullLink);
            return link.ShortLink;
        }
    }
}