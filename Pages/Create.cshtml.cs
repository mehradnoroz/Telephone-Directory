
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TelephoneDirectory.Data.AppDbContext;
using TelephoneDirectory.Models.contact;
using System.Globalization;
namespace TelephoneDirectory.Pages;

public class CreateModel : PageModel
{
    private AppDbContext databaseContext;

    public CreateModel(AppDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    [BindProperty]
    public Contact Contact { get; set; } = new Contact
    {
        FullName = "",
        Mobile = ""
    };
    [BindProperty]
    public string? ShamsiBirthDate { get; set; }

    [BindProperty]
    public IFormFile? UploadedPhoto { get; set; }

    public void OnGet(){}

    public bool ConvertToGregorianDate(string shamsiDate, out DateOnly gregorianDate)
    {
        gregorianDate = default;
        string[] dateParts = shamsiDate.Split('/');
        if (dateParts.Length != 3)
        {
            return false;
        }
        if (!int.TryParse(dateParts[0], out int year) ||
            !int.TryParse(dateParts[1], out int month) ||
            !int.TryParse(dateParts[2], out int day))
        {
            return false;
        }

        try
        {
            PersianCalendar calendar = new PersianCalendar();
            DateTime date = calendar.ToDateTime(year, month, day, 0, 0, 0, 0);

            gregorianDate = DateOnly.FromDateTime(date);
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
    }
    public IActionResult OnPost()
    {
        if (string.IsNullOrWhiteSpace(Contact.FullName))
        {
            ModelState.AddModelError("Contact.FullName", "Please enter your name.");
        }

        if (string.IsNullOrWhiteSpace(Contact.Mobile) ||
            Contact.Mobile.Length != 11 ||
            !Contact.Mobile.All(char.IsDigit))
        {
            ModelState.AddModelError("Contact.Mobile", "Please enter phone number.");
        }
        if (string.IsNullOrWhiteSpace(ShamsiBirthDate) == false)
        {
            DateOnly gregorianDate;

            if (ConvertToGregorianDate(ShamsiBirthDate, out gregorianDate))
            {
                Contact.BirthDate = gregorianDate;
            }
            else
            {
                ModelState.AddModelError("ShamsiBirthDate", "Please enter a valid birth date.");
            }
        }
        if (ModelState.IsValid == false)
        {
            return Page();
        }

        if (databaseContext.contats.Any(contact => contact.Mobile == Contact.Mobile))
        {
            ModelState.AddModelError("Contact.Mobile", "This phone number already exists.");
            return Page();
        }

        if (UploadedPhoto != null)
        {
            MemoryStream memoryStream = new MemoryStream();
            UploadedPhoto.CopyTo(memoryStream);
            Contact.Photo = memoryStream.ToArray();
        }

        databaseContext.contats.Add(Contact);
        databaseContext.SaveChanges();

        return RedirectToPage("/Index");
    }
}
