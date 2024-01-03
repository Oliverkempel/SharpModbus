﻿namespace Disomat_Modbus.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ReadDiscreteInputsResponse
    {
        public string sentFrame { get; set; }
        public string responseFrame { get; set; }
        public int requestedStartAddress { get; set; }
        public int requestedAmountOfAddresses { get; set; }
        public List<DiscreteInputOutput> DiscreteInputs { get; set; }
    }
}
