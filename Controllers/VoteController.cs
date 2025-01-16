using System.ComponentModel.Design;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Matching;
using OVS.Models;

namespace OVS.Controllers
{
    public class VoteController : Controller
    {
        private readonly AppDbContext _context;
        public VoteController(AppDbContext con)
        {
            _context = con;
        }
        public IActionResult VoteHistory()
        {
            var userDetails = JsonSerializer.Deserialize<User>(HttpContext.Request.Cookies["UserDetails"]);
            var votes = _context.VoteHistories.Where(e=> e.UserID == userDetails.Id).ToList();
            return View(votes);
        }
        public ActionResult CastVote(int electionId)
        {
            // Fetch all candidates for the given election in one query
            var candidates = _context.Candidates
                                     .Where(c => _context.ElectionCandidates
                                     .Any(e => e.ElectionId == electionId && e.CandidateId == c.CandidateId))
                                     .ToList();

            ViewBag.ElectionId = electionId;
            ViewBag.ElectionTitle = _context.Elections.Where(e=> e.ElectionId == electionId).Select(s => s.Title).FirstOrDefault();
            return View(candidates);
        }


        [HttpPost]
        public async Task<ActionResult> CastVote(int electionId, int candidateId)
        {
            var userCookie = HttpContext.Request.Cookies["UserDetails"];
            if (string.IsNullOrEmpty(userCookie))
            {
                return RedirectToAction("Login", "Account");
            }

            var userDetails = JsonSerializer.Deserialize<User>(userCookie);

            // Fetch the election, candidate, and electionCandidate in one go
            var electionCandidate = _context.ElectionCandidates
                                            .Where(e => e.ElectionId == electionId && e.CandidateId == candidateId)
                                            .FirstOrDefault();

            if (electionCandidate == null)
            {
                TempData["ErrorMessage"] = "Invalid candidate or election.";
                return RedirectToAction("LiveElections", "Elections");
            }

            var election = _context.Elections.Where(e => e.ElectionId == electionId).FirstOrDefault();
            var candidate = _context.Candidates.Where(e => e.CandidateId == candidateId).FirstOrDefault();

            // Check if the user has already voted in this election
            if (candidate != null && election != null)
            {
                var existingVote = _context.VoteHistories
                                      .FirstOrDefault(v => v.UserID == userDetails.Id && v.ElectionID == electionId);

                if (existingVote != null)
                {
                    TempData["Message"] = "You have already voted in this election.";
                    return RedirectToAction("LiveElections", "Elections");
                }

                // Update vote count and save the vote
                electionCandidate.VoteCount += 1;

                VoteHistory voteHistory = new VoteHistory
                {
                    CandidateID = candidateId,
                    ElectionID = electionId,
                    CandidateName = candidate.Name,
                    ElectionTitle = election.Title,
                    UserID = userDetails.Id,
                    VoteDateTime = DateTime.Now,
                    Status = "Valid"
                };

                _context.VoteHistories.Add(voteHistory);

                // Set the previous winner to not winning
                var data = _context.ElectionCandidates.Where(ec => ec.isWinning == true).FirstOrDefault();

                if (data != null)
                {
                    data.isWinning = false;
                    await _context.SaveChangesAsync();
                }

                // Find the candidate with the most votes and set as the winner
                var maxVoteCandidate = _context.ElectionCandidates
                                               .Where(ec => ec.ElectionId == electionId)
                                               .OrderByDescending(ec => ec.VoteCount)
                                               .FirstOrDefault();

                if (maxVoteCandidate != null)
                {
                    maxVoteCandidate.isWinning = true;
                    await _context.SaveChangesAsync();
                }

                TempData["Message"] = "Your vote has been successfully cast!";
            }
            else
            {
                TempData["Message"] = "Error while Casting your Vote!";
            }

            return RedirectToAction("LiveElections", "Elections");
        }




    }
}
