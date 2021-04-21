using System.Linq;
using VueExample.Providers.Srv6.Interfaces;
using System.Collections.Generic;
using System.IO;
using VueExample.ViewModels;
using System.Threading.Tasks;
using System;
using VueExample.Models.SRV6.Uploader;

namespace VueExample.Providers.Srv6
{
    public class FolderService : IFolderService
    {
        private readonly IElementService _elementService;
        private readonly IFileGraphicUploaderService _fileGraphicUploaderService;
        private readonly IProcessProvider _processProvider;
        private readonly ICodeProductProvider _codeProductProvider;
        public FolderService(IElementService elementService, ICodeProductProvider codeProductProvider, IProcessProvider processProvider, IFileGraphicUploaderService fileGraphicUploaderService)
        {
            _elementService = elementService;
            _processProvider = processProvider;
            _codeProductProvider = codeProductProvider;
            _fileGraphicUploaderService = fileGraphicUploaderService;
        }
        public List<string> GetAllCodeProductInUploaderDirectory(string directoryPath)
        {
            directoryPath = GetTruePath(directoryPath);
            var directoriesNameList = new List<string>();
            var directoriesArray = System.IO.Directory.GetDirectories(directoryPath);
            foreach (var directory in directoriesArray)
            {
                var dir = new DirectoryInfo(directory);
                var dirName = dir.Name;
                directoriesNameList.Add(dirName);
            }           
            return directoriesNameList;
        }

        public List<string> GetAllWaferInCodeProductFolder(string directoryPath, string codeProductFolderName)
        {
            directoryPath = GetTruePath(directoryPath);
            var directoriesNameList = new List<string>();
            var directoriesArray = System.IO.Directory.GetDirectories($"{directoryPath}\\{codeProductFolderName}\\meas");
            foreach (var directory in directoriesArray)
            {
                var dir = new DirectoryInfo(directory);
                var dirName = dir.Name;
                directoriesNameList.Add(dirName);
            }           
            return directoriesNameList;
        }

        public List<string> GetAllMeasurementRecordingFolder(string directoryPath, string codeProductFolderName, string waferFolderName)
        {
            directoryPath = GetTruePath(directoryPath);
            var directoriesNameList = new List<string>();
            var directoriesArray = System.IO.Directory.GetDirectories($"{directoryPath}\\{codeProductFolderName}\\meas\\{waferFolderName}");
            foreach (var directory in directoriesArray)
            {
                var dir = new DirectoryInfo(directory);
                var dirName = dir.Name;
                directoriesNameList.Add(dirName);
            }           
            return directoriesNameList;
        }

        public Dictionary<string, UploadingFileData> GetDataFromLNRFile(string path) 
        {
            path = GetTruePath(path);
            var dataDictionary = new Dictionary<string, UploadingFileData>();
            var parseString = String.Join<string>("$", File.ReadAllLines(path).ToList());
            var codeDataList = parseString.Split(new[] { "Count," }, StringSplitOptions.None).ToList();
            codeDataList.RemoveAt(0);
            foreach (var codeData in codeDataList)
            {
                var uploaderFileData = new UploadingFileData();
                var dataList = codeData.Split('$').ToList();
                dataDictionary.Add(dataList[0], uploaderFileData);
                var graphicNamesList = dataList[1].Split(',');
                for (int i = 1; i < graphicNamesList.Length; i++)
                {
                    uploaderFileData.ValueLists.Add(graphicNamesList[i], new List<string>());
                }
                for (int i = 2; i < dataList.Count; i++)
                {
                    if(dataList[i] != String.Empty && dataList[i].FirstOrDefault() != 'S')
                    {
                        var dataArray = dataList[i].Split(',');
                        uploaderFileData.AbscissList.Add(dataArray[0]);
                        for (int j = 1; j < graphicNamesList.Length; j++)
                        {
                            uploaderFileData.ValueLists[graphicNamesList[j]].Add(dataArray[j]);
                        }
                    }
                  
                }
            }
            return dataDictionary;

        }

