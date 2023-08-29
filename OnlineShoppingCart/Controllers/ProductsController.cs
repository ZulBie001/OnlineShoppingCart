using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingCart.Data;
using OnlineShoppingCart.Handlers;
using OnlineShoppingCart.Models;
using System.Drawing;

namespace OnlineShoppingCart.Controllers
{
    //[Authorized]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products
                .Select(m => new ProductViewModel
                {
                    Brand = m.Brand.Name,
                    Name = m.Name,
                    Category = m.Category.Name,
                    Description = m.Description,
                    Price = m.Price,
                    Slug = m.Slug,
                    ReleaseDate = m.ReleaseDate,
                    Id = m.Id,
                    Stock = m.Stock,
                    ImageUrl = m.Images.OrderBy(n => n.DbEntryTime).Select(n => n.URL).FirstOrDefault()
                }).ToList();
            return View(products);
        }

        public IActionResult Create()
        {
            if (!_context.Categories.Any(m => m.Status && m.Type == CategoryType.Category))
            {
                TempData["notification"] = "No category exists with the active status";
                return RedirectToAction("Create", "Categories");
            }

            if (!_context.Categories.Any(m => m.Status && m.Type == CategoryType.Brand))
            {
                TempData["notification"] = "No brand exists with the active status";
                return RedirectToAction("Create", "Categories");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product model)
        {
            if (ModelState.IsValid)
            {
                if (model.Uploads?.Any() == true)
                {
                    //if(model.Images == null)
                    //{
                    //    model.Images = new();
                    //}
                    model.Images ??= new(); // new List<ProductImage>();
                    int imageRank = 0;
                    string basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    string appPath = Path.Combine("images", "products");
                    string directryPath = Path.Combine(basePath, appPath);
                    Directory.CreateDirectory(directryPath);

                    foreach (var item in model.Uploads)
                    {
                        if (item.Length > 0)
                        {
                            string fileName = Path.GetRandomFileName().Replace(".", "") + Path.GetExtension(item.FileName);

                            using var stream = new FileStream(Path.Combine(directryPath, fileName), FileMode.Create);
                            item.CopyTo(stream);
                            model.Images.Add(new ProductImage
                            {
                                Rank = ++imageRank,
                                ProductId = model.Id,
                                URL = Path.Combine(appPath, fileName).Replace("\\", "/")
                            });
                        }
                    }

                    _context.Add(model);
                    int r = _context.SaveChanges();

                    return RedirectToAction(nameof(Index));
                }
            }
            return View(model);
        }

        public IActionResult Edit(string id)
        {
            var product = _context.Products.Include(p => p.Category).Include(p => p.Brand).Include(p => p.Images).Where(m => m.Id == id).FirstOrDefault();

            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product model, List<string> deletedImagesIds)
        {
            if (ModelState.IsValid)
            {
                if (deletedImagesIds != null && deletedImagesIds.Any())
                {
                    foreach (var id in deletedImagesIds)
                    {
                        var image = _context.ProductImages.Find(id)
;
                        if (image != null)
                        {
                            var imageUrl = image.URL;
                            _context.ProductImages.Remove(image);

                            var currentDirectory = Directory.GetCurrentDirectory();
                            var fileUrl = Path.Combine(currentDirectory, "wwwroot", imageUrl.Replace("/", "\\"));
                            if (System.IO.File.Exists(fileUrl))
                            {
                                System.IO.File.Delete(fileUrl);
                            }
                        }
                    }
                }

                if (model.Uploads != null && model.Uploads.Count > 0)
                {
                    int rank = 1;
                    foreach (var image in model.Uploads)
                    {
                        if (image != null && image.Length > 0)
                        {
                            var fileExtension = Path.GetExtension(image.FileName);

                            string basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                            string imagesPath = Path.Combine(basePath, "/images", "products");
                            Directory.CreateDirectory(imagesPath);

                            string fileName = Path.GetRandomFileName().Replace(".", "") + fileExtension;
                            string filePath = Path.Combine(imagesPath, fileName);

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                image.CopyTo(fileStream);
                            }


                            string relativePath = Path.Combine("~/images/products", fileName).Replace("\\", "/");
                            _context.ProductImages.Add(new ProductImage { URL = relativePath, Rank = rank++, ProductId = model.Id });
                        }
                    }
                }

                _context.Products.Update(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }
        public IActionResult Delete(string id)
        {
            var product = _context.Products.Include(p => p.Category).Include(p => p.Brand).Include(p => p.Images).Where(m => m.Id == id).FirstOrDefault();
            return View(product);
        }
        [HttpPost]
        public IActionResult Delete(Product model)
        {
            var images = _context.ProductImages.Where(p => p.ProductId == model.Id).ToList();
            foreach (var image in images)
            {
                var currentDirectory = Directory.GetCurrentDirectory();
                var fileUrl = Path.Combine(currentDirectory, "wwwroot", image.URL.TrimStart('/').Replace("/", "\\"));

                if (System.IO.File.Exists(fileUrl))
                {
                    try
                    {
                        System.IO.File.Delete(fileUrl);
                    }
                    catch (Exception)
                    {

                    }
                }

                _context.ProductImages.Remove(image);
            }

            var associatedProducts = _context.Products.Where(p => p.CategoryId == model.Id);
            foreach (var product in associatedProducts)
            {
                product.CategoryId = null;
            }
            TempData["DeleteMessage"] = "Category successfully deleted.";
            _context.Products.Remove(model);
            _context.SaveChanges();
            return RedirectToAction("index");
        }
    }
}
