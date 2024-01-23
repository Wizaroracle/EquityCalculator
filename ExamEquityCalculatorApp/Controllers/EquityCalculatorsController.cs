using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExamEquityCalculatorApp.Data;
using ExamEquityCalculatorApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamEquityCalculatorApp.Controllers
{
    public class EquityCalculatorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EquityCalculatorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EquityCalculators
        public async Task<IActionResult> Index()
        {
            return View(await _context.EquityCalculator.ToListAsync());
        }

        // GET: EquityCalculators/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equityCalculator = await _context.EquityCalculator
                .FirstOrDefaultAsync(m => m.Id == id);
            if (equityCalculator == null)
            {
                return NotFound();
            }

            return View(equityCalculator);
        }

        // GET: EquityCalculators/ShowEnterForm
        public IActionResult ShowEnterForm()
        {
            return View();
        }
       

        [HttpPost]
        public IActionResult Calculate(EquityCalculator model)
        {
            if (ModelState.IsValid)
            {
                // Performing the calculations and storing results as JSON string in TempData
                var results = CalculateEquityResults(model);

                // Saving the input to the database
                _context.Add(model);
                _context.SaveChanges();

                // Retrieving the saved entity from the database
                var savedModel = _context.EquityCalculator.OrderByDescending(e => e.Id).FirstOrDefault();

                // Saving the calculated results as JSON string in TempData
                TempData["EquityResults"] = Newtonsoft.Json.JsonConvert.SerializeObject(results);

                // Redirecting to the summary page with the saved model Id
                return RedirectToAction("Summary", new { id = savedModel.Id });
            }

            // If model state is not valid, redisplay the form
            return View("EnterForm", model);
        }

        public IActionResult Summary(int id)
        {
            // Retrieving the saved entity from the database using the id
            var savedModel = _context.EquityCalculator.FirstOrDefault(e => e.Id == id);

            if (savedModel == null)
            {
                // Handling the case where the entity with the specified id is not found
                return NotFound();
            }

            // Retrieving results from TempData as JSON string and deserialize it
            var resultsJson = TempData["EquityResults"] as string;
            var results = Newtonsoft.Json.JsonConvert.DeserializeObject<List<object>>(resultsJson);

            // Passing both the saved model and the calculated results to the view
            ViewData["SavedModel"] = savedModel;
            return View(results);
        }

        // POST: EquityCalculators/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SellingPrice,ReservationDate,EquityTerm")] EquityCalculator equityCalculator)
        {
            if (ModelState.IsValid)
            {
                _context.Add(equityCalculator);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(equityCalculator);
        }

        // GET: EquityCalculators/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equityCalculator = await _context.EquityCalculator.FindAsync(id);
            if (equityCalculator == null)
            {
                return NotFound();
            }
            return View(equityCalculator);
        }
        // GET: ViewSchedule
        public IActionResult ViewSchedule(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve the entity from the database based on the provided id
            var equityCalculator = _context.EquityCalculator.Find(id);

            if (equityCalculator == null)
            {
                return NotFound();
            }

            // Perform the calculations
            var results = CalculateEquityResults(equityCalculator);

            // Save the calculated results as JSON string in TempData
            TempData["EquityResults"] = Newtonsoft.Json.JsonConvert.SerializeObject(results);

            // Redirect to the summary page
            return RedirectToAction("Summary", new { id = equityCalculator.Id });
        }

        // POST: EquityCalculators/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SellingPrice,ReservationDate,EquityTerm")] EquityCalculator equityCalculator)
        {
            if (id != equityCalculator.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(equityCalculator);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EquityCalculatorExists(equityCalculator.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(equityCalculator);
        }

        // GET: EquityCalculators/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equityCalculator = await _context.EquityCalculator
                .FirstOrDefaultAsync(m => m.Id == id);
            if (equityCalculator == null)
            {
                return NotFound();
            }

            return View(equityCalculator);
        }

        // POST: EquityCalculators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var equityCalculator = await _context.EquityCalculator.FindAsync(id);
            if (equityCalculator != null)
            {
                _context.EquityCalculator.Remove(equityCalculator);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: api/EquityCalculators
        [HttpGet("api/GetEquityCalculators")]
        public async Task<IActionResult> GetEquityCalculators()
        {
            var equityCalculators = await _context.EquityCalculator.ToListAsync();
            return Ok(equityCalculators);
        }

        // POST: api/EquityCalculators
        [HttpPost("api/PostEquityCalculator")]
        public async Task<IActionResult> PostEquityCalculator(EquityCalculator equityCalculator)
        {
            if (ModelState.IsValid)
            {
                // Add the EquityCalculator to the context and save changes
                _context.Add(equityCalculator);
                await _context.SaveChangesAsync();

                // Calculate equity results for the given model
                var results = CalculateEquityResults(equityCalculator);

                // Set response headers
                Response.Headers.Add("SellingPrice", equityCalculator.SellingPrice.ToString());
                Response.Headers.Add("ReservationDate", equityCalculator.ReservationDate.ToString("MM/dd/yyyy"));
                Response.Headers.Add("EquityTerm", equityCalculator.EquityTerm.ToString());

                // Return the calculated results in the response body
                return Ok(results);
            }

            return BadRequest(ModelState);
        }



        // DELETE: api/EquityCalculators/5
        [HttpDelete("api/DeleteEquityCalculator/{id}")]
        public async Task<IActionResult> DeleteEquityCalculator(int id)
        {
            var equityCalculator = await _context.EquityCalculator.FindAsync(id);

            if (equityCalculator == null)
            {
                return NotFound();
            }

            _context.EquityCalculator.Remove(equityCalculator);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EquityCalculatorExists(int id)
        {
            return _context.EquityCalculator.Any(e => e.Id == id);
        }

        private List<object> CalculateEquityResults(EquityCalculator model)
        {
            var results = new List<object>();
            decimal balance = model.SellingPrice;

            for (int term = 1; term <= model.EquityTerm; term++)
            {
                // Logic for Calculating the first term to last term of specified Sellingprice, Reservation Date, and Term

                var dueDate = model.ReservationDate.AddMonths(term); //Adding months as per looped 
                var monthlyAmount = balance;

                var deduction = balance >= 2000m ? 2000m : 500m; // Utilized ternary operation to avoid the overlapping of the balance
                balance -= deduction;
                monthlyAmount = balance / term;

                var interest = 0.05m * balance;

                var insurance = 0.01m * monthlyAmount;

                var totalAmount = monthlyAmount + interest + insurance;

                if (term == model.EquityTerm) //This will keep the last term equal to zero
                {
                    deduction = balance;
                    balance = 0;
                    monthlyAmount = 0;
                    interest = 0;
                    insurance = 0;
                    totalAmount = 0;

                }

                var termResult = new
                {
                    Term = term,
                    DeductionFromSellingPrice = deduction,
                    Balance = balance.ToString("C"),
                    DueDate = dueDate.ToString("MM/dd/yyyy"),
                    MonthlyAmount = monthlyAmount.ToString("C"),
                    Interest = interest.ToString("C"),
                    Insurance = insurance.ToString("C"),
                    TotalAmount = totalAmount.ToString("C")
                };

                results.Add(termResult);
            }

            return results;
        }
    }
}
