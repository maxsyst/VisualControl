using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VueExample.Contexts;
using VueExample.Exceptions;
using VueExample.Models.SRV6.Uploader;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.ViewModels;
using VueExample.ViewModels.FileNameUploader;

namespace VueExample.Providers.Srv6
{
    public class FileGraphicUploaderService : IFileGraphicUploaderService
    {
        private readonly Srv6Context _srv6Context;
        public FileGraphicUploaderService(Srv6Context srv6Context)
        {
            _srv6Context = srv6Context;
        }

        public async Task<GraphicName> AddGraphicToFileName(int fileNameId, GraphicNameUploaderViewModel graphicNameUploaderViewModel)
        {
            var graphicName = await _srv6Context.GraphicNames.Join(_srv6Context.FileNameGraphics.Where(x => x.Variant == graphicNameUploaderViewModel.Variant && x.FileNameId == fileNameId),
                                                       c => c.Id,
                                                       p => p.GraphicNameId,
                                                       (c,p) => p.GraphicName).Where(x => x.Name == graphicNameUploaderViewModel.Name).FirstOrDefaultAsync();
            if(graphicName is null)
            {
                var graphic = new GraphicName{Name = graphicNameUploaderViewModel.Name};
                _srv6Context.GraphicNames.Add(graphic);
                await _srv6Context.SaveChangesAsync();
                _srv6Context.FileNameGraphics.Add(new FileNameGraphic{FileNameId = fileNameId,
                                                            GraphicNameId = graphic.Id,
                                                            Variant = graphicNameUploaderViewModel.Variant});
                await _srv6Context.SaveChangesAsync();
                return graphic;
            }
            else
            {
                    throw new DuplicateException("Такой график уже существует");
            }
        }

        public async Task CopyFileNamesToAnotherProcess(int sourceProcessId, int destProcessId)
        {
            var fileNamesList = await GetAllFileNamesByProcessId(sourceProcessId);
            foreach (var fileName in fileNamesList)
            {
                var graphicNamesList = await GetGraphicsByFileName(fileName.Id);
                var newFileName = await CreateFileName(new FileNameUploaderViewModel{Name = fileName.Name, ProcessId = destProcessId, GraphicNames = graphicNamesList.ToList()});
            }
        }

        public async Task<FileName> CreateFileName(FileNameUploaderViewModel fileNameViewModel)
        {
            if(_srv6Context.FileNames.Count(x => x.ProcessId == fileNameViewModel.ProcessId && x.Name == fileNameViewModel.Name) > 0)
            {
                throw new DuplicateException("Уже существует файл с таким именем");
            }
            var fileName = new FileName{Id = fileNameViewModel.Id, Name = fileNameViewModel.Name, ProcessId = fileNameViewModel.ProcessId};
            _srv6Context.FileNames.Add(fileName);
            var graphicsList = fileNameViewModel.GraphicNames.Select(x => new GraphicName {Name = x.Name}).ToList();
            _srv6Context.GraphicNames.AddRange(graphicsList);
            await _srv6Context.SaveChangesAsync();
            var fileNameGraphics = graphicsList.Select(x => new FileNameGraphic{GraphicNameId = x.Id, FileNameId = fileName.Id, Variant = fileNameViewModel.GraphicNames.FirstOrDefault(g => g.Name == x.Name).Variant});
            _srv6Context.FileNameGraphics.AddRange(fileNameGraphics);
            await _srv6Context.SaveChangesAsync();
            return fileName;
        }

        public async Task DeleteFileName(int fileNameId, int processId)
        {
            FileName fileName = new FileName() { Id = fileNameId, ProcessId = processId };
            _srv6Context.Entry(fileName).State = EntityState.Deleted;
            await _srv6Context.SaveChangesAsync();
        }

        public async Task DeleteGraphicFromFileName(int fileNameId, GraphicNameUploaderViewModel graphicNameUploaderViewModel)
        {
            var fileNameGraphic = await _srv6Context.FileNameGraphics.FirstOrDefaultAsync(x => x.Variant == graphicNameUploaderViewModel.Variant
                                                                && x.GraphicNameId == graphicNameUploaderViewModel.Id
                                                                && x.FileNameId == fileNameId);
            _srv6Context.FileNameGraphics.Remove(fileNameGraphic);
            await _srv6Context.SaveChangesAsync();
        }

        public async Task<IList<FileName>> GetAllFileNamesByProcessId(int processId) => await _srv6Context.FileNames.Where(x => x.ProcessId == processId).ToListAsync();

        public async Task<IList<GraphicNameUploaderViewModel>> GetGraphicsByFileName(int fileNameId)
        {
            var graphicNameUploaderViewModelList =  await _srv6Context.FileNames
                                                    .Where(x => x.Id == fileNameId)
                                                    .Join(_srv6Context.FileNameGraphics,
                                                        c => c.Id,
                                                        p => p.FileNameId,
                                                        (c,p) => new GraphicNameUploaderViewModel{Id = p.GraphicNameId, Name = p.GraphicName.Name, Variant = p.Variant})
                                                    .ToListAsync();
            return graphicNameUploaderViewModelList;
        }
    }
}