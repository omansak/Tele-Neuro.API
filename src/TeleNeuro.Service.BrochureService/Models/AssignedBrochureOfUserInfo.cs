using System;
using TeleNeuro.Entities;

namespace TeleNeuro.Service.BrochureService.Models
{
    public class AssignedBrochureOfUserInfo
    {
        public int BrochureId { get; set; }
        public string BrochureName { get; set; }
        public Document Document { get; set; }
        public DateTime AssignDate { get; set; }
    }
}
