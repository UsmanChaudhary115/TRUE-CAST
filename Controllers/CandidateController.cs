using Microsoft.AspNetCore.Mvc;
using OVS.Models;
using System.Collections.Generic;
using System.Linq;

namespace OVS.Controllers
{
    public class CandidateController : Controller
    {
        private readonly AppDbContext _context;
        public CandidateController(AppDbContext con)
        {
            _context = con;
        }
        // GET: Candidate/AddCandidate
        public IActionResult AddCandidate()
        {
            return View();
        }

        // POST: Candidate/AddCandidate
        [HttpPost]
        public IActionResult AddCandidate(Candidate candidate)
        {
            Console.WriteLine(candidate.Name);
            _context.Candidates.Add(candidate);
            _context.SaveChanges();
            return RedirectToAction("CandidateList");
            //return View(candidate);
        }

        // GET: Candidate/UpdateCandidate/{id}
        public IActionResult UpdateCandidate(int id)
        {
            var candidate = _context.Candidates.FirstOrDefault(c => c.CandidateId == id);
            if (candidate == null) return NotFound();
            return View(candidate);
        }

        // POST: Candidate/UpdateCandidate/{id}
        [HttpPost]
        public IActionResult UpdateCandidate(int id, Candidate updatedCandidate)
        {
            var candidate = _context.Candidates.FirstOrDefault(c => c.CandidateId == id);
            if (candidate == null) return NotFound();
            candidate.Name = updatedCandidate.Name;
            candidate.PartyAffiliation = updatedCandidate.PartyAffiliation;
            candidate.CNIC = updatedCandidate.CNIC;
            candidate.ContactInfo = updatedCandidate.ContactInfo;
            candidate.Manifesto = updatedCandidate.Manifesto;
            _context.SaveChanges();
            return RedirectToAction("CandidateList");
        }

        // GET: Candidate/RemoveCandidate/{id}
        public IActionResult RemoveCandidate(int id)
        {
            var candidate = _context.Candidates.FirstOrDefault(c => c.CandidateId == id);
            if (candidate == null) return NotFound();
            return View(candidate);
        }

        // POST: Candidate/RemoveCandidate/{id}
        [HttpPost]
        public IActionResult RemoveCandidateConfirmed(int id)
        {
            var candidate = _context.Candidates.FirstOrDefault(c => c.CandidateId == id);
            if (candidate == null) return NotFound();

            _context.Candidates.Remove(candidate);
            _context.SaveChanges();

            var data = _context.ElectionCandidates.Where(c => c.CandidateId == id).ToList();
            foreach(var item in data)
            {
                _context.ElectionCandidates.Remove(item);
                _context.SaveChanges();
            }
            return RedirectToAction("CandidateList");
        }

        // GET: Candidate/CandidateList
        public IActionResult CandidateList()
        {
            var candidates = _context.Candidates.ToList();
            return View(candidates);
        }

        // GET: Candidate/CandidateDetails/{id}
        public IActionResult CandidateDetails(int id)
        {
            var candidate = _context.Candidates.FirstOrDefault(c => c.CandidateId == id);
            if (candidate == null) return NotFound();

            return View(candidate); // Pass the candidate to the view
        }

        //remove form election
        [HttpPost]
        public IActionResult Remove(int electionId, int candidateId)
        {
            var electionCandidate = _context.ElectionCandidates
                            .Where(e => e.ElectionId == electionId && e.CandidateId == candidateId)
                            .FirstOrDefault();

            if (electionCandidate != null)
            {
                _context.ElectionCandidates.Remove(electionCandidate);
                _context.SaveChanges();
            }
            return RedirectToAction("GetCandidates", new { electionId });
        }
        public IActionResult GetCandidates(int electionId)
        {
            var candidateIDs = _context.ElectionCandidates
                .Where(e => e.ElectionId == electionId)
                .Select(c => c.CandidateId)
                .ToList();

            // Initialize the candidates list
            List<Candidate> candidates = new List<Candidate>();

            // Fetch the candidates for the election
            foreach (var item in candidateIDs)
            {
                var candidate = _context.Candidates.FirstOrDefault(c => c.CandidateId == item);
                if (candidate != null)
                {
                    candidates.Add(candidate); // Add the candidate to the list if found
                }
            }
            ViewBag.ElectionId = electionId;
            // Return the list of candidates
            return View("CandidateListForElection", candidates);
        }
        // GET: AddCandidateToElection
        [HttpGet]
        public IActionResult AddCandidateToElection(int electionId)
        {
            // Fetch the election title
            var election = _context.Elections.FirstOrDefault(e => e.ElectionId == electionId);
            if (election == null)
            {
                return NotFound("Election not found.");
            }

            var candidates = _context.Candidates.ToList();

            // Pass data to the view
            ViewBag.ElectionTitle = election.Title;
            ViewBag.ElectionId = electionId;

            return View(candidates);
        }

        // POST: AddCandidateToElection
        [HttpPost]
        public IActionResult AddCandidateToElection(int electionId, int candidateId)
        {
            Console.WriteLine("ElectionId: " + electionId + " candidateId:" + candidateId);
            // Check if the candidate is already added to the election
            var existingEntry = _context.ElectionCandidates
                .FirstOrDefault(ec => ec.ElectionId == electionId && ec.CandidateId == candidateId);
            if (existingEntry != null)
            {
                TempData["Message"] = "Candidate is already added to this election.";
                return RedirectToAction("AddCandidateToElection", new { electionId });
            }

            // Add the candidate to the election
            _context.ElectionCandidates.Add(new ElectionCandidate
            {
                ElectionId = electionId,
                CandidateId = candidateId,
                VoteCount = 0
            });
            _context.SaveChanges();

            TempData["Message"] = "Candidate successfully added to the election.";
            return RedirectToAction("AddCandidateToElection", new { electionId });
        }
    }
}
