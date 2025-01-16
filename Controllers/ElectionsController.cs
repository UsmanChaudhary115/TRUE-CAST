using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OVS.Models;

public class ElectionsController : Controller
{
    private readonly AppDbContext _context;

    public ElectionsController(AppDbContext con)
    {
        _context = con;
    }

    // Election Dashboard for Admin
    public IActionResult ElectionDashboardForAdmin()
    {
        var elections = _context.Elections.ToList();
        return View(elections);
    }

    // Create Election
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Election election)
    {
        if (ModelState.IsValid)
        {
            election.IsLive = DateTime.Now >= election.StartDate && DateTime.Now <= election.EndDate;
            _context.Elections.Add(election);
            _context.SaveChanges();
            return RedirectToAction("ElectionDashboardForAdmin");
        }
        return View(election);
    }

    // Election Details for User
    public IActionResult ElectionDetailsForUser(int id)
    {
        var election = _context.Elections.FirstOrDefault(e => e.ElectionId == id);
        if (election == null)
            return NotFound();

        return View(election);
    }

    // Election Details for Admin
    public IActionResult ElectionDetailsForAdmin(int id)
    {
        var election = _context.Elections.FirstOrDefault(e => e.ElectionId == id);
        if (election == null)
            return NotFound();

        return View(election);
    }

    // Edit Election
    public IActionResult Edit(int id)
    {
        var election = _context.Elections.FirstOrDefault(e => e.ElectionId == id);
        if (election == null)
            return NotFound();

        return View(election);
    }

    [HttpPost]
    public IActionResult Edit(Election updatedElection)
    {
        if (ModelState.IsValid)
        {
            var election = _context.Elections.FirstOrDefault(e => e.ElectionId == updatedElection.ElectionId);
            if (election == null)
                return NotFound();

            election.Title = updatedElection.Title;
            election.Description = updatedElection.Description;
            election.Summary = updatedElection.Summary;
            election.StartDate = updatedElection.StartDate;
            election.EndDate = updatedElection.EndDate;
            election.IsLive = DateTime.Now >= updatedElection.StartDate && DateTime.Now <= updatedElection.EndDate;

            _context.SaveChanges();
            return RedirectToAction("ElectionDashboardForAdmin");
        }
        return View(updatedElection);
    }

    // Delete Election
    public IActionResult Delete(int id)
    {
        var election = _context.Elections.FirstOrDefault(e => e.ElectionId == id);
        if (election == null)
            return NotFound();

        return View(election);
    }

    [HttpPost]
    public IActionResult DeleteConfirmed(int id)
    {
        var election = _context.Elections.FirstOrDefault(e => e.ElectionId == id);
        if (election == null)
            return NotFound();

        _context.Elections.Remove(election);
        _context.SaveChanges();
        return RedirectToAction("ElectionDashboardForAdmin");
    }

    

    // Election Guidelines
    public IActionResult ElectoinGuidelines()
    {
        ViewBag.ClosestElection = _context.Elections
                               .Where(e => e.StartDate >= DateTime.Now)
                               .OrderBy(e => e.StartDate)
                               .Select(e => e.StartDate)
                               .FirstOrDefault();
        return View();
    }

    // Live Elections
    public IActionResult LiveElections()
    {
        var liveElections = _context.Elections
                            .Where(e => e.StartDate <= DateTime.Now && e.EndDate >= DateTime.Now)
                            .ToList();

        return View(liveElections);
    }

    // Upcoming Elections
    public IActionResult UpcomingElections()
    {
        var upcomingElections = _context.Elections
                                .Where(e => e.StartDate > DateTime.Now)  
                                .ToList();

        return View(upcomingElections);
    }

    public IActionResult PreviousElections()
    {
        var previousElections = _context.Elections
                                .Where(e => e.EndDate < DateTime.Now)  
                                .ToList();

        return View(previousElections);
    }
    [HttpPost]
    public IActionResult ShowResult(int ElectionId)
    {
        ViewBag.ElectionId = ElectionId;
        // Fetch the election results for the given ElectionId
        var election = _context.Elections.Where(e => e.ElectionId == ElectionId).FirstOrDefault();

        ElectionResult existingResult = _context.ElectionResults.Where(e => e.ElectionTitle == election.Title).FirstOrDefault();

        if (existingResult != null)
        {
            return View(existingResult);
        }


        var maxVoteCandidateId = _context.ElectionCandidates
                                   .Where(ec => ec.ElectionId == ElectionId)
                                   .OrderByDescending(ec => ec.VoteCount)
                                   .Select(s => s.CandidateId)
                                   .FirstOrDefault();


        var maxVoteCandidate = _context.Candidates.Where(c => c.CandidateId == maxVoteCandidateId).FirstOrDefault();

        ElectionResult result = null;
        if (maxVoteCandidate != null)
        {
            result = new ElectionResult
            {
                CandidateName = maxVoteCandidate.Name,
                CandidatePartyAffiliation = maxVoteCandidate.PartyAffiliation,
                ElectionTitle = election.Title,
                ElectionStartDate = election.StartDate,
                ElectionEndDate = election.EndDate,
                VotesReceived = _context.ElectionCandidates
                                   .Where(ec => ec.ElectionId == ElectionId)
                                   .OrderByDescending(ec => ec.VoteCount)
                                   .Select(s => s.VoteCount)
                                   .FirstOrDefault(),
                TotalVotes = _context.ElectionCandidates
                                .Where(ec => ec.ElectionId == ElectionId)
                                .Sum(ec => ec.VoteCount)
            };
            _context.ElectionResults.Add(result);
            _context.SaveChanges();
        }
        return View(result);
    }

}
