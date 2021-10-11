using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VueExample.Contexts;
using VueExample.Exceptions;
using VueExample.Models;
using VueExample.Models.SRV6;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.ViewModels;

namespace VueExample.Providers.Srv6
{
    public class DieTypeProvider : IDieTypeProvider
    {
        private readonly Srv6Context _srv6Context;
        private readonly IMapper _mapper;
        public DieTypeProvider(IMapper mapper, Srv6Context srv6Context)
        {
            _srv6Context = srv6Context;
            _mapper = mapper;
        }        
        public async Task<DieType> Create(DieTypeUpdatingViewModel dieTypeViewModel)
        {            
            var dieType = new DieType{Name = dieTypeViewModel.Name};
            _srv6Context.DieTypes.Add(dieType);
            await _srv6Context.SaveChangesAsync();
            var elementsList = new List<Element>();
            foreach (var element in dieTypeViewModel.ElementsList)
            {
                var createdElement = new Element{Name = element.Name, Comment = element.Comment, TypeId = element.TypeId};
                _srv6Context.Elements.Add(createdElement);
                elementsList.Add(createdElement);
                    
            }
            await _srv6Context.SaveChangesAsync();

            foreach (var element in elementsList)
            {
                    _srv6Context.DieTypeElements.Add(new Entities.DieTypeElement{ElementId = element.ElementId, DieTypeId = dieType.DieTypeId});
            }
            await _srv6Context.SaveChangesAsync();
            foreach (var idcp in dieTypeViewModel.CodeProductIdsList)
            {
                _srv6Context.DieTypeCodeProducts.Add(new DieTypeCodeProduct{DieTypeId = dieType.DieTypeId, CodeProductId = idcp});
            }
            await _srv6Context.SaveChangesAsync();
            return dieType;
        }

        public async Task<DieType> Update(DieTypeViewModel dieTypeViewModel)
        {
            if(String.IsNullOrEmpty(dieTypeViewModel.Name))
                throw new ValidationErrorException();
            
            var dieType = await _srv6Context.DieTypes.FirstOrDefaultAsync(x => x.DieTypeId == dieTypeViewModel.Id) ?? throw new RecordNotFoundException();
             _srv6Context.Entry(dieType).CurrentValues.SetValues(_mapper.Map<DieTypeViewModel, DieType>(dieTypeViewModel));
            await _srv6Context.SaveChangesAsync();
            return dieType;
        }

        public async Task<List<DieType>> GetAll() => await _srv6Context.DieTypes.ToListAsync();

        public async Task<DieTypeUpdatingViewModel> GetCodeProductsAndElements(int id)
        {
            var elementsList =      await _srv6Context.Elements
                                    .Join(_srv6Context.DieTypeElements
                                    .Where(x => x.DieTypeId == id), c => c.ElementId, p => p.ElementId, (c,p) => p.Element)
                                    .AsNoTracking()
                                    .ToListAsync();

            var codeProductIdsList = await _srv6Context.CodeProducts
                                    .Join(_srv6Context.DieTypeCodeProducts
                                    .Where(x => x.DieTypeId == id), c => c.IdCp, p => p.Id, (c,p) => p.CodeProduct)
                                    .AsNoTracking().Select(x => x.IdCp)
                                    .ToListAsync();

            var dieType =           await _srv6Context.DieTypes
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(x => x.DieTypeId == id);
                
            var elementsViewModelList = _mapper.Map<List<Element>, List<ElementViewModel>>(elementsList);
            foreach(var element in elementsViewModelList)
            {
                if(await _srv6Context.MeasurementRecordingElements.AnyAsync(x => x.ElementId == element.ElementId))
                {
                    element.IsAvaliableToDelete = false;
                }
            }
            return new DieTypeUpdatingViewModel{Name = dieType.Name, 
                                                CodeProductIdsList = codeProductIdsList.ToList(), 
                                                ElementsList = elementsViewModelList}; 
        }

        public async Task<Tuple<CodeProductViewModel, string>> UpdateCodeProductsMap(int dieTypeId, int codeProductId)
        {
            var action = String.Empty;
            var dieTypeCodeProduct = await _srv6Context.DieTypeCodeProducts.FirstOrDefaultAsync(x => x.DieTypeId == dieTypeId && x.CodeProductId == codeProductId);
            if(dieTypeCodeProduct is null)
            {
                dieTypeCodeProduct = new DieTypeCodeProduct{CodeProductId = codeProductId, DieTypeId = dieTypeId};
                await _srv6Context.AddAsync(dieTypeCodeProduct);
                action = "INSERTED";
            }
            else
            {
                _srv6Context.DieTypeCodeProducts.Remove(dieTypeCodeProduct);
                action = "DELETED";
            }
            await _srv6Context.SaveChangesAsync();
            var codeProduct = await _srv6Context.CodeProducts.FirstOrDefaultAsync(x => x.IdCp == codeProductId);               
            return new Tuple<CodeProductViewModel, string>(_mapper.Map<CodeProduct, CodeProductViewModel>(codeProduct), codeProduct is null ? "ERROR" : action);
        }

        public async Task<List<DieType>> GetByCodeProductId(int codeProductId)
        {
            var dieTypesList =  await _srv6Context.DieTypes
                                .Join(_srv6Context.DieTypeCodeProducts.Where(x => x.CodeProductId == codeProductId),
                                      c => c.DieTypeId, 
                                      p => p.DieTypeId, 
                                      (c,p) => p.DieType)
                                .AsNoTracking()
                                .ToListAsync();
            return dieTypesList;
        }
        
        public async Task<DieType> GetByName(string name)
        {
            var dieType =  await _srv6Context.DieTypes.FirstOrDefaultAsync( x => x.Name == name);
            return dieType is null ? new DieType() : dieType;
        }

        public async Task<List<DieType>> GetByWaferId(string waferId)
        {
            var codeProductId = (Int32)(await _srv6Context.Wafers.Where(x => x.WaferId == waferId).FirstOrDefaultAsync()).CodeProductId;
            return await this.GetByCodeProductId(codeProductId);
        }
    }
}