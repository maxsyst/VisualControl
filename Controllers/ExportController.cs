using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using VueExample.Models.SRV6.Export;
using VueExample.StatisticsCore.Abstract;
using VueExample.ViewModels;
using OfficeOpenXml;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System;
using OfficeOpenXml.Style;
using AutoMapper;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using VueExample.Providers.Srv6.Interfaces;
using System.IO.Compression;

namespace VueExample.Controllers
{
    [Route("api/[controller]")]

    public class ExportController : Controller
    {
        private readonly IExportProvider _exportProvider;
        private readonly IMeasurementRecordingService _measurementRecordingService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IMapper _mapper;
        public ExportController(IExportProvider exportProvider, IMeasurementRecordingService measurementRecordingService, IMapper mapper, IWebHostEnvironment hostingEnvironment)
        {
            _exportProvider = exportProvider;
            _measurementRecordingService = measurementRecordingService;
            _hostingEnvironment = hostingEnvironment;
            _mapper = mapper;
        }

        [HttpGet]
        // [ProducesResponseType(typeof(GraphicViewModel), StatusCodes.Status200OK)]
        // [ProducesResponseType(typeof(List<Error>), StatusCodes.Status404NotFound)]
        [Route(("get/kurb"))]
        public IActionResult GetKurbatovByMeasurementId([FromQuery] int measurementRecordingId, [FromQuery] string statNames)
        {
            // string n = "S21<sub>(2.5GHz)</sub>(коэффициент передачи)$S<sub>22(5GHz)</sub>";
            var result = _exportProvider.Export(measurementRecordingId, statNames, "$", 1.5);
            return Ok(result);
        }

        [HttpPost]
        [Route("create-kurb")]
        public async Task<IActionResult> CreateKurb([FromBody] JObject kurbatovXLSBodyJObject)
        {
            var kurbatovXLSBody = kurbatovXLSBodyJObject.ToObject<KurbatovXLSBodyModel>();
            var xlsList = kurbatovXLSBody.kurbatovXLSViewModelList.Select(x => _mapper.Map<KurbatovXLS>(x)).ToList();             
            foreach (var xls in xlsList)
            {
                await _exportProvider.PopulateKurbatovXLSByValues(xls);
            }

            foreach (var xls in xlsList)
            {
                if(xls.IsNeedToCopyS2P) {
                    var mrName = (await _measurementRecordingService.GetById(xls.kpList[0].MeasurementRecordingId)).Name.Remove(0,3);
                    var files = Directory.GetFiles(@"T:\\opts\\pnxe\\" + kurbatovXLSBody.WaferId + "_" + mrName).Where(x => !xls.DirtyCodesList.Contains(x.Split("\\").Last().Split('_').ElementAt(1))).ToList();
                    var zipFile = @"T:\\opts\\protocols\\" + kurbatovXLSBody.WaferId + "_" + mrName.Remove(0, 5) + ".zip";
                    using (var archive = ZipFile.Open(zipFile, ZipArchiveMode.Create))
                    {
                        foreach (var fPath in files)
                        {
                            archive.CreateEntryFromFile(fPath, Path.GetFileName(fPath));
                        }
                    }
                }
            }

            var XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileDownloadName = $"{Guid.NewGuid().ToString()}.xlsx";
            var reportsFolder = "reports";

            using (var package = createExcelPackage(xlsList, kurbatovXLSBody.WaferId, kurbatovXLSBody.MSLNumber, kurbatovXLSBody.Date))
            {
                package.SaveAs(new FileInfo(Path.Combine(_hostingEnvironment.WebRootPath, reportsFolder, fileDownloadName)));
            }

            return File($"~/{reportsFolder}/{fileDownloadName}", XlsxContentType, fileDownloadName);
        }

        [HttpGet]
        [Route(("pattern/vp"))]
        public IActionResult GetPatternVP()
        {
            var xlsList = new List<KurbatovXLSViewModel>();
            var row6 = new KurbatovXLSViewModel();
            row6.ElementName = "TC1";
            row6.OperationNumber = "570.00.00";
            row6.StageName = "Снятие рабочей пластины с пластины-носителя";
            row6.parameters.Add(new KurbatovParameter{ParameterName = "Cmicap", Lower = 0.1E-12, Upper = 0.6E-12, DividerId = 1, RussianParameterName="Емкость элемента, Ф", ParameterNameStat="C при U=0.06В"});

            var row7 = new KurbatovXLSViewModel();
            row7.ElementName = "TC5";
            row7.OperationNumber = "570.00.00";
            row7.StageName = "Снятие рабочей пластины с пластины-носителя";
            row7.parameters.Add(new KurbatovParameter{ParameterName = "rDS(on)", Lower = 1.2, Upper = 2.3, Divider = 1000/75, DividerId = 3, RussianParameterName="Сопротивление открытого канала, Ом*мм", ParameterNameStat="r<sub>DS(on)</sub> (сопротивление открытого канала при Uси = 0.02В)"});
            row7.parameters.Add(new KurbatovParameter{ParameterName = "Ugs(off)", Lower = -1.9, Upper = -1.25, DividerId = 1, RussianParameterName="Напряжение отсечки, В", ParameterNameStat="U<sub>GS(off)</sub> (напряжение отсечки при Idss/1000)"});
            row7.parameters.Add(new KurbatovParameter{ParameterName = "gmax", Lower = 0.3, Divider = 0.075, DividerId = 3, RussianParameterName="Максимум крутизны, См/мм", ParameterNameStat="g<sub>max</sub> (максимум крутизны)"});
            row7.parameters.Add(new KurbatovParameter{ParameterName = "Igss(-3V)", Lower = -3E-5, Divider = 0.075, DividerId = 3, RussianParameterName="Ток утечки затвора, А/мм", ParameterNameStat="I<sub>GSS(-3V)</sub> (ток утечки затвора при Uзи=-3В)"});
            row7.parameters.Add(new KurbatovParameter{ParameterName = "Idss(3V)", Lower = 0.3, Upper = 0.6, Divider = 0.075, DividerId = 3, RussianParameterName="Начальный ток стока, A/мм", ParameterNameStat="I<sub>DSS(3V)</sub> (начальный ток стока)"});
            row7.parameters.Add(new KurbatovParameter{ParameterName = "Ubr", Lower = 10.0, DividerId = 1, RussianParameterName="Напряжение пробоя сток-исток, В", ParameterNameStat="U<sub>BR</sub> (напряжение пробоя сток-исток)"});
            row7.parameters.Add(new KurbatovParameter{ParameterName = "S21(5GHz)", Lower = 7.8, DividerId = 1, RussianParameterName="Коэффициент передачи, дБ", ParameterNameStat="S21<sub>(5GHz)</sub>(коэффициент передачи)"});
            
            var row8 = new KurbatovXLSViewModel();
            row8.ElementName = "TC6";
            row8.OperationNumber = "570.00.00";
            row8.StageName = "Снятие рабочей пластины с пластины-носителя";
            row8.parameters.Add(new KurbatovParameter{ParameterName = "S21(on)", Lower = -1.5, DividerId = 1, RussianParameterName="Коэффициент передачи в открытом состоянии на частоте 20 ГГц, дБ", ParameterNameStat="S<sub>21ON(20GHz)</sub>"});
            row8.parameters.Add(new KurbatovParameter{ParameterName = "S21(off)", Upper = -15, DividerId = 1, RussianParameterName="Коэффициент передачи в закрытом состоянии на частоте 1 ГГц, дБ", ParameterNameStat="S<sub>21OFF(1GHz)</sub>"});
            row8.parameters.Add(new KurbatovParameter{ParameterName = "rDS(on)", Lower = 1.2, Upper = 3.0, Divider=0.6, DividerId = 10, RussianParameterName="Сопротивление открытого канала, Ом*мм", ParameterNameStat="R<sub>ds(on)</sub> (сопротивление открытого канала)"});
            row8.parameters.Add(new KurbatovParameter{ParameterName = "Idss(3V)", Lower = 0.3, Upper = 0.6, Divider=0.6, DividerId = 10, RussianParameterName="Начальный ток стока, A/мм", ParameterNameStat="I<sub>DSS(3V)</sub> (начальный ток стока)"});
            row8.parameters.Add(new KurbatovParameter{ParameterName = "Ugs(off)", Lower = -1.9, Upper = -1.25, DividerId = 1, RussianParameterName="Напряжение отсечки, В", ParameterNameStat="U<sub>GS(off)</sub> (напряжение отсечки при Idss/1000)"});
            
            
            var row9 = new KurbatovXLSViewModel();
            row9.ElementName = "TC8";
            row9.OperationNumber = "570.00.00";
            row9.StageName = "Снятие рабочей пластины с пластины-носителя";
            row9.parameters.Add(new KurbatovParameter{ParameterName = "Risol", Lower = 0.1, DividerId = 1, RussianParameterName="Сопротивление межприборной изоляции, ГОм", ParameterNameStat="Risol"});
            
            var row10 = new KurbatovXLSViewModel();
            row10.ElementName = "TC10";
            row10.OperationNumber = "570.00.00";
            row10.StageName = "Снятие рабочей пластины с пластины-носителя";
            row10.parameters.Add(new KurbatovParameter{ParameterName = "Rgate", Upper = 30, DividerId = 1, RussianParameterName="Сопротивление затвора, Ом", ParameterNameStat="Rgate"});

            var row11 = new KurbatovXLSViewModel();
            row11.ElementName = "TC12";
            row11.OperationNumber = "570.00.00";
            row11.StageName = "Снятие рабочей пластины с пластины-носителя";
            row11.parameters.Add(new KurbatovParameter{ParameterName = "Rc", Upper = 0.4, DividerId = 1, RussianParameterName="Контактное сопротивление омических контактов, Ом*мм", ParameterNameStat="Rc"});
            row11.parameters.Add(new KurbatovParameter{ParameterName = "Rs", Lower = 135, Upper = 185, DividerId = 1, RussianParameterName="Слоевое cопротивление, Ом/кв", ParameterNameStat="Rs"});
                        
            var row12 = new KurbatovXLSViewModel();
            row12.ElementName = "TC13";
            row12.OperationNumber = "570.00.00";
            row12.StageName = "Снятие рабочей пластины с пластины-носителя";
            row12.parameters.Add(new KurbatovParameter{ParameterName = "Rtfr_1", Lower = 40, Upper = 60, DividerId = 1, RussianParameterName="Удельное поверхностное сопротивление тонкопленочного резистора, Ом/кв", ParameterNameStat="TFR1_1sq"});

            var row13 = new KurbatovXLSViewModel();
            row13.ElementName = "TC14";
            row13.OperationNumber = "570.00.00";
            row13.StageName = "Снятие рабочей пластины с пластины-носителя";
            row13.parameters.Add(new KurbatovParameter{ParameterName = "Rtfr_2", Lower = 480, Upper = 720, DividerId = 1, RussianParameterName="Удельное поверхностное сопротивление тонкопленочного резистора, Ом/кв", ParameterNameStat="TFR2_1sq"});

            var row15 = new KurbatovXLSViewModel();
            row15.ElementName = "TC20";
            row15.OperationNumber = "570.00.00";
            row15.StageName = "Снятие рабочей пластины с пластины-носителя";
            row15.parameters.Add(new KurbatovParameter{ParameterName = "Rhole", Upper = 3, DividerId = 1, RussianParameterName="Сопротивление металлизированного отверстия, Ом", ParameterNameStat="Rhole"});

            var row16 = new KurbatovXLSViewModel();
            row16.ElementName = "TC21";
            row16.OperationNumber = "570.00.00";
            row16.StageName = "Снятие рабочей пластины с пластины-носителя";
            row16.parameters.Add(new KurbatovParameter{ParameterName = "Rind", Upper = 4, DividerId = 1, RussianParameterName="Сопротивление катушки индуктивности, Ом", ParameterNameStat="Rind"});
            
            var row17 = new KurbatovXLSViewModel();
            row17.ElementName = "TC23";
            row17.OperationNumber = "570.00.00";
            row17.StageName = "Снятие рабочей пластины с пластины-носителя";
            row17.parameters.Add(new KurbatovParameter{ParameterName = "Сmim", Lower = 270, Upper = 350, DividerId = 1, RussianParameterName="Удельная емкость МДМ-конденсатора, пФ/мм2", ParameterNameStat="C<sub>MIM</sub> при U=0.06B (удельная ёмкость МДМ-конденсатора)"});
            row17.parameters.Add(new KurbatovParameter{ParameterName = "Ubrc", Lower = 20, DividerId = 1, RussianParameterName="Пробивное напряжение МДМ-конденсатора, В", ParameterNameStat="U<sub>BRC</sub> (пробивное напряжение МДМ-конденсатора)"});
          
            xlsList.Add(row6);
            xlsList.Add(row7);
            xlsList.Add(row8);
            xlsList.Add(row9);
            xlsList.Add(row10);
            xlsList.Add(row11);
            xlsList.Add(row12);
            xlsList.Add(row13);
            xlsList.Add(row15);
            xlsList.Add(row16);
            xlsList.Add(row17);
            return Ok(xlsList);
        }

