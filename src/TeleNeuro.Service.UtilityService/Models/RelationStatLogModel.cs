namespace TeleNeuro.Service.UtilityService.Models
{
    public class RelationStatLogModel
    {
        public int? ProgramId { get; set; }
        public int? ExerciseId { get; set; }
        public string ActionKey { get; set; }
        public string ActionArgument { get; set; }
        public int? UserProgramRelationId { get; set; }
        public int? ExerciseProgramRelationId { get; set; }
        public int? UserId { get; set; }
    }
}
