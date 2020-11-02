using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VueExample.Contexts;
using VueExample.Models.SRV6;
using VueExample.Providers.Srv6.Interfaces;
using System.Collections.Generic;
using VueExample.ViewModels;
using AutoMapper;
using VueExample.Models;

namespace VueExample.Providers.Srv6
{
    public class ElementService : IElementService
    {
        private readonly Srv6Context _srv6Context;
        private readonly IMapper _mapper;
        public ElementService(IMapper mapper, Srv6Context srv6Context)
        {
            _srv6Context = srv6Context;
            _mapper = mapper;
        }

        public async Task AddToDieType(int elementId, int dieTypeId)
        {
            _srv6Context.DieTypeElements.Add(new Entities.DieTypeElement{ElementId = elementId, DieTypeId = dieTypeId});
            await _srv6Context.SaveChangesAsync();
        }

        public async Task Delete(int id) 
        {
            var dieTypeElements = await _srv6Context.DieTypeElements.Where(x => x.ElementId == id).ToListAsync();
            _srv6Context.DieTypeElements.RemoveRange(dieTypeElements);
            await _srv6Context.SaveChangesAsync();
            var element = await _srv6Context.Elements.FirstOrDefaultAsync(x => x.ElementId == id);
            _srv6Context.Elements.Remove(element);
            await _srv6Context.SaveChangesAsync();
        }

        public async Task<Element> Create(ElementViewModel elementViewModel)
        {
            var element = _mapper.Map<ElementViewModel, Element>(elementViewModel);
            await _srv6Context.Elements.AddAsync(element);
            await _srv6Context.SaveChangesAsync();
            return element;
        }
        
        public async Task<Element> Update(ElementViewModel elementViewModel)
        {
            var element = await _srv6Context.Elements.FirstOrDefaultAsync(x => x.ElementId == elementViewModel.ElementId);
            _srv6Context.Entry(element).CurrentValues.SetValues(_mapper.Map<ElementViewModel, Element>(elementViewModel));
            await _srv6Context.SaveChangesAsync();
            return element;
        }

        public async Task<List<Element>> GetByDieType(int dieTypeId)
        {
            var elementList = await _srv6Context.Elements.Join(_srv6Context.DieTypeElements, 
                                                        c => c.ElementId, 
                                                        p => p.ElementId, 
                                                        (c,p) => new {Element = c, DieTypeElement = p})
                                                .Join(_srv6Context.DieTypes, 
                                                        c => c.DieTypeElement.DieTypeId,
                                                        p => p.DieTypeId,
                                                        (c,p) => new {DieType = p, Element = c.Element})
                                                .Where(x => x.DieType.DieTypeId == dieTypeId)
                                                .Select(x => x.Element)     
                                                .ToListAsync();
            return elementList;
        }

        public async Task<Element> GetById(int elementId) => await _srv6Context.Elements.FindAsync(elementId);

        public async Task<List<Element>> GetByIdmr(int idmr) 
        {
            var elementList = await _srv6Context.Elements.Join(_srv6Context.MeasurementRecordingElements, 
                                                        c => c.ElementId, 
                                                        p => p.ElementId, 
                                                        (c,p) => new {Element = c, MeasurementRecordingElement = p})
                                                .Join(_srv6Context.MeasurementRecordings, 
                                                        c => c.MeasurementRecordingElement.MeasurementRecordingId,
                                                        p => p.Id,
                                                        (c,p) => new {MeasurementRecording = p, Element = c.Element})
                                                .Where(x => x.MeasurementRecording.Id == idmr)
                                                .Select(x => x.Element)     
                                                .ToListAsync();
            return elementList;
        }

        public async Task<Element> GetByNameAndWafer(string name, string waferId)
        {
            var elementList = from codeProduct in _srv6Context.CodeProducts
                              join wafer in _srv6Context.Wafers on codeProduct.IdCp equals wafer.CodeProductId
                              join dieTypeCodeProduct in _srv6Context.DieTypeCodeProducts on codeProduct.IdCp equals dieTypeCodeProduct.CodeProductId
                              join dieType in _srv6Context.DieTypes on dieTypeCodeProduct.DieTypeId equals dieType.DieTypeId
                              join dieTypeElementType in _srv6Context.DieTypeElements on dieType.DieTypeId equals dieTypeElementType.DieTypeId
                              join element in _srv6Context.Elements on dieTypeElementType.ElementId equals element.ElementId
                              where wafer.WaferId == waferId && element.Name == name
                              select new Element {Name = element.Name, 
                                                  ElementId = element.ElementId, 
                                                  TypeId = element.TypeId, 
                                                  Comment = element.Comment, 
                                                  PhotoPath = element.PhotoPath, 
                                                  DocName = element.DocName};
            return await elementList.FirstOrDefaultAsync();            
        }

        public async Task<Element> UpdateElementOnIdmr(int measurementRecordingId, int newElementId)
        {
            var measurementRecordingElement = _srv6Context.MeasurementRecordingElements.FirstOrDefault(x => x.MeasurementRecordingId == measurementRecordingId);
            if(measurementRecordingElement is null) 
            {
                _srv6Context.MeasurementRecordingElements.Add(new MeasurementRecordingElement{ElementId = newElementId, MeasurementRecordingId = measurementRecordingId});
            }
            else
            {
                measurementRecordingElement.ElementId = newElementId;
            }
            await _srv6Context.SaveChangesAsync();
            return await _srv6Context.Elements.FirstOrDefaultAsync(x => x.ElementId == newElementId);
        }

        public async Task<Element> GetByDieTypeIdAndName(int dieTypeId, string name)
        {
            var elementList = await GetByDieType(dieTypeId);
            return elementList.Count > 0 ? elementList.FirstOrDefault(x => x.Name == name) : new Element();
        }
    }
}