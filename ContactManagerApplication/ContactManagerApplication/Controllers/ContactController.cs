using ContactManagerApplication.Models;
using Microsoft.AspNetCore.Mvc;

namespace ContactManagerApplication.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var contacts = _context.Contacts.ToList();
            return View(contacts);
        }

        [HttpPost]
        public IActionResult UploadFile(IFormFile csvFile)
        {
            if (csvFile == null || csvFile.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Please upload a valid CSV file.");
                return RedirectToAction("Index");
            }

            using (var reader = new StreamReader(csvFile.OpenReadStream()))
            {
                var contacts = new List<Contact>();
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    var values = line.Split(',');

                    if (values.Length != 5) continue;

                    contacts.Add(new Contact
                    {
                        Name = values[0],
                        DateOfBirth = DateTime.ParseExact(values[1], "yyyy-MM-dd", null),
                        Married = bool.Parse(values[2]),
                        Phone = values[3],
                        Salary = decimal.Parse(values[4], System.Globalization.CultureInfo.InvariantCulture)
                    });
                }

                _context.Contacts.AddRange(contacts);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EditInline(Contact contact)
        {
            if (string.IsNullOrWhiteSpace(contact.Name))
            {
                return Json(new { success = false, message = "Name cannot be empty." });
            }
            if (string.IsNullOrWhiteSpace(contact.Phone))
            {
                return Json(new { success = false, message = "Invalid phone format." });
            }
            if (contact.DateOfBirth > DateTime.Now)
            {
                return Json(new { success = false, message = "Date of birth cannot be in the future." });
            }
            if (contact.Salary <= 0)
            {
                return Json(new { success = false, message = "Salary must be a positive number." });
            }

            var existingContact = _context.Contacts.Find(contact.Id);
            if (existingContact == null)
            {
                return NotFound();
            }

            existingContact.Name = contact.Name;
            existingContact.DateOfBirth = contact.DateOfBirth;
            existingContact.Married = contact.Married;
            existingContact.Phone = contact.Phone;
            existingContact.Salary = contact.Salary;

            _context.SaveChanges();

            return Json(new { success = true, message = "Contact updated successfully." });
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var contact = _context.Contacts.Find(id);
            if (contact == null)
            {
                return NotFound();
            }

            _context.Contacts.Remove(contact);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
