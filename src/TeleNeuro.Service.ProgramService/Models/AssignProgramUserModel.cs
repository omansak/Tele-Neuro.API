﻿namespace TeleNeuro.Service.ProgramService.Models
{
    public class AssignProgramUserModel
    {
        public int ProgramId { get; set; }
        public int UserId { get; set; }
        public int AssignedUserId { get; set; }
    }
}
