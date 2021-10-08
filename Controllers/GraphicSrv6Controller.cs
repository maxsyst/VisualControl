using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VueExample.Providers.Srv6.Interfaces;
using System.Threading.Tasks;
using VueExample.ViewModels;
using VueExample.Enums;

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
        public async Task<IActionResult> GetGraphicNameByKeyGraphicState(string keyGraphicState) => 

            Ok((await _graphicService.GetGraphicByKeyGraphicState(keyGraphicState)).Name);

        [HttpGet]
        public async Task<IActionResult> GetAvailiableGraphicsByKeyGraphicStateList(string keyGraphicStateJSON)
        {
            var keyGraphicStateList = JsonConvert.DeserializeObject<List<string>>(keyGraphicStateJSON);
            var availiableGraphicList = new List<GraphicWithKeyGraphicStateViewModel>();
            var graphicIdHashSet = new HashSet<int>(keyGraphicStateList.Select(x => Convert.ToInt32(x.Split('_').First())));
            foreach (var graphicId in graphicIdHashSet.OrderBy(x => x))
            {
                var graphicWithKeyGraphicStateViewModel = new GraphicWithKeyGraphicStateViewModel();
                var graphic = await _graphicService.GetById(graphicId);
                graphicWithKeyGraphicStateViewModel.GraphicName = graphic.Name;
                graphicWithKeyGraphicStateViewModel.KeyGraphicState = $"{graphicId}_{Enum.GetName(typeof(GraphicType), graphicId)}";
                availiableGraphicList.Add(graphicWithKeyGraphicStateViewModel);
            }
            return Ok(availiableGraphicList);
        }
    }
}