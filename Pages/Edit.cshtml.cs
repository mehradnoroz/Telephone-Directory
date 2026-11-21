using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TelephoneDirectory.Data.AppDbContext;
using TelephoneDirectory.Models.contact;

namespace TelephoneDirectory.Pages;

public class EditModel : PageModel
{
    private AppDbContext databaseContext;

    [BindProperty]
    public Contact Contact { get; set; }

    [BindProperty]
    public string? ShamsiBirthDate { get; set; }


    [BindProperty]
    public IFormFile? UploadedPhoto { get; set; }

    public EditModel(AppDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public IActionResult OnGet(int id)
    {
        Contact = databaseContext.contats.FirstOrDefault(contact => contact.Id == id);

        if (Contact == null)
        {
            return NotFound();
        }

        if (Contact.BirthDate != null)
        {
            PersianCalendar calendar = new PersianCalendar();

            DateTime date = Contact.BirthDate.Value.ToDateTime(TimeOnly.MinValue);

            int year = calendar.GetYear(date);
            int month = calendar.GetMonth(date);
            int day = calendar.GetDayOfMonth(date);

            ShamsiBirthDate = $"{year}/{month:00}/{day:00}";
        }

        return Page();
    }

    public IActionResult OnPost(int id)
    {
        Contact oldContact = databaseContext.contats.FirstOrDefault(contact => contact.Id == id);

        if (oldContact == null)
        {
            return NotFound();
        }



        if (string.IsNullOrWhiteSpace(Contact.FullName))
        {
            ModelState.AddModelError("Contact.FullName", "Please enter your full name.");
        }

        if (string.IsNullOrWhiteSpace(Contact.Mobile) ||
            Contact.Mobile.Length != 11 ||
            !Contact.Mobile.All(char.IsDigit))
        {
            ModelState.AddModelError("Contact.Mobile", "Please enter an 11-digit phone number.");
        }


        if (string.IsNullOrWhiteSpace(ShamsiBirthDate) == false)
        {
            string[] dateParts = ShamsiBirthDate.Split('/');

            if (dateParts.Length == 3 &&
                int.TryParse(dateParts[0], out int year) &&
                int.TryParse(dateParts[1], out int month) &&
                int.TryParse(dateParts[2], out int day))
            {
                try
                {
                    PersianCalendar calendar = new PersianCalendar();

                    DateTime gregorianDate = calendar.ToDateTime(year, month, day, 0, 0, 0, 0);

                    Contact.BirthDate = DateOnly.FromDateTime(gregorianDate);
                }
                catch (ArgumentOutOfRangeException)
                {
                    ModelState.AddModelError("ShamsiBirthDate", "Please enter a valid birth date.");
                }
            }
            else
            {
                ModelState.AddModelError("ShamsiBirthDate", "Please enter a valid birth date.");
            }
        }
        else
        {
            Contact.BirthDate = null;
        }        

        if (ModelState.IsValid == false)
        {
            Contact.Id = id;
            return Page();
        }

        bool mobileExists = databaseContext.contats.Any(contact =>
            contact.Mobile == Contact.Mobile && contact.Id != id);

        if (mobileExists)
        {
            ModelState.AddModelError("Contact.Mobile", "This phone number already exists.");
            Contact.Id = id;
            return Page();
        }

        oldContact.FullName = Contact.FullName;
        oldContact.Mobile = Contact.Mobile;
        oldContact.BirthDate = Contact.BirthDate;

        if (UploadedPhoto != null)
        {
            MemoryStream memoryStream = new MemoryStream();
            UploadedPhoto.CopyTo(memoryStream);
            oldContact.Photo = memoryStream.ToArray();
        }

        databaseContext.SaveChanges();

        return RedirectToPage("/Index");
    }
}
