using System;
using System.Collections.Generic;
using VueExample.Models.Vertx;

namespace VueExample.ViewModels.Vertx.InputModels
{
    public class MeasurementInputModel
    {
        public string Name { get; set; }
        public string MeasurementAttemptId { get; set; }
        public string MeasurementChannel { get; set; }
        public string Vgate { get; set; }
        public string Vpower { get; set; }
        public List<string> Goal { get; set; } = new List<string>();
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public List<string> Notes { get; set; } = new List<string>();
        public DateTime CreationDate { get; set; }
        public string TemperatureSensor { get; set; }
        public string Matching { get; set; }
        public string MatchingBoard { get; set; }
    }

    public class MeasurementWithMdvInputModel
    {
        public MeasurementInputModel MeasurementInputModel { get; set; }
        public string WaferId { get; set; }
        public string Code { get; set; }
    }
}