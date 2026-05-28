using System;
using System.Collections.Generic;
using System.Text;

namespace CyberInfo.Models
{
    /// <summary>Multiple-choice cybersecurity scenario for the quiz feature.</summary>
    public class Scenario
    {
        public string Description { get; }
        public List<string> Options { get; }
        public int CorrectOptionIndex { get; }
        public string Feedback { get; }

        public Scenario(string desc, List<string> opts, int correct, string feedback)
        { Description = desc; Options = opts; CorrectOptionIndex = correct; Feedback = feedback; }

    }
}