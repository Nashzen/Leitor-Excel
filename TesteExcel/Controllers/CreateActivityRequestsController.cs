using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using TesteExcel.Models;

namespace TesteExcel.Controllers
{
    public class CreateActivityRequestsController : Controller
    {
        private readonly BancoContext _context;

        public CreateActivityRequestsController(BancoContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<JsonResult> UploadFile(IFormFile file)
        {
            try
            {
                var filePath = Path.GetTempFileName();

                IList<string> erros = new List<string>();

                if (file.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);

                        try
                        {
                            var activities = new List<CreateActivityRequests>();

                            using (ExcelPackage xlPackage = new ExcelPackage(stream))
                            {
                                var myWorksheet = xlPackage.Workbook.Worksheets.First();
                                var totalRows = myWorksheet.Dimension.End.Row;
                                var totalColumns = myWorksheet.Dimension.End.Column;

                                var sb = new StringBuilder();
                                for (int rowNum = 1; rowNum <= totalRows; rowNum++)
                                {
                                    var row = myWorksheet.Cells[rowNum, 1, rowNum, totalColumns].Select(c => c.Value == null ? string.Empty : c.Value.ToString());

                                    if (row.Count() > 0 && !string.IsNullOrEmpty(row.ToList()[0]))
                                    {
                                        var request = new CreateActivityRequests();
                                        request.Description = row.ToList()[0];
                                        request.Quantity = row.ToString()[1];
                                        request.Points = row.ToString()[2];
                                        request.Period = row.ToString()[3];

                                        activities.Add(request);
                                    }
                                }

                                foreach (var activity in activities)
                                {
                                    //Chama o service da atividade para salvar o registro
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
                //if (_serviceCity.Notifications.Any())
                //{
                //    foreach (var notification in _serviceCity.Notifications)
                //    {
                //        erros.Add(notification.Message);
                //    }
                //}

                return Json(erros);
            }
            catch (Exception ex)
            {
                ViewBag.Message = "File upload failed!!";
                return Json("Erro");
            }
        }

        // GET: CreateActivityRequests
        public async Task<IActionResult> Index()
        {
            return View(await _context.CreateActivityRequests.ToListAsync());
        }

        // GET: CreateActivityRequests/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var createActivityRequests = await _context.CreateActivityRequests
                .FirstOrDefaultAsync(m => m.Id == id);
            if (createActivityRequests == null)
            {
                return NotFound();
            }

            return View(createActivityRequests);
        }

        // GET: CreateActivityRequests/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CreateActivityRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Quantity,Points,Period,SelectedVerificationType")] CreateActivityRequests createActivityRequests)
        {
            if (ModelState.IsValid)
            {
                createActivityRequests.Id = Guid.NewGuid();
                _context.Add(createActivityRequests);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(createActivityRequests);
        }

        // GET: CreateActivityRequests/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var createActivityRequests = await _context.CreateActivityRequests.FindAsync(id);
            if (createActivityRequests == null)
            {
                return NotFound();
            }
            return View(createActivityRequests);
        }

        // POST: CreateActivityRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Description,Quantity,Points,Period,SelectedVerificationType")] CreateActivityRequests createActivityRequests)
        {
            if (id != createActivityRequests.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(createActivityRequests);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CreateActivityRequestsExists(createActivityRequests.Id))
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
            return View(createActivityRequests);
        }

        // GET: CreateActivityRequests/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var createActivityRequests = await _context.CreateActivityRequests
                .FirstOrDefaultAsync(m => m.Id == id);
            if (createActivityRequests == null)
            {
                return NotFound();
            }

            return View(createActivityRequests);
        }

        // POST: CreateActivityRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var createActivityRequests = await _context.CreateActivityRequests.FindAsync(id);
            _context.CreateActivityRequests.Remove(createActivityRequests);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CreateActivityRequestsExists(Guid id)
        {
            return _context.CreateActivityRequests.Any(e => e.Id == id);
        }
    }
}
