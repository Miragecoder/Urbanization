using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirage.Urbanization.Web.ClientMessages
{
    class YearAndMonthChangedState
    {
        public string yearAndMonthDescription { get; set; }
        public LabelAndValue[] overallLabelsAndValues { get; set; }
        public LabelAndValue[] cityBudgetLabelsAndValues { get; set; }
        public LabelAndValue[] issueLabelAndValues { get; set; }
        public LabelAndValue[] generalOpinion { get; set; }
    }

    class LabelAndValue
    {
        public string label { get; set; }
        public string value { get; set; }
    }
}