        [HttpGet]
        [Route(("pattern/sky"))]
        public IActionResult GetPatternSKY()
        {
            var xlsList = new List<KurbatovXLSViewModel>();
            var row1 = new KurbatovXLSViewModel();
            row1.ElementName = "TC16";
            row1.OperationNumber = "295.40.00";
            row1.StageName = "Этап1";
            row1.parameters.Add(new KurbatovParameter{ParameterName = "Rm2", Upper = 3, DividerId = 1, RussianParameterName="Сопротивление линии металлизации второго уровня, Ом", ParameterNameStat="R<sub>M2</sub>"});
            xlsList.Add(row1);

            var row7 = new KurbatovXLSViewModel();
            row7.ElementName = "Cap1";
            row7.OperationNumber = "295.45.00";
            row7.StageName = "Этап1";
            row7.parameters.Add(new KurbatovParameter{ParameterName = "Сmim", Lower = 275, Upper = 345, DividerId = 1, RussianParameterName="Удельная емкость МДМ-конденсатора, пФ/мм2", ParameterNameStat="C<sub>MIM</sub>"});
            row7.parameters.Add(new KurbatovParameter{ParameterName = "Ubr", Lower = 35.0, DividerId = 1, RussianParameterName="Напряжение пробоя сток-исток, В", ParameterNameStat="V<sub>(br)</sub> (напряжение при Ig=100нА)"});
            xlsList.Add(row7);

            var row8 = new KurbatovXLSViewModel();
            row8.ElementName = "Cap2";
            row8.OperationNumber = "295.55.00";
            row8.StageName = "Этап1";
            row8.parameters.Add(new KurbatovParameter{ParameterName = "Сmim", Lower = 125, Upper = 165, DividerId = 1, RussianParameterName="Удельная емкость МДМ-конденсатора, пФ/мм2", ParameterNameStat="C<sub>MIM</sub>"});
            row8.parameters.Add(new KurbatovParameter{ParameterName = "Ubr", Lower = 80.0, DividerId = 1, RussianParameterName="Пробивное напряжение конденсатора, пФ/мм2, В", ParameterNameStat="V<sub>(br)</sub> (напряжение при Ig=100нА)"});
            xlsList.Add(row8);

            var row9 = new KurbatovXLSViewModel();
            row9.ElementName = "Cap3";
            row9.OperationNumber = "295.65.00";
            row9.StageName = "Этап1";
            row9.parameters.Add(new KurbatovParameter{ParameterName = "Сmim", Lower = 525, Upper = 685, DividerId = 1, RussianParameterName="Удельная емкость МДМДМ-конденсатора, пФ/мм2", ParameterNameStat="C<sub>MIM</sub>"});
            row9.parameters.Add(new KurbatovParameter{ParameterName = "Ubr", Lower = 35.0, DividerId = 1, RussianParameterName="Пробивное напряжение конденсатора, пФ/мм2, В", ParameterNameStat="V<sub>(br)</sub> (напряжение при Ig=100нА)"});
            xlsList.Add(row9);
            
            return Ok(xlsList);
       }

        [HttpGet]
        [Route(("pattern/sky_"))]
        public IActionResult GetPatternSKY_()
        {
            var xlsList = new List<KurbatovXLSViewModel>();
            var row1 = new KurbatovXLSViewModel();
            row1.ElementName = "TC3";
            row1.OperationNumber = "105.10.00";
            row1.StageName = "Этап1";
            row1.parameters.Add(new KurbatovParameter{ParameterName = "Risol", Lower = 1E9, DividerId = 1, RussianParameterName="Сопротивление межприборной изоляции, ГОм", ParameterNameStat="R<sub>IS(6V)</sub>"});

            var row2 = new KurbatovXLSViewModel();
            row2.ElementName = "TC14";
            row2.OperationNumber = "180.55.00";
            row2.StageName = "Этап2";
            row2.parameters.Add(new KurbatovParameter{ParameterName = "Rm0", Upper = 20, DividerId = 1, RussianParameterName="Сопротивление линии металлизации нулевого уровня, Ом", ParameterNameStat="R<sub>M0</sub>"});

            var row3 = new KurbatovXLSViewModel();
            row3.ElementName = "TC15";
            row3.OperationNumber = "180.55.00";
            row3.StageName = "Этап2";
            row3.parameters.Add(new KurbatovParameter{ParameterName = "Rm1", Upper = 20, DividerId = 1, RussianParameterName="Сопротивление линии металлизации первого уровня, Ом", ParameterNameStat="R<sub>M1</sub>"});

            var row4 = new KurbatovXLSViewModel();
            row4.ElementName = "TC16";
            row4.OperationNumber = "180.55.00";
            row4.StageName = "Этап2";
            row4.parameters.Add(new KurbatovParameter{ParameterName = "Rm2", Upper = 3, DividerId = 1, RussianParameterName="Сопротивление линии металлизации второго уровня, Ом", ParameterNameStat="R<sub>M2</sub>"});

            var row5 = new KurbatovXLSViewModel();
            row5.ElementName = "TC4";
            row5.OperationNumber = "260.50.00";
            row5.StageName = "Этап3";
            row5.parameters.Add(new KurbatovParameter{ParameterName = "Rtfr1", Lower = 42, Upper = 58, DividerId = 1, RussianParameterName="Удельное поверхностное сопротивление тонкопленочного резистора, Ом/кв", ParameterNameStat="RTFR1"});

            var row6 = new KurbatovXLSViewModel();
            row6.ElementName = "TC5";
            row6.OperationNumber = "260.50.00";
            row6.StageName = "Этап3";
            row6.parameters.Add(new KurbatovParameter{ParameterName = "Rtfr2", Lower = 485, Upper = 715, DividerId = 1, RussianParameterName="Удельное поверхностное сопротивление тонкопленочного резистора, Ом/кв", ParameterNameStat="RTFR2"});

            var row7 = new KurbatovXLSViewModel();
            row7.ElementName = "TC6";
            row7.OperationNumber = "280.50.00";
            row7.StageName = "Этап3";
            row7.parameters.Add(new KurbatovParameter{ParameterName = "Сmim", Lower = 275, Upper = 345, DividerId = 1, RussianParameterName="Удельная емкость МДМ-конденсатора, П", ParameterNameStat="C<sub>MIM</sub>"});
            row7.parameters.Add(new KurbatovParameter{ParameterName = "Ubr", Lower = 40.0, DividerId = 1, RussianParameterName="Напряжение пробоя сток-исток, В", ParameterNameStat="V<sub>(br)</sub> (напряжение при Ig=100нА)"});
            
            var row8 = new KurbatovXLSViewModel();
            row8.ElementName = "TC7";
            row8.OperationNumber = "280.50.00";
            row8.StageName = "Этап3";
            row8.parameters.Add(new KurbatovParameter{ParameterName = "Сmim", Lower = 140, Upper = 185, DividerId = 1, RussianParameterName="Удельная емкость МДМ-конденсатора, пФ/мм2", ParameterNameStat="C<sub>MIM</sub>"});
            row8.parameters.Add(new KurbatovParameter{ParameterName = "Ubr", Lower = 90.0, DividerId = 1, RussianParameterName="Пробивное напряжение конденсатора, пФ/мм2, В", ParameterNameStat="V<sub>(br)</sub> (напряжение при Ig=100нА)"});

            var row9 = new KurbatovXLSViewModel();
            row9.ElementName = "TC8";
            row9.OperationNumber = "280.50.00";
            row9.StageName = "Этап3";
            row9.parameters.Add(new KurbatovParameter{ParameterName = "Сmim", Lower = 550, Upper = 690, DividerId = 1, RussianParameterName="Удельная емкость МДМДМ-конденсатора, пФ/мм2", ParameterNameStat="C<sub>MIM</sub>"});
            row9.parameters.Add(new KurbatovParameter{ParameterName = "Ubr", Lower = 40.0, DividerId = 1, RussianParameterName="Пробивное напряжение конденсатора, пФ/мм2, В", ParameterNameStat="V<sub>(br)</sub> (напряжение при Ig=100нА)"});

            var row10 = new KurbatovXLSViewModel();
            row10.ElementName = "TC3";
            row10.OperationNumber = "560.00.00";
            row10.StageName = "Этап4";
            row10.parameters.Add(new KurbatovParameter{ParameterName = "Risol", Lower = 1E9, DividerId = 1, RussianParameterName="Сопротивление межприборной изоляции, ГОм", ParameterNameStat="R<sub>IS(6V)</sub>"});
          
            var row11 = new KurbatovXLSViewModel();
            row11.ElementName = "TC4";
            row11.OperationNumber = "560.00.00";
            row11.StageName = "Этап4";
            row11.parameters.Add(new KurbatovParameter{ParameterName = "Rtfr1", Lower = 42, Upper = 58, DividerId = 1, RussianParameterName="Удельное поверхностное сопротивление тонкопленочного резистора, Ом/кв", ParameterNameStat="RTFR1"});

            var row12 = new KurbatovXLSViewModel();
            row12.ElementName = "TC5";
            row12.OperationNumber = "560.00.00";
            row12.StageName = "Этап4";
            row12.parameters.Add(new KurbatovParameter{ParameterName = "Rtfr2", Lower = 485, Upper = 715, DividerId = 1, RussianParameterName="Удельное поверхностное сопротивление тонкопленочного резистора, Ом/кв", ParameterNameStat="RTFR2"});
           
            var row13 = new KurbatovXLSViewModel();
            row13.ElementName = "TC6";
            row13.OperationNumber = "560.00.00";
            row13.StageName = "Этап4";
            row13.parameters.Add(new KurbatovParameter{ParameterName = "Сmim", Lower = 275, Upper = 345, DividerId = 1, RussianParameterName="Удельная емкость МДМ-конденсатора, пФ/мм2", ParameterNameStat="C<sub>MIM</sub>"});
            row13.parameters.Add(new KurbatovParameter{ParameterName = "Ubr", Lower = 40.0, DividerId = 1, RussianParameterName="Пробивное напряжение конденсатора,В", ParameterNameStat="V<sub>(br)</sub> (напряжение при Ig=100нА)"});
            
            var row14 = new KurbatovXLSViewModel();
            row14.ElementName = "TC7";
            row14.OperationNumber = "560.00.00";
            row14.StageName = "Этап4";
            row14.parameters.Add(new KurbatovParameter{ParameterName = "Сmim", Lower = 140, Upper = 185, DividerId = 1, RussianParameterName="Удельная емкость МДМ-конденсатора, пФ/мм2", ParameterNameStat="C<sub>MIM</sub>"});
            row14.parameters.Add(new KurbatovParameter{ParameterName = "Ubr", Lower = 90.0, DividerId = 1, RussianParameterName="Пробивное напряжение конденсатора, В", ParameterNameStat="V<sub>(br)</sub> (напряжение при Ig=100нА)"});

            var row15 = new KurbatovXLSViewModel();
            row15.ElementName = "TC8";
            row15.OperationNumber = "560.00.00";
            row15.StageName = "Этап4";
            row15.parameters.Add(new KurbatovParameter{ParameterName = "Сmim", Lower = 550, Upper = 690, DividerId = 1, RussianParameterName="Удельная емкость МДМДМ-конденсатора, пФ/мм2", ParameterNameStat="C<sub>MIM</sub>"});
            row15.parameters.Add(new KurbatovParameter{ParameterName = "Ubr", Lower = 40.0, DividerId = 1, RussianParameterName="Пробивное напряжение конденсатора, В", ParameterNameStat="V<sub>(br)</sub> (напряжение при Ig=100нА)"});

            var row16 = new KurbatovXLSViewModel();
            row16.ElementName = "TC12";
            row16.OperationNumber = "560.00.00";
            row16.StageName = "Этап4";
            row16.parameters.Add(new KurbatovParameter{ParameterName = "Rl", Upper = 3.6, DividerId = 1, RussianParameterName="Сопротивление катушки индуктивности, Ом", ParameterNameStat="Rl"});

            var row17 = new KurbatovXLSViewModel();
            row17.ElementName = "TC13";
            row17.OperationNumber = "560.00.00";
            row17.StageName = "Этап4";
            row17.parameters.Add(new KurbatovParameter{ParameterName = "Rlw", Upper = 6.8, DividerId = 1, RussianParameterName="Сопротивление катушки индуктивности сдвоенной, Ом", ParameterNameStat="Rlw"});

            var row18 = new KurbatovXLSViewModel();
            row18.ElementName = "TC20";
            row18.OperationNumber = "560.00.00";
            row18.StageName = "Этап4";
            row18.parameters.Add(new KurbatovParameter{ParameterName = "Rhole", Upper = 2.7, DividerId = 1, RussianParameterName="Сопротивление металлизированного отверстия, Ом", ParameterNameStat="R<sub>VIA</sub>"});
           
            xlsList.Add(row1);
            xlsList.Add(row2);
            xlsList.Add(row3);
            xlsList.Add(row4);
            xlsList.Add(row5);
            xlsList.Add(row6);
            xlsList.Add(row7);
            xlsList.Add(row8);
            xlsList.Add(row9);
            xlsList.Add(row10);
            xlsList.Add(row11);
            xlsList.Add(row12);
            xlsList.Add(row13);
            xlsList.Add(row14);
            xlsList.Add(row15);
            xlsList.Add(row16);
            xlsList.Add(row17);
            xlsList.Add(row18);
            return Ok(xlsList);
        }

