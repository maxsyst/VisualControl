using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using VueExample.Contexts;
using VueExample.Providers.Srv6.Interfaces;
using MathNet.Numerics.Distributions;
using System;
using VueExample.Entities;
using VueExample.Providers;

public class NormalizeService : INormalizeService
{
    private readonly Srv6Context _srv6Context;
    private readonly IDieProvider _dieProvider;

    public NormalizeService(Srv6Context srv6Context, IDieProvider dieProvider)
    {
        _srv6Context = srv6Context;
        _dieProvider = dieProvider;
    }
    public async Task CreateNewNormalizeHistogram(int idmr, int graphicId, string waferId, double mean, double stddev)
    {
        var list = new List<DieGraphics>();
        var normal = new Normal(mean, stddev);
        var dies = await _dieProvider.GetDiesByWaferId(waferId);
        foreach (var die in dies)
        {
            var dieGraphic = new DieGraphics{ MeasurementRecordingId = idmr, 
                                              DieId = die.DieId,
                                              GraphicId = graphicId, 
                                              ValueString = die.Code + 'X' + Convert.ToString(normal.Sample(), CultureInfo.InvariantCulture)};
            list.Add(dieGraphic);
        }
        _srv6Context.DieGraphics.AddRange(list);
        await _srv6Context.SaveChangesAsync();
    }
    public async Task NormalizeHistogram(int idmr, double mean, double stddev)
    {
        var dieGraphics = _srv6Context.DieGraphics.Where(x => x.MeasurementRecordingId == idmr).ToList();
        var normal = new Normal(mean, stddev);
        foreach (var dieGraphic in dieGraphics)
        {
            var dieCode = dieGraphic.ValueString.Split('X')[0];
            dieGraphic.ValueString = dieCode + 'X' + Convert.ToString(normal.Sample(), CultureInfo.InvariantCulture);
        }
        _srv6Context.DieGraphics.UpdateRange(dieGraphics);
        await _srv6Context.SaveChangesAsync();
    }
}