using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TelephoneDirectory.Data.AppDbContext;
using TelephoneDirectory.Models.contact;

namespace TelephoneDirectory.Pages;

public class DetailsModel : PageModel
{
    private AppDbContext databaseContext;

    public Contact? Contact { get; set; }

    public DetailsModel(AppDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }
    public string ConvertToShamsiDate(DateOnly? gregorianDate)
    {
        if (gregorianDate == null)
        {
            return "";
        }

        PersianCalendar calendar = new PersianCalendar();

        DateTime date = gregorianDate.Value.ToDateTime(TimeOnly.MinValue);

        int year = calendar.GetYear(date);
        int month = calendar.GetMonth(date);
        int day = calendar.GetDayOfMonth(date);

        return $"{year}/{month:00}/{day:00}";
    }
    public IActionResult OnGet(int id)
    {
        Contact = databaseContext.contats.FirstOrDefault(contact=>contact.Id == id);

        if (Contact == null)
        {
            return NotFound();
        }

        return Page();
    }
}