        [HttpGet]
        [Route(("pattern/skyVP"))]
        public IActionResult GetPatternSKYVP()
        {
            var xlsList = new List<KurbatovXLSViewModel>();
            var row10 = new KurbatovXLSViewModel();
            row10.ElementName = "TC3";
            row10.OperationNumber = "570.00.00";
            row10.StageName = "ПриемкаВП";
            row10.parameters.Add(new KurbatovParameter{ParameterName = "Risol", Lower = 1E8, DividerId = 1, RussianParameterName="Сопротивление межприборной изоляции, Ом", ParameterNameStat="R<sub>IS(6V)</sub>"});
          
            var row11 = new KurbatovXLSViewModel();
            row11.ElementName = "TC4";
            row11.OperationNumber = "570.00.00";
            row11.StageName = "ПриемкаВП";
            row11.parameters.Add(new KurbatovParameter{ParameterName = "Rtfr1", Lower = 40, Upper = 60, DividerId = 1, RussianParameterName="Удельное поверхностное сопротивление тонкопленочного резистора, Ом/кв", ParameterNameStat="RTFR1"});

            var row12 = new KurbatovXLSViewModel();
            row12.ElementName = "TC5";
            row12.OperationNumber = "570.00.00";
            row12.StageName = "ПриемкаВП";
            row12.parameters.Add(new KurbatovParameter{ParameterName = "Rtfr2", Lower = 480, Upper = 720, DividerId = 1, RussianParameterName="Удельное поверхностное сопротивление тонкопленочного резистора, Ом/кв", ParameterNameStat="RTFR2"});
           
            var row13 = new KurbatovXLSViewModel();
            row13.ElementName = "TC6";
            row13.OperationNumber = "570.00.00";
            row13.StageName = "ПриемкаВП";
            row13.parameters.Add(new KurbatovParameter{ParameterName = "Сmim1", Lower = 270, Upper = 350, DividerId = 1, RussianParameterName="Удельная емкость МДМ-конденсатора, пФ/мм2", ParameterNameStat="C<sub>MIM</sub>"});
            row13.parameters.Add(new KurbatovParameter{ParameterName = "Ubrc1", Lower = 30.0, DividerId = 1, RussianParameterName="Пробивное напряжение конденсатора,В", ParameterNameStat="V<sub>(br)</sub> (напряжение при Ig=100нА)"});
            
            var row14 = new KurbatovXLSViewModel();
            row14.ElementName = "TC7";
            row14.OperationNumber = "570.00.00";
            row14.StageName = "ПриемкаВП";
            row14.parameters.Add(new KurbatovParameter{ParameterName = "Сmim2", Lower = 120, Upper = 170, DividerId = 1, RussianParameterName="Удельная емкость МДМ-конденсатора, пФ/мм2", ParameterNameStat="C<sub>MIM</sub>"});
            row14.parameters.Add(new KurbatovParameter{ParameterName = "Ubrc2", Lower = 70.0, DividerId = 1, RussianParameterName="Пробивное напряжение конденсатора, В", ParameterNameStat="V<sub>(br)</sub> (напряжение при Ig=100нА)"});

            var row15 = new KurbatovXLSViewModel();
            row15.ElementName = "TC8";
            row15.OperationNumber = "570.00.00";
            row15.StageName = "ПриемкаВП";
            row15.parameters.Add(new KurbatovParameter{ParameterName = "Сmimim", Lower = 520, Upper = 690, DividerId = 1, RussianParameterName="Удельная емкость МДМДМ-конденсатора, пФ/мм2", ParameterNameStat="C<sub>MIM</sub>"});
            row15.parameters.Add(new KurbatovParameter{ParameterName = "Ubr3", Lower = 30.0, DividerId = 1, RussianParameterName="Пробивное напряжение конденсатора, В", ParameterNameStat="V<sub>(br)</sub> (напряжение при Ig=100нА)"});

            var row16 = new KurbatovXLSViewModel();
            row16.ElementName = "TC12";
            row16.OperationNumber = "570.00.00";
            row16.StageName = "ПриемкаВП";
            row16.parameters.Add(new KurbatovParameter{ParameterName = "Rl", Upper = 4.0, DividerId = 1, RussianParameterName="Сопротивление катушки индуктивности, Ом", ParameterNameStat="Rl"});

            var row17 = new KurbatovXLSViewModel();
            row17.ElementName = "TC13";
            row17.OperationNumber = "570.00.00";
            row17.StageName = "ПриемкаВП";
            row17.parameters.Add(new KurbatovParameter{ParameterName = "Rlw", Upper = 7.0, DividerId = 1, RussianParameterName="Сопротивление катушки индуктивности сдвоенной, Ом", ParameterNameStat="Rlw"});

            var row18 = new KurbatovXLSViewModel();
            row18.ElementName = "TC20";
            row18.OperationNumber = "570.00.00";
            row18.StageName = "ПриемкаВП";
            row18.parameters.Add(new KurbatovParameter{ParameterName = "Rhole", Upper = 3.0, DividerId = 1, RussianParameterName="Сопротивление металлизированного отверстия, Ом", ParameterNameStat="R<sub>VIA</sub>"});
           
            xlsList.Add(row10);
            xlsList.Add(row11);
            xlsList.Add(row12);
            xlsList.Add(row13);
            xlsList.Add(row14);
            xlsList.Add(row15);
            xlsList.Add(row16);
            xlsList.Add(row17);
            xlsList.Add(row18);
            return Ok(xlsList);
        }

        [HttpGet]
        [Route(("pattern/zionpcm"))]
        public IActionResult GetPatternZionPcm()
        {
            var xlsList = new List<KurbatovXLSViewModel>();
            var row10 = new KurbatovXLSViewModel();
            row10.ElementName = "TC8_Ris";
            row10.OperationNumber = "570.00.00";
            row10.StageName = "ПриемкаВП";
            row10.parameters.Add(new KurbatovParameter{ParameterName = "Risol", Lower = 1E8, DividerId = 1, RussianParameterName="Сопротивление межприборной изоляции, Ом", ParameterNameStat="R<sub>IS(10V)</sub>"});
          
            var row11 = new KurbatovXLSViewModel();
            row11.ElementName = "TC13_TFR1";
            row11.OperationNumber = "570.00.00";
            row11.StageName = "ПриемкаВП";
            row11.parameters.Add(new KurbatovParameter{ParameterName = "RTFR_1", Lower = 40, Upper = 60, DividerId = 1, RussianParameterName="Удельное поверхностное сопротивление тонкопленочного резистора, Ом/кв", ParameterNameStat="RTFR1"});

            var row13 = new KurbatovXLSViewModel();
            row13.ElementName = "TC23_Cmim";
            row13.OperationNumber = "570.00.00";
            row13.StageName = "ПриемкаВП";
            row13.parameters.Add(new KurbatovParameter{ParameterName = "Сmim", Lower = 270, Upper = 350, DividerId = 1, RussianParameterName="Удельная емкость МДМ-конденсатора, пФ/мм2", ParameterNameStat="C<sub>MIM</sub> при U=0.06B (удельная ёмкость МДМ-конденсатора)"});
            row13.parameters.Add(new KurbatovParameter{ParameterName = "Ubrc", Lower = 20.0, DividerId = 1, RussianParameterName="Пробивное напряжение конденсатора,В", ParameterNameStat="U<sub>BRC</sub> (пробивное напряжение МДМ-конденсатора)"});
            
            var row16 = new KurbatovXLSViewModel();
            row16.ElementName = "TC21_L";
            row16.OperationNumber = "570.00.00";
            row16.StageName = "ПриемкаВП";
            row16.parameters.Add(new KurbatovParameter{ParameterName = "Rl", Upper = 4.0, DividerId = 1, RussianParameterName="Сопротивление катушки индуктивности, Ом", ParameterNameStat="Rl"});

            var row18 = new KurbatovXLSViewModel();
            row18.ElementName = "TC20_Rhole";
            row18.OperationNumber = "570.00.00";
            row18.StageName = "ПриемкаВП";
            row18.parameters.Add(new KurbatovParameter{ParameterName = "Rhole", Upper = 3.0, DividerId = 1, RussianParameterName="Сопротивление металлизированного отверстия, Ом", ParameterNameStat="Rhole"});
           
            xlsList.Add(row10);
            xlsList.Add(row11);
            xlsList.Add(row13);
            xlsList.Add(row16);
            xlsList.Add(row18);
            return Ok(xlsList);
        }

