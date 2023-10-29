using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using prctc_masterDetails.Models;
using prctc_masterDetails.Models.ViewModels;

namespace prctc_masterDetails.Controllers
{
    public class CandidatesController : Controller
    {

        private readonly JobDbContext db;
        public IWebHostEnvironment _he;
        public CandidatesController(JobDbContext db, IWebHostEnvironment _he)
        {
            this.db = db;
            this._he = _he;
        }
        public async Task<IActionResult> Index()
        {
            return View(await db.Candidates.Include(x => x.CandidateSkills).ThenInclude(y => y.Skill).ToListAsync());
        }
        public IActionResult AddnewSkill(int? id)
        {
            ViewBag.skills = new SelectList(db.Skills, "SkillId", "SkillName", id.ToString() ?? "");
            return PartialView("_addNewSkill");
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CandidateVM candidateVM, int[] SkillId)
        {
            if (ModelState.IsValid)
            {
                Candidate candidate = new Candidate()
                {
                    CandidateName = candidateVM.CandidateName,
                    DateOfBirth = candidateVM.DateOfBirth,
                    Phone = candidateVM.Phone,
                    Fresher = candidateVM.Fresher
                };
                //image
                var file = candidateVM.ImagePath;
                string weebroot = _he.WebRootPath;
                string folder = "Images";
                string imgFileName = Path.GetFileName(candidateVM.ImagePath.FileName);
                string fileToSave = Path.Combine(weebroot, folder, imgFileName);
                if (file != null)
                {
                    using (var stream = new FileStream(fileToSave, FileMode.Create))
                    {
                        candidateVM.ImagePath.CopyTo(stream);
                        candidate.Image = "/" + folder + "/" + imgFileName;
                    }
                }
                foreach (var item in SkillId)
                {
                    CandidateSkill candidateSkill = new CandidateSkill()
                    {
                        Candidate = candidate,
                        CandidateId = candidate.CandidateId,
                        SkillId = item
                    };
                    db.CandidateSkills.Add(candidateSkill);
                }
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View();
        }
        public async Task<IActionResult> Edit(int? id)
        {
            var candidate = await db.Candidates.FirstOrDefaultAsync(x => x.CandidateId == id);
            CandidateVM candidateVM = new CandidateVM()
            {
                CandidateId = candidate.CandidateId,
                CandidateName = candidate.CandidateName,
                DateOfBirth = candidate.DateOfBirth,
                Phone = candidate.Phone,
                Image = candidate.Image,
                Fresher = candidate.Fresher
            };
            var existSkill = db.CandidateSkills.Where(x => x.CandidateId == id).ToList();
            foreach (var item in existSkill)
            {
                candidateVM.SkillList.Add(item.SkillId);
            }
            return View(candidateVM);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(CandidateVM candidateVM, int[] skillId)
        {
            if (ModelState.IsValid)
            {
                Candidate candidate = new Candidate()
                {
                    CandidateId = candidateVM.CandidateId,
                    CandidateName = candidateVM.CandidateName,
                    DateOfBirth = candidateVM.DateOfBirth,
                    Phone = candidateVM.Phone,
                    Fresher = candidateVM.Fresher,
                    Image = candidateVM.Image
                };
                var file = candidateVM.ImagePath;
                if (file != null)
                {
                    string webroot = _he.WebRootPath;
                    string folder = "Images";
                    string imgFileName = Path.GetFileName(candidateVM.ImagePath.FileName);
                    string fileToSave = Path.Combine(webroot, folder, imgFileName);
                    using (var stream = new FileStream(fileToSave, FileMode.Create))
                    {
                        candidateVM.ImagePath.CopyTo(stream);
                        candidate.Image = "/" + folder + "/" + imgFileName;
                    }
                }

                var existSkill = db.CandidateSkills.Where(x => x.CandidateId == candidate.CandidateId).ToList();
                foreach (var item in existSkill)
                {
                    db.CandidateSkills.Remove(item);
                }
                foreach (var item in skillId)
                {
                    CandidateSkill candidateSkill = new CandidateSkill()
                    {

                        CandidateId = candidate.CandidateId,
                        SkillId = item
                    };
                    db.CandidateSkills.Add(candidateSkill);
                }
                db.Update(candidate);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        public IActionResult Delete(int? id)
        {
            Candidate candidate=db.Candidates.First(x=>x.CandidateId == id);
            var candidateSkills = db.CandidateSkills.Where(x => x.CandidateId == id).ToList();
            CandidateVM candidateVM = new CandidateVM()
            {
                CandidateId = candidate.CandidateId,
                CandidateName = candidate.CandidateName,
                Phone = candidate.Phone,
                Image = candidate.Image,
                Fresher = candidate.Fresher
            };
            if(candidateSkills.Count > 0)
            {
                foreach(var item in candidateSkills)
                {
                    candidateVM.SkillList.Add(item.SkillId);
                }
            }
            return View(candidateVM);
        }
        [HttpPost]
        [ActionName("Delete")]
        public IActionResult Delete(int id)
        {
            Candidate candidate = db.Candidates.Find(id);
            if(candidate == null)
            {
                return NotFound();
            }
            db.Entry(candidate).State = EntityState.Deleted;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}


