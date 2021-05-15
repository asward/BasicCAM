using BasicCAM.Core.Solutions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace BasicCAM.Tests.Solution
{
    public class CAMSolutionTests
    {
        [Fact]
        public void CAMSolutionPlungesAdded()
        {
            var proj = Seed.SeedProject();
            var sol = new CAMSolution(proj);



        }
    }
}