        [HttpGet]
        [Route(("pattern/zion"))]
        public IActionResult GetPatternZion()
        {
            var xlsList = new List<KurbatovXLSViewModel>();
            var row1 = new KurbatovXLSViewModel();
            row1.ElementName = "039_S21";
            row1.OperationNumber = "570.00.00";
            row1.StageName = "ПриемкаВП";
            row1.IsNeedToCopyS2P = true;
            row1.parameters.Add(new KurbatovParameter{ParameterName = "S21(2GHz)", Lower = -2.3, DividerId = 1, RussianParameterName="Коэффициент передачи S21 на частоте 2 ГГц, дБ", ParameterNameStat="S21<sub>(2GHz)</sub>"});
            row1.parameters.Add(new KurbatovParameter{ParameterName = "S21(5GHz)", Lower = -5.0, DividerId = 1, RussianParameterName="Коэффициент передачи S21 на частоте 5 ГГц, дБ", ParameterNameStat="S21<sub>(5GHz)</sub>"});
            row1.parameters.Add(new KurbatovParameter{ParameterName = "S21(7GHz)", Lower = -4.5, DividerId = 1, RussianParameterName="Коэффициент передачи S21 на частоте 7 ГГц, дБ", ParameterNameStat="S21<sub>(7GHz)</sub>"});
            xlsList.Add(row1);
            var row11 = new KurbatovXLSViewModel();
            row11.ElementName = "039_S31";
            row11.OperationNumber = "570.00.00";
            row11.StageName = "ПриемкаВП";
            row11.IsNeedToCopyS2P = true;
            row11.parameters.Add(new KurbatovParameter{ParameterName = "S31(2GHz)", Lower = -3.1, DividerId = 1, RussianParameterName="Коэффициент передачи S31 на частоте 2 ГГц, дБ", ParameterNameStat="S31<sub>(2GHz)</sub>"});
            row11.parameters.Add(new KurbatovParameter{ParameterName = "S31(5GHz)", Lower = -5.5, DividerId = 1, RussianParameterName="Коэффициент передачи S31 на частоте 5 ГГц, дБ", ParameterNameStat="S31<sub>(5GHz)</sub>"});
            row11.parameters.Add(new KurbatovParameter{ParameterName = "S31(7GHz)", Lower = -5.3, DividerId = 1, RussianParameterName="Коэффициент передачи S31 на частоте 7 ГГц, дБ", ParameterNameStat="S31<sub>(7GHz)</sub>"});
            xlsList.Add(row11);
            var row2 = new KurbatovXLSViewModel();
            row2.ElementName = "040_S21";
            row2.OperationNumber = "570.00.00";
            row2.StageName = "ПриемкаВП";
            row2.IsNeedToCopyS2P = true;
            row2.parameters.Add(new KurbatovParameter{ParameterName = "S21(2GHz)", Lower = -5.8, DividerId = 1, RussianParameterName="Коэффициент передачи S21 на частоте 2 ГГц, дБ", ParameterNameStat="S21<sub>(2GHz)</sub>"});
            row2.parameters.Add(new KurbatovParameter{ParameterName = "S21(5GHz)", Lower = -13.3, DividerId = 1, RussianParameterName="Коэффициент передачи S21 на частоте 5 ГГц, дБ", ParameterNameStat="S21<sub>(5GHz)</sub>"});
            row2.parameters.Add(new KurbatovParameter{ParameterName = "S21(7GHz)", Lower = -11.5, DividerId = 1, RussianParameterName="Коэффициент передачи S21 на частоте 7 ГГц, дБ", ParameterNameStat="S21<sub>(7GHz)</sub>"});
            xlsList.Add(row2);
            var row22 = new KurbatovXLSViewModel();
            row22.ElementName = "040_S31";
            row22.OperationNumber = "570.00.00";
            row22.StageName = "ПриемкаВП";
            row22.IsNeedToCopyS2P = true;
            row22.parameters.Add(new KurbatovParameter{ParameterName = "S31(2GHz)", Lower = -5.8, DividerId = 1, RussianParameterName="Коэффициент передачи S31 на частоте 2 ГГц, дБ", ParameterNameStat="S31<sub>(2GHz)</sub>"});
            row22.parameters.Add(new KurbatovParameter{ParameterName = "S31(5GHz)", Lower = -12.6, DividerId = 1, RussianParameterName="Коэффициент передачи S31 на частоте 5 ГГц, дБ", ParameterNameStat="S31<sub>(5GHz)</sub>"});
            row22.parameters.Add(new KurbatovParameter{ParameterName = "S31(7GHz)", Lower = -15.0, DividerId = 1, RussianParameterName="Коэффициент передачи S31 на частоте 7 ГГц, дБ", ParameterNameStat="S31<sub>(7GHz)</sub>"});
            xlsList.Add(row22);
            var row3 = new KurbatovXLSViewModel();
            row3.ElementName = "041_S21";
            row3.OperationNumber = "570.00.00";
            row3.StageName = "ПриемкаВП";
            row3.IsNeedToCopyS2P = true;
            row3.parameters.Add(new KurbatovParameter{ParameterName = "S21(2GHz)", Lower = -3.3, DividerId = 1, RussianParameterName="Коэффициент передачи S21  на частоте 2 ГГц, дБ", ParameterNameStat="S21<sub>(2GHz)</sub>"});
            row3.parameters.Add(new KurbatovParameter{ParameterName = "S21(5GHz)", Lower = -5.3, DividerId = 1, RussianParameterName="Коэффициент передачи S21 на частоте 5 ГГц, дБ", ParameterNameStat="S21<sub>(5GHz)</sub>"});
            row3.parameters.Add(new KurbatovParameter{ParameterName = "S21(7GHz)", Lower = -4.5, DividerId = 1, RussianParameterName="Коэффициент передачи S21 на частоте 7 ГГц, дБ", ParameterNameStat="S21<sub>(7GHz)</sub>"});
            xlsList.Add(row3);
            var row33 = new KurbatovXLSViewModel();
            row33.ElementName = "041_S31";
            row33.OperationNumber = "570.00.00";
            row33.StageName = "ПриемкаВП";
            row33.IsNeedToCopyS2P = true;
            row33.parameters.Add(new KurbatovParameter{ParameterName = "S31(2GHz)", Lower = -3.3, DividerId = 1, RussianParameterName="Коэффициент передачи S31 на частоте 2 ГГц, дБ", ParameterNameStat="S31<sub>(2GHz)</sub>"});
            row33.parameters.Add(new KurbatovParameter{ParameterName = "S31(5GHz)", Lower = -5.3, DividerId = 1, RussianParameterName="Коэффициент передачи S31 на частоте 5 ГГц, дБ", ParameterNameStat="S31<sub>(5GHz)</sub>"});
            row33.parameters.Add(new KurbatovParameter{ParameterName = "S31(7GHz)", Lower = -4.5, DividerId = 1, RussianParameterName="Коэффициент передачи S31 на частоте 7 ГГц, дБ", ParameterNameStat="S31<sub>(7GHz)</sub>"});
            xlsList.Add(row33);
            var row4 = new KurbatovXLSViewModel();
            row4.ElementName = "042_S21";
            row4.OperationNumber = "570.00.00";
            row4.StageName = "ПриемкаВП";
            row4.IsNeedToCopyS2P = true;
            row4.parameters.Add(new KurbatovParameter{ParameterName = "S21(4GHz)", Lower = -2.8, DividerId = 1, RussianParameterName="Коэффициент передачи S21 на частоте 4 ГГц, дБ", ParameterNameStat="S21<sub>(4GHz)</sub>"});
            row4.parameters.Add(new KurbatovParameter{ParameterName = "S21(9.5GHz)", Lower = -3.8, DividerId = 1, RussianParameterName="Коэффициент передачи S21 на частоте 9.5 ГГц, дБ", ParameterNameStat="S21<sub>(9.5GHz)</sub>"});
            row4.parameters.Add(new KurbatovParameter{ParameterName = "S21(13.5GHz)", Lower = -3.8, DividerId = 1, RussianParameterName="Коэффициент передачи S21 на частоте 13.5 ГГц, дБ", ParameterNameStat="S21<sub>(13.5GHz)</sub>"});
            xlsList.Add(row4);
            var row44 = new KurbatovXLSViewModel();
            row44.ElementName = "042_S31";
            row44.OperationNumber = "570.00.00";
            row44.StageName = "ПриемкаВП";
            row44.IsNeedToCopyS2P = true;
            row44.parameters.Add(new KurbatovParameter{ParameterName = "S31(4GHz)", Lower = -3.8, DividerId = 1, RussianParameterName="Коэффициент передачи S31 на частоте 4 ГГц, дБ", ParameterNameStat="S31<sub>(4GHz)</sub>"});
            row44.parameters.Add(new KurbatovParameter{ParameterName = "S31(9.5GHz)", Lower = -4.1, DividerId = 1, RussianParameterName="Коэффициент передачи S31 на частоте 9.5 ГГц, дБ", ParameterNameStat="S31<sub>(9.5GHz)</sub>"});
            row44.parameters.Add(new KurbatovParameter{ParameterName = "S31(13.5GHz)", Lower = -4.2, DividerId = 1, RussianParameterName="Коэффициент передачи S31 на частоте 13.5 ГГц, дБ", ParameterNameStat="S31<sub>(13.5GHz)</sub>"});
            xlsList.Add(row44);
            var row5 = new KurbatovXLSViewModel();
            row5.ElementName = "043_S21";
            row5.OperationNumber = "570.00.00";
            row5.StageName = "ПриемкаВП";
            row5.IsNeedToCopyS2P = true;
            row5.parameters.Add(new KurbatovParameter{ParameterName = "S21(5.5GHz)", Lower = -2.8, DividerId = 1, RussianParameterName="Коэффициент передачи S21 на частоте 5.5 ГГц, дБ", ParameterNameStat="S21<sub>(5.5GHz)</sub>"});
            row5.parameters.Add(new KurbatovParameter{ParameterName = "S21(9.5GHz)", Lower = -12.5, DividerId = 1, RussianParameterName="Коэффициент передачи S21 на частоте 9.5 ГГц, дБ", ParameterNameStat="S21<sub>(9.5GHz)</sub>"});
            row5.parameters.Add(new KurbatovParameter{ParameterName = "S21(13.5GHz)", Lower = -3.5, DividerId = 1, RussianParameterName="Коэффициент передачи S21 на частоте 13.5 ГГц, дБ", ParameterNameStat="S21<sub>(13.5GHz)</sub>"});
            xlsList.Add(row5);
            var row55 = new KurbatovXLSViewModel();
            row55.ElementName = "043_S31";
            row55.OperationNumber = "570.00.00";
            row55.StageName = "ПриемкаВП";
            row55.IsNeedToCopyS2P = true;
            row55.parameters.Add(new KurbatovParameter{ParameterName = "S31(5.5GHz)", Lower = -2.8, DividerId = 1, RussianParameterName="Коэффициент передачи S31 на частоте 5.5 ГГц, дБ", ParameterNameStat="S31<sub>(5.5GHz)</sub>"});
            row55.parameters.Add(new KurbatovParameter{ParameterName = "S31(9.5GHz)", Lower = -10.5, DividerId = 1, RussianParameterName="Коэффициент передачи S31 на частоте 9.5 ГГц, дБ", ParameterNameStat="S31<sub>(9.5GHz)</sub>"});
            row55.parameters.Add(new KurbatovParameter{ParameterName = "S31(13.5GHz)", Lower = -4.1, DividerId = 1, RussianParameterName="Коэффициент передачи S31 на частоте 13.5 ГГц, дБ", ParameterNameStat="S31<sub>(13.5GHz)</sub>"});
            xlsList.Add(row55);
            var row6 = new KurbatovXLSViewModel();
            row6.ElementName = "044_S21";
            row6.OperationNumber = "570.00.00";
            row6.StageName = "ПриемкаВП";
            row6.IsNeedToCopyS2P = true;
            row6.parameters.Add(new KurbatovParameter{ParameterName = "S21(5.5GHz)", Lower = -3.0, DividerId = 1, RussianParameterName="Коэффициент передачи S21 на частоте 5.5 ГГц, дБ", ParameterNameStat="S21<sub>(5.5GHz)</sub>"});
            row6.parameters.Add(new KurbatovParameter{ParameterName = "S21(9.5GHz)", Lower = -14, DividerId = 1, RussianParameterName="Коэффициент передачи S21 на частоте 9.5 ГГц, дБ", ParameterNameStat="S21<sub>(9.5GHz)</sub>"});
            row6.parameters.Add(new KurbatovParameter{ParameterName = "S21(13.5GHz)", Lower = -3.2, DividerId = 1, RussianParameterName="Коэффициент передачи S21 на частоте 13.5 ГГц, дБ", ParameterNameStat="S21<sub>(13.5GHz)</sub>"});
            xlsList.Add(row6);
            var row66 = new KurbatovXLSViewModel();
            row66.ElementName = "044_S31";
            row66.OperationNumber = "570.00.00";
            row66.StageName = "ПриемкаВП";
            row66.IsNeedToCopyS2P = true;
            row66.parameters.Add(new KurbatovParameter{ParameterName = "S31(5.5GHz)", Lower = -2.8, DividerId = 1, RussianParameterName="Коэффициент передачи S31 на частоте 5.5 ГГц, дБ", ParameterNameStat="S31<sub>(5.5GHz)</sub>"});
            row66.parameters.Add(new KurbatovParameter{ParameterName = "S31(9.5GHz)", Lower = -10.3, DividerId = 1, RussianParameterName="Коэффициент передачи S31 на частоте 9.5 ГГц, дБ", ParameterNameStat="S31<sub>(9.5GHz)</sub>"});
            row66.parameters.Add(new KurbatovParameter{ParameterName = "S31(13.5GHz)", Lower = -3.9, DividerId = 1, RussianParameterName="Коэффициент передачи S31 на частоте 13.5 ГГц, дБ", ParameterNameStat="S31<sub>(13.5GHz)</sub>"});
            xlsList.Add(row66);
            return Ok(xlsList);
        }



