using System.ComponentModel.DataAnnotations.Schema;

namespace Natia.Core.Entities
{
    [Table("neurals")]
    public class Neurall : AbstractEntity
    {
        public DateTime ActionDate { get; set; }

        public string? WhatNatiaSaid { get; set; }

        public bool IsError { get; set; } = false;

        public bool IsCritical { get; set; } = false;

        public Topic WhatWasTopic { get; set; }

        public Priority Priority { get; set; }

        public string? ChannelName { get; set; }

        public string? Satellite { get; set; }

        public string? SuggestedSolution { get; set; }

        public string? ErrorMessage { get; set; }

        public string? ErrorDetails { get; set; }
    }

    public enum Topic
    {
        არხი = 1,
        სატელიტი = 2,
        სადგურისტემპერატურა = 3,
        მუხიანისაიპი = 4,
        ტეორი = 5,
        ენკოდერი = 6,
        მულტისვიჩები = 7,
        დეველოპერისშეცდომა = 8,
        სხვა = 9,
        ნათიასგრძნიბები = 10,
        სარელეოსადგური = 11,
    }

    public enum Priority
    {
        კრიტიკული = 1,
        საშუალო = 2,
        მარტივი = 3,
        სხვა = 4
    }
}
