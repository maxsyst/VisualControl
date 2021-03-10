using System;
using System.Collections.Generic;
using VueExample.Models.Vertx;

namespace VueExample.ViewModels.Vertx.ResponseModels
{
    public class MeasurementResponseModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Vgate { get; set; }
        public string Vpower { get; set; }
        public List<string> Goal { get; set; }
        public int DurationPreSeconds { get; set; }
        public int DurationSeconds { get; set; }
        public string MeasurementAttemptId { get; set; }
        public string MeasurementChannel { get; set; }
        public string TemperatureSensor { get; set; }
        public string Matching { get; set; }
        public string MatchingBoard { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime FinishDate { get; set; }
        public IList<Comment> Comments { get; set; }
        public IList<string> Notes { get; set; }
    }

    public class MeasurementResponseModelWithPoints
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public List<Point> Points { get; set; }
    }
}