        [HttpGet]
        [Route(("pattern/va50n"))]
        public IActionResult GetPatternVa50N()
        {
            var xlsList = new List<KurbatovXLSViewModel>();
            var row1 = new KurbatovXLSViewModel();
            row1.ElementName = "SE1";
            row1.OperationNumber = "570.00.00";
            row1.StageName = "Измерение библиотеки";
            row1.parameters.Add(new KurbatovParameter{ParameterName = "Rtfr1", Lower = 40, Upper = 60, DividerId = 1, RussianParameterName="Удельное поверхностное сопротивление тонкопленочного резистора, Ом/кв", ParameterNameStat="TFR1"});
          
            var row2 = new KurbatovXLSViewModel();
            row2.ElementName = "SE2";
            row2.OperationNumber = "570.00.00";
            row2.StageName = "Измерение библиотеки";
            row2.parameters.Add(new KurbatovParameter{ParameterName = "Rtfr1", Lower = 40, Upper = 60, DividerId = 1, RussianParameterName="Удельное поверхностное сопротивление тонкопленочного резистора, Ом/кв", ParameterNameStat="TFR1"});

            var row3 = new KurbatovXLSViewModel();
            row3.ElementName = "SE3";
            row3.OperationNumber = "570.00.00";
            row3.StageName = "Измерение библиотеки";
            row3.parameters.Add(new KurbatovParameter{ParameterName = "Rtfr1", Lower = 40, Upper = 60, DividerId = 1, RussianParameterName="Удельное поверхностное сопротивление тонкопленочного резистора, Ом/кв", ParameterNameStat="TFR1"});

            var row4 = new KurbatovXLSViewModel();
            row4.ElementName = "SE4";
            row4.OperationNumber = "570.00.00";
            row4.StageName = "Измерение библиотеки";
            row4.parameters.Add(new KurbatovParameter{ParameterName = "Rtfr1", Lower = 40, Upper = 60, DividerId = 1, RussianParameterName="Удельное поверхностное сопротивление тонкопленочного резистора, Ом/кв", ParameterNameStat="TFR1"});

            var row5 = new KurbatovXLSViewModel();
            row5.ElementName = "SE5";
            row5.OperationNumber = "570.00.00";
            row5.StageName = "Измерение библиотеки";
            row5.parameters.Add(new KurbatovParameter{ParameterName = "Rtfr2", Lower = 480, Upper = 720, DividerId = 1, RussianParameterName="Удельное поверхностное сопротивление тонкопленочного резистора, Ом/кв", ParameterNameStat="TFR2"});
           
            var row6 = new KurbatovXLSViewModel();
            row6.ElementName = "SE6";
            row6.OperationNumber = "570.00.00";
            row6.StageName = "Измерение библиотеки";
            row6.parameters.Add(new KurbatovParameter{ParameterName = "Rtfr2", Lower = 480, Upper = 720, DividerId = 1, RussianParameterName="Удельное поверхностное сопротивление тонкопленочного резистора, Ом/кв", ParameterNameStat="TFR2"});

            var row7 = new KurbatovXLSViewModel();
            row7.ElementName = "SE7";
            row7.OperationNumber = "570.00.00";
            row7.StageName = "Измерение библиотеки";
            row7.parameters.Add(new KurbatovParameter{ParameterName = "Rtfr2", Lower = 480, Upper = 720, DividerId = 1, RussianParameterName="Удельное поверхностное сопротивление тонкопленочного резистора, Ом/кв", ParameterNameStat="TFR2"});

            var row8 = new KurbatovXLSViewModel();
            row8.ElementName = "SE8";
            row8.OperationNumber = "570.00.00";
            row8.StageName = "Измерение библиотеки";
            row8.parameters.Add(new KurbatovParameter{ParameterName = "Rtfr2", Lower = 480, Upper = 720, DividerId = 1, RussianParameterName="Удельное поверхностное сопротивление тонкопленочного резистора, Ом/кв", ParameterNameStat="TFR2"});

            var row9 = new KurbatovXLSViewModel();
            row9.ElementName = "SE9";
            row9.OperationNumber = "570.00.00";
            row9.StageName = "Измерение библиотеки";
            row9.parameters.Add(new KurbatovParameter{ParameterName = "Сmim1", Lower = 270, Upper = 350, DividerId = 1, RussianParameterName="Удельная емкость МДМ-конденсатора, пФ/мм2", ParameterNameStat="CMIM1"});
            row9.parameters.Add(new KurbatovParameter{ParameterName = "Ubrc1", Lower = 30.0, DividerId = 1, RussianParameterName="Пробивное напряжение конденсатора,В", ParameterNameStat="V<sub>brc</sub> (пробивное напряжение МДМ-конденсатора)"});

            var row10 = new KurbatovXLSViewModel();
            row10.ElementName = "SE10";
            row10.OperationNumber = "570.00.00";
            row10.StageName = "Измерение библиотеки";
            row10.parameters.Add(new KurbatovParameter{ParameterName = "Сmim1", Lower = 270, Upper = 350, DividerId = 1, RussianParameterName="Удельная емкость МДМ-конденсатора, пФ/мм2", ParameterNameStat="CMIM1"});
            row10.parameters.Add(new KurbatovParameter{ParameterName = "Ubrc1", Lower = 30.0, DividerId = 1, RussianParameterName="Пробивное напряжение конденсатора,В", ParameterNameStat="V<sub>brc</sub> (пробивное напряжение МДМ-конденсатора)"});

            var row11 = new KurbatovXLSViewModel();
            row11.ElementName = "SE11";
            row11.OperationNumber = "570.00.00";
            row11.StageName = "Измерение библиотеки";
            row11.parameters.Add(new KurbatovParameter{ParameterName = "Сmim1", Lower = 270, Upper = 350, DividerId = 1, RussianParameterName="Удельная емкость МДМ-конденсатора, пФ/мм2", ParameterNameStat="CMIM1"});
            row11.parameters.Add(new KurbatovParameter{ParameterName = "Ubrc1", Lower = 30.0, DividerId = 1, RussianParameterName="Пробивное напряжение конденсатора,В", ParameterNameStat="V<sub>brc</sub> (пробивное напряжение МДМ-конденсатора)"});

            var row12 = new KurbatovXLSViewModel();
            row12.ElementName = "SE12";
            row12.OperationNumber = "570.00.00";
            row12.StageName = "Измерение библиотеки";
            row12.parameters.Add(new KurbatovParameter{ParameterName = "Сmim1", Lower = 270, Upper = 350, DividerId = 1, RussianParameterName="Удельная емкость МДМ-конденсатора, пФ/мм2", ParameterNameStat="CMIM1"});
            row12.parameters.Add(new KurbatovParameter{ParameterName = "Ubrc1", Lower = 30.0, DividerId = 1, RussianParameterName="Пробивное напряжение конденсатора,В", ParameterNameStat="V<sub>brc</sub> (пробивное напряжение МДМ-конденсатора)"});

            var row13 = new KurbatovXLSViewModel();
            row13.ElementName = "SE13";
            row13.OperationNumber = "570.00.00";
            row13.StageName = "Измерение библиотеки";
            row13.parameters.Add(new KurbatovParameter{ParameterName = "Сmim2", Lower = 120, Upper = 165, DividerId = 1, RussianParameterName="Удельная емкость МДМ-конденсатора, пФ/мм2", ParameterNameStat="CMIM2"});
            row13.parameters.Add(new KurbatovParameter{ParameterName = "Ubrc2", Lower = 70.0, DividerId = 1, RussianParameterName="Пробивное напряжение конденсатора,В", ParameterNameStat="V<sub>brc</sub> (пробивное напряжение МДМ-конденсатора)"});

            var row14 = new KurbatovXLSViewModel();
            row14.ElementName = "SE14";
            row14.OperationNumber = "570.00.00";
            row14.StageName = "Измерение библиотеки";
            row14.parameters.Add(new KurbatovParameter{ParameterName = "Сmim2", Lower = 120, Upper = 165, DividerId = 1, RussianParameterName="Удельная емкость МДМ-конденсатора, пФ/мм2", ParameterNameStat="CMIM2"});
            row14.parameters.Add(new KurbatovParameter{ParameterName = "Ubrc2", Lower = 70.0, DividerId = 1, RussianParameterName="Пробивное напряжение конденсатора,В", ParameterNameStat="V<sub>brc</sub> (пробивное напряжение МДМ-конденсатора)"});

            var row15 = new KurbatovXLSViewModel();
            row15.ElementName = "SE15";
            row15.OperationNumber = "570.00.00";
            row15.StageName = "Измерение библиотеки";
            row15.parameters.Add(new KurbatovParameter{ParameterName = "Сmim2", Lower = 120, Upper = 165, DividerId = 1, RussianParameterName="Удельная емкость МДМ-конденсатора, пФ/мм2", ParameterNameStat="CMIM2"});
            row15.parameters.Add(new KurbatovParameter{ParameterName = "Ubrc2", Lower = 70.0, DividerId = 1, RussianParameterName="Пробивное напряжение конденсатора,В", ParameterNameStat="V<sub>brc</sub> (пробивное напряжение МДМ-конденсатора)"});

            var row16 = new KurbatovXLSViewModel();
            row16.ElementName = "SE16";
            row16.OperationNumber = "570.00.00";
            row16.StageName = "Измерение библиотеки";
            row16.parameters.Add(new KurbatovParameter{ParameterName = "Сmim2", Lower = 120, Upper = 165, DividerId = 1, RussianParameterName="Удельная емкость МДМ-конденсатора, пФ/мм2", ParameterNameStat="CMIM2"});
            row16.parameters.Add(new KurbatovParameter{ParameterName = "Ubrc2", Lower = 70.0, DividerId = 1, RussianParameterName="Пробивное напряжение конденсатора,В", ParameterNameStat="V<sub>brc</sub> (пробивное напряжение МДМ-конденсатора)"});

            var row17 = new KurbatovXLSViewModel();
            row17.ElementName = "SE17";
            row17.OperationNumber = "570.00.00";
            row17.StageName = "Измерение библиотеки";
            row17.parameters.Add(new KurbatovParameter{ParameterName = "Сmimim", Lower = 520, Upper = 680, DividerId = 1, RussianParameterName="Удельная емкость МДМ-конденсатора, пФ/мм2", ParameterNameStat="CMIMIM"});
            row17.parameters.Add(new KurbatovParameter{ParameterName = "Ubrc3", Lower = 30.0, DividerId = 1, RussianParameterName="Пробивное напряжение конденсатора,В", ParameterNameStat="V<sub>brc</sub> (пробивное напряжение МДМ-конденсатора)"});
            
            var row18 = new KurbatovXLSViewModel();
            row18.ElementName = "SE18";
            row18.OperationNumber = "570.00.00";
            row18.StageName = "Измерение библиотеки";
            row18.parameters.Add(new KurbatovParameter{ParameterName = "Сmimim", Lower = 520, Upper = 680, DividerId = 1, RussianParameterName="Удельная емкость МДМ-конденсатора, пФ/мм2", ParameterNameStat="CMIMIM"});
            row18.parameters.Add(new KurbatovParameter{ParameterName = "Ubrc3", Lower = 30.0, DividerId = 1, RussianParameterName="Пробивное напряжение конденсатора,В", ParameterNameStat="V<sub>brc</sub> (пробивное напряжение МДМ-конденсатора)"});

            var row19 = new KurbatovXLSViewModel();
            row19.ElementName = "SE19";
            row19.OperationNumber = "570.00.00";
            row19.StageName = "Измерение библиотеки";
            row19.parameters.Add(new KurbatovParameter{ParameterName = "Сmimim", Lower = 520, Upper = 680, DividerId = 1, RussianParameterName="Удельная емкость МДМ-конденсатора, пФ/мм2", ParameterNameStat="CMIMIM"});
            row19.parameters.Add(new KurbatovParameter{ParameterName = "Ubrc3", Lower = 30.0, DividerId = 1, RussianParameterName="Пробивное напряжение конденсатора,В", ParameterNameStat="V<sub>brc</sub> (пробивное напряжение МДМ-конденсатора)"});
            
            var row20 = new KurbatovXLSViewModel();
            row20.ElementName = "SE20";
            row20.OperationNumber = "570.00.00";
            row20.StageName = "Измерение библиотеки";
            row20.parameters.Add(new KurbatovParameter{ParameterName = "Сmimim", Lower = 520, Upper = 680, DividerId = 1, RussianParameterName="Удельная емкость МДМ-конденсатора, пФ/мм2", ParameterNameStat="CMIMIM"});
            row20.parameters.Add(new KurbatovParameter{ParameterName = "Ubrc3", Lower = 30.0, DividerId = 1, RussianParameterName="Пробивное напряжение конденсатора,В", ParameterNameStat="V<sub>brc</sub> (пробивное напряжение МДМ-конденсатора)"});

            var row21 = new KurbatovXLSViewModel();
            row21.ElementName = "SE21";
            row21.OperationNumber = "570.00.00";
            row21.StageName = "Измерение библиотеки";
            row21.parameters.Add(new KurbatovParameter{ParameterName = "S11", Upper = -15, DividerId = 1, RussianParameterName="Коэффициент отражения на частоте 20 ГГц, дБ", ParameterNameStat="S11<sub>(20GHz)</sub>"});
            row21.parameters.Add(new KurbatovParameter{ParameterName = "S21", Lower = -1, DividerId = 1, RussianParameterName="Коэффициент передачи на частоте 20 ГГц, дБ", ParameterNameStat="S21<sub>(20GHz)</sub>"});

            var row22 = new KurbatovXLSViewModel();
            row22.ElementName = "SE22";
            row22.OperationNumber = "570.00.00";
            row22.StageName = "Измерение библиотеки";
            row22.parameters.Add(new KurbatovParameter{ParameterName = "S11", Upper = -15, DividerId = 1, RussianParameterName="Коэффициент отражения на частоте 20 ГГц, дБ", ParameterNameStat="S11<sub>(20GHz)</sub>"});
            row22.parameters.Add(new KurbatovParameter{ParameterName = "S21", Lower = -1, DividerId = 1, RussianParameterName="Коэффициент передачи на частоте 20 ГГц, дБ", ParameterNameStat="S21<sub>(20GHz)</sub>"});

            var row23 = new KurbatovXLSViewModel();
            row23.ElementName = "SE23";
            row23.OperationNumber = "570.00.00";
            row23.StageName = "Измерение библиотеки";
            row23.parameters.Add(new KurbatovParameter{ParameterName = "S11", Upper = -1, DividerId = 1, RussianParameterName="Коэффициент отражения на частоте 5 ГГц, дБ", ParameterNameStat="S11<sub>(5GHz)</sub>"});
            row23.parameters.Add(new KurbatovParameter{ParameterName = "S21", Lower = -7, DividerId = 1, RussianParameterName="Коэффициент передачи на частоте 5 ГГц, дБ", ParameterNameStat="S21<sub>(5GHz)</sub>"});

            var row24 = new KurbatovXLSViewModel();
            row24.ElementName = "SE24";
            row24.OperationNumber = "570.00.00";
            row24.StageName = "Измерение библиотеки";
            row24.parameters.Add(new KurbatovParameter{ParameterName = "S11", Upper = -0.5, DividerId = 1, RussianParameterName="Коэффициент отражения на частоте 5 ГГц, дБ", ParameterNameStat="S11<sub>(5GHz)</sub>"});
            row24.parameters.Add(new KurbatovParameter{ParameterName = "S21", Lower = -9, DividerId = 1, RussianParameterName="Коэффициент передачи на частоте 5 ГГц, дБ", ParameterNameStat="S21<sub>(5GHz)</sub>"});
           
            var row25 = new KurbatovXLSViewModel();
            row25.ElementName = "SE25";
            row25.OperationNumber = "570.00.00";
            row25.StageName = "Измерение библиотеки";
            row25.parameters.Add(new KurbatovParameter{ParameterName = "S11", Upper = -5, DividerId = 1, RussianParameterName="Коэффициент отражения на частоте 20 ГГц, дБ", ParameterNameStat="S11<sub>(20GHz)</sub>"});
            row25.parameters.Add(new KurbatovParameter{ParameterName = "S21", Lower = -1.7, DividerId = 1, RussianParameterName="Коэффициент передачи на частоте 20 ГГц, дБ", ParameterNameStat="S21<sub>(20GHz)</sub>"});

            var row26 = new KurbatovXLSViewModel();
            row26.ElementName = "SE26";
            row26.OperationNumber = "570.00.00";
            row26.StageName = "Измерение библиотеки";
            row26.parameters.Add(new KurbatovParameter{ParameterName = "S11", Upper = -2, DividerId = 1, RussianParameterName="Коэффициент отражения на частоте 20 ГГц, дБ", ParameterNameStat="S11<sub>(20GHz)</sub>"});
            row26.parameters.Add(new KurbatovParameter{ParameterName = "S21", Lower = -5, DividerId = 1, RussianParameterName="Коэффициент передачи на частоте 20 ГГц, дБ", ParameterNameStat="S21<sub>(20GHz)</sub>"});

            var row27 = new KurbatovXLSViewModel();
            row27.ElementName = "SE27";
            row27.OperationNumber = "570.00.00";
            row27.StageName = "Измерение библиотеки";
            row27.parameters.Add(new KurbatovParameter{ParameterName = "S11", Upper = -1, DividerId = 1, RussianParameterName="Коэффициент отражения на частоте 5 ГГц, дБ", ParameterNameStat="S11<sub>(5GHz)</sub>"});
            row27.parameters.Add(new KurbatovParameter{ParameterName = "S21", Lower = -8, DividerId = 1, RussianParameterName="Коэффициент передачи на частоте 5 ГГц, дБ", ParameterNameStat="S21<sub>(5GHz)</sub>"});

            var row28 = new KurbatovXLSViewModel();
            row28.ElementName = "SE28";
            row28.OperationNumber = "570.00.00";
            row28.StageName = "Измерение библиотеки";
            row28.parameters.Add(new KurbatovParameter{ParameterName = "S11", Upper = -0.5, DividerId = 1, RussianParameterName="Коэффициент отражения на частоте 5 ГГц, дБ", ParameterNameStat="S11<sub>(5GHz)</sub>"});
            row28.parameters.Add(new KurbatovParameter{ParameterName = "S21", Lower = -20, DividerId = 1, RussianParameterName="Коэффициент передачи на частоте 5 ГГц, дБ", ParameterNameStat="S21<sub>(5GHz)</sub>"});

            var row29 = new KurbatovXLSViewModel();
            row29.ElementName = "SE29";
            row29.OperationNumber = "570.00.00";
            row29.StageName = "Измерение библиотеки";
            row29.parameters.Add(new KurbatovParameter{ParameterName = "S21", Lower = -0.7, DividerId = 1, RussianParameterName="Коэффициент передачи на частоте 20 ГГц, дБ", ParameterNameStat="S21<sub>(20GHz)</sub>"});

            var row30 = new KurbatovXLSViewModel();
            row30.ElementName = "SE30";
            row30.OperationNumber = "570.00.00";
            row30.StageName = "Измерение библиотеки";
            row30.parameters.Add(new KurbatovParameter{ParameterName = "S21", Lower = -0.7, DividerId = 1, RussianParameterName="Коэффициент передачи на частоте 20 ГГц, дБ", ParameterNameStat="S21<sub>(20GHz)</sub>"});

            var row31 = new KurbatovXLSViewModel();
            row31.ElementName = "SE31";
            row31.OperationNumber = "570.00.00";
            row31.StageName = "Измерение библиотеки";
            row31.parameters.Add(new KurbatovParameter{ParameterName = "S21", Lower = -1.5, DividerId = 1, RussianParameterName="Коэффициент передачи на частоте 20 ГГц, дБ", ParameterNameStat="S21<sub>(20GHz)</sub>"});

            var row32 = new KurbatovXLSViewModel();
            row32.ElementName = "SE32";
            row32.OperationNumber = "570.00.00";
            row32.StageName = "Измерение библиотеки";
            row32.parameters.Add(new KurbatovParameter{ParameterName = "S21", Lower = -2.0, DividerId = 1, RussianParameterName="Коэффициент передачи на частоте 20 ГГц, дБ", ParameterNameStat="S21<sub>(20GHz)</sub>"});
            
            var row33 = new KurbatovXLSViewModel();
            row33.ElementName = "SE33";
            row33.OperationNumber = "570.00.00";
            row33.StageName = "Измерение библиотеки";
            row33.parameters.Add(new KurbatovParameter{ParameterName = "S21", Lower = -1.2, DividerId = 1, RussianParameterName="Коэффициент передачи на частоте 20 ГГц, дБ", ParameterNameStat="S21<sub>(20GHz)</sub>"});
            
            var row34 = new KurbatovXLSViewModel();
            row34.ElementName = "SE34";
            row34.OperationNumber = "570.00.00";
            row34.StageName = "Измерение библиотеки";
            row34.parameters.Add(new KurbatovParameter{ParameterName = "S21", Lower = -0.7, DividerId = 1, RussianParameterName="Коэффициент передачи на частоте 20 ГГц, дБ", ParameterNameStat="S21<sub>(20GHz)</sub>"});

            var row35 = new KurbatovXLSViewModel();
            row35.ElementName = "SE35";
            row35.OperationNumber = "570.00.00";
            row35.StageName = "Измерение библиотеки";
            row35.parameters.Add(new KurbatovParameter{ParameterName = "Rp", Upper = 2, DividerId = 1, RussianParameterName="Сопротивление контактной площадки, Ом", ParameterNameStat="Rp"});
            
            var row36 = new KurbatovXLSViewModel();
            row36.ElementName = "SE36";
            row36.OperationNumber = "570.00.00";
            row36.StageName = "Измерение библиотеки";
            row36.parameters.Add(new KurbatovParameter{ParameterName = "Rhole", Upper = 5, DividerId = 1, RussianParameterName="Сопротивление переходного отверстия, Ом", ParameterNameStat="Rhole"});
            row36.parameters.Add(new KurbatovParameter{ParameterName = "S11", Lower = -2, DividerId = 1, RussianParameterName="Коэффициент отражения на частоте 20 ГГц, дБ", ParameterNameStat="S11<sub>(20GHz)</sub>"});

            xlsList.Add(row1);
            xlsList.Add(row2);
            xlsList.Add(row3);
            xlsList.Add(row4);
            xlsList.Add(row5);
            xlsList.Add(row6);
            xlsList.Add(row7);
            xlsList.Add(row8);
            xlsList.Add(row9);
            xlsList.Add(row10);
            xlsList.Add(row11);
            xlsList.Add(row12);
            xlsList.Add(row13);
            xlsList.Add(row14);
            xlsList.Add(row15);
            xlsList.Add(row16);
            xlsList.Add(row17);
            xlsList.Add(row18);
            xlsList.Add(row19);
            xlsList.Add(row20);
            xlsList.Add(row21);
            xlsList.Add(row22);
            xlsList.Add(row23);
            xlsList.Add(row24);
            xlsList.Add(row25);
            xlsList.Add(row26);
            xlsList.Add(row27);
            xlsList.Add(row28);
            xlsList.Add(row29);
            xlsList.Add(row30);
            xlsList.Add(row31);
            xlsList.Add(row32);
            xlsList.Add(row33);
            xlsList.Add(row34);
            xlsList.Add(row35);
            xlsList.Add(row36);
            return Ok(xlsList);
        }


