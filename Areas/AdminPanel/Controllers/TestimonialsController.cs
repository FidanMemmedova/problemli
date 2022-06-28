using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lawyer.Data;
using Lawyer.Models;
using Microsoft.AspNetCore.Hosting;
using Lawyer.Helpers;
using System.IO;

namespace Lawyer.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class TestimonialsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public TestimonialsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Testimonials.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Testimonial testimonial)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (!testimonial.Photo.CheckFileSize(200))
            {
                ModelState.AddModelError("", "image must be less than 200kb");
                return View();
            }
            if (!testimonial.Photo.CheckFileType("image/"))
            {
                ModelState.AddModelError("", "file must be image");
                return View();
            }
            string uniqueFileName = UploadedFile(testimonial);
            testimonial.Image = uniqueFileName;
            _context.Add(testimonial);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private string UploadedFile(Testimonial testimonial)
        {
            string uniqueFileName = null;

            if (testimonial.Photo != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "img");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + testimonial.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    testimonial.Photo.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var testimonial = await _context.Testimonials.FindAsync(id);
            if (testimonial == null)
            {
                return NotFound();
            }
            return View(testimonial);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Testimonial new_testimonial)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var old_testimonial = _context.Testimonials.Find(id);
            if (old_testimonial == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (!new_testimonial.Photo.CheckFileSize(200))
            {
                ModelState.AddModelError("", "image must be less than 200kb");
                return View();
            }
            if (!new_testimonial.Photo.CheckFileType("image/"))
            {
                ModelState.AddModelError("", "file must be image");
                return View();
            }
            var path = Helper.GetPath(_env.WebRootPath, "img", old_testimonial.Image);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            new_testimonial.Image = await new_testimonial.Photo.SaveFileAsync(_env.WebRootPath, "img");
            old_testimonial.Image = new_testimonial.Image;
            old_testimonial.Fullname = new_testimonial.Fullname;
            old_testimonial.Position = new_testimonial.Position;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id==null)
            {
                return BadRequest();
            }
            var testimonial = _context.Testimonials.Find(id);
            if (testimonial == null)
            {
                return NotFound();
            }
            var path = Helper.GetPath(_env.WebRootPath, "img", testimonial.Image);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            _context.Testimonials.Remove(testimonial);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TestimonialExists(int id)
        {
            return _context.Testimonials.Any(e => e.Id == id);
        }
    }
 }
   

