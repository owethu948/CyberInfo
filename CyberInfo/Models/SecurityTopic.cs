using System;
using System.Collections.Generic;
using System.Text;

namespace CyberInfo.Models
{
    /// <summary>Cybersecurity topic with auto-properties (Part 1 requirement).</summary>
    public class SecurityTopic
    {
        public string Name { get; }
        public string WhatIs { get; }
        public string ShortDescription { get; }
        public string WhenWhere { get; }
        public string Prevention { get; }
        public string Reference { get; }

        public SecurityTopic(string name, string whatIs, string shortDesc,
                     string whenWhere, string prevention, string reference)
        {
            Name = name; WhatIs = whatIs; ShortDescription = shortDesc;
            WhenWhere = whenWhere; Prevention = prevention; Reference = reference;
        }

        public string BuildGuiResponse() =>
            $"📘  {Name.ToUpper()}\n" +
            $"{"─".PadRight(55, '─')}\n\n" +
            $"❓  WHAT IS {Name.ToUpper()}?\n{WhatIs}\n\n" +
            $"⚠️   WHY IT MATTERS:\n{ShortDescription}\n\n" +
            $"📍  WHEN & WHERE YOU'RE AT RISK:\n{WhenWhere}\n\n" +
            $"🛡️   HOW TO PROTECT YOURSELF:\n{Prevention}\n\n" +
            $"📖  Source: {Reference}";
    }
}
