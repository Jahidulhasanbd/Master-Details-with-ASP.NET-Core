using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace prctc_masterDetails.Models.ViewModels
{
    public class CandidateVM
    {
        public CandidateVM()
        {
            this.SkillList = new List<int>();
        }
        public int CandidateId { get; set; }
        [Required, StringLength(50), Display(Name = "Candidate Name")]
        public string CandidateName { get; set; } = default!;
        [Required, Column(TypeName = "date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true), Display(Name = "Date Of Birth")]
        public DateTime DateOfBirth { get; set; }
        public int Phone { get; set; }
        public IFormFile? ImagePath { get; set; }
        public string? Image { get; set; } = default!;
        public bool Fresher { get; set; }
       public List<int> SkillList { get; set; }

    }

}