        [HttpGet]
        [Route(("pattern/kurb"))]
        public IActionResult GetPatternKurb()
        {
            var xlsList = new List<KurbatovXLSViewModel>();
            var row1 = new KurbatovXLSViewModel();
            row1.ElementName = "TC16";            
            row1.OperationNumber = "180.50.00";
            row1.IsAddedToCommonWorksheet = false;
            row1.StageName = "Формирование разводки и нижней обкладки";
            row1.parameters.Add(new KurbatovParameter{ParameterName = "Ro-m1", Upper = 17, DividerId = 1, RussianParameterName="Сопротивление цепочки, Ом", ParameterNameStat="Rchain_OK_RAZV"});
            
            var row2 = new KurbatovXLSViewModel();
            row2.ElementName = "TC17";
            row2.OperationNumber = "180.55.00";
            row2.IsAddedToCommonWorksheet = false;
            row2.StageName = "Формирование разводки и нижней обкладки";
            row2.parameters.Add(new KurbatovParameter{ParameterName = "Rg-m1", Upper = 12, DividerId = 1, RussianParameterName="Сопротивление цепочки, Ом", ParameterNameStat="Rchain_GATE_RAZV"});
            
            var row3 = new KurbatovXLSViewModel();
            row3.ElementName = "TC18";
            row3.OperationNumber = "295.65.00";
            row3.IsAddedToCommonWorksheet = false;
            row3.StageName = "Формирование верхней обкладки, мостов, катушек, гальваники";
            row3.parameters.Add(new KurbatovParameter{ParameterName = "Rm1-m2", Upper = 7, DividerId = 1, RussianParameterName="Сопротивление цепочки, Ом", ParameterNameStat="Rchain_RAZV_GALV"});
           
            var row4 = new KurbatovXLSViewModel();
            row4.ElementName = "TC8";
            row4.OperationNumber = "317.10.00";
            row4.IsAddedToCommonWorksheet = false;
            row4.StageName = "Формирование защиты ВСВ";
            row4.parameters.Add(new KurbatovParameter{ParameterName = "Risol", Lower = 0.15, DividerId = 1, RussianParameterName="Сопротивление межприборной изоляции, ГОм", ParameterNameStat="Risol"});
            
            var row5 = new KurbatovXLSViewModel();
            row5.ElementName = "TC5";
            row5.OperationNumber = "317.25.00";
            row5.IsAddedToCommonWorksheet = false;
            row5.StageName = "Формирование защиты ВСВ";
            row5.parameters.Add(new KurbatovParameter{ParameterName = "rDS(on)", Lower = 1.25, Upper = 2.1, DividerId = 3, Divider = 1000/75, RussianParameterName="Сопротивление открытого канала, Ом*мм", ParameterNameStat="r<sub>DS(on)</sub> (сопротивление открытого канала при Uси = 0.02В)"});
            row5.parameters.Add(new KurbatovParameter{ParameterName = "Ugs(off)", Lower = -1.8, Upper = -1.3, DividerId = 1, RussianParameterName="Напряжение отсечки, В", ParameterNameStat="U<sub>GS(off)</sub> (напряжение отсечки при Idss/1000)"});
            row5.parameters.Add(new KurbatovParameter{ParameterName = "gmax", Lower = 0.32, Divider = 0.075, DividerId = 3, RussianParameterName="Максимум крутизны, См/мм", ParameterNameStat="g<sub>max</sub> (максимум крутизны)"});
            row5.parameters.Add(new KurbatovParameter{ParameterName = "Igss(-3V)", Lower = -2E-5, Divider = 0.075, DividerId = 3, RussianParameterName="Ток утечки затвора, А/мм", ParameterNameStat="I<sub>GSS(-3V)</sub> (ток утечки затвора при Uзи=-3В)"});
            row5.parameters.Add(new KurbatovParameter{ParameterName = "Idss(3V)", Lower = 0.32, Upper = 0.56, Divider = 0.075, DividerId = 3, RussianParameterName="Начальный ток стока, A/мм", ParameterNameStat="I<sub>DSS(3V)</sub> (начальный ток стока)"});
            row5.parameters.Add(new KurbatovParameter{ParameterName = "Ubr", Lower = 11.0, DividerId = 1, RussianParameterName="Напряжение пробоя сток-исток, В", ParameterNameStat="U<sub>BR</sub> (напряжение пробоя сток-исток)"});
            row5.parameters.Add(new KurbatovParameter{ParameterName = "S21(5GHz)", Lower = 8.0, DividerId = 1, RussianParameterName="Коэффициент передачи, дБ", ParameterNameStat="S21<sub>(5GHz)</sub>(коэффициент передачи)"});
         
            var row6 = new KurbatovXLSViewModel();
            row6.ElementName = "TC1";
            row6.OperationNumber = "560.00.00";
            row6.StageName = "Снятие рабочей пластины с пластины-носителя";
            row6.parameters.Add(new KurbatovParameter{ParameterName = "Cmicap", Lower = 0.1E-12, Upper = 0.6E-12, DividerId = 1, RussianParameterName="Емкость элемента, Ф", ParameterNameStat="C при U=0.06В"});

            var row7 = new KurbatovXLSViewModel();
            row7.ElementName = "TC5";
            row7.OperationNumber = "560.00.00";
            row7.StageName = "Снятие рабочей пластины с пластины-носителя";
            row7.parameters.Add(new KurbatovParameter{ParameterName = "rDS(on)", Lower = 1.25, Upper = 2.1, Divider = 1000/75, DividerId = 3, RussianParameterName="Сопротивление открытого канала, Ом*мм", ParameterNameStat="r<sub>DS(on)</sub> (сопротивление открытого канала при Uси = 0.02В)"});
            row7.parameters.Add(new KurbatovParameter{ParameterName = "Ugs(off)", Lower = -1.8, Upper = -1.3, DividerId = 1, RussianParameterName="Напряжение отсечки, В", ParameterNameStat="U<sub>GS(off)</sub> (напряжение отсечки при Idss/1000)"});
            row7.parameters.Add(new KurbatovParameter{ParameterName = "gmax", Lower = 0.32, Divider = 0.075, DividerId = 3, RussianParameterName="Максимум крутизны, См/мм", ParameterNameStat="g<sub>max</sub> (максимум крутизны)"});
            row7.parameters.Add(new KurbatovParameter{ParameterName = "Igss(-3V)", Lower = -2E-5, Divider = 0.075, DividerId = 3, RussianParameterName="Ток утечки затвора, А/мм", ParameterNameStat="I<sub>GSS(-3V)</sub> (ток утечки затвора при Uзи=-3В)"});
            row7.parameters.Add(new KurbatovParameter{ParameterName = "Idss(3V)", Lower = 0.32, Upper = 0.56, Divider = 0.075, DividerId = 3, RussianParameterName="Начальный ток стока, A/мм", ParameterNameStat="I<sub>DSS(3V)</sub> (начальный ток стока)"});
            row7.parameters.Add(new KurbatovParameter{ParameterName = "Ubr", Lower = 11.0, DividerId = 1, RussianParameterName="Напряжение пробоя сток-исток, В", ParameterNameStat="U<sub>BR</sub> (напряжение пробоя сток-исток)"});
            row7.parameters.Add(new KurbatovParameter{ParameterName = "S21(5GHz)", Lower = 8, DividerId = 1, RussianParameterName="Коэффициент передачи, дБ", ParameterNameStat="S21<sub>(5GHz)</sub>(коэффициент передачи)"});
            
            var row8 = new KurbatovXLSViewModel();
            row8.ElementName = "TC6"; 
            row8.OperationNumber = "560.00.00";
            row8.StageName = "Снятие рабочей пластины с пластины-носителя";
            row8.parameters.Add(new KurbatovParameter{ParameterName = "S21(on)", Lower = -1.5, DividerId = 1, RussianParameterName="Коэффициент передачи в открытом состоянии на частоте 20 ГГц, дБ", ParameterNameStat="S<sub>21ON(20GHz)</sub>"});
            row8.parameters.Add(new KurbatovParameter{ParameterName = "S21(off)", Upper = -15, DividerId = 1, RussianParameterName="Коэффициент передачи в закрытом состоянии на частоте 1 ГГц, дБ", ParameterNameStat="S<sub>21OFF(1GHz)</sub>"});
            row8.parameters.Add(new KurbatovParameter{ParameterName = "rDS(on)", Lower = 1.2, Upper = 2.7, Divider=0.6, DividerId = 10, RussianParameterName="Сопротивление открытого канала, Ом*мм", ParameterNameStat="R<sub>ds(on)</sub> (сопротивление открытого канала)"});
            row8.parameters.Add(new KurbatovParameter{ParameterName = "Idss(3V)", Lower = 0.32, Upper = 0.56, Divider=0.6, DividerId = 10, RussianParameterName="Начальный ток стока, A/мм", ParameterNameStat="I<sub>DSS(3V)</sub> (начальный ток стока)"});
            row8.parameters.Add(new KurbatovParameter{ParameterName = "Ugs(off)", Lower = -1.8, Upper = -1.3, DividerId = 1, RussianParameterName="Напряжение отсечки, В", ParameterNameStat="U<sub>GS(off)</sub> (напряжение отсечки при Idss/1000)"});
            
            
            var row9 = new KurbatovXLSViewModel();
            row9.ElementName = "TC8";
            row9.OperationNumber = "560.00.00";
            row9.StageName = "Снятие рабочей пластины с пластины-носителя";
            row9.parameters.Add(new KurbatovParameter{ParameterName = "Risol", Lower = 0.15, DividerId = 1, RussianParameterName="Сопротивление межприборной изоляции, ГОм", ParameterNameStat="Risol"});
            
            var row10 = new KurbatovXLSViewModel();
            row10.ElementName = "TC10";
            row10.OperationNumber = "560.00.00";
            row10.StageName = "Снятие рабочей пластины с пластины-носителя";
            row10.parameters.Add(new KurbatovParameter{ParameterName = "Rgate", Upper = 27, DividerId = 1, RussianParameterName="Сопротивление затвора, Ом", ParameterNameStat="Rgate"});

            var row11 = new KurbatovXLSViewModel();
            row11.ElementName = "TC12";
            row11.OperationNumber = "560.00.00";
            row11.StageName = "Снятие рабочей пластины с пластины-носителя";
            row11.parameters.Add(new KurbatovParameter{ParameterName = "Rc", Upper = 0.35, DividerId = 1, RussianParameterName="Контактное сопротивление омических контактов, Ом*мм", ParameterNameStat="Rc"});
            row11.parameters.Add(new KurbatovParameter{ParameterName = "Rs", Lower = 140, Upper = 180, DividerId = 1, RussianParameterName="Слоевое cопротивление, Ом/кв", ParameterNameStat="Rs"});
                        
            var row12 = new KurbatovXLSViewModel();
            row12.ElementName = "TC13";
            row12.OperationNumber = "560.00.00";
            row12.StageName = "Снятие рабочей пластины с пластины-носителя";
            row12.parameters.Add(new KurbatovParameter{ParameterName = "Rtfr_1", Lower = 42, Upper = 58, DividerId = 1, RussianParameterName="Удельное поверхностное сопротивление тонкопленочного резистора, Ом/кв", ParameterNameStat="TFR1_1sq"});

            var row13 = new KurbatovXLSViewModel();
            row13.ElementName = "TC14";
            row13.OperationNumber = "560.00.00";
            row13.StageName = "Снятие рабочей пластины с пластины-носителя";
            row13.parameters.Add(new KurbatovParameter{ParameterName = "Rtfr_2", Lower = 485, Upper = 715, DividerId = 1, RussianParameterName="Удельное поверхностное сопротивление тонкопленочного резистора, Ом/кв", ParameterNameStat="TFR2_1sq"});

            var row14 = new KurbatovXLSViewModel();
            row14.ElementName = "TC19";
            row14.OperationNumber = "560.00.00";
            row14.StageName = "Снятие рабочей пластины с пластины-носителя";
            row14.parameters.Add(new KurbatovParameter{ParameterName = "Rm2", Upper = 7, DividerId = 1, RussianParameterName="Сопротивление элемента 'змейка', Ом", ParameterNameStat="Rline"});

            var row15 = new KurbatovXLSViewModel();
            row15.ElementName = "TC20";
            row15.OperationNumber = "560.00.00";
            row15.StageName = "Снятие рабочей пластины с пластины-носителя";
            row15.parameters.Add(new KurbatovParameter{ParameterName = "Rhole", Upper = 2.7, DividerId = 1, RussianParameterName="Сопротивление металлизированного отверстия, Ом", ParameterNameStat="Rhole"});

            var row16 = new KurbatovXLSViewModel();
            row16.ElementName = "TC21";
            row16.OperationNumber = "560.00.00";
            row16.StageName = "Снятие рабочей пластины с пластины-носителя";
            row16.parameters.Add(new KurbatovParameter{ParameterName = "Rind", Upper = 3.6, DividerId = 1, RussianParameterName="Сопротивление катушки индуктивности, Ом", ParameterNameStat="Rind"});
            
            var row17 = new KurbatovXLSViewModel();
            row17.ElementName = "TC23";
            row17.OperationNumber = "560.00.00";
            row17.StageName = "Снятие рабочей пластины с пластины-носителя";
            row17.parameters.Add(new KurbatovParameter{ParameterName = "Сmim", Lower = 275, Upper = 345, DividerId = 1, RussianParameterName="Удельная емкость МДМ-конденсатора, пФ/мм2", ParameterNameStat="C<sub>MIM</sub> при U=0.06B (удельная ёмкость МДМ-конденсатора)"});
            row17.parameters.Add(new KurbatovParameter{ParameterName = "Ubrc", Lower = 20, DividerId = 1, RussianParameterName="Пробивное напряжение МДМ-конденсатора, В", ParameterNameStat="U<sub>BRC</sub> (пробивное напряжение МДМ-конденсатора)"});
          
            xlsList.Add(row1);
            xlsList.Add(row2);
            xlsList.Add(row3);
            xlsList.Add(row4);
            xlsList.Add(row5);
            xlsList.Add(row6);
            xlsList.Add(row7);
            xlsList.Add(row8);
            xlsList.Add(row9);
            xlsList.Add(row10);
            xlsList.Add(row11);
            xlsList.Add(row12);
            xlsList.Add(row13);
            xlsList.Add(row14);
            xlsList.Add(row15);
            xlsList.Add(row16);
            xlsList.Add(row17);

            return Ok(xlsList);
        }

