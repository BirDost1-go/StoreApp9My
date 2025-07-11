using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreApp9My.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreApp9My.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDBContext _context;

        private readonly IWebHostEnvironment _webHostEnvironment; // Add this field

        //public ProductsController(ApplicationDBContext context)
        //public ProductsController(ApplicationDBContext context)
        //    : this(context, null!) // Use null-forgiving operator to suppress CS8618 warning  
        //{
        //}
        public ProductsController(ApplicationDBContext context, IWebHostEnvironment webHostEnvironment) // Update constructor
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment; // Initialize the field
        }

        //public ProductsController(ApplicationDBContext context)
        //{
        //    _context = context;
        //}

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var applicationDBContext = _context.Products.Include(p => p.People);
            return View(await applicationDBContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.People)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["PeopleId"] = new SelectList(_context.Peoples, "Id", "Phone");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,Stock,ImageUrl,CreatedAt,PeopleId,ImageFile,ImageLink")] Product product)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string imageDirectory = Path.Combine(wwwRootPath, "images");
                if (!Directory.Exists(imageDirectory))
                {
                    Directory.CreateDirectory(imageDirectory); // Create the directory if it doesn't exist
                }

                if (product.ImageFile != null && product.ImageFile.Length > 0)
                {
                    // wwwroot yolunu bul
                    //string wwwRootPath = _webHostEnvironment.WebRootPath;

                    // Benzersiz dosya adı oluştur
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(product.ImageFile.FileName);

                    // Tam path
                    string path = Path.Combine(wwwRootPath, "images", fileName);

                    // Kaydet
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(stream);
                    }

                    // DB'ye kaydedilecek path
                    product.ImageUrl = "/images/" + fileName;
                }
                else if (!string.IsNullOrWhiteSpace(product.ImageLink))
                {
                    using (HttpClient client = new HttpClient())
                    {
                        try
                        {
                            var bytes = await client.GetByteArrayAsync(product.ImageLink);
                            string extension = Path.GetExtension(product.ImageLink);
                            if (string.IsNullOrEmpty(extension) || extension.Length > 5)
                            {
                                extension = ".jpg";
                            }
                            string fileName = Guid.NewGuid().ToString() + extension;
                            string path = Path.Combine(imageDirectory, fileName);

                            await System.IO.File.WriteAllBytesAsync(path, bytes);
                            product.ImageUrl = "/images/" + fileName;
                        }
                        catch
                        {
                            ModelState.AddModelError("ImageLink", "Invalid image link. Please provide a valid URL.");
                            ViewData["PeopleId"] = new SelectList(_context.Peoples, "Id", "Phone", product.PeopleId);
                            return View(product);
                        }
                    }
                }
                else
                {
                    product.ImageUrl = "/images/default.png"; // Default image if no file or link is provided
                }
                _context.Add(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "The picture added succesfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["PeopleId"] = new SelectList(_context.Peoples, "Id", "Phone", product.PeopleId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["PeopleId"] = new SelectList(_context.Peoples, "Id", "Phone", product.PeopleId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,Stock,ImageUrl,CreatedAt,PeopleId")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            ViewData["PeopleId"] = new SelectList(_context.Peoples, "Id", "Phone", product.PeopleId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.People)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
