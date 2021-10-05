using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VueExample.Providers.Srv6.Interfaces;
using System.Threading.Tasks;
using VueExample.ViewModels;

namespace VueExample.Controllers
{
    [Route("api/[controller]/[action]")]
    public class GraphicSrv6Controller : Controller
    {
        private readonly ISRV6GraphicService _graphicService;
        public GraphicSrv6Controller(ISRV6GraphicService graphicService)
        {
            _graphicService = graphicService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateS2P([Bind("CodeProductId", "GraphicS2PType")] GraphicS2PViewModel s2pGraphic)
        {
            var createdGraphic = await _graphicService.CreateS2P(s2pGraphic.CodeProductId, s2pGraphic.GraphicS2PType);           
            return Created("", createdGraphic);
        }

        [HttpGet]
        [ResponseCache(CacheProfileName = "Default60")]
        public async Task<IActionResult> GetGraphicNameByKeyGraphicState(string keyGraphicState)
        {
            return Ok((await _graphicService.GetGraphicByKeyGraphicState(keyGraphicState)).Name);
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailiableGraphicsByKeyGraphicStateList(string keyGraphicStateJSON)
        {
            var keyGraphicStateList = JsonConvert.DeserializeObject<List<string>>(keyGraphicStateJSON);
            var availiableGraphicList = new List<ViewModels.GraphicWithKeyGraphicStateViewModel>();
            foreach (var kgs in keyGraphicStateList)
            {
                var graphicWithKeyGraphicStateViewModel = new ViewModels.GraphicWithKeyGraphicStateViewModel();
                var graphicId = Convert.ToInt32(kgs.Split('_').FirstOrDefault());
                graphicWithKeyGraphicStateViewModel.GraphicName = (await _graphicService.GetById(graphicId)).Name;
                graphicWithKeyGraphicStateViewModel.KeyGraphicState = kgs;
                availiableGraphicList.Add(graphicWithKeyGraphicStateViewModel);
            }

           return Ok(availiableGraphicList);
            
        }
    }
}