        [HttpGet]
        [Route(("pattern/ckba"))]
        public IActionResult GetPatternCkba()
        {
            var xlsList = new List<KurbatovXLSViewModel>();
                  
            var row1 = new KurbatovXLSViewModel();
            row1.ElementName = "Risol_TC8";
            row1.OperationNumber = "560.00.00";
            row1.StageName = "Снятие рабочей пластины с пластины-носителя";
            row1.parameters.Add(new KurbatovParameter{ParameterName = "Risol", Lower = 0.1, DividerId = 1, RussianParameterName="Сопротивление межприборной изоляции, ГОм", ParameterNameStat="Risol"});

            var row2 = new KurbatovXLSViewModel();
            row2.ElementName = "Rhole_TC20";
            row2.OperationNumber = "560.00.00";
            row2.StageName = "Снятие рабочей пластины с пластины-носителя";
            row2.parameters.Add(new KurbatovParameter{ParameterName = "Rhole", Upper = 3, DividerId = 1, RussianParameterName="Сопротивление металлизированного отверстия, Ом", ParameterNameStat="Rhole"});
            
            var row3 = new KurbatovXLSViewModel();
            row3.ElementName = "Rind_TC21"; 
            row3.OperationNumber = "560.00.00";
            row3.StageName = "Снятие рабочей пластины с пластины-носителя";
            row3.parameters.Add(new KurbatovParameter{ParameterName = "Rind", Upper = 3.6, DividerId = 1, RussianParameterName="Сопротивление катушки индуктивности, Ом", ParameterNameStat="Rind"});
            
            var row4 = new KurbatovXLSViewModel();
            row4.ElementName = "CAP_TC23";
            row4.OperationNumber = "560.00.00";
            row4.StageName = "Снятие рабочей пластины с пластины-носителя";
            row4.parameters.Add(new KurbatovParameter{ParameterName = "Сmim", Lower = 270, Upper = 350, DividerId = 1, RussianParameterName="Удельная емкость МДМ-конденсатора, пФ/мм2", ParameterNameStat="C<sub>MIM</sub> при U=0.06B (удельная ёмкость МДМ-конденсатора)"});
            row4.parameters.Add(new KurbatovParameter{ParameterName = "Ubrc", Lower = 20, DividerId = 1, RussianParameterName="Пробивное напряжение МДМ-конденсатора, В", ParameterNameStat="U<sub>BRC</sub> (пробивное напряжение МДМ-конденсатора)"});
          
            xlsList.Add(row1);
            xlsList.Add(row2);
            xlsList.Add(row3);
            xlsList.Add(row4);
       
            return Ok(xlsList);
        }
       