        public Dictionary<string, UploadingFileData> GetDataFromHSTGFile(string path) 
        {
            path = GetTruePath(path);
            var dataDictionary = new Dictionary<string, UploadingFileData>();
            var rawFileData = File.ReadAllLines(path).ToList();
            rawFileData.RemoveAll(x => x == ",");
            rawFileData.RemoveAll(x => x.Contains("Setup"));
            var parseString = String.Join<string>("$", rawFileData) + "$";
            var codeDataList = parseString.Split(new[] { "Count," }, StringSplitOptions.None).ToList();
            codeDataList.RemoveAt(0);
            foreach (var codeData in codeDataList)
            {
                var uploaderFileData = new UploadingFileData();
                var dataList = codeData.Split('$').ToList();
                dataDictionary.Add(dataList[0], uploaderFileData);
                var graphicNamesList = dataList[1].Split(',');
                var dataArray = dataList[2].Split(',');
                for (int i = 0; i < graphicNamesList.Length; i++)
                {
                    uploaderFileData.ValueLists.Add(graphicNamesList[i], new List<string>{dataArray[i]});                  
                }      
            }
            return dataDictionary;
        }

        public async Task<List<SimpleOperationUploaderViewModel>> GetSimpleOperations(string directoryPath, string codeProductName, string waferName, int dieTypeId, List<string> measurementRecordings)
        {
            var simpleOperationList = new List<SimpleOperationUploaderViewModel>(); 
            directoryPath = GetTruePath(directoryPath);
            var fileNames = await _fileGraphicUploaderService.GetAllFileNamesByProcessId((await _processProvider.GetProcessByCodeProductId((await _codeProductProvider.GetByWaferId(waferName)).IdCp)).ProcessId);
            foreach (var meas in measurementRecordings)
            {
                var directoriesArray = System.IO.Directory.GetDirectories($"{directoryPath}\\{codeProductName}\\meas\\{waferName}\\{meas}");
                foreach (var directory in directoriesArray)
                {
                    var dirElementName = new DirectoryInfo(directory).Name;
                    var simpleOperationArray = Directory.EnumerateFiles($"{directoryPath}\\{codeProductName}\\meas\\{waferName}\\{meas}\\{dirElementName}", "*.*", SearchOption.TopDirectoryOnly)
                                                        .Where(s => s.EndsWith(".csv") || s.EndsWith(".s2p"));                    
                    foreach (var simpleOperationFileName in simpleOperationArray)
                    {
                        var simpleOperation = new SimpleOperationUploaderViewModel{Guid = Guid.NewGuid().ToString()};
                        simpleOperation.Name = $"{meas}";
                        simpleOperation.Stage = new StageViewModel();
                        simpleOperation.Path = simpleOperationFileName;
                        var element = (await _elementService.GetByDieType(dieTypeId)).FirstOrDefault(x => x.Name == dirElementName);
                        simpleOperation.Element = new ElementUploading{Name = dirElementName, ElementId = element?.ElementId, Comment = element?.Comment};
                        simpleOperation.FileName = new FileNameUploaderUViewModel{Name = Path.GetFileName(simpleOperationFileName)};
                        var fileNameWithoutExpansion = simpleOperation.FileName.Name.Contains('.') 
                                                       ? simpleOperation.FileName.Name.Substring(0, simpleOperation.FileName.Name.Length - simpleOperation.FileName.Name.Split('.').Last().Length - 1) 
                                                       : String.Empty;
                        var fileName = fileNames.FirstOrDefault(f => f.Name == fileNameWithoutExpansion);
                        if(fileName != null)
                        {
                            simpleOperation.FileName.Id = fileName.Id;
                            simpleOperation.FileName.ProcessId = fileName.ProcessId;
                            var graphicNamesList = (await _fileGraphicUploaderService.GetGraphicsByFileName(fileName.Id)).ToList();
                            var graphicNamesDict = new Dictionary<int, string>();
                            foreach (var graphic in graphicNamesList)
                            {
                                if(graphicNamesDict.ContainsKey(graphic.Variant))
                                {
                                    graphicNamesDict[graphic.Variant] = graphicNamesDict[graphic.Variant] + ";" + graphic.Name;
                                }
                                else
                                {
                                    graphicNamesDict.Add(graphic.Variant, graphic.Name);
                                }
                            }
                            simpleOperation.FileName.GraphicNames = graphicNamesDict.Values.ToList();
                            simpleOperation.FileName.SelectedGraphicNames = simpleOperation.FileName.GraphicNames.FirstOrDefault();
                        }                        
                        simpleOperationList.Add(simpleOperation);
                    }
                    
                }           
            }
            return simpleOperationList;
        }

        private string GetTruePath(string path)
        {
            const string _path = @"\\Srv1\\Common\";
            path = path.Replace(@"T:\\", _path);
            return path;
        }
        
    }
}