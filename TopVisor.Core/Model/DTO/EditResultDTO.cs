using System;

// ReSharper disable InconsistentNaming

namespace TopVisor.Core.Model.DTO
{
    public class EditResultDTO
    {
        public UInt32? result { get; set; }
        public String message { get; set; }
        public Boolean error { get; set; }
    }
}