        private ExcelPackage createExcelPackage(List<KurbatovXLS> xlsList, string waferId, string mslNumber, string date)
        {       
         
            var package = new ExcelPackage();
            package.Workbook.Properties.Title = "Report";
            package.Workbook.Properties.Author = "Roma Molchanov";
            var commonWorksheet = package.Workbook.Worksheets.Add("Сводная");
            commonWorksheet.Cells[1, 1].Value = "№ тестовой структуры";
            commonWorksheet.Cells[1, 2].Value = "Порядковый номер и наименование параметра, единица измерения";
            commonWorksheet.Cells[1, 3].Value = "Буквенное обозначение параметра";
            commonWorksheet.Cells[1, 4].Value = "Результат измерений";
            commonWorksheet.Cells[1, 5].Value = "Процент годных ТС";
            commonWorksheet.Cells[1, 6].Value = $"Приложение 1 к МСЛ №{mslNumber}";
            commonWorksheet.Cells[1, 6, 1, 8].Merge = true;
            commonWorksheet.Cells[2, 6].Value = $"Пластина {waferId}";
            commonWorksheet.Cells[2, 6, 2, 8].Merge = true;
            commonWorksheet.Cells[3, 6].Value = $"Дата: {date}";
            commonWorksheet.Cells[3, 6, 3, 8].Merge = true;
            commonWorksheet.Row(1).Height = 20;
            for (int i = 1; i < 6; i++)
            {
                commonWorksheet.Column(i).Width = 20;
                commonWorksheet.Column(i).Style.WrapText = true;
                commonWorksheet.Column(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                commonWorksheet.Column(i).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }
            commonWorksheet.Column(2).Width = 30;
           
            var currentCursor = 2;

            foreach (var kurbatovXLS in xlsList.Select((value, i) => new { i, value }))
            {             
                  
                var worksheet = package.Workbook.Worksheets.Add(kurbatovXLS.value.OperationNumber + "_" + kurbatovXLS.value.ElementName);
               
           
                worksheet.Cells[6, 1, kurbatovXLS.value.kpList[0].advList.Count + 9, 2 + kurbatovXLS.value.kpList.Count].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[6, 1, kurbatovXLS.value.kpList[0].advList.Count + 9, 2 + kurbatovXLS.value.kpList.Count].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[6, 1, kurbatovXLS.value.kpList[0].advList.Count + 9, 2 + kurbatovXLS.value.kpList.Count].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[6, 1, kurbatovXLS.value.kpList[0].advList.Count + 9, 2 + kurbatovXLS.value.kpList.Count].Style.Border.Left.Style = ExcelBorderStyle.Thin;
             
                worksheet.Cells["A1:C5"].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                worksheet.Cells["A1:C5"].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                worksheet.Cells["A1:C5"].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                worksheet.Cells["A1:C5"].Style.Border.Left.Style = ExcelBorderStyle.Thick;

                worksheet.Cells[1, 1].Value = "Номер операции:";
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 3].Value = kurbatovXLS.value.OperationNumber;
                worksheet.Cells[1, 1, 1, 2].Merge = true;

                worksheet.Cells[1, 4].Value = $"Приложение {kurbatovXLS.i + 2} к МСЛ №{mslNumber}";
                worksheet.Cells[1, 4, 1, 6].Merge = true;
                worksheet.Cells[2, 4].Value = $"Пластина {waferId}";
                worksheet.Cells[2, 4, 2, 6].Merge = true;
                worksheet.Cells[3, 4].Value = $"Дата: {date}";
                worksheet.Cells[3, 4, 3, 6].Merge = true;
                worksheet.Column(4).Width = 20;
                worksheet.Column(5).Width = 20;
                worksheet.Column(4).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Column(4).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
             
                worksheet.Cells[2, 1].Value = "Элемент:";
                worksheet.Cells[2, 1].Style.Font.Bold = true;
                worksheet.Cells[2, 3].Value = kurbatovXLS.value.ElementName;
                worksheet.Cells[2, 1, 2, 2].Merge = true;
                
                worksheet.Cells[3, 1].Value = "Количество кристаллов:";
                worksheet.Cells[3, 1].Style.Font.Bold = true;
                worksheet.Cells[3, 3].Value = kurbatovXLS.value.DieQuantity;
                worksheet.Cells[3, 1, 3, 2].Merge = true;
                
                worksheet.Cells[4, 1].Value = "Количество годных:";
                worksheet.Cells[4, 1].Style.Font.Bold = true;
                worksheet.Cells[4, 3].Value =  kurbatovXLS.value.DieQuantity - kurbatovXLS.value.DirtyCodesList.Count;
                worksheet.Cells[4, 1, 4, 2].Merge = true;
            
                worksheet.Cells[5, 1].Value = "Процент годных:";
                worksheet.Cells[5, 1].Style.Font.Bold = true;
                worksheet.Cells[5, 3].Value =  100 - kurbatovXLS.value.DirtyPercentage + "%";
                worksheet.Cells[5, 1, 5, 2].Merge = true;
            
                worksheet.Column(1).Width = 13;
                worksheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Column(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                
                worksheet.Column(2).Width = 20;

                worksheet.Cells[6, 2].Value = "Контролируемый параметр, ед. изм.";
                worksheet.Cells[6, 2].Style.Font.Bold = true;
                worksheet.Cells[6, 2, 6, 1 + kurbatovXLS.value.kpList.Count].Merge = true;
             
                List<AtomicDieValue> list = kurbatovXLS.value.kpList.FirstOrDefault()?.advList;
                worksheet.Cells[9, 1].Value = "№ кристалла";
                worksheet.Cells[9, 1].Style.Font.Bold = true;

                worksheet.Cells[9, 2 + kurbatovXLS.value.kpList.Count].Value = "Соответствие";
                worksheet.Cells[9, 2 + kurbatovXLS.value.kpList.Count].Style.Font.Bold = true;
                worksheet.Column(2 + kurbatovXLS.value.kpList.Count).Width = 13;
                worksheet.Column(2 + kurbatovXLS.value.kpList.Count).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Column(2 + kurbatovXLS.value.kpList.Count).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                for (int i = 0; i < list.Count; i++)
                {
                    AtomicDieValue item = (AtomicDieValue)list[i];
                    worksheet.Cells[10 + i, 1].Value = item.DieCode;
                
                    if(kurbatovXLS.value.DirtyCodesList.Contains(item.DieCode))
                    {
                        worksheet.Cells[10 + i, 2 + kurbatovXLS.value.kpList.Count].Value = "Брак";
                    }
                    else
                    {
                        worksheet.Cells[10 + i, 2 + kurbatovXLS.value.kpList.Count].Value = "Годен";
                    }
                
                }
                for (int i = 0; i < kurbatovXLS.value.kpList.Count; i++)
                {
                    if(kurbatovXLS.value.IsAddedToCommonWorksheet)
                    {
                        if(i == 0)
                        {
                            commonWorksheet.Cells[currentCursor, 1].Value = kurbatovXLS.value.ElementName;
                            commonWorksheet.Cells[currentCursor, 5].Value = 100.0 - kurbatovXLS.value.DirtyPercentage;
                            commonWorksheet.Cells[currentCursor, 1, currentCursor + kurbatovXLS.value.kpList.Count - 1, 1].Merge = true;
                            commonWorksheet.Cells[currentCursor, 5, currentCursor + kurbatovXLS.value.kpList.Count - 1, 5].Merge = true;
                        }                   
                        commonWorksheet.Cells[currentCursor, 2].Value = (i+1) + " " + kurbatovXLS.value.kpList[i].RussianParameterName;
                        commonWorksheet.Cells[currentCursor, 3].Value = kurbatovXLS.value.kpList[i].ParameterName;
                        var averageGood = kurbatovXLS.value.kpList[i].AverageGood;
                        if ((Math.Abs(averageGood) >= 10000 || Math.Abs(averageGood) < 1E-2))
                        {
                            commonWorksheet.Cells[currentCursor, 4].Value = averageGood.ToString("0.00E0");
                        }
                        else
                        {
                            commonWorksheet.Cells[currentCursor, 4].Value = averageGood.ToString("0.00");
                        }                        
                        currentCursor++;
                    }
                    
                    worksheet.Cells[9, 2 + i].Value = "Измеренное значение";
                    worksheet.Column(2 + i).Width = 20;
                    worksheet.Column(2 + i).Style.WrapText = true;
                    worksheet.Column(2 + i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Column(2 + i).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[9, 2 + i].Style.Font.Bold = true;
                    worksheet.Cells[7, 2 + i].Value = kurbatovXLS.value.kpList[i].RussianParameterName;
                    var bounds = String.Empty;
                    if(!Double.IsNaN(kurbatovXLS.value.kpList[i].Lower))
                        bounds = bounds + "не менее " + kurbatovXLS.value.kpList[i].Lower;
                    if(!Double.IsNaN(kurbatovXLS.value.kpList[i].Upper))
                    {
                        if(!String.IsNullOrWhiteSpace(bounds))
                              bounds = bounds + ";" + Environment.NewLine;
                        bounds = bounds + "не более " + kurbatovXLS.value.kpList[i].Upper;
                    }
                      
                    worksheet.Cells[8, 2 + i].Value = bounds;
                    for (int j = 0; j < kurbatovXLS.value.kpList[i].advList.Count; j++)
                    {
                         var value = kurbatovXLS.value.kpList[i].advList[j].Value;
                         if ((Math.Abs(value) >= 10000 || Math.Abs(value) < 1E-2))
                         {
                            worksheet.Cells[10 + j, 2 + i].Value = value.ToString("0.00E0");
                         }
                         else
                         {
                            worksheet.Cells[10 + j, 2 + i].Value = value.ToString("0.00");
                         }
                      
                        
                    } 
                }
            }

            commonWorksheet.Cells[1, 1, currentCursor - 1, 5].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            commonWorksheet.Cells[1, 1, currentCursor - 1, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            commonWorksheet.Cells[1, 1, currentCursor - 1, 5].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            commonWorksheet.Cells[1, 1, currentCursor - 1, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            
            return package;
        }
    